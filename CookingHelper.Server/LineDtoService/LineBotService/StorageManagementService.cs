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
                                    Type = ActionTypeEnum.Postback,
                                    Label = "新增物品至庫存",
                                    Text = "新增物品至庫存",
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
            // todo

            // 取得 StoreList
            // 排成 Text string builder??
            // 整理 FlexMessage
            // 加入 StorageManagement sort button
            // 換頁 功能 flexMessage
            // Quick reply 查詢 刪除




            //! 在 Status 引用另一個 class FlexComponent的class
            //! 取資料 查 取資料的方式
            /*
                把所有的資料 依照存放地方排序 FlexMessage 顯示 複製編號
                "依編號,存放位置,物品名稱,詳細位置,數量,購買日期(p),有效日期(e)排列" MaxWidth?? [] 測試
                1 冰箱 蘋果 8 (p)2022-08-07 (e)2023-09-07

                選擇排序方式 文字 FlexMessage
                button	依購買日期近排序/過期日期排序切換,
                依存放位置排序//? 可不可以橫列 box maxWidth
                    沒輸入就不排, 預設再存方位置的方式
                        可以用 System.Linq 找這種功能
                
                換頁, 顯示總共有幾筆資料, 決定顯示 15筆或其他  FlexMessage

                quicky reply??
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
                GetButton(), //? 像是 StorageManagement 在方一個中間class
            };
        }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    public FlexMessageObject<FlexBubbleContainer> GetButton()
    {
        return new FlexMessageObject<FlexBubbleContainer>
        {
            AltText = "text",
            QuickReply = new QuickReplyItemDto
            {
                Items = new List<QuickReplyButtonDto>
                {
                    new QuickReplyButtonDto
                    {
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Message,
                            Label = "查詢",
                            Text = "查詢",
                            InputOption = PostbackInputOptionEnum.OpenKeyboard,
                        }
                    },
                    new QuickReplyButtonDto
                    {
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Message,
                            Label = "刪除",
                            Text = "刪除",
                            InputOption = PostbackInputOptionEnum.OpenKeyboard,
                        }
                    }
                }
            },
            Contents = new FlexBubbleContainer
            {
                Type = FlexContainerTypeEnum.Bubble,
                Body = new FlexComponent
                {
                    Type = FlexComponentTypeEnum.Box,
                    Layout = FlexComponentLayoutTypeEnum.Vertical,
                    Contents = new List<FlexComponent>
                    {
                        new FlexComponent
                        {
                            Type = FlexComponentTypeEnum.Box,
                            Layout = FlexComponentLayoutTypeEnum.Vertical,
                            Contents = new List<FlexComponent>
                            {
                                new FlexComponent
                                {
                                    Type = FlexComponentTypeEnum.Text,
                                    Text = "依編號,存放位置,物品名稱,詳細位置,數量,購買日期(p),有效日期(e)排列"
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
                                        Label = "依過期日期排序",
                                        Text = "依過期日期排序"
                                    }
                                },
                            }
                        },
                        new FlexComponent
                        {
                            Type = FlexComponentTypeEnum.Box,
                            Layout = FlexComponentLayoutTypeEnum.Vertical,
                            Contents = new List<FlexComponent>
                            {
                                new FlexComponent
                                {
                                    Type = FlexComponentTypeEnum.Text,
                                    Text = "1 冰箱 蘋果 (p)2022-08-07 (e)2023-08-09"
                                },
                            }
                        },
                        new FlexComponent
                        {
                            Type = FlexComponentTypeEnum.Box,
                            Layout = FlexComponentLayoutTypeEnum.Vertical,
                            Contents = new List<FlexComponent>
                            {
                                new FlexComponent
                                {
                                    Type = FlexComponentTypeEnum.Text,
                                    Text = "2 冰箱 鳳梨 (p)2022-08-07 (e)2023-08-09"
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
                    }
                }
            }
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
