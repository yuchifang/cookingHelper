using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using CookingHelper.Model;
using static CookingHelper.LineDto.BaseMessageObject;

public class RecipeListAddition
{
    private static dynamic _ReplyMessageListStatic = new List<object>();
    public static InputRecipeInfo _InputRecipeInfoStatic = new InputRecipeInfo();

    private readonly RecipeListService _recipeListService;

    public RecipeListAddition(RecipeListService RecipeListService)
    {
        _recipeListService = RecipeListService;
    }

    //! InputStorageBaseClass 名稱要改一下
    public async Task InputRecipeList(WebhookEventDto WebHookEventDto)
    {
        string? WebHookEventMessage = WebHookEventDto.Message!.Text;
        if (WebHookEventMessage == "取消新增")
        {
            _InputRecipeInfoStatic = new InputRecipeInfo();
            await _recipeListService.GetRecipeList(WebHookEventDto);
            return;
        }
        var StatusProcessor = new Dictionary<string, Action>
        {
            { "Init", InitStatus },
            { "Name", () => NameStatus(WebHookEventMessage!) }
        };
        StatusProcessor[_InputRecipeInfoStatic.Status]();

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    public void InitStatus()
    {
        var StorageStatus = InputStorageBaseClass.Instance;
        LineBotService._WebhookEventStatusStatic = KeywordGroup.RecipeListAddition;
        _ReplyMessageListStatic = new List<object>(
            [
                new TextMessageObject { Text = "依食譜名稱,圖片,食材,步驟輸入", },
                new TextMessageObject { Text = "食譜名稱一定要填入, 沒填入將無法紀錄", },
                new TextMessageObject
                {
                    Text = "請輸入食譜名稱:",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            StorageStatus.GetQuickReplyButton(
                                ActionTypeEnum.Message,
                                "取消新增",
                                "取消新增"
                            )
                        }
                    }
                }
            ]
        );
        _InputRecipeInfoStatic.Status = "Name";
    }

    public void NameStatus(string WebHookEventMessage)
    {
        var StorageStatus = InputStorageBaseClass.Instance;
        _InputRecipeInfoStatic.Name = WebHookEventMessage;
        _ReplyMessageListStatic = StorageStatus.GetRegularReply("請輸入圖片:");
        _InputRecipeInfoStatic.Status = "ImageContent";
    }

    //! 建立 Image webhook

    //! update Database
    //! 使用下面的
    //! 限制大小長寬寬度??
    //! 轉 byte[] 存 _InputRecipeInfoStatic
    public async Task ImageContentStatus(WebhookEventDto WebHookEventDto)
    {
        Console.WriteLine(WebHookEventDto);
    }
}

public class InputRecipeInfo : RecipeItem
{
    private string _Status = "Init";
    public string Status
    {
        get => _Status;
        set
        {
            if (value != "Init" && value != "Name" && value != "ImageContent" && value != "Step")
            {
                throw new ArgumentException("Value Error");
            }
            _Status = value;
        }
    }
}
