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
            var OrderedStoreItemList = StoreList
                .StoreItemList.OrderBy(Item => Item.Place)
                .AsQueryable();

            LineBotService._WebhookEventStatusStatic = KeywordGroup.StorageManagement;
            // todo
            //? 簡化程式碼 引用 UI??
            //? 加入假資料
            //? 如果欄位超過幾個就換頁/ 用 take??
            //? 換頁, 顯示總共有幾筆資料, 決定顯示 15筆或其他  FlexMessage

            if (WebHookEventMessage == "依購買日期排序")
            {
                OrderedStoreItemList =
                    (IQueryable<StoreItem>)
                        ((IOrderedEnumerable<StoreItem>)OrderedStoreItemList).ThenBy(
                            Item => Item.PurchaseDate,
                            new CustomComparer()
                        );
            }
            if (WebHookEventMessage == "依有效日期排序")
            {
                OrderedStoreItemList =
                    (IQueryable<StoreItem>)
                        ((IOrderedEnumerable<StoreItem>)OrderedStoreItemList).ThenBy(
                            Item => Item.ExpiryDate,
                            new CustomComparer()
                        );
            }

            var StorageFieldUIList = OrderedStoreItemList.Select(GetStorageUIField).ToList();

            //! 取資料 查 取資料的方式
            //! Storage 查詢 下一頁用 postBack
            //! 建立假資料
            //! IEnumerable and Queryable
            /*
                編號 存放位置 物品名稱 ...
                資料 ...
                ?編號server 產生

                查詢功能 Flexmessage
                    查到用 flex message 顯示
                    取消查詢
                    最下面加個 修改,刪除, 返回

                刪除功能 選擇編號 ex: 010/020
                    取消刪除
                全部刪除

            */
            _ReplyMessageListStatic = new List<object>
            {
                GetStorageManagementUIBlock(StorageFieldUIList), //? 像是 StorageManagement 在方一個中間class
            };
        }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    public FlexComponent GetStorageUIField(StoreItem StoreItem, int index)
    {
        var LocationText = StoreItem.Location != null ? $" {StoreItem.Location}" : "";
        var AmountText = StoreItem.Amount != null ? $" {StoreItem.Amount}" : "";
        var PurchaseDateText =
            StoreItem.PurchaseDate != null
                ? $" (p){DateOnlyToString((DateOnly)StoreItem.PurchaseDate, null)}"
                : "";
        var ExpiryDateText =
            StoreItem.ExpiryDate != null
                ? $" (e){DateOnlyToString((DateOnly)StoreItem.ExpiryDate, null)}"
                : "";

        return new FlexComponent
        {
            Type = FlexComponentTypeEnum.Box,
            Layout = FlexComponentLayoutTypeEnum.Vertical,
            PaddingBottom = "10px",
            Contents = new List<FlexComponent>
            {
                new FlexComponent
                {
                    Wrap = true,
                    Type = FlexComponentTypeEnum.Text,
                    Size = "xl",

                    Text =
                        $"{index + 1} {StoreItem.Place} {StoreItem.Name}{LocationText}{AmountText}{PurchaseDateText}{ExpiryDateText}"
                },
            }
        };
    }

    public FlexMessageObject<FlexBubbleContainer> GetStorageManagementUIBlock(
        List<FlexComponent> StorageFieldUIList
    )
    {
        var StorageUITable = new List<FlexComponent>
        {
            new FlexComponent
            {
                Type = FlexComponentTypeEnum.Box,
                Layout = FlexComponentLayoutTypeEnum.Vertical,
                Contents = new List<FlexComponent>
                {
                    new FlexComponent
                    {
                        Size = "md",
                        Wrap = true,
                        Type = FlexComponentTypeEnum.Text,
                        Text = "依編號,儲存位置,物品名稱,詳細位置,數量,購買日期(p),有效日期(e)排列"
                    },
                }
            },
            new FlexComponent
            {
                Type = FlexComponentTypeEnum.Box,
                Layout = FlexComponentLayoutTypeEnum.Horizontal,
                Contents = new List<FlexComponent>
                {
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Button,
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Message,
                            Label = "依購買日期排序",
                            Text = "依購買日期排序"
                        }
                    },
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Button,
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Message,
                            Label = "依有效日期排序",
                            Text = "依有效日期排序"
                        }
                    },
                }
            },
            new FlexComponent
            {
                Type = FlexComponentTypeEnum.Box,
                Layout = FlexComponentLayoutTypeEnum.Horizontal,
                Contents = new List<FlexComponent>
                {
                    new FlexComponent
                    {
                        Align = "center",
                        Gravity = "center",
                        Type = FlexComponentTypeEnum.Text,
                        Text = "目前頁數 1/15"
                    },
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Button,
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Message,
                            Label = "換下一頁",
                            Text = "換下一頁"
                        }
                    },
                }
            },
        };
        StorageUITable.InsertRange(2, StorageFieldUIList);
        return new FlexMessageObject<FlexBubbleContainer>
        {
            AltText = "StorageManagementUIBlock",

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
                    },
                    new QuickReplyButtonDto
                    {
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Postback,
                            Label = "庫存查詢",
                            Data = "庫存查詢",
                            InputOption = PostbackInputOptionEnum.OpenKeyboard,
                        }
                    },
                    new QuickReplyButtonDto
                    {
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Postback,
                            Label = "刪除",
                            Text = "刪除",
                            Data = "刪除",
                            InputOption = PostbackInputOptionEnum.OpenKeyboard,
                        }
                    }
                }
            },
            Contents = new FlexBubbleContainer
            {
                Size = "giga",
                Type = FlexContainerTypeEnum.Bubble,
                Body = new FlexComponent
                {
                    Type = FlexComponentTypeEnum.Box,
                    Layout = FlexComponentLayoutTypeEnum.Vertical,
                    PaddingAll = "10px",
                    PaddingBottom = "0px",
                    Contents = StorageUITable
                }
            }
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
