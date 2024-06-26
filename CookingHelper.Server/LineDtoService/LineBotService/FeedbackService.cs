using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using static CookingHelper.LineDto.BaseMessageEventObject;

namespace CookingHelper.LineDtoService;

public class FeedbackLogicService
{
    public FeedbackLogicService() { }

    public async Task<LineBotWebhookServiceReturnType<TextMessageEventObject>> Init(
        WebhookEventDto WebHookEventDto,
        string _WebhookEventState
    )
    {
        var ReplyMessageList = new List<TextMessageEventObject>();
        ReplyMessageList.Add(
            new TextMessageEventObject
            {
                Text = "反饋分類",
                QuickReply = new QuickReplyItemDto
                {
                    Items = new List<QuickReplyButtonDto>
                    {
                        new QuickReplyButtonDto
                        {
                            Action = new ActionDto
                            {
                                Type = ActionTypeEnum.Postback,
                                Label = "問題反饋",
                                Data = "quick reply postback action",
                                InputOption = PostbackInputOptionEnum.OpenKeyboard,
                            }
                        },
                        new QuickReplyButtonDto
                        {
                            Action = new ActionDto
                            {
                                Type = ActionTypeEnum.Postback,
                                Label = "系統建議",
                                Data = "quick reply postback action",
                                InputOption = PostbackInputOptionEnum.OpenKeyboard,
                            }
                        },
                    }
                }
            }
        );

        _WebhookEventState = KeywordGroup.InputFeedback;
        return new LineBotWebhookServiceReturnType<TextMessageEventObject>
        {
            WebhookEventState = _WebhookEventState,
            replyMessageRequest = new ReplyMessageRequestDto<TextMessageEventObject>
            {
                ReplyToken = WebHookEventDto.ReplyToken!,
                Messages = ReplyMessageList
            },
        };
    }
}
