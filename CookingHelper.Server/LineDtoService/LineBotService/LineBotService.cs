using System.Net.Http.Headers;
using System.Text;
using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.ProviderGroup;

namespace CookingHelper.LineDtoService;

public class LineBotService
{
    private readonly UserListDatabaseService _userListDatabaseService;

    private readonly ShoppingListLogicService _shoppingListLogicService;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _client;

    private readonly JsonProvider _jsonProvider = new JsonProvider();

    private readonly string replyMessageUri = "https://api.line.me/v2/bot/message/reply";

    private readonly IConfiguration _configuration;
    protected static string _WebhookEventState = "";

    public LineBotService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        UserListDatabaseService UserListDatabaseService,
        ShoppingListLogicService ShoppingListLogicService
    )
    {
        _httpClientFactory = httpClientFactory;
        _client = _httpClientFactory.CreateClient();
        _configuration = configuration;
        _userListDatabaseService = UserListDatabaseService;
        _shoppingListLogicService = ShoppingListLogicService;
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
                    _WebhookEventState = "";
                }
                if (
                    WebHookEventDto.Message.Text == KeywordGroup.PurchaseList
                    || _WebhookEventState == KeywordGroup.InputPurchaseList
                )
                {
                    _WebhookEventState = KeywordGroup.InputPurchaseList;
                    var StatusSettingData = await _shoppingListLogicService.UpdateShoppingList(
                        WebHookEventDto,
                        _WebhookEventState
                    );

                    replyMessageRequest = StatusSettingData.replyMessageRequest;
                    _WebhookEventState = StatusSettingData.WebhookEventState;
                }
                else if (WebHookEventDto.Message.Text == KeywordGroup.StorageManagement) { }
                else
                {
                    replyMessageRequest = new ReplyMessageRequestDto<TextMessageObject>
                    {
                        ReplyToken = WebHookEventDto.ReplyToken!,
                        Messages = new List<TextMessageObject>
                        {
                            new TextMessageObject { Text = WebHookEventDto.Message.Text! }
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
