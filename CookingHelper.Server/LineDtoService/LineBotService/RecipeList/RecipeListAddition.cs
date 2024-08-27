using System.Net.Http.Headers;
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

    private readonly HttpClient _client;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    private readonly string getContentUri = "https://api-data.line.me/v2/bot/message/{0}/content";

    public RecipeListAddition(
        RecipeListService RecipeListService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration
    )
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _client = _httpClientFactory.CreateClient();
        _recipeListService = RecipeListService;
    }

    //! InputStorageBaseClass 名稱要改一下
    public async Task InputRecipeList(WebhookEventDto WebHookEventDto)
    {
        string? WebHookEventMessage = WebHookEventDto.Message!.Text;
        if (WebHookEventMessage == "新增食譜")
        {
            _InputRecipeInfoStatic = new InputRecipeInfo();
        }

        if (WebHookEventMessage == "取消新增")
        {
            _InputRecipeInfoStatic = new InputRecipeInfo();
            await _recipeListService.GetRecipeList(WebHookEventDto);
            return;
        }
        var StatusProcessor = new Dictionary<string, Action>
        {
            { "Init", InitStatus },
            { "Name", () => NameStatus(WebHookEventMessage!) },
            { "ImageContent", InputImageError },
            { "Ingredients", () => IngredientsStatus(WebHookEventMessage!) }
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
                new TextMessageObject { Text = "食譜名稱,食材,步驟一定要填入, 沒填入將無法紀錄", },
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
        _ReplyMessageListStatic = new List<object>(
            [
                new TextMessageObject
                {
                    Text = "請輸入食譜圖片: (限制4MB)",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            StorageStatus.GetQuickReplyButton(
                                ActionTypeEnum.Message,
                                "取消新增",
                                "取消新增"
                            ),
                            StorageStatus.GetQuickReplyButton(ActionTypeEnum.Message, "略過", "略過")
                        }
                    }
                }
            ]
        );
        _InputRecipeInfoStatic.Status = "ImageContent";
    }

    //! update Database
    public async Task ImageContentStatusImageEvent(WebhookEventDto WebHookEventDto)
    {
        var messageId = WebHookEventDto.Message!.Id;

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            String.Format(getContentUri, messageId)
        );
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            _configuration["LineBot:ChannelAccessToken"]
        );

        var response = await _client.SendAsync(request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var image = await response.Content.ReadAsByteArrayAsync();
        var StorageStatus = InputStorageBaseClass.Instance;
        if (image.Length > 4 * 1024 * 1024)
        {
            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject { Text = "超過限制大小" },
                    new TextMessageObject
                    {
                        Text = "請輸入食譜圖片: (限制4MB)",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                StorageStatus.GetQuickReplyButton(
                                    ActionTypeEnum.Message,
                                    "取消新增",
                                    "取消新增"
                                ),
                                StorageStatus.GetQuickReplyButton(
                                    ActionTypeEnum.Message,
                                    "略過",
                                    "略過"
                                )
                            }
                        }
                    }
                ]
            );
        }
        else
        {
            _InputRecipeInfoStatic.ImageContent = image;
            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject
                    {
                        Text = "請輸入食材:",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                StorageStatus.GetQuickReplyButton(
                                    ActionTypeEnum.Message,
                                    "取消新增",
                                    "取消新增"
                                ),
                            }
                        }
                    }
                ]
            );
            _InputRecipeInfoStatic.Status = "Ingredients";
        }
        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    public void InputImageError()
    {
        var StorageStatus = InputStorageBaseClass.Instance;
        _ReplyMessageListStatic = new List<object>(
            [
                new TextMessageObject { Text = "輸入錯誤" },
                new TextMessageObject
                {
                    Text = "請輸入食譜圖片: (限制4MB)",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            StorageStatus.GetQuickReplyButton(
                                ActionTypeEnum.Message,
                                "取消新增",
                                "取消新增"
                            ),
                            StorageStatus.GetQuickReplyButton(ActionTypeEnum.Message, "略過", "略過")
                        }
                    }
                }
            ]
        );
    }

    public void IngredientsStatus(string WebHookEventMessage)
    {
        var StorageStatus = InputStorageBaseClass.Instance;
        _InputRecipeInfoStatic.Ingredients = WebHookEventMessage;
        _ReplyMessageListStatic = new List<object>(
            [
                new TextMessageObject
                {
                    Text = "請輸入步驟",
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
        _InputRecipeInfoStatic.Status = "Step";
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
            if (
                value != "Init"
                && value != "Name"
                && value != "ImageContent"
                && value != "Step"
                && value != "Ingredients"
            )
            {
                throw new ArgumentException("Value Error");
            }
            _Status = value;
        }
    }
}
