using System.Net.Http.Headers;
using System.Text;
using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.ProviderGroup;

namespace CookingHelper.LineDtoService;

public class LineBotService
{
    private readonly ShoppingListDatabaseService _userListDatabaseService;

    private readonly ShoppingListLogicService _shoppingListLogicService;

    private readonly StorageManagementService _storageManagementService;

    private readonly StorageManagementDatabaseService _storageManagementDatabaseService;

    private readonly StorageManagementPurchaseService _storageManagementPurchaseService;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _client;

    private readonly JsonProvider _jsonProvider = new JsonProvider();

    private readonly string replyMessageUri = "https://api.line.me/v2/bot/message/reply";

    private readonly IConfiguration _configuration;
    public static string _StaticWebhookEventState = "";

    public LineBotService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ShoppingListDatabaseService UserListDatabaseService,
        ShoppingListLogicService ShoppingListLogicService,
        StorageManagementService StorageManagementService,
        StorageManagementDatabaseService StorageManagementDatabaseService,
        StorageManagementPurchaseService StorageManagementPurchaseService
    )
    {
        _httpClientFactory = httpClientFactory;
        _client = _httpClientFactory.CreateClient();
        _configuration = configuration;
        _userListDatabaseService = UserListDatabaseService;
        _shoppingListLogicService = ShoppingListLogicService;
        _storageManagementService = StorageManagementService;
        _storageManagementDatabaseService = StorageManagementDatabaseService;
        _storageManagementPurchaseService = StorageManagementPurchaseService;
    }

    public async Task ReceiveWebhook(WebhookRequestBodyDto WebHookRequestBody)
    {
        foreach (WebhookEventDto WebHookEventDto in WebHookRequestBody.Events)
        {
            switch (WebHookEventDto.Type)
            {
                case WebhookEventTypeEnum.Message:
                    if (WebHookEventDto.Message.Type == MessageTypeEnum.Text)
                    {
                        await ReceiveMessageWebhookEvent(WebHookEventDto);
                    }
                    break;
                case WebhookEventTypeEnum.Follow:
                    Console.WriteLine($"使用者{WebHookEventDto.Source!.UserId}將我們新增為好友！");
                    await _userListDatabaseService.AddEmptyShoppingListText(
                        WebHookEventDto.Source!.UserId!
                    );
                    await _storageManagementDatabaseService.AddEmptyStorageData(
                        WebHookEventDto.Source!.UserId!
                    );
                    break;
            }
        }
    }

    private async Task ReceiveMessageWebhookEvent(WebhookEventDto WebHookEventDto)
    {
        dynamic replyMessageRequest = new ReplyMessageRequestDto<BaseMessageObject>();

        switch (WebHookEventDto.Message.Type)
        {
            case MessageTypeEnum.Text:
                if (WebHookEventDto.Message.Text == "返回目錄")
                {
                    _StaticWebhookEventState = "s";
                }
                if (
                    WebHookEventDto.Message.Text == KeywordGroup.PurchaseList
                    || _StaticWebhookEventState == KeywordGroup.InputPurchaseList
                )
                {
                    _StaticWebhookEventState = KeywordGroup.InputPurchaseList;
                    var StatusSettingData = await _shoppingListLogicService.Init(
                        WebHookEventDto,
                        _StaticWebhookEventState
                    );

                    replyMessageRequest = StatusSettingData.replyMessageRequest;
                    _StaticWebhookEventState = StatusSettingData.WebhookEventState;
                }
                else if (WebHookEventDto.Message.Text == KeywordGroup.StorageManagement)
                {
                    var StatusSettingData = await _storageManagementService.Init(
                        WebHookEventDto,
                        _StaticWebhookEventState
                    );
                    replyMessageRequest = StatusSettingData.replyMessageRequest;
                    _StaticWebhookEventState = StatusSettingData.WebhookEventState;
                }
                else if (WebHookEventDto.Message.Text == "新增物品至庫存")
                {
                    var StatusSettingData = await _storageManagementPurchaseService.InputStorage(
                        WebHookEventDto,
                        _StaticWebhookEventState
                    );
                    replyMessageRequest = StatusSettingData.replyMessageRequest;
                    _StaticWebhookEventState = StatusSettingData.WebhookEventState;
                }
                else
                {
                    replyMessageRequest = new ReplyMessageRequestDto<TextMessageObject>
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
        }
        await ReplyMessageHandler("text", replyMessageRequest);
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
