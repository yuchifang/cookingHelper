using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.Model;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

namespace CookingHelper.LineDtoService;

public class StorageManagementService
{
    private readonly StorageManagementDatabaseService _storageManagementDatabaseService;
    private static int _PageIndexStatic = 1;
    private static int _PageSizeStatic = 10;
    public static dynamic _ReplyMessageListStatic = new List<object>();

    public StorageManagementService(
        StorageManagementDatabaseService StorageManagementDatabaseService
    )
    {
        _storageManagementDatabaseService = StorageManagementDatabaseService;
    }

    public async Task GetStorage(WebhookEventDto WebHookEventDto)
    {
        var WebHookEventMessage = WebHookEventDto.Message!.Text!;
        var StoreList = await _storageManagementDatabaseService.GetStoreListData(
            WebHookEventDto!.Source!.UserId!
        );
        if (WebHookEventMessage == KeywordGroup.StorageManagement)
        {
            _PageIndexStatic = 1;
        }
        if (StoreList.StoreItemList.Count == 0)
        {
            _ReplyMessageListStatic = new List<object>
            {
                new TextMessageObject
                {
                    Text = "庫存中沒有物品, 點擊按鈕, 輸入想要紀錄的物品",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Postback,
                                    Label = "新增物品至庫存",
                                    Text = "新增物品至庫存", // 有用
                                    Data = "新增物品至庫存",
                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                }
                            }
                        }
                    }
                },
            };
        }
        else
        {
            var MethodGroup = StorageSearchBaseClass.Instance;

            var OrderedStoreItemList = StoreList
                .StoreItemList.OrderBy(Item => Item.Place)
                .AsQueryable();
            if (WebHookEventMessage == "下一頁")
            {
                _PageIndexStatic += 1;
            }
            else if (WebHookEventMessage == "上一頁")
            {
                _PageIndexStatic -= 1;
            }

            var SplitStoreItemList = Paginate(
                OrderedStoreItemList,
                _PageIndexStatic,
                _PageSizeStatic,
                out bool hasNextPage,
                out bool hasPrevPage
            );

            LineBotService._WebhookEventStatusStatic = KeywordGroup.StorageManagement;

            //! 建立假資料

            /*
                編號 存放位置 物品名稱 ...
                資料 ...
                ?編號server 產生


                刪除功能 選擇編號 ex: 010/020
                    取消刪除
                全部刪除

            */

            if (WebHookEventMessage == "依購買日期排序")
            {
                SplitStoreItemList =
                    (IQueryable<StoreItem>)
                        ((IOrderedEnumerable<StoreItem>)SplitStoreItemList).ThenBy(
                            Item => Item.PurchaseDate,
                            new CustomComparer()
                        );
            }
            if (WebHookEventMessage == "依有效日期排序")
            {
                SplitStoreItemList =
                    (IQueryable<StoreItem>)
                        ((IOrderedEnumerable<StoreItem>)SplitStoreItemList).ThenBy(
                            Item => Item.ExpiryDate,
                            new CustomComparer()
                        );
            }

            var StorageFieldUIList = SplitStoreItemList
                .Select(MethodGroup.GetStorageUIField)
                .ToList();

            _ReplyMessageListStatic = new List<object>
            {
                MethodGroup.GetStorageManagementUIBlock(
                    StorageFieldUIList,
                    hasNextPage,
                    hasPrevPage
                ),
            };
        }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }
}

public class InputStorageInfo : StorageInfo
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
}

public class StorageInfo
{
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
