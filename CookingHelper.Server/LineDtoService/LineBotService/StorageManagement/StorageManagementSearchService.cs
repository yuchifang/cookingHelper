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
        IQueryable<StorageInfo> SearchResultDataList = Enumerable
            .Empty<StorageInfo>()
            .AsQueryable();

        string WebHookEventMessage = WebHookEventDto.Message!.Text!;

        if (WebHookEventMessage == "取消查詢")
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
        else
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
            IQueryable<StoreItem>? SearchedStoreItemList =
                await _storageManagementDatabaseService.GetSearchedStorageList(
                    UserTypeStorageInfo,
                    WebHookEventDto.Source.UserId
                );

            if (SearchedStoreItemList.ToList().Count == 0)
            {
                _ReplyMessageListStatic = new List<object>
                {
                    new TextMessageObject { Text = "找不到此物品" }
                };
                return;
            }
            var SearchedStorageInfoList = SearchedStoreItemList.Select(x => (StorageInfo)x);
            _memoryCache.Set("Storage", SearchedStorageInfoList);
            SearchResultDataList = SearchedStorageInfoList;
        }
        // 初始搜尋上面code會對 XXX付值, 不是則是從Cache拿值
        var MethodGroup = StorageSearchBaseStruct.Instance;
        if (SearchResultDataList.Count() == 0)
        {
            if (_memoryCache.TryGetValue("Storage", out IQueryable<StorageInfo> SearchedList))
            {
                SearchResultDataList = SearchedList;
            }
        }
        if (SearchResultDataList != null && SearchResultDataList.Any())
        {
            _ReplyMessageListStatic = MethodGroup.GetSearchResultUIBlock(
                SearchResultDataList,
                _PageIndexStatic,
                _PageSizeStatic
            );
        }

        //! 修改,刪除, 返回
        //! Service 不能繼承
        // 這裡 StorageBase Class and StorageManageService


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
}
