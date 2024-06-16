using System.Net.Http.Headers;
using System.Text;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.ProviderGroup;

namespace CookingHelper.LineDtoService;
public class LineBotService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _client;

    private readonly JsonProvider _jsonProvider = new JsonProvider();

    private readonly string replyMessageUri = "https://api.line.me/v2/bot/message/reply";


    private readonly IConfiguration _configuration;
    public LineBotService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _client = _httpClientFactory.CreateClient();
        _configuration = configuration;

    }

    // 接收到使用者訊息會觸發此 API??
    public void ReceiveWebhook(WebhookRequestBodyDto requestBody)
    {
        foreach (WebhookEventDto eventObject in requestBody.Events)
        {
            switch (eventObject.Type)
            {
                case WebhookEventTypeEnum.Message:
                    if (eventObject.Message.Type == MessageTypeEnum.Text)
                    {
                        ReceiveMessageWebhookEvent(eventObject);
                        Console.WriteLine(eventObject.Message.Text + "123456");
                    }
                    break;
            }
        }
    }

    private void ReceiveMessageWebhookEvent(WebhookEventDto eventDto)
    {
        dynamic replyMessage = new ReplyMessageRequestDto<BaseMessageDto>();

        switch (eventDto.Message.Type)
        {
            // 收到文字訊息
            case MessageTypeEnum.Text:
                // 訊息內容等於 "測試" 時
                if (eventDto.Message.Text == "採買清單")
                {
                    Console.WriteLine("hereD");
                    replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                    {
                        ReplyToken = eventDto.ReplyToken,
                        Messages = new List<TextMessageDto>{
                            new TextMessageDto{
                                    Text="採買清單",
                            }
                        }
                    };
                }
                else
                {
                    Console.WriteLine($"{eventDto.ReplyToken}");
                    replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                    {
                        ReplyToken = eventDto.ReplyToken,
                        Messages = new List<TextMessageDto>{
                            new TextMessageDto{
                                    Text=eventDto.Message.Text,
                            }
                        }
                    };
                }
                break;
        }
        ReplyMessageHandler("text", replyMessage);
    }
    /// <summary>
    /// 接收到回覆請求時，在將請求傳至 Line 前多一層處理(目前為預留)
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="requestBody"></param>
    public void ReplyMessageHandler<T>(string messageType, ReplyMessageRequestDto<T> requestBody)
    {
        ReplyMessage(requestBody);
    }

    public async void ReplyMessage<T>(ReplyMessageRequestDto<T> request)
    {

        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["LineBot:ChannelAccessToken"]); //帶入 channel access token
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