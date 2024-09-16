using System.Net.Http.Headers;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using CookingHelper.Model;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

public class RecipeListAdditionService
{
    private static dynamic _ReplyMessageListStatic = new List<object>();
    public static InputRecipeInfo _InputRecipeInfoStatic = new InputRecipeInfo();

    private readonly RecipeListService _recipeListService;

    private readonly RecipeListDatabaseService _recipeListDatabaseService;

    private readonly HttpClient _client;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    private readonly string getContentUri = "https://api-data.line.me/v2/bot/message/{0}/content";

    public RecipeListAdditionService(
        RecipeListService RecipeListService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        RecipeListDatabaseService RecipeListDatabaseService
    )
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _client = _httpClientFactory.CreateClient();
        _recipeListService = RecipeListService;
        _recipeListDatabaseService = RecipeListDatabaseService;
    }

    public async Task InputRecipeList(WebhookEventDto WebHookEventDto)
    {
        string? WebHookEventMessage = WebHookEventDto.Message!.Text;
        if (WebHookEventMessage == "新增食譜")
        {
            _InputRecipeInfoStatic = new InputRecipeInfo();
        }
        else if (WebHookEventMessage == "取消新增")
        {
            LineBotService._WebhookEventStatusStatic = KeywordGroup.RecipeList;
            if (_InputRecipeInfoStatic.ImagePath != null)
            {
                string filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    _InputRecipeInfoStatic.ImagePath
                );
                File.Delete(filePath);
            }
            _InputRecipeInfoStatic = new InputRecipeInfo();

            await _recipeListService.GetRecipeList(WebHookEventDto);
            return;
        }
        else if (WebHookEventMessage == "略過")
        {
            WebHookEventMessage = null;
        }
        else if (WebHookEventMessage == "重新填寫步驟")
        {
            _InputRecipeInfoStatic.Step = new List<string>();
        }
        else if (WebHookEventMessage == "填寫完成")
        {
            LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
            {
                ReplyToken = WebHookEventDto.ReplyToken!,
                Messages = RecipeListAdditionBaseClass.Instance.GetRecipeAdditionConfirmHint(
                    _InputRecipeInfoStatic
                )
            };
            return;
        }
        else if (WebHookEventMessage == "新增")
        {
            LineBotService._WebhookEventStatusStatic = KeywordGroup.RecipeList;
            await _recipeListDatabaseService.AddRecipe(
                _InputRecipeInfoStatic,
                WebHookEventDto.Source!.UserId!
            );
            _InputRecipeInfoStatic = new InputRecipeInfo();
            await _recipeListService.GetRecipeList(WebHookEventDto);
            RecipeListService._ReplyMessageListStatic.Insert(
                0,
                new TextMessageObject { Text = "新增完成" }
            );
            return;
        }

        var StatusProcessor = new Dictionary<string, Action>
        {
            { "Init", InitStatus },
            { "Name", () => NameStatus(WebHookEventMessage!) },
            { "ImageContent", () => InputImageExceptionHandle(WebHookEventMessage!) },
            { "Ingredients", () => IngredientsStatus(WebHookEventMessage!) },
            { "Step", () => StepStatus(WebHookEventMessage!) },
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
        LineBotService._WebhookEventStatusStatic = KeywordGroup.RecipeListAddition;
        var RecipeMethodGroup = RecipeListAdditionBaseClass.Instance;
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
                            RecipeMethodGroup.GetQuickReplyButton(
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
        var RecipeMethodGroup = RecipeListAdditionBaseClass.Instance;
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
                            RecipeMethodGroup.GetQuickReplyButton(
                                ActionTypeEnum.Message,
                                "取消新增",
                                "取消新增"
                            ),
                            RecipeMethodGroup.GetQuickReplyButton(
                                ActionTypeEnum.Message,
                                "略過",
                                "略過"
                            )
                        }
                    }
                }
            ]
        );
        _InputRecipeInfoStatic.Status = "ImageContent";
    }

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

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var imageBytes = await response.Content.ReadAsByteArrayAsync();
        var RecipeMethodGroup = RecipeListAdditionBaseClass.Instance;
        if (imageBytes.Length > 4 * 1024 * 1024)
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
                                RecipeMethodGroup.GetQuickReplyButton(
                                    ActionTypeEnum.Message,
                                    "取消新增",
                                    "取消新增"
                                ),
                                RecipeMethodGroup.GetQuickReplyButton(
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
            var imageFileName = Guid.NewGuid();
            var imagePath = $"UploadFile/RecipeImage/{imageFileName}.png";
            ConvertBytesToPng(imageBytes, imagePath);

            _InputRecipeInfoStatic.ImagePath = imagePath;
            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject
                    {
                        Text = "請輸入食材:",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                RecipeMethodGroup.GetQuickReplyButton(
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

    public void InputImageExceptionHandle(string WebHookEventMessage)
    {
        var RecipeMethodGroup = RecipeListAdditionBaseClass.Instance;
        if (WebHookEventMessage == null)
        {
            // 選擇略過的情況
            _InputRecipeInfoStatic.ImagePath = null;

            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject
                    {
                        Text = "請輸入食材:",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                RecipeMethodGroup.GetQuickReplyButton(
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
        else
        {
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
                                RecipeMethodGroup.GetQuickReplyButton(
                                    ActionTypeEnum.Message,
                                    "取消新增",
                                    "取消新增"
                                ),
                                RecipeMethodGroup.GetQuickReplyButton(
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
    }

    public void IngredientsStatus(string WebHookEventMessage)
    {
        var RecipeMethodGroup = RecipeListAdditionBaseClass.Instance;
        _InputRecipeInfoStatic.Ingredients = WebHookEventMessage;
        _ReplyMessageListStatic = new List<object>(
            [
                new TextMessageObject
                {
                    Text = "請輸入步驟 限制20個",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            RecipeMethodGroup.GetQuickReplyButton(
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

    public void StepStatus(string WebHookEventMessage)
    {
        var RecipeMethodGroup = RecipeListAdditionBaseClass.Instance;
        if (_InputRecipeInfoStatic.Step.Count == 20)
        {
            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject
                    {
                        Text = "已達到步驟次數上限",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                RecipeMethodGroup.GetQuickReplyButton(
                                    ActionTypeEnum.Message,
                                    "填寫完成",
                                    "填寫完成"
                                ),
                                RecipeMethodGroup.GetQuickReplyButton(
                                    ActionTypeEnum.Message,
                                    "重新填寫步驟",
                                    "重新填寫步驟"
                                ),
                            }
                        }
                    }
                ]
            );
        }
        else
        {
            _InputRecipeInfoStatic.Step.Add(WebHookEventMessage);
            _ReplyMessageListStatic = new List<object>(
                [
                    new TextMessageObject
                    {
                        Text = "請輸入步驟",
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                RecipeMethodGroup.GetQuickReplyButton(
                                    ActionTypeEnum.Message,
                                    "取消新增",
                                    "取消新增"
                                ),
                                RecipeMethodGroup.GetQuickReplyButton(
                                    ActionTypeEnum.Message,
                                    "填寫完成",
                                    "填寫完成"
                                )
                            }
                        }
                    }
                ]
            );
        }
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
                && value != "Edit"
            )
            {
                throw new ArgumentException("Value Error");
            }
            _Status = value;
        }
    }
}
