using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

public class StorageManagementSearchService
{
    private readonly StorageManagementService _storageManagementService;

    private static List<object> _ReplyMessageListStatic = new List<object>();

    public StorageManagementSearchService(StorageManagementService StorageManagementService)
    {
        _storageManagementService = StorageManagementService;
    }

    public async Task SearchStorage(WebhookEventDto WebHookEventDto)
    {
        string? WebHookEventMessage = WebHookEventDto.Message!.Text;
        if (WebHookEventMessage == "取消查詢")
        {
            await _storageManagementService.GetStorage(WebHookEventDto);
            return;
        }
        else
        {
            StringToStorageInfo(
                WebHookEventMessage,
                out Dictionary<string, string> StorageInfoDic,
                out string ErrorText
            );
            if (ErrorText != "")
            {
                _ReplyMessageListStatic = new List<object>
                {
                    new TextMessageObject { Text = "發生錯誤: 此欄位出現問題 " + ErrorText },
                };
            }
            else
            {
                // StorageInfo 做 search
                //產生 FlexMessage 供選擇

                //?
                /*
                    查詢功能 Flexmessage
                    查到用 flex message 顯示
                    取消查詢
                    最下面加個 修改,刪除, 返回
                */
            }
        }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    public async Task SearchStoragePostBack(WebhookEventDto WebHookEventDto)
    {
        LineBotService._WebhookEventStatusStatic = "庫存查詢";

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = new List<object>
            {
                new TextMessageObject { Text = "依格式輸入查詢資訊" },
                new TextMessageObject
                {
                    Text = "若要尋找物品名稱為蘋果, 請輸入物品名稱:蘋果. \n要填入多筆資訊, 請用/號隔開, 如物品名稱:蘋果/過期日期:20230809",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Message,
                                    Label = "取消查詢",
                                    Text = "取消查詢",
                                }
                            },
                        }
                    }
                }
            }
        };
        return;
    }
}
