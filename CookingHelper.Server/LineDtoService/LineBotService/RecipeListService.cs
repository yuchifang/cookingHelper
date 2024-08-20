using CookingHelper.Data;
using CookingHelper.LineDto;

public class RecipeListService
{
    public RecipeListDatabaseService _recipeListDatabaseService;

    public RecipeListService(RecipeListDatabaseService RecipeListDatabaseService)
    {
        _recipeListDatabaseService = RecipeListDatabaseService;
    }

    public async Task GetRecipeList(WebhookEventDto WebHookEventDto)
    {
        var WebHookEventMessage = WebHookEventDto.Message!.Text!;
    }
}
