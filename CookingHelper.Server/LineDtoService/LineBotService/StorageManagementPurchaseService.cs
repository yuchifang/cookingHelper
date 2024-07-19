using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.LineDto.BaseMessageObject;

namespace CookingHelper.LineDtoService;

//! 寫完要更新 Database migrations

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

    class InitStatus : StorageInputStatus
    {
        public override void Init()
        {
            LineBotService._WebhookEventStatusStatic = "新增物品至庫存";
            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject { Text = "依儲存位置,物品名稱,詳細位置,購買日期,過期日期輸入", },
                    new TextMessageObject { Text = "儲存位置及物品名稱一定要填入, 沒填入將無法紀錄", },
                    new TextMessageObject
                    {
                        Text = "請輸入儲存位置:",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                GetCancelAdditionQuickReplyButton()
                            }
                        }
                    }
                ]
            );
            _InputStorageInfoStatic.Status = "place";
        }
    }

    class PlaceStatus : StorageInputStatus
    {
        private readonly string _WebHookEventMessage;

        public PlaceStatus(string WebHookEventMessage)
        {
            _WebHookEventMessage = WebHookEventMessage;
        }

        public override void Init()
        {
            _InputStorageInfoStatic.Place = _WebHookEventMessage;
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
                                GetCancelAdditionQuickReplyButton()
                            }
                        }
                    }
                ]
            );
            _InputStorageInfoStatic.Status = "name";
        }
    }

    class NameStatus : StorageInputStatus
    {
        private readonly string _WebHookEventMessage;

        public NameStatus(string WebHookEventMessage)
        {
            _WebHookEventMessage = WebHookEventMessage;
        }

        public override void Init()
        {
            _InputStorageInfoStatic.Name = _WebHookEventMessage;
            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject
                    {
                        Text = "請輸入數量:",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                GetCancelAdditionQuickReplyButton(),
                                GetSkipQuickReplyButton(),
                                GetAdditionCompleteQuickReplyButton()
                            }
                        }
                    }
                ]
            );
            _InputStorageInfoStatic.Status = "amount";
        }
    }

    class AmountStatus : StorageInputStatus
    {
        private readonly string? _WebHookEventMessage;

        public AmountStatus(string? WebHookEventMessage)
        {
            _WebHookEventMessage = WebHookEventMessage;
        }

        public override void Init()
        {
            _InputStorageInfoStatic.Amount = _WebHookEventMessage;
            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject
                    {
                        Text = "請輸入詳細位置:",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                GetCancelAdditionQuickReplyButton(),
                                GetSkipQuickReplyButton(),
                                GetAdditionCompleteQuickReplyButton()
                            }
                        }
                    }
                ]
            );
            _InputStorageInfoStatic.Status = "location";
        }
    }

    class LocationStatus : StorageInputStatus
    {
        private readonly string? _WebHookEventMessage;

        public LocationStatus(string? WebHookEventMessage)
        {
            _WebHookEventMessage = WebHookEventMessage;
        }

        public override void Init()
        {
            _InputStorageInfoStatic.Location = _WebHookEventMessage;
            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject
                    {
                        Text = "請輸入購買日期(格式: YYYYMMDD):",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                GetCancelAdditionQuickReplyButton(),
                                GetSkipQuickReplyButton(),
                                GetAdditionCompleteQuickReplyButton()
                            }
                        }
                    }
                ]
            );
            _InputStorageInfoStatic.Status = "purchaseDate";
        }
    }

    class PurchaseDateStatus : StorageInputStatus
    {
        private readonly string? _WebHookEventMessage;

        public PurchaseDateStatus(string? WebHookEventMessage)
        {
            _WebHookEventMessage = WebHookEventMessage;
        }

        public override void Init()
        {
            var dateString = _WebHookEventMessage;

            if (dateString == null)
            {
                _InputStorageInfoStatic.PurchaseDate = null;
                _ReplyMessageListStatic = new List<object>(
                    [
                        new TextMessageObject
                        {
                            Text = "請輸入過期日期(格式: YYYYMMDD):",
                            QuickReply = new QuickReplyItemDto
                            {
                                Items = new List<QuickReplyButtonDto>
                                {
                                    GetCancelAdditionQuickReplyButton(),
                                    GetSkipQuickReplyButton(),
                                    GetAdditionCompleteQuickReplyButton()
                                }
                            }
                        }
                    ]
                );
                _InputStorageInfoStatic.Status = "expiryDate";
            }
            else if (DateOnly.TryParseExact(dateString, "yyyyMMdd", out DateOnly PurchaseDate))
            {
                _InputStorageInfoStatic.PurchaseDate = PurchaseDate;
                _ReplyMessageListStatic = new List<object>(
                    [
                        new TextMessageObject
                        {
                            Text = "請輸入過期日期(格式: YYYYMMDD):",
                            QuickReply = new QuickReplyItemDto
                            {
                                Items = new List<QuickReplyButtonDto>
                                {
                                    GetCancelAdditionQuickReplyButton(),
                                    GetSkipQuickReplyButton(),
                                    GetAdditionCompleteQuickReplyButton()
                                }
                            }
                        }
                    ]
                );
                _InputStorageInfoStatic.Status = "expiryDate";
            }
            else
            {
                _ReplyMessageListStatic = DateTypeErrorHint("$ 購買日期格式錯誤, 請重新輸入(格式: YYYYMMDD)");
            }
        }
    }

    class ExpiryDateStatus : StorageInputStatus
    {
        private readonly string? _WebHookEventMessage;

        public ExpiryDateStatus(string? WebHookEventMessage)
        {
            _WebHookEventMessage = WebHookEventMessage;
        }

        public override void Init()
        {
            var dateString = _WebHookEventMessage;
            if (dateString == null)
            {
                _InputStorageInfoStatic.ExpiryDate = null;
                _ReplyMessageListStatic = GetAdditionConfirmHint(_InputStorageInfoStatic);
            }
            else if (DateOnly.TryParseExact(dateString, "yyyyMMdd", out DateOnly ExpiryDate))
            {
                _InputStorageInfoStatic.ExpiryDate = ExpiryDate;
                _ReplyMessageListStatic = GetAdditionConfirmHint(_InputStorageInfoStatic);
            }
            else
            {
                _ReplyMessageListStatic = DateTypeErrorHint("$ 過期日期格式錯誤, 請重新輸入(格式: YYYYMMDD)");
            }
        }
    }

    class EditStatus : StorageInputStatus
    {
        private readonly string? _WebHookEventMessage;

        public EditStatus(string? WebHookEventMessage)
        {
            _WebHookEventMessage = WebHookEventMessage;
        }

        public override void Init()
        {
            // 將使用者輸入的字串, 依"/"切分成每個欄位, 依每個欄位":" 切分成Key, Value, 並驗證日期格式,
            // 格式正確則顯示更新的資訊, 格式錯誤 顯示原本資訊

            var UserTextFieldArray = _WebHookEventMessage!.Split(
                "/",
                StringSplitOptions.RemoveEmptyEntries
            );
            var StorageTableList = new List<List<string>>();
            var GetError = "";
            try
            {
                foreach (var item in UserTextFieldArray)
                {
                    char[] Colon = { ':', '：' };
                    var ValuePairArray = item.Split(Colon, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToArray();

                    var ExamineKey = ValuePairArray[0];

                    if (ExamineKey == "購買日期" || ExamineKey == "有效日期")
                    {
                        if (
                            DateOnly.TryParseExact(ValuePairArray[1], "yyyyMMdd", out DateOnly Date)
                        )
                        {
                            StorageTableList.Add(ValuePairArray.ToList());
                        }
                        else
                        {
                            GetError = $"{ValuePairArray[0]}, {ValuePairArray[1]}";
                            break;
                        }
                    }

                    if (
                        (
                            Array.Find(
                                StorageManagementKeywordGroup.ExamineArray,
                                Key => Key == ExamineKey
                            )
                        ) != null
                    )
                    {
                        if (ExamineKey == "購買日期" || ExamineKey == "有效日期")
                        {
                            continue;
                        }

                        StorageTableList.Add(ValuePairArray.ToList());
                    }
                    else
                    {
                        GetError = $"{ValuePairArray[0]}, {ValuePairArray[1]}";
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            if (GetError == "")
            {
                foreach (var KeyValueList in StorageTableList)
                {
                    switch (KeyValueList[0])
                    {
                        case StorageManagementKeywordGroup.Place:
                            _InputStorageInfoStatic.Place = KeyValueList[1];
                            break;
                        case StorageManagementKeywordGroup.Name:
                            _InputStorageInfoStatic.Name = KeyValueList[1];
                            break;
                        case StorageManagementKeywordGroup.Location:
                            _InputStorageInfoStatic.Location = KeyValueList[1];
                            break;
                        case StorageManagementKeywordGroup.Amount:
                            _InputStorageInfoStatic.Amount = KeyValueList[1];
                            break;
                        case StorageManagementKeywordGroup.PurchaseDate:
                            DateOnly PurchaseDate = DateOnly.ParseExact(
                                KeyValueList[1],
                                "yyyyMMdd"
                            );
                            _InputStorageInfoStatic.PurchaseDate = PurchaseDate;
                            break;
                        case StorageManagementKeywordGroup.ExpiryDate:
                            DateOnly ExpiryDate = DateOnly.ParseExact(KeyValueList[1], "yyyyMMdd");
                            _InputStorageInfoStatic.ExpiryDate = ExpiryDate;
                            break;
                    }
                }

                _ReplyMessageListStatic = GetAdditionConfirmHint(_InputStorageInfoStatic);
            }
            else
            {
                _ReplyMessageListStatic = GetAdditionConfirmHint(_InputStorageInfoStatic);

                _ReplyMessageListStatic.Add(
                    new TextMessageObject { Text = "發生錯誤: 此欄位出現問題 " + GetError }
                );
            }
        }
    }

    public async Task InputStorage(WebhookEventDto WebHookEventDto)
    {
        string? WebHookEventMessage = WebHookEventDto.Message!.Text;

        if (WebHookEventMessage == "新增物品至庫存")
            _InputStorageInfoStatic.Status = "init";

        if (WebHookEventMessage == "取消新增")
        {
            _InputStorageInfoStatic = new InputStorageInfo();
            await _storageManagementService.Init(WebHookEventDto);
            return;
        }

        if (WebHookEventMessage == "略過")
        {
            WebHookEventMessage = null;
        }

        if (WebHookEventMessage == "新增完成")
        {
            await _storageManagementDatabaseService.AddStoreItemData(
                WebHookEventDto.Source!.UserId!,
                _InputStorageInfoStatic
            );
            _InputStorageInfoStatic = new InputStorageInfo();
            LineBotService._WebhookEventStatusStatic = KeywordGroup.StorageManagement;
            StorageManagementService._ReplyMessageListStatic.Add(
                new TextMessageObject { Text = "新增完成" }
            );
            await _storageManagementService.Init(WebHookEventDto);
            return;
            //? 使用 StorageManagementDatabase 呼叫 新增的方法
            //? 將 _InputStorageInfoStatic 清空
            //? return 回傳新增成功
            //? 回到 StorageManagement
        }

        var _StatusProcessorClass = new Dictionary<string, StorageInputStatus>
        {
            { "init", new InitStatus() },
            { "place", new PlaceStatus(WebHookEventMessage!) },
            { "name", new NameStatus(WebHookEventMessage!) },
            { "amount", new AmountStatus(WebHookEventMessage) },
            { "location", new LocationStatus(WebHookEventMessage) },
            { "purchaseDate", new PurchaseDateStatus(WebHookEventMessage) },
            { "expiryDate", new ExpiryDateStatus(WebHookEventMessage) },
            { "edit", new EditStatus(WebHookEventMessage) }
        };

        Console.WriteLine(WebHookEventMessage);

        _StatusProcessorClass[_InputStorageInfoStatic.Status].Init();

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    //? 新增完成 還沒做或改為 填寫完成
    //? 完成修改 狀態要改嗎 _InputStorageInfoStatic.Status??
    // 回覆一個 flexMessage 顯你使用者剛剛輸入的資料
    //      顯示按鈕完成, 修改, 取消
    //          修改 用字串表示 物品名稱:XXX/
    //          錯誤顯示輸入錯誤
    // 將資料樹入到資料庫?
    // 清空 _InputStorageDataStatusStatic, _inputStorageStatus="init"
    // 回復成功訊息
    // 回到 有資料的那頁

    public async Task EditAddedResultConfirmPostBack(WebhookEventDto WebHookEventDto)
    {
        //? 這樣寫好嗎
        //! FlexMessage Final Result 要改一下要加欄位名稱
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
}
