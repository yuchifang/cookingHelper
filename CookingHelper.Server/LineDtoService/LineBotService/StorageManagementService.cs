using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using static CookingHelper.LineDto.BaseMessageObject;

namespace CookingHelper.LineDtoService;

public class StorageManagementService
{
    private readonly StorageManagementDatabaseService _storageManagementDatabaseService;

    public static dynamic _ReplyMessageListStatic = new List<object>();

    public StorageManagementService(
        StorageManagementDatabaseService StorageManagementDatabaseService
    )
    {
        _storageManagementDatabaseService = StorageManagementDatabaseService;
    }

    public async Task Init(WebhookEventDto WebHookEventDto)
    {
        var StoreList = await _storageManagementDatabaseService.GetStoreListData(
            WebHookEventDto!.Source!.UserId!
        );
        // 創建 一個 Purchase
        if (StoreList.StoreItemList.Count == 0)
        {
            _ReplyMessageListStatic.Add(
                new TextMessageObject
                {
                    //? 只能按輸入按鈕才能輸入 要黨無效輸入
                    //? 按了按鈕 _WebhookEventState = KeywordGroup.StorageManagement
                    Text = "庫存中沒有物品, 點擊按鈕, 輸入想要紀錄的物品",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "新增物品至庫存",
                                    Text = "新增物品至庫存",
                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                }
                            }
                        }
                    }
                }
            );
        }
        else
        {
            //! 在 Status 引用另一個 class FlexComponent的class
            //! 取資料 查 取資料的方式
            /*
                把所有的資料 依照存放地方排序 FlexMessage 顯示 複製編號
                "依編號,存放位置,物品名稱,詳細位置,數量,購買日期(p),有效日期(e)排列" MaxWidth?? [] 測試
                1 冰箱 蘋果 8 (p)2022-08-07 (e)2023-09-07

                選擇排序方式 文字
                button	依存放日期近排序/遠切換,依存放位置排序,//? 可不可以橫列 box maxWidth
                換頁,
                查詢 button,
                刪除 button
                ? 預設存放地方排序


                編號 存放位置 物品名稱 ...
                資料 ...
                ?編號server 產生

                查詢功能 Flexmessage
                查到用 flex message 顯示
                最下面加個 修改,刪除, 返回

                刪除功能 選擇編號 ex: 010/020
            */
            _ReplyMessageListStatic = new List<object>
            {
                new TextMessageObject
                {
                    //? 只能按輸入按鈕才能輸入 要黨無效輸入
                    //? 按了按鈕 _WebhookEventState = KeywordGroup.StorageManagement
                    Text = "庫存中沒有物品, 點擊按鈕, 輸入想要紀錄的物品",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "新增物品至庫存",
                                    Text = "新增物品至庫存",
                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                }
                            }
                        }
                    }
                }
            };
        }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<TextMessageObject>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }
}

public class InputStorageInfo
{
    private string _Status = "init";
    public string Status
    {
        get => _Status;
        set
        {
            if (
                value != "place"
                && value != "name"
                && value != "location"
                && value != "purchaseDate"
                && value != "expiryDate"
                && value != "amount"
                && value != "init"
                && value != "end"
                && value != "edit"
            )
            {
                throw new ArgumentException("Value Error");
            }
            _Status = value;
        }
    }

    // 儲存位置
    // 物品名稱
    // 詳細位置
    // 數量
    // 購買日期
    // 有效日期
    public string Place { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Location { get; set; }
    public string? Amount { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }
}
