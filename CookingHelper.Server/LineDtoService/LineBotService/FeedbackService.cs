using CookingHelper.Enum;
using CookingHelper.LineDto;
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
        if (WebHookEventDto.Message!.Text == KeywordGroup.Feedback)
        {
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
        }
        if (_WebhookEventState == KeywordGroup.InputFeedback)
        {
            // 改 rich menu 主目錄按鈕文字 //?這個
            // 改 cooking helper 聊天室底色
            // 在 github 加 mit //?這個
            // 信用卡繳費 //?這個

            //? 註冊時 建立資料庫
            // 1. 更新資料庫 找筆記 找 mic
            // 2.FeedbackGroupId 要怎麼產生
            // 測試
            //? 新增 在對應的類別作輸入
            //? 記住 選哪個類別 試試 postback action?? //?這個
        }

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
