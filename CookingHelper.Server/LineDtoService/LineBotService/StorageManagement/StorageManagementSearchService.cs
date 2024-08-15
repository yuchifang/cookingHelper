using System.Text.Json;
using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using CookingHelper.Model;
using Microsoft.Extensions.Caching.Memory;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

public class StorageManagementSearchService
{
    private readonly StorageManagementService _storageManagementService;
    private readonly StorageManagementDatabaseService _storageManagementDatabaseService;

    private static SearchStorageEditInfo _StorageEditInfoStatic = new SearchStorageEditInfo();

    private static int _PageIndexStatic = 1;
    private static int _PageSizeStatic = 12;

    private static List<object> _ReplyMessageListStatic = new List<object>();
    private readonly IMemoryCache _memoryCache;

    public StorageManagementSearchService(
        StorageManagementService StorageManagementService,
        StorageManagementDatabaseService StorageManagementDatabaseService,
        IMemoryCache memoryCache
    )
    {
        _memoryCache = memoryCache;
        _storageManagementService = StorageManagementService;
        _storageManagementDatabaseService = StorageManagementDatabaseService;
    }

    public async Task SearchStorage(WebhookEventDto WebHookEventDto)
    {
        IQueryable<StoreItem> SearchResult = Enumerable.Empty<StoreItem>().AsQueryable();
        var MethodGroup = StorageSearchBaseClass.Instance;
        string WebHookEventMessage = WebHookEventDto.Message!.Text!;

        if (WebHookEventMessage == "取消查詢" || WebHookEventMessage == "返回")
        {
            await _storageManagementService.GetStorage(WebHookEventDto);
            return;
        }
        else if (WebHookEventMessage == "上一頁")
        {
            _PageIndexStatic -= 1;
        }
        else if (WebHookEventMessage == "下一頁")
        {
            _PageIndexStatic += 1;
        }
        else if (WebHookEventMessage == "取消修改" || WebHookEventMessage == "取消")
        {
            _StorageEditInfoStatic.Status = "search";
            _StorageEditInfoStatic = new SearchStorageEditInfo();
        }
        else if (WebHookEventMessage == "更新")
        {
            _StorageEditInfoStatic.Status = "search";
            await _storageManagementDatabaseService.UpdateStoreItem(
                _StorageEditInfoStatic,
                WebHookEventDto.Source!.UserId!
            );
            _StorageEditInfoStatic = new SearchStorageEditInfo();
            await _storageManagementService.GetStorage(WebHookEventDto);
            return;
        }
        else if (WebHookEventMessage == "返回查詢結果") { }
        // 處理使用者查詢及修改
        else
        {
            // 依使用者輸入
            StringSlashAndColonToStorageInfo(
                WebHookEventMessage,
                out StorageInfo UserTypeStorageInfo,
                out string InputErrorText
            );
            if (InputErrorText != "")
            {
                await _storageManagementService.GetStorage(WebHookEventDto);

                StorageManagementService._ReplyMessageListStatic.Insert(
                    0,
                    new TextMessageObject { Text = "發生錯誤: 此欄位出現問題 " + InputErrorText }
                );

                return;
            }

            // 修改
            if (_StorageEditInfoStatic.Status == "edit")
            {
                await GetSearchStorageConfirmHint(WebHookEventDto, UserTypeStorageInfo);
                return;
            }
            // 查詢
            _memoryCache.Remove("StorageSearch");
            IQueryable<StoreItem>? SearchedStoreItem =
                await _storageManagementDatabaseService.GetSearchedStoreItem(
                    UserTypeStorageInfo,
                    WebHookEventDto.Source.UserId
                );

            if (SearchedStoreItem.ToList().Count == 0)
            {
                LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
                {
                    ReplyToken = WebHookEventDto.ReplyToken!,
                    Messages = new List<object> { new TextMessageObject { Text = "找不到此物品" } }
                };

                return;
            }

            _memoryCache.Set("StorageSearch", SearchedStoreItem);
            SearchResult = SearchedStoreItem;
        }

        // 初始查詢上面code會對 SearchResultDataList 付值, 不是則是從Cache拿值
        if (SearchResult.Count() == 0)
        {
            if (_memoryCache.TryGetValue("StorageSearch", out IQueryable<StoreItem> SearchedCache))
            {
                SearchResult = SearchedCache;
            }
        }
        if (SearchResult != null && SearchResult.Any())
        {
            _ReplyMessageListStatic = MethodGroup.GetSearchResultUIBlock(
                SearchResult,
                _PageIndexStatic,
                _PageSizeStatic
            );
        }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    public async Task InitSearchStorageHintPostBack(WebhookEventDto WebHookEventDto)
    {
        LineBotService._WebhookEventStatusStatic = "庫存查詢";

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = new List<object>
            {
                new TextMessageObject { Text = "依格式輸入查詢資訊" },
                new TextMessageObject
                {
                    Text = "若要尋找物品名稱為蘋果, 請輸入物品名稱:蘋果. \n要填入多筆資訊, 請用/號隔開, 如物品名稱:蘋果/有效日期:20230809",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "取消查詢",
                                    Text = "取消查詢",
                                }
                            },
                        }
                    }
                }
            }
        };
        return;
    }

    public async Task DeleteStoragePostBack(WebhookEventDto WebHookEventDto)
    {
        var userId = WebHookEventDto.Source!.UserId!;
        var StoreItem = JsonSerializer.Deserialize<StoreItem>(WebHookEventDto.Postback!.Data![1..]);
        if (StoreItem != null)
        {
            await _storageManagementDatabaseService.DeleteStoreItem(StoreItem, userId);
        }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = new List<object>
            {
                new TextMessageObject
                {
                    Text = "刪除完成",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "返回",
                                    Text = "返回",
                                }
                            },
                        }
                    }
                }
            }
        };
    }

    public async Task DeleteStorageInfoConfirmPostBack(WebhookEventDto WebHookEventDto)
    {
        var MethodGroup = StorageSearchBaseClass.Instance;
        var StoreItem = JsonSerializer.Deserialize<StoreItem>(WebHookEventDto.Postback!.Data![1..]);
        var StorageInfoTable = MethodGroup.GetStorageInfoTable(StoreItem!);

        var StorageTable = new List<FlexComponent>
        {
            new FlexComponent { Type = FlexComponentTypeEnum.Text, Text = "確認刪除" },
            new FlexComponent { Type = FlexComponentTypeEnum.Separator, Margin = "xxl" },
            new FlexComponent
            {
                Type = FlexComponentTypeEnum.Box,
                Layout = FlexComponentLayoutTypeEnum.Vertical,
                Contents = new List<FlexComponent>
                {
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Button,
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Postback,
                            Label = "刪除",
                            DisplayText = "刪除",
                            Data = "d" + JsonSerializer.Serialize(StoreItem),
                        }
                    },
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Button,
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Message,
                            Label = "返回查詢結果",
                            Text = "返回查詢結果",
                        }
                    }
                }
            }
        };
        StorageTable.InsertRange(1, StorageInfoTable);

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = new List<object>
            {
                new FlexMessageObject<FlexBubbleContainer>
                {
                    AltText = "確認刪除",
                    Contents = new FlexBubbleContainer
                    {
                        Type = FlexContainerTypeEnum.Bubble,
                        Styles = new FlexBubbleContainerStyle
                        {
                            Footer = new FlexBlockStyle { Separator = false }
                        },
                        Body = new FlexComponent
                        {
                            Type = FlexComponentTypeEnum.Box,
                            Layout = FlexComponentLayoutTypeEnum.Vertical,

                            Contents = StorageTable
                        }
                    }
                },
            }
        };
    }

    public async Task EditStorageInfoPostBack(WebhookEventDto WebHookEventDto)
    {
        var StoreItem = JsonSerializer.Deserialize<StoreItem>(WebHookEventDto.Postback!.Data![1..]);
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
        _StorageEditInfoStatic.Status = "edit";
        if (StoreItem.Place != null)
            _StorageEditInfoStatic.Place = StoreItem.Place;
        if (StoreItem.Name != null)
            _StorageEditInfoStatic.Name = StoreItem.Name;
        if (StoreItem.Location != null)
            _StorageEditInfoStatic.Location = StoreItem.Location;
        if (StoreItem.Amount != null)
            _StorageEditInfoStatic.Amount = StoreItem.Amount;
        if (StoreItem.PurchaseDate != null)
            _StorageEditInfoStatic.PurchaseDate = StoreItem.PurchaseDate;
        if (StoreItem.ExpiryDate != null)
            _StorageEditInfoStatic.ExpiryDate = StoreItem.ExpiryDate;

        _StorageEditInfoStatic.StoreItemId = StoreItem.StoreItemId;

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    public async Task GetSearchStorageConfirmHint(
        WebhookEventDto WebHookEventDto,
        StorageInfo UserTypeStorageInfo
    )
    {
        var MethodGroup = StorageSearchBaseClass.Instance;
        if (UserTypeStorageInfo.Place != null)
            _StorageEditInfoStatic.Place = UserTypeStorageInfo.Place;
        if (UserTypeStorageInfo.Name != null)
            _StorageEditInfoStatic.Name = UserTypeStorageInfo.Name;
        if (UserTypeStorageInfo.Location != null)
            _StorageEditInfoStatic.Location = UserTypeStorageInfo.Location;
        if (UserTypeStorageInfo.Amount != null)
            _StorageEditInfoStatic.Amount = UserTypeStorageInfo.Amount;
        if (UserTypeStorageInfo.PurchaseDate != null)
            _StorageEditInfoStatic.PurchaseDate = UserTypeStorageInfo.PurchaseDate;
        if (UserTypeStorageInfo.ExpiryDate != null)
            _StorageEditInfoStatic.ExpiryDate = UserTypeStorageInfo.ExpiryDate;

        _ReplyMessageListStatic = MethodGroup.GetAdditionConfirmHint(
            _StorageEditInfoStatic,
            new FlexComponent
            {
                Type = FlexComponentTypeEnum.Box,
                Layout = FlexComponentLayoutTypeEnum.Vertical,
                Contents = new List<FlexComponent>
                {
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Button,
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Message,
                            Label = "更新",
                            Text = "更新"
                        }
                    },
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Button,
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Postback,
                            Label = "修改",
                            Data = "e" + JsonSerializer.Serialize(_StorageEditInfoStatic),
                            InputOption = PostbackInputOptionEnum.OpenKeyboard
                        }
                    },
                    new FlexComponent
                    {
                        Type = FlexComponentTypeEnum.Button,
                        Action = new ActionDto
                        {
                            Type = ActionTypeEnum.Message,
                            Label = "取消",
                            Text = "取消",
                        }
                    }
                }
            }
        );
        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
        return;
    }
}

public class SearchStorageEditInfo : StoreItem
{
    private string _Status = "search";
    public string Status
    {
        get => _Status;
        set
        {
            if (value != "search" && value != "edit")
            {
                throw new ArgumentException("Value Error");
            }
            _Status = value;
        }
    }
}
