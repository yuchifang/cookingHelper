using System.Net.Http.Headers;
using System.Text;
using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.ProviderGroup;

namespace CookingHelper.LineDtoService;

public class LineBotService
{
    private readonly ShoppingListDatabaseService _shoppingListDatabaseService;

    private readonly ShoppingListService _shoppingListService;

    private readonly StorageManagementService _storageManagementService;

    private readonly StorageManagementDatabaseService _storageManagementDatabaseService;

    private readonly StorageManagementAdditionService _storageManagementPurchaseService;

    private readonly StorageManagementSearchService _storageManagementSearchService;

    private readonly RecipeListService _recipeListService;
    private readonly RecipeListAdditionService _recipeListAdditionService;
    private readonly RecipeListSearchService _recipeListSearchService;

    private readonly HttpClient _client;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    private readonly JsonProvider _jsonProvider = new JsonProvider();

    private readonly string replyMessageUri = "https://api.line.me/v2/bot/message/reply";

    public static string _WebhookEventStatusStatic = "";

    public static dynamic? _ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>();

    public LineBotService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ShoppingListDatabaseService UserListDatabaseService,
        ShoppingListService ShoppingListLogicService,
        StorageManagementService StorageManagementService,
        StorageManagementDatabaseService StorageManagementDatabaseService,
        StorageManagementAdditionService StorageManagementPurchaseService,
        StorageManagementSearchService StorageManagementSearchService,
        RecipeListService RecipeListService,
        RecipeListAdditionService RecipeListAdditionService,
        RecipeListSearchService RecipeListSearchService
    )
    {
        _httpClientFactory = httpClientFactory;
        _client = _httpClientFactory.CreateClient();
        _configuration = configuration;
        _shoppingListDatabaseService = UserListDatabaseService;
        _shoppingListService = ShoppingListLogicService;
        _storageManagementService = StorageManagementService;
        _storageManagementDatabaseService = StorageManagementDatabaseService;
        _storageManagementPurchaseService = StorageManagementPurchaseService;
        _storageManagementSearchService = StorageManagementSearchService;
        _recipeListService = RecipeListService;
        _recipeListAdditionService = RecipeListAdditionService;
        _recipeListSearchService = RecipeListSearchService;
    }

    public async Task ReceiveWebhook(WebhookRequestBodyDto WebHookRequestBody)
    {
        foreach (WebhookEventDto WebHookEventDto in WebHookRequestBody.Events)
        {
            switch (WebHookEventDto.Type)
            {
                case WebhookEventTypeEnum.Message:
                    await ReceiveMessageWebhookEvent(WebHookEventDto);
                    break;
                case WebhookEventTypeEnum.Postback:
                    await ReceivePostbackWebhookEvent(WebHookEventDto);

                    break;
                case WebhookEventTypeEnum.Follow:
                    Console.WriteLine($"使用者{WebHookEventDto.Source!.UserId}將我們新增為好友！");
                    await _shoppingListDatabaseService.AddEmptyShoppingListText(
                        WebHookEventDto.Source!.UserId!
                    );

                    break;
            }
        }
    }

    private async Task ReceivePostbackWebhookEvent(WebhookEventDto WebHookEventDto)
    {
        if (WebHookEventDto.Postback!.Data == KeywordGroup.StorageManagement)
        {
            await _storageManagementService.GetStorage(WebHookEventDto);
        }
        else if (
            _WebhookEventStatusStatic == KeywordGroup.StorageManagementAdded
            && WebHookEventDto.Postback!.Data == "修改"
        )
        {
            _storageManagementPurchaseService.EditAddedStorageHintPostBack(WebHookEventDto);
        }
        else if (WebHookEventDto.Postback!.Data == KeywordGroup.StorageManagementSearch)
        {
            _storageManagementSearchService.InitSearchStorageHintPostBack(WebHookEventDto);
        }
        else if (_WebhookEventStatusStatic == KeywordGroup.StorageManagementSearch)
        {
            if (WebHookEventDto.Postback.Data![0..1] == "c")
            {
                _storageManagementSearchService.DeleteStorageInfoConfirmPostBack(WebHookEventDto);
            }
            else if (WebHookEventDto.Postback.Data[0..1] == "d")
            {
                await _storageManagementSearchService.DeleteStoragePostBack(WebHookEventDto);
            }
            else if (WebHookEventDto.Postback.Data[0..1] == "e")
            {
                _storageManagementSearchService.EditStorageInfoPostBack(WebHookEventDto);
            }
        }
        else if (
            WebHookEventDto.Postback!.Data == KeywordGroup.RecipeList
            || _WebhookEventStatusStatic == KeywordGroup.RecipeList
            || _WebhookEventStatusStatic == KeywordGroup.RecipeListSearch
        )
        {
            if (WebHookEventDto.Postback!.Data![0..1] == "d")
            {
                await _recipeListService.DeleteRecipePostBack(WebHookEventDto);
            }
            else
            {
                await _recipeListService.GetRecipeList(WebHookEventDto);
            }
        }
        else if (WebHookEventDto.Postback!.Data == KeywordGroup.PurchaseList)
        {
            await _shoppingListService.Init(WebHookEventDto);
        }

        if (_ReplyMessageRequestStatic != null)
        {
            await ReplyMessageHandler("text", _ReplyMessageRequestStatic);
        }
    }

    private async Task ReceiveMessageWebhookEvent(WebhookEventDto WebHookEventDto)
    {
        var WebhookEventMessageType = WebHookEventDto.Message!.Type;
        switch (WebhookEventMessageType)
        {
            case "text":
                var WebHookEventMessage = WebHookEventDto.Message!.Text!;
                if (
                    WebHookEventMessage == KeywordGroup.RecipeList
                    || WebHookEventMessage == KeywordGroup.StorageManagement
                    || WebHookEventMessage == KeywordGroup.PurchaseList
                )
                {
                    if (RecipeListAdditionService._InputRecipeInfoStatic.ImagePath != null)
                    {
                        string filePath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            RecipeListAdditionService._InputRecipeInfoStatic.ImagePath
                        );
                        File.Delete(filePath);
                    }
                    else
                    {
                        _ReplyMessageRequestStatic = new ReplyMessageRequestDto<TextMessageObject>
                        {
                            ReplyToken = WebHookEventDto.ReplyToken!,
                            Messages = new List<TextMessageObject>
                            {
                                new TextMessageObject { Text = "無法記錄此字串, 請重新輸入", }
                            }
                        };
                    }
                }
                else if (_WebhookEventStatusStatic == KeywordGroup.InputPurchaseList)
                {
                    _WebhookEventStatusStatic = KeywordGroup.InputPurchaseList;
                    await _shoppingListService.Init(WebHookEventDto);
                } //? Storage
                else if (
                    _WebhookEventStatusStatic == KeywordGroup.StorageManagementSearch
                    && WebHookEventMessage != KeywordGroup.StorageManagement
                )
                {
                    await _storageManagementSearchService.SearchStorage(WebHookEventDto);
                }
                else if (
                    WebHookEventMessage == KeywordGroup.StorageManagementAdded
                    || _WebhookEventStatusStatic == KeywordGroup.StorageManagementAdded
                )
                {
                    await _storageManagementPurchaseService.InputStorage(WebHookEventDto);
                }
                else if (_WebhookEventStatusStatic == KeywordGroup.StorageManagement)
                {
                    await _storageManagementService.GetStorage(WebHookEventDto);
                } //? Storage
                //? RecipeList
                else if (
                    WebHookEventMessage == KeywordGroup.RecipeListSearch
                    || _WebhookEventStatusStatic == KeywordGroup.RecipeListSearch
                )
                {
                    await _recipeListSearchService.SearchRecipe(WebHookEventDto);
                }
                else if (
                    WebHookEventMessage == KeywordGroup.RecipeListAddition
                    || _WebhookEventStatusStatic == KeywordGroup.RecipeListAddition
                )
                {
                    await _recipeListAdditionService.InputRecipeList(WebHookEventDto);
                }
                else if (_WebhookEventStatusStatic == KeywordGroup.RecipeList)
                {
                    await _recipeListService.GetRecipeList(WebHookEventDto);
                } //? RecipeList
                else
                {
                    _ReplyMessageRequestStatic = new ReplyMessageRequestDto<TextMessageObject>
                    {
                        ReplyToken = WebHookEventDto.ReplyToken!,
                        Messages = new List<TextMessageObject>
                        {
                            new TextMessageObject
                            {
                                Text = WebHookEventDto.Message.Text! + " 無效輸入, 請依步驟執行"
                            }
                        }
                    };
                }
                break;
            case "image":
                await _recipeListAdditionService.ImageContentStatusImageEvent(WebHookEventDto);
                break;
        }
        if (_ReplyMessageRequestStatic != null)
        {
            await ReplyMessageHandler("text", _ReplyMessageRequestStatic);
        }
    }

    public async Task ReplyMessageHandler<T>(
        string messageType,
        ReplyMessageRequestDto<T> requestBody
    )
    {
        await ReplyMessage(requestBody);
    }

    public async Task ReplyMessage<T>(ReplyMessageRequestDto<T> request)
    {
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            _configuration["LineBot:ChannelAccessToken"]
        );
        var json = _jsonProvider.Serialize(request);
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(replyMessageUri),
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _client.SendAsync(requestMessage);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
}

public class LineBotWebhookServiceReturnType<T>
{
    public string WebhookEventState { get; set; } = default!;
    public ReplyMessageRequestDto<T> replyMessageRequest = default!;
}
