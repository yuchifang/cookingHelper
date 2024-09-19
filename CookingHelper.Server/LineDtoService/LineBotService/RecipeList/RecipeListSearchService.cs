using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

public class RecipeListSearchService
{
    private static int _PageIndexStatic = 1;
    private static int _PageSizeStatic = 10;
    private readonly RecipeListService _recipeListService;
    private readonly RecipeListDatabaseService _recipeListDatabaseService;

    public RecipeListSearchService(
        RecipeListService RecipeListService,
        RecipeListDatabaseService RecipeListDatabaseService
    )
    {
        _recipeListService = RecipeListService;
        _recipeListDatabaseService = RecipeListDatabaseService;
    }

    public static dynamic _ReplyMessageListStatic = new List<object>();

    public async Task SearchRecipe(WebhookEventDto WebHookEventDto)
    {
        string WebHookEventMessage = WebHookEventDto.Message!.Text!;
        LineBotService._WebhookEventStatusStatic = KeywordGroup.RecipeListSearch;
        var RecipeMethodGroup = RecipeListAdditionBaseClass.Instance;

        if (WebHookEventMessage == "取消查詢" || WebHookEventMessage == "返回")
        {
            LineBotService._WebhookEventStatusStatic = KeywordGroup.RecipeList;
            await _recipeListService.GetRecipeList(WebHookEventDto);

            return;
        }
        else if (WebHookEventMessage == KeywordGroup.RecipeListSearch)
        {
            _ReplyMessageListStatic = new List<object>
            {
                new TextMessageObject { Text = "依格式輸入查詢資訊" },
                new TextMessageObject { Text = "若要尋找食譜名稱裡面有番茄的食譜, 請輸入食譜名稱:番茄" },
                new TextMessageObject { Text = "若要尋找食材裡面有蛋的食譜, 請輸入食材:蛋" },
                new TextMessageObject { Text = "要填入多筆資訊, 請用/號隔開, 如食譜名稱:蘋果/食材:蛋" },
                new TextMessageObject
                {
                    Text = "若要尋找多種食材, 請輸入食材:蛋,牛奶. 只能依據食材,食譜名稱搜尋",
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
            };
        }
        else
        {
            StringSlashAndColonToRecipeInfo(
                WebHookEventMessage,
                out RecipeInfo RecipeInfo,
                out string ErrorText
            );
            if (ErrorText != "")
            {
                _ReplyMessageListStatic = new List<object>
                {
                    new TextMessageObject
                    {
                        Text = ErrorText,
                        QuickReply = new QuickReplyItemDto
                        {
                            Items = new List<QuickReplyButtonDto>
                            {
                                RecipeMethodGroup.GetQuickReplyButton(
                                    ActionTypeEnum.Message,
                                    "返回",
                                    "返回"
                                )
                            }
                        }
                    }
                };
                LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
                {
                    ReplyToken = WebHookEventDto.ReplyToken!,
                    Messages = _ReplyMessageListStatic
                };
                return;
            }

            var RecipeItem = _recipeListDatabaseService.GetSearchedRecipeItem(RecipeInfo);
            if (RecipeItem.Any() == false)
            {
                _ReplyMessageListStatic = new List<object>
                {
                    new TextMessageObject
                    {
                        Text = "找不到食譜, 請重新輸入",
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
                };
                LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
                {
                    ReplyToken = WebHookEventDto.ReplyToken!,
                    Messages = _ReplyMessageListStatic
                };
                return;
            }

            if (WebHookEventMessage == "下一頁")
            {
                _PageIndexStatic += 1;
            }
            else if (WebHookEventMessage == "上一頁")
            {
                _PageIndexStatic -= 1;
            }
            var PaginatedRecipeItem = Paginate(
                RecipeItem.AsQueryable(),
                _PageIndexStatic,
                _PageSizeStatic,
                out bool hasNextPage,
                out bool hasPrevPage
            );

            var FlexCarouselContents = new List<FlexBubbleContainer>();

            foreach (var Item in PaginatedRecipeItem.ToList())
            {
                FlexCarouselContents.Add(
                    RecipeMethodGroup.GetFlexBubbleContainer(
                        Item,
                        RecipeMethodGroup.DeleteButtonGroup(Item)
                    )
                );
            }

            var RecipeListUI = new List<object>
            {
                new FlexMessageObject<FlexCarouselContainer>
                {
                    AltText = "食譜查詢",
                    Contents = new FlexCarouselContainer
                    {
                        Type = FlexContainerTypeEnum.Carousel,
                        Contents = FlexCarouselContents
                    }
                }
            };

            ((FlexMessageObject<FlexCarouselContainer>)RecipeListUI[0]).QuickReply =
                new QuickReplyItemDto
                {
                    Items = new List<QuickReplyButtonDto>
                    {
                        new QuickReplyButtonDto
                        {
                            Action = new ActionDto
                            {
                                Type = ActionTypeEnum.Message,
                                Label = "返回",
                                Text = "返回",
                                Data = "quick reply postback action",
                            }
                        },
                    }
                };

            if (hasNextPage && hasPrevPage)
            {
                (
                    (FlexMessageObject<FlexCarouselContainer>)RecipeListUI[0]
                ).QuickReply!.Items.InsertRange(
                    0,
                    [
                        RecipeMethodGroup.GetQuickReplyButton(ActionTypeEnum.Message, "下一頁", "下一頁"),
                        RecipeMethodGroup.GetQuickReplyButton(ActionTypeEnum.Message, "上一頁", "上一頁")
                    ]
                );
            }
            else if (hasNextPage)
            {
                (
                    (FlexMessageObject<FlexCarouselContainer>)RecipeListUI[0]
                ).QuickReply!.Items.Insert(
                    0,
                    RecipeMethodGroup.GetQuickReplyButton(ActionTypeEnum.Message, "下一頁", "下一頁")
                );
            }
            else if (hasPrevPage)
            {
                (
                    (FlexMessageObject<FlexCarouselContainer>)RecipeListUI[0]
                ).QuickReply!.Items.Insert(
                    0,
                    RecipeMethodGroup.GetQuickReplyButton(ActionTypeEnum.Message, "上一頁", "上一頁")
                );
            }
            _ReplyMessageListStatic = RecipeListUI;
        }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }
}
