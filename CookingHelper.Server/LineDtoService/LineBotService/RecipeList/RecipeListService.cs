using System.Text.Json;
using CookingHelper.Enum;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using CookingHelper.Model;
using static CookingHelper.LineDto.BaseMessageObject;
using static CookingHelper.Utils;

public class RecipeListService
{
    public static dynamic _ReplyMessageListStatic = new List<object>();
    public RecipeListDatabaseService _recipeListDatabaseService;

    private static int _PageIndexStatic = 1;
    private static int _PageSizeStatic = 10;

    public RecipeListService(RecipeListDatabaseService RecipeListDatabaseService)
    {
        _recipeListDatabaseService = RecipeListDatabaseService;
    }

    public async Task GetRecipeList(WebhookEventDto WebHookEventDto)
    {
        var RecipeMethodGroup = RecipeListAdditionBaseClass.Instance;
        var WebHookEventMessage = WebHookEventDto.Message!.Text!;
        var UserList = await _recipeListDatabaseService.GetRecipeList(
            WebHookEventDto.Source!.UserId!
        );
        if (UserList.RecipeList.Count() == 0)
        {
            _ReplyMessageListStatic = new List<object>
            {
                new TextMessageObject
                {
                    Text = "清單中沒有食譜, 點擊按鈕, 輸入想要紀錄的食譜",
                    QuickReply = new QuickReplyItemDto
                    {
                        Items = new List<QuickReplyButtonDto>
                        {
                            new QuickReplyButtonDto
                            {
                                Action = new ActionDto
                                {
                                    Type = ActionTypeEnum.Postback,
                                    Label = "新增食譜",
                                    Text = "新增食譜",
                                    Data = "quick reply postback action",
                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                }
                            }
                        }
                    }
                }
            };
        }
        else
        {
            var OrderedByNameRecipeItem = UserList
                .RecipeList.OrderBy(Item => Item.Name)
                .AsQueryable();

            if (WebHookEventMessage == "下一頁")
            {
                _PageIndexStatic += 1;
            }
            else if (WebHookEventMessage == "上一頁")
            {
                _PageIndexStatic -= 1;
            }

            var PaginatedRecipeItem = Paginate(
                OrderedByNameRecipeItem,
                _PageIndexStatic,
                _PageSizeStatic,
                out bool hasNextPage,
                out bool hasPrevPage
            );

            var FlexCarouselContents = new List<FlexBubbleContainer>();

            foreach (RecipeItem RecipeItem in PaginatedRecipeItem.ToList())
            {
                FlexCarouselContents.Add(
                    RecipeMethodGroup.GetFlexBubbleContainer(
                        RecipeItem,
                        RecipeMethodGroup.DeleteButtonGroup(RecipeItem)
                    )
                );
            }

            var RecipeListUI = new List<object>
            {
                new FlexMessageObject<FlexCarouselContainer>
                {
                    AltText = "食譜",
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
                                Type = ActionTypeEnum.Postback,
                                Label = "新增食譜",
                                Text = "新增食譜",
                                Data = "quick reply postback action",
                                InputOption = PostbackInputOptionEnum.OpenKeyboard,
                            }
                        },
                        new QuickReplyButtonDto
                        {
                            Action = new ActionDto
                            {
                                Type = ActionTypeEnum.Postback,
                                Label = "食譜查詢",
                                Text = "食譜查詢",
                                Data = "quick reply postback action",
                                InputOption = PostbackInputOptionEnum.OpenKeyboard,
                            }
                        }
                    }
                };

            if (hasNextPage && hasPrevPage)
            {
                (
                    (FlexMessageObject<FlexCarouselContainer>)RecipeListUI[0]
                ).QuickReply.Items.InsertRange(
                    0,
                    [
                        RecipeMethodGroup.GetQuickReplyButton(ActionTypeEnum.Message, "下一頁", "下一頁"),
                        RecipeMethodGroup.GetQuickReplyButton(ActionTypeEnum.Message, "上一頁", "上一頁")
                    ]
                );
            }
            else if (hasNextPage)
            {
                ((FlexMessageObject<FlexCarouselContainer>)RecipeListUI[0]).QuickReply.Items.Insert(
                    0,
                    RecipeMethodGroup.GetQuickReplyButton(ActionTypeEnum.Message, "下一頁", "下一頁")
                );
            }
            else if (hasPrevPage)
            {
                ((FlexMessageObject<FlexCarouselContainer>)RecipeListUI[0]).QuickReply.Items.Insert(
                    0,
                    RecipeMethodGroup.GetQuickReplyButton(ActionTypeEnum.Message, "上一頁", "上一頁")
                );
            }

            /*
                ! 顯示的 button 還要改
                圖片 cache 處理
                ! 查詢 刪除
                
            */
            _ReplyMessageListStatic = RecipeListUI;
        }

        LineBotService._ReplyMessageRequestStatic = new ReplyMessageRequestDto<object>
        {
            ReplyToken = WebHookEventDto.ReplyToken!,
            Messages = _ReplyMessageListStatic
        };
    }

    public async Task DeleteRecipePostBack(WebhookEventDto WebHookEventDto)
    {
        var userId = WebHookEventDto.Source!.UserId!;
        var RecipeItem = JsonSerializer.Deserialize<RecipeItem>(
            WebHookEventDto.Postback!.Data![1..]
        );
        if (RecipeItem != null)
        {
            await _recipeListDatabaseService.DeleteRecipeItem(RecipeItem, userId);
        }
        await GetRecipeList(WebHookEventDto);
    }
}
