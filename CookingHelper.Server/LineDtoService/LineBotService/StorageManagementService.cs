using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using static CookingHelper.LineDto.BaseMessageObject;

namespace CookingHelper.LineDtoService;

public class StorageManagementService
{
    private readonly StorageManagementDatabaseService _storageManagementDatabaseService;

    public StorageManagementService(
        StorageManagementDatabaseService StorageManagementDatabaseService
    )
    {
        _storageManagementDatabaseService = StorageManagementDatabaseService;
    }

    public async Task Init(WebhookEventDto WebHookEventDto)
    {
        var ReplyMessageList = new List<TextMessageObject>();

        var StoreList = await _storageManagementDatabaseService.GetStoreListData(
            WebHookEventDto!.Source!.UserId!
        );
        // 創建 一個 Purchase
        if (StoreList.StoreItemList.Count == 0)
        {
            ReplyMessageList.Add(
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
                                    Data = "quick reply postback action",
                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                }
                            }
                        }
                    }
                }
            );
        }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<TextMessageObject>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = ReplyMessageList
        };
    }
}
