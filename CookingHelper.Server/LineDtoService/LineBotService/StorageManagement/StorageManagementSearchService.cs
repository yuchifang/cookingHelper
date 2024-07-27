using CookingHelper.DatabaseService;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

public class StorageManagementSearchService
{
    private readonly StorageManagementService _storageManagementService;
    private readonly StorageManagementDatabaseService _storageManagementDatabaseService;

    private static List<object> _ReplyMessageListStatic = new List<object>();

    public StorageManagementSearchService(
        StorageManagementService StorageManagementService,
        StorageManagementDatabaseService StorageManagementDatabaseService
    )
    {
        _storageManagementService = StorageManagementService;
        _storageManagementDatabaseService = StorageManagementDatabaseService;
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
                out StorageInfo StorageInfo,
                out string ErrorText
            );
            if (ErrorText != "")
            {
                await _storageManagementService.GetStorage(WebHookEventDto);

                StorageManagementService._ReplyMessageListStatic.Insert(
                    0,
                    new TextMessageObject { Text = "發生錯誤: 此欄位出現問題 " + ErrorText }
                );

                return;
            }
            else
            {
                //? 確認寫法有沒有問題
                //? 整理
                //! 判斷是否為空
                //! 個別處理
                //! alttext 功用

                var SearchStorageItemData =
                    await _storageManagementDatabaseService.SearchStorageList(
                        StorageInfo,
                        WebHookEventDto.Source.UserId
                    );
                if (SearchStorageItemData.Count == 0)
                {
                    _ReplyMessageListStatic = new List<object>
                    {
                        new TextMessageObject { Text = "找不到此物品" }
                    };
                }
                else
                {
                    _ReplyMessageListStatic = new List<object>
                    {
                        new FlexMessageObject<FlexCarouselContainer>
                        {
                            AltText = "Display Temporary Input",
                            Contents = new FlexCarouselContainer
                            {
                                Type = FlexContainerTypeEnum.Carousel,
                                Contents = new List<FlexBubbleContainer>
                                {
                                    new FlexBubbleContainer
                                    {
                                        Type = FlexContainerTypeEnum.Bubble,
                                        Body = new FlexComponent
                                        {
                                            Type = FlexComponentTypeEnum.Box,
                                            Layout = FlexComponentLayoutTypeEnum.Vertical,

                                            Contents = new List<FlexComponent>
                                            {
                                                new FlexComponent
                                                {
                                                    Type = FlexComponentTypeEnum.Box,
                                                    Layout = FlexComponentLayoutTypeEnum.Horizontal,
                                                    AlignItems = "center",
                                                    Contents = new List<FlexComponent>
                                                    {
                                                        new FlexComponent
                                                        {
                                                            Type = FlexComponentTypeEnum.Text,
                                                            Text =
                                                                StorageManagementKeywordGroup.Place,
                                                            Size = "xs",
                                                        },
                                                        new FlexComponent
                                                        {
                                                            Type = FlexComponentTypeEnum.Text,
                                                            // Text = placeValueText,
                                                            Text = "add",
                                                            Size = "xl",
                                                            Align = "end"
                                                        }
                                                    }
                                                },
                                                // new FlexComponent
                                                // {
                                                //     Type = FlexComponentTypeEnum.Box,
                                                //     Layout = FlexComponentLayoutTypeEnum.Vertical,
                                                //     Margin = "xxl",
                                                //     Spacing = "sm",
                                                //     Contents = FieldTable
                                                // },
                                                new FlexComponent
                                                {
                                                    Type = FlexComponentTypeEnum.Separator,
                                                    Margin = "xxl"
                                                },
                                                new FlexComponent
                                                {
                                                    Type = FlexComponentTypeEnum.Box,
                                                    Layout = FlexComponentLayoutTypeEnum.Vertical,
                                                    Contents = new List<FlexComponent>
                                                    {
                                                        new FlexComponent
                                                        {
                                                            Type = FlexComponentTypeEnum.Button,
                                                            Action = new ActionDto
                                                            {
                                                                Type = ActionTypeEnum.Message,
                                                                Label = "新增",
                                                                Text = "新增"
                                                            }
                                                        },
                                                        new FlexComponent
                                                        {
                                                            Type = FlexComponentTypeEnum.Button,
                                                            Action = new ActionDto
                                                            {
                                                                Type = ActionTypeEnum.Postback,
                                                                Label = "修改",
                                                                Data = "修改",
                                                                InputOption =
                                                                    PostbackInputOptionEnum.OpenKeyboard
                                                            }
                                                        },
                                                        new FlexComponent
                                                        {
                                                            Type = FlexComponentTypeEnum.Button,
                                                            Action = new ActionDto
                                                            {
                                                                Type = ActionTypeEnum.Message,
                                                                Label = "取消新增",
                                                                Text = "取消新增",
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    };
                }

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
