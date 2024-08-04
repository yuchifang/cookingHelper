using CookingHelper.Enum;
using CookingHelper.LineDto;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

namespace CookingHelper.LineDtoService;

class InputStorageBaseClass : UIWithData
{
    public static InputStorageBaseClass Instance = new InputStorageBaseClass();

    public List<object> GetRegularReply(string replyText)
    {
        return new List<object>(
            [
                new TextMessageObject
                {
                    Text = replyText,
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            GetQuickReplyButton(ActionTypeEnum.Message, "填寫完成", "填寫完成"),
                            GetQuickReplyButton(ActionTypeEnum.Message, "略過", "略過"),
                            GetQuickReplyButton(ActionTypeEnum.Message, "取消新增", "取消新增")
                        }
                    }
                }
            ]
        );
    }
}
