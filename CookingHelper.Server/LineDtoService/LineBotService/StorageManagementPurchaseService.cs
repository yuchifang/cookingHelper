using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.LineDto.BaseMessageObject;

namespace CookingHelper.LineDtoService;

//! 寫完要更新 Database migrations
public class StorageManagementPurchaseService
{
    public StorageManagementPurchaseService() { }

    public async Task InputStorage(WebhookEventDto WebHookEventDto)
    {
        var ReplyMessageList = new List<TextMessageObject>();
        LineBotService._WebhookEventStateStatic = "新增物品至庫存";
        //? 寫一個紀錄 Storage 的狀態的state ?? 測試 不同呼叫 內層的static 會部會改變


        ReplyMessageList.AddRange(
            [
                new TextMessageObject { Text = "儲存位置及物品名稱一定要填入, 沒填入將無法紀錄", },
                new TextMessageObject
                {
                    Text = "請輸入儲存位置:",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "略過",
                                    Text = "略過",
                                }
                            },
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "完成",
                                    Text = "完成",
                                }
                            }
                        }
                    }
                }
            ]
        );

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<TextMessageObject>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = ReplyMessageList
        };
    }
}
