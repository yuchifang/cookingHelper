using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

namespace CookingHelper.LineDtoService;

public class StorageManagementPurchaseService
{
    private readonly StorageManagementService _storageManagementService;
    private readonly StorageManagementDatabaseService _storageManagementDatabaseService;

    public StorageManagementPurchaseService(
        StorageManagementService StorageManagementService,
        StorageManagementDatabaseService StorageManagementDatabaseService
    )
    {
        _storageManagementService = StorageManagementService;
        _storageManagementDatabaseService = StorageManagementDatabaseService;
    }

    public static InputStorageInfo _InputStorageInfoStatic = new InputStorageInfo();

    private static dynamic _ReplyMessageListStatic = new List<object>();

    public async Task InputStorage(WebhookEventDto WebHookEventDto)
    {
        string? WebHookEventMessage = WebHookEventDto.Message!.Text;

        if (WebHookEventMessage == "新增物品至庫存")
            _InputStorageInfoStatic.Status = "init";

        if (WebHookEventMessage == "取消新增")
        {
            _InputStorageInfoStatic = new InputStorageInfo();
            await _storageManagementService.GetStorage(WebHookEventDto);
            return;
        }

        if (WebHookEventMessage == "略過")
        {
            WebHookEventMessage = null;
        }

        if (WebHookEventMessage == "填寫完成")
        {
            LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
            {
                ReplyToken = WebHookEventDto.ReplyToken!,
                Messages = InputStorageBaseClass.Instance.GetAdditionConfirmHint(
                    _InputStorageInfoStatic,
                    null
                )
            };
            return;
        }

        if (WebHookEventMessage == "新增")
        {
            await _storageManagementDatabaseService.AddStoreItemData(
                WebHookEventDto.Source!.UserId!,
                _InputStorageInfoStatic
            );
            _InputStorageInfoStatic = new InputStorageInfo();

            await _storageManagementService.GetStorage(WebHookEventDto);
            StorageManagementService._ReplyMessageListStatic.Insert(
                0,
                new TextMessageObject { Text = "新增完成" }
            );
            return;
        }

        var StatusProcessor = new Dictionary<string, Action>
        {
            { "init", InitStatus },
            { "place", () => PlaceStatus(WebHookEventMessage!) },
            { "name", () => NameStatus(WebHookEventMessage!) },
            { "amount", () => AmountStatus(WebHookEventMessage!) },
            { "location", () => LocationStatus(WebHookEventMessage!) },
            { "purchaseDate", () => PurchaseDateStatus(WebHookEventMessage!) },
            { "expiryDate", () => ExpiryDateStatus(WebHookEventMessage!) },
            { "edit", () => EditStatus(WebHookEventMessage!) }
        };

        StatusProcessor[_InputStorageInfoStatic.Status]();

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    public async Task EditAddedResultConfirmPostBack(WebhookEventDto WebHookEventDto)
    {
        _ReplyMessageListStatic = new List<object>(
            [
                new TextMessageObject { Text = "若要修改欄位, 例如修改物品名稱, 請輸入: 物品名稱:XXX並送出. XXX為要修改的資料", },
                new TextMessageObject { Text = "送出的結果為: 物品名稱:XXX", },
                new TextMessageObject
                {
                    Text = "若要修改多個欄位, 請輸入: 購買日期:YYYYMMDD/有效日期:YYYYMMDD/數量:XX並送出",
                },
                new TextMessageObject
                {
                    Text = "請依上述說明, 輸入要改的欄位",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "取消修改",
                                    Text = "取消修改",
                                }
                            }
                        }
                    }
                }
            ]
        );
        _InputStorageInfoStatic.Status = "edit";
        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    public void InitStatus()
    {
        var StorageStatus = InputStorageBaseClass.Instance;
        LineBotService._WebhookEventStatusStatic = "新增物品至庫存";
        _ReplyMessageListStatic = new List<object>(
            [
                new TextMessageObject { Text = "依儲存位置,物品名稱,詳細位置,購買日期,有效日期輸入", },
                new TextMessageObject { Text = "儲存位置及物品名稱一定要填入, 沒填入將無法紀錄", },
                new TextMessageObject
                {
                    Text = "請輸入儲存位置:",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            StorageStatus.GetQuickReplyButton(
                                ActionTypeEnum.Message,
                                "取消新增",
                                "取消新增"
                            )
                        }
                    }
                }
            ]
        );
        _InputStorageInfoStatic.Status = "place";
    }

    public void PlaceStatus(string WebHookEventMessage)
    {
        var StorageStatus = InputStorageBaseClass.Instance;
        _InputStorageInfoStatic.Place = WebHookEventMessage;
        _ReplyMessageListStatic = new List<object>(
            [
                new TextMessageObject { Text = "此欄位為必填, 沒填入將無法紀錄", },
                new TextMessageObject
                {
                    Text = "請輸入物品名稱:",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            StorageStatus.GetQuickReplyButton(
                                ActionTypeEnum.Message,
                                "取消新增",
                                "取消新增"
                            )
                        }
                    }
                }
            ]
        );
        _InputStorageInfoStatic.Status = "name";
    }

    public void NameStatus(string WebHookEventMessage)
    {
        var StorageStatus = InputStorageBaseClass.Instance;
        _InputStorageInfoStatic.Name = WebHookEventMessage;
        _ReplyMessageListStatic = StorageStatus.GetRegularReply("請輸入數量:");
        _InputStorageInfoStatic.Status = "amount";
    }

    public void AmountStatus(string WebHookEventMessage)
    {
        var StorageStatus = InputStorageBaseClass.Instance;
        _InputStorageInfoStatic.Amount = WebHookEventMessage;
        _ReplyMessageListStatic = StorageStatus.GetRegularReply("請輸入詳細位置:");
        _InputStorageInfoStatic.Status = "location";
    }

    public void LocationStatus(string WebHookEventMessage)
    {
        var StorageStatus = InputStorageBaseClass.Instance;
        _InputStorageInfoStatic.Location = WebHookEventMessage;
        _ReplyMessageListStatic = StorageStatus.GetRegularReply("請輸入購買日期(格式: YYYYMMDD):");
        _InputStorageInfoStatic.Status = "purchaseDate";
    }

    public void PurchaseDateStatus(string WebHookEventMessage)
    {
        var StorageStatus = InputStorageBaseClass.Instance;
        var dateString = WebHookEventMessage;

        _ReplyMessageListStatic = StorageStatus.GetRegularReply("請輸入有效日期(格式: YYYYMMDD):");

        if (dateString == null)
        {
            _InputStorageInfoStatic.PurchaseDate = null;
            _InputStorageInfoStatic.Status = "expiryDate";
        }
        else if (DateOnly.TryParseExact(dateString, "yyyyMMdd", out DateOnly PurchaseDate))
        {
            _InputStorageInfoStatic.PurchaseDate = PurchaseDate;
            _InputStorageInfoStatic.Status = "expiryDate";
        }
        else
        {
            _ReplyMessageListStatic = StorageStatus.DateTypeErrorHint(
                "$ 購買日期格式錯誤, 請重新輸入(格式: YYYYMMDD)"
            );
        }
    }

    public void ExpiryDateStatus(string WebHookEventMessage)
    {
        var StorageStatus = InputStorageBaseClass.Instance;
        var dateString = WebHookEventMessage;
        if (dateString == null)
        {
            _InputStorageInfoStatic.ExpiryDate = null;
            _ReplyMessageListStatic = StorageStatus.GetAdditionConfirmHint(
                _InputStorageInfoStatic,
                null
            );
        }
        else if (DateOnly.TryParseExact(dateString, "yyyyMMdd", out DateOnly ExpiryDate))
        {
            _InputStorageInfoStatic.ExpiryDate = ExpiryDate;
            _ReplyMessageListStatic = StorageStatus.GetAdditionConfirmHint(
                _InputStorageInfoStatic,
                null
            );
        }
        else
        {
            _ReplyMessageListStatic = StorageStatus.DateTypeErrorHint(
                "$ 有效日期格式錯誤, 請重新輸入(格式: YYYYMMDD)"
            );
        }
    }

    public void EditStatus(string WebHookEventMessage)
    {
        var MethodGroup = InputStorageBaseClass.Instance;
        StringSlashAndColonToStorageInfo(
            WebHookEventMessage!,
            out StorageInfo StorageInfoData,
            out string ErrorText
        );
        if (ErrorText != "")
        {
            _ReplyMessageListStatic = MethodGroup.GetAdditionConfirmHint(
                _InputStorageInfoStatic,
                null
            );

            _ReplyMessageListStatic.Add(
                new TextMessageObject { Text = "發生錯誤: 此欄位出現問題 " + ErrorText }
            );
        }
        else
        {
            if (StorageInfoData.Place != null)
                _InputStorageInfoStatic.Place = StorageInfoData.Place;
            if (StorageInfoData.Name != null)
                _InputStorageInfoStatic.Name = StorageInfoData.Name;
            if (StorageInfoData.Location != null)
                _InputStorageInfoStatic.Location = StorageInfoData.Location;
            if (StorageInfoData.Amount != null)
                _InputStorageInfoStatic.Amount = StorageInfoData.Amount;
            if (StorageInfoData.PurchaseDate != null)
                _InputStorageInfoStatic.PurchaseDate = StorageInfoData.PurchaseDate;
            if (StorageInfoData.ExpiryDate != null)
                _InputStorageInfoStatic.ExpiryDate = StorageInfoData.ExpiryDate;
            _ReplyMessageListStatic = MethodGroup.GetAdditionConfirmHint(
                _InputStorageInfoStatic,
                null
            );
        }
    }
}
