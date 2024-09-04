using CookingHelper.LineDto;
using CookingHelper.LineDtoService;

public class RecipeListSearchService
{
    public RecipeListSearchService() { }

    public async Task GetSearchHint(WebhookEventDto WebHookEventDto)
    {
        LineBotService._WebhookEventStatusStatic = KeywordGroup.RecipeListSearch;
    }
}
