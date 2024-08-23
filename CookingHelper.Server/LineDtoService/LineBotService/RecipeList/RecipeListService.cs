using CookingHelper.Data;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using static CookingHelper.LineDto.BaseMessageObject;

public class RecipeListService
{
    public static dynamic _ReplyMessageListStatic = new List<object>();
    public RecipeListDatabaseService _recipeListDatabaseService;

    public RecipeListService(RecipeListDatabaseService RecipeListDatabaseService)
    {
        _recipeListDatabaseService = RecipeListDatabaseService;
    }

    public async Task GetRecipeList(WebhookEventDto WebHookEventDto)
    {
        var WebHookEventMessage = WebHookEventDto.Message!.Text!;
        var UserList = await _recipeListDatabaseService.GetRecipeList(
            WebHookEventDto.Source!.UserId!
        );
        if (UserList.RecipeList.Count() == 0)
        {
            _ReplyMessageListStatic = new List<object>
            {
                new TextMessageObject
                {
                    Text = "清單中沒有食譜, 點擊按鈕, 輸入想要紀錄的食譜",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Postback,
                                    Label = "新增食譜",
                                    Text = "新增食譜",
                                    Data = "新增食譜",
                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                }
                            }
                        }
                    }
                }
            };
        }
        else { }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }
}
