using System.Net.Http.Headers;
using System.Text;
using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.ProviderGroup;

namespace CookingHelper.LineDtoService;
public class LineBotService
{
    private readonly ShoppingListService _shoppingListService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _client;

    private readonly JsonProvider _jsonProvider = new JsonProvider();

    private readonly string replyMessageUri = "https://api.line.me/v2/bot/message/reply";


    private readonly IConfiguration _configuration;
    public LineBotService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ShoppingListService ShoppingListService)
    {
        _httpClientFactory = httpClientFactory;
        _client = _httpClientFactory.CreateClient();
        _configuration = configuration;
        _shoppingListService = ShoppingListService;

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
                        ReceiveMessageWebhookEvent(WebHookEventDto);
                    }
                    break;
                case WebhookEventTypeEnum.Follow:
                    Console.WriteLine($"使用者{WebHookEventDto.Source!.UserId}將我們新增為好友！");
                    await _shoppingListService.UserAddedFriend(WebHookEventDto.Source!.UserId!);
                    break;
            }
        }
    }

    private async Task ReceiveMessageWebhookEvent(WebhookEventDto WebHookEventDto)
    {
        dynamic replyMessage = new ReplyMessageRequestDto<BaseMessageEventObject>();

        switch (WebHookEventDto.Message.Type)
        {
            // 收到文字訊息
            case MessageTypeEnum.Text:
                if (WebHookEventDto.Message.Text == "採買清單")
                {
                    var UserData = await _shoppingListService.GetUserData(WebHookEventDto.Source!.UserId!);



                    replyMessage = new ReplyMessageRequestDto<TextMessageEventObject>
                    {
                        ReplyToken = WebHookEventDto.ReplyToken,
                        Messages = new List<TextMessageEventObject>{
                            new TextMessageEventObject{
                                    Text="採買清單",
                            }
                        }
                    };
                }
                else
                {
                    replyMessage = new ReplyMessageRequestDto<TextMessageEventObject>
                    {
                        ReplyToken = WebHookEventDto.ReplyToken,
                        Messages = new List<TextMessageEventObject>{
                            new TextMessageEventObject{
                                    Text=WebHookEventDto.Message.Text,
                            }
                        }
                    };
                }
                break;

        }
        ReplyMessageHandler("text", replyMessage);
    }

    public void ReplyMessageHandler<T>(string messageType, ReplyMessageRequestDto<T> requestBody)
    {
        ReplyMessage(requestBody);
    }

    public async Task ReplyMessage<T>(ReplyMessageRequestDto<T> request)
    {

        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["LineBot:ChannelAccessToken"]);
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