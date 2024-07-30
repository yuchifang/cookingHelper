using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using CookingHelper.Model;
using Microsoft.Extensions.Caching.Memory;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

public class StorageManagementSearchService : StorageSearch
{
    private readonly StorageManagementService _storageManagementService;
    private readonly StorageManagementDatabaseService _storageManagementDatabaseService;

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
        string WebHookEventMessage = WebHookEventDto.Message!.Text!;
        var SearchStorageProcessor = new Dictionary<string, StorageSearch>
        {
            { "上一頁", new PrevPage() },
            { "下一頁", new NextPage() }
        };
        if (WebHookEventMessage == "取消查詢")
        {
            await _storageManagementService.GetStorage(WebHookEventDto);
            return;
        }
        else if (!SearchStorageProcessor.ContainsKey(WebHookEventMessage))
        {
            _memoryCache.Remove("Storage");

            StringToStorageInfo(
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
            var SearchedStoreItem = await _storageManagementDatabaseService.SearchStorageList(
                UserTypeStorageInfo,
                WebHookEventDto.Source.UserId
            );

            if (SearchedStoreItem.ToList().Count == 0)
            {
                _ReplyMessageListStatic = new List<object>
                {
                    new TextMessageObject { Text = "找不到此物品" }
                };
                return;
            }

            _memoryCache.Set("Storage", SearchedStoreItem);

            var SearchedStoreItemEnumerable = Paginate(
                SearchedStoreItem,
                _PageIndexStatic,
                _PageSizeStatic,
                out bool hasNextPage,
                out bool hasPrevPage
            );
            var SearchedStorageInfoItem = SearchedStoreItemEnumerable.Select(x => (StorageInfo)x);

            _ReplyMessageListStatic = GetSearchUIBlock(SearchedStorageInfoItem.ToList());

            if (hasNextPage && hasPrevPage)
            {
                ((FlexMessageObject<FlexCarouselContainer>)_ReplyMessageListStatic[0]).QuickReply =
                    new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            GetQuickReplyButton(ActionTypeEnum.Message, "下一頁", "下一頁"),
                            GetQuickReplyButton(ActionTypeEnum.Message, "上一頁", "上一頁"),
                        }
                    };
            }
            else if (hasNextPage)
            {
                ((FlexMessageObject<FlexCarouselContainer>)_ReplyMessageListStatic[0]).QuickReply =
                    new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            GetQuickReplyButton(ActionTypeEnum.Message, "下一頁", "下一頁"),
                        }
                    };
            }
            else if (hasPrevPage)
            {
                ((FlexMessageObject<FlexCarouselContainer>)_ReplyMessageListStatic[0]).QuickReply =
                    new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            GetQuickReplyButton(ActionTypeEnum.Message, "上一頁", "上一頁"),
                        }
                    };
            }
        }
        else
        {
            SearchStorageProcessor[WebHookEventMessage].Init(_memoryCache);
        }

        //? 整理 7/29 學到新的東西
        // 整理 comment
        // 整理 new Side project


        //產生 FlexMessage 供選擇
        //! 修改,刪除, 返回
        //! Service 不能繼承
        //! 下一頁有問題


        //?
        /*
            查詢功能 Flexmessage
            查到用 flex message 顯示
            取消查詢
            最下面加個 修改,刪除, 返回
        */
        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    public async Task SearchStoragePostBack(WebhookEventDto WebHookEventDto)
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
                    Text = "若要尋找物品名稱為蘋果, 請輸入物品名稱:蘋果. \n要填入多筆資訊, 請用/號隔開, 如物品名稱:蘋果/過期日期:20230809",
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

    class PrevPage : StorageSearch
    {
        public override void Init(IMemoryCache _memoryCache)
        {
            _PageIndexStatic -= 1;
            if (_memoryCache.TryGetValue("Storage", out IEnumerable<StorageInfo> SearchedList))
            {
                var SearchedStoreItemEnumerable = Paginate(
                    SearchedList!,
                    _PageIndexStatic,
                    _PageSizeStatic,
                    out bool hasNextPage,
                    out bool hasPrevPage
                );

                _ReplyMessageListStatic = GetSearchUIBlock(SearchedStoreItemEnumerable.ToList());

                if (hasNextPage && hasPrevPage)
                {
                    (
                        (FlexMessageObject<FlexCarouselContainer>)_ReplyMessageListStatic[0]
                    ).QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            GetQuickReplyButton(ActionTypeEnum.Message, "下一頁", "下一頁"),
                            GetQuickReplyButton(ActionTypeEnum.Message, "上一頁", "上一頁"),
                        }
                    };
                }
                else if (hasNextPage)
                {
                    (
                        (FlexMessageObject<FlexCarouselContainer>)_ReplyMessageListStatic[0]
                    ).QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            GetQuickReplyButton(ActionTypeEnum.Message, "下一頁", "下一頁"),
                        }
                    };
                }
                else if (hasPrevPage)
                {
                    (
                        (FlexMessageObject<FlexCarouselContainer>)_ReplyMessageListStatic[0]
                    ).QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            GetQuickReplyButton(ActionTypeEnum.Message, "上一頁", "上一頁"),
                        }
                    };
                }
            }
        }
    }

    class NextPage : StorageSearch
    {
        public override void Init(IMemoryCache _memoryCache)
        {
            _PageIndexStatic += 1;
            if (_memoryCache.TryGetValue("Storage", out IEnumerable<StorageInfo> SearchedList))
            {
                var SearchedStoreItemEnumerable = Paginate(
                    SearchedList,
                    _PageIndexStatic,
                    _PageSizeStatic,
                    out bool hasNextPage,
                    out bool hasPrevPage
                );

                _ReplyMessageListStatic = GetSearchUIBlock(SearchedStoreItemEnumerable.ToList());

                if (hasNextPage && hasPrevPage)
                {
                    (
                        (FlexMessageObject<FlexCarouselContainer>)_ReplyMessageListStatic[0]
                    ).QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            GetQuickReplyButton(ActionTypeEnum.Message, "下一頁", "下一頁"),
                            GetQuickReplyButton(ActionTypeEnum.Message, "上一頁", "上一頁"),
                        }
                    };
                }
                else if (hasNextPage)
                {
                    (
                        (FlexMessageObject<FlexCarouselContainer>)_ReplyMessageListStatic[0]
                    ).QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            GetQuickReplyButton(ActionTypeEnum.Message, "下一頁", "下一頁"),
                        }
                    };
                }
                else if (hasPrevPage)
                {
                    (
                        (FlexMessageObject<FlexCarouselContainer>)_ReplyMessageListStatic[0]
                    ).QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            GetQuickReplyButton(ActionTypeEnum.Message, "上一頁", "上一頁"),
                        }
                    };
                }
            }
        }
    }
}
/*
    class SearchUserTypeText : StorageSearch
    {
        private readonly StorageManagementDatabaseService _storageManagementDatabaseService;
        private readonly StorageManagementService _storageManagementService;
        private readonly IMemoryCache _memoryCache;
        private readonly WebhookEventDto _webhookEventDto;

        public SearchUserTypeText(
            StorageManagementDatabaseService StorageManagementDatabaseService,
            StorageManagementService StorageManagementService,
            IMemoryCache MemoryCache,
            WebhookEventDto WebHookEventDto
        )
        {
            _storageManagementDatabaseService = StorageManagementDatabaseService;
            _storageManagementService = StorageManagementService;
            _memoryCache = MemoryCache;
            _webhookEventDto = WebHookEventDto;
        }

        public async void Init()
        {
            _memoryCache.Remove("Storage");
            string WebHookEventMessage = _webhookEventDto.Message!.Text!;
            StringToStorageInfo(
                WebHookEventMessage,
                out StorageInfo UserTypeStorageInfo,
                out string InputErrorText
            );
            if (InputErrorText != "")
            {
                await _storageManagementService.GetStorage(_webhookEventDto);

                StorageManagementService._ReplyMessageListStatic.Insert(
                    0,
                    new TextMessageObject { Text = "發生錯誤: 此欄位出現問題 " + InputErrorText }
                );

                return;
            }
            var SearchedStoreItem = await _storageManagementDatabaseService.SearchStorageList(
                UserTypeStorageInfo,
                _webhookEventDto.Source.UserId
            );

            if (SearchedStoreItem.ToList().Count == 0)
            {
                _ReplyMessageListStatic = new List<object>
                {
                    new TextMessageObject { Text = "找不到此物品" }
                };
                return;
            }
            //
            _memoryCache.Set("Storage", SearchedStoreItem);

            var SearchedStoreItemEnumerable = Paginate(
                SearchedStoreItem,
                _PageIndexStatic,
                _PageSizeStatic,
                out bool hasNextPage,
                out bool hasPrevPage
            );
            var SearchedStorageInfoItem = SearchedStoreItemEnumerable.Select(x => (StorageInfo)x);

            _ReplyMessageListStatic = GetSearchUIBlock(SearchedStorageInfoItem.ToList());

            if (hasNextPage && hasPrevPage)
            {
                ((FlexMessageObject<FlexCarouselContainer>)_ReplyMessageListStatic[0]).QuickReply =
                    new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            GetQuickReplyButton(ActionTypeEnum.Message, "下一頁", "下一頁"),
                            GetQuickReplyButton(ActionTypeEnum.Message, "上一頁", "上一頁"),
                        }
                    };
            }
            else if (hasNextPage)
            {
                ((FlexMessageObject<FlexCarouselContainer>)_ReplyMessageListStatic[0]).QuickReply =
                    new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            GetQuickReplyButton(ActionTypeEnum.Message, "下一頁", "下一頁"),
                        }
                    };
            }
            else if (hasPrevPage)
            {
                ((FlexMessageObject<FlexCarouselContainer>)_ReplyMessageListStatic[0]).QuickReply =
                    new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            GetQuickReplyButton(ActionTypeEnum.Message, "上一頁", "上一頁"),
                        }
                    };
            }
        }
    }
}
*/
