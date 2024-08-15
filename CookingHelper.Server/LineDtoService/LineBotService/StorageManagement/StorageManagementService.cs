using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.Model;
using Microsoft.Extensions.Caching.Memory;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

namespace CookingHelper.LineDtoService;

public class StorageManagementService
{
    private readonly IMemoryCache _memoryCache;

    private readonly StorageManagementDatabaseService _storageManagementDatabaseService;
    private static int _PageIndexStatic = 1;
    private static int _PageSizeStatic = 10;

    private static string storageStatic = "display";
    public static string _StorageStatic
    {
        get => storageStatic;
        set
        {
            if (value != "delete" && value != "display")
            {
                throw new ArgumentException("Value Error");
            }
            storageStatic = value;
        }
    }
    public static dynamic _ReplyMessageListStatic = new List<object>();

    public StorageManagementService(
        StorageManagementDatabaseService StorageManagementDatabaseService,
        IMemoryCache memoryCache
    )
    {
        _memoryCache = memoryCache;
        _storageManagementDatabaseService = StorageManagementDatabaseService;
    }

    public async Task GetStorage(WebhookEventDto WebHookEventDto)
    {
        var WebHookEventMessage = WebHookEventDto.Message!.Text!;
        var MethodGroup = StorageSearchBaseClass.Instance;
        if (WebHookEventMessage == "返回")
        {
            _StorageStatic = "display";
        }
        if (_StorageStatic == "delete")
        {
            if ( //?
                _memoryCache.TryGetValue("StorageSearched", out IEnumerable<StoreItem> StorageSearched)
            )
            {
                /*
                    1/5/2
                    1~3/7/8
                    2~4/3/9
                    2~2/3/9
                    4~3/3/9
                    4~10/3/9
                */
                StringSlashAndTildeToList(
                    WebHookEventMessage,
                    out List<int> DeleteNumber,
                    out string ErrorText
                );
                if (ErrorText != "")
                {
                    _ReplyMessageListStatic = new List<object>
                    {
                        new TextMessageObject
                        {
                            Text = ErrorText,
                            QuickReply = new QuickReplyItemDto
                            {
                                Items = new List<QuickReplyButtonDto>
                                {
                                    MethodGroup.GetQuickReplyButton(
                                        ActionTypeEnum.Message,
                                        "返回",
                                        "返回"
                                    )
                                }
                            }
                        }
                    };
                    LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
                    {
                        ReplyToken = WebHookEventDto.ReplyToken!,
                        Messages = _ReplyMessageListStatic
                    };
                    return;
                }
                var DeleteStoreItem = StorageSearched!
                    .Select((item, index) => new { ItemID = item.StoreItemId, Index = index })
                    .Where(item => DeleteNumber.Contains(item.Index + 1))
                    .AsEnumerable()
                    .Select(item => item.ItemID);

                await _storageManagementDatabaseService.DeleteStoreItemList(DeleteStoreItem);
                _StorageStatic = "display";
            }
        }

        var StoreList = await _storageManagementDatabaseService.GetStoreList(
            WebHookEventDto!.Source!.UserId!
        );
        if (WebHookEventMessage == KeywordGroup.StorageManagement)
        {
            _PageIndexStatic = 1;
        }

        if (WebHookEventMessage == "刪除")
        {
            _ReplyMessageListStatic = new List<object>
            {
                new TextMessageObject { Text = "刪除請輸入編號, 若刪除編號1的項目, 請輸入數字1即可.", },
                new TextMessageObject
                {
                    Text = "刪除多個項目用/分隔如 1/5/2, 刪除相連編號可用~符號如1~3/5 及刪除編號1,編號2,編號3及編號5的項目",

                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            MethodGroup.GetQuickReplyButton(ActionTypeEnum.Message, "返回", "返回")
                        }
                    }
                },
            };
            _StorageStatic = "delete";
            LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
            {
                ReplyToken = WebHookEventDto.ReplyToken!,
                Messages = _ReplyMessageListStatic
            };
            return;
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
            var OrderedByPlaceStoreItem = StoreList
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

            var PaginatedStoreItem = Paginate(
                OrderedByPlaceStoreItem,
                _PageIndexStatic,
                _PageSizeStatic,
                out bool hasNextPage,
                out bool hasPrevPage
            );

            LineBotService._WebhookEventStatusStatic = KeywordGroup.StorageManagement;

            //! 建立假資料

            if (WebHookEventMessage == "依購買日期排序")
            {
                PaginatedStoreItem =
                    (IQueryable<StoreItem>)
                        ((IOrderedEnumerable<StoreItem>)PaginatedStoreItem).ThenBy(
                            Item => Item.PurchaseDate,
                            new CustomComparer()
                        );
            }
            if (WebHookEventMessage == "依有效日期排序")
            {
                PaginatedStoreItem =
                    (IQueryable<StoreItem>)
                        ((IOrderedEnumerable<StoreItem>)PaginatedStoreItem).ThenBy(
                            Item => Item.ExpiryDate,
                            new CustomComparer()
                        );
            }

            var StorageFieldUIList = PaginatedStoreItem
                .Select(MethodGroup.GetStorageUIField)
                .ToList();
            _memoryCache.Set("StorageSearched", (IEnumerable<StoreItem>)PaginatedStoreItem);
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
