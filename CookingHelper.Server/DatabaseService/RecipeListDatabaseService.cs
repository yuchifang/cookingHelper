using System.Text.Json.Serialization;
using CookingHelper.Data;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;
using static CookingHelper.Utils;

public class RecipeListDatabaseService
{
    private readonly UserListDbContext _userListDbContext;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _ServiceProvider;

    public RecipeListDatabaseService(
        UserListDbContext UserListDbContext,
        IConfiguration configuration,
        IServiceProvider ServiceProvider
    )
    {
        _userListDbContext = UserListDbContext;
        _configuration = configuration;
        _ServiceProvider = ServiceProvider;
    }

    public async Task<UserListRecipeList> GetRecipeList(string userId)
    {
        var UserList = await _userListDbContext
            .UserList.Select(u => new UserListRecipeList
            {
                UserId = u.UserId,
                RecipeList = u.RecipeList
            })
            .SingleAsync(u => u.UserId == userId);
        return UserList;
    }

    public async Task AddRecipe(
        RecipeItem RecipeItem,
        string userId,
        WebhookEventDto WebHookEventDto
    )
    {
        try
        {
            await _userListDbContext.RecipeItem.AddAsync(
                new RecipeItem
                {
                    UserId = userId,
                    Name = RecipeItem.Name,
                    Step = RecipeItem.Step,
                    ImagePath = RecipeItem.ImagePath,
                    Ingredients = RecipeItem.Ingredients
                }
            );
            await _userListDbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("********");
            Console.WriteLine(ex?.InnerException?.Message);
            Console.WriteLine("********");
            await _ServiceProvider
                .GetService<LineBotService>()!
                .ErrorHandler($"${userId} AddRecipe Error", WebHookEventDto);
        }
    }

    public async Task DeleteRecipeItem(
        RecipeItem RecipeItem,
        string userId,
        WebhookEventDto WebHookEventDto
    )
    {
        try
        {
            var UserList = await GetRecipeList(userId);
            if (UserList != null)
            {
                RecipeItem? RemoveItem = UserList.RecipeList.Single(item =>
                    item.RecipeItemId == RecipeItem.RecipeItemId
                );
                if (RemoveItem != null)
                {
                    if (RemoveItem.ImagePath != null)
                    {
                        var filePath =
                            $"{_configuration.GetValue<string>(WebHostDefaults.ContentRootKey)}/{RecipeItem.ImagePath}";
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                    _userListDbContext.RecipeItem.Remove(RemoveItem);
                    await _userListDbContext.SaveChangesAsync();
                }
                else
                {
                    await _ServiceProvider
                        .GetService<LineBotService>()!
                        .ErrorHandler($"${userId} RemoveItem = null", WebHookEventDto);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("********");
            Console.WriteLine(ex?.InnerException?.Message);
            Console.WriteLine("********");
            await _ServiceProvider
                .GetService<LineBotService>()!
                .ErrorHandler($"${userId} DeleteRecipeItem Error", WebHookEventDto);
        }
    }

    public IQueryable<RecipeItem> GetSearchRecipeResult(string SearchText)
    {
        var RecipeItem = _userListDbContext.RecipeItem.Where(RecipeItem =>
            RecipeItem.Name.IndexOf(SearchText) != -1
            || RecipeItem.Ingredients.IndexOf(SearchText) != -1
        );
        return RecipeItem;
    }

    public class UserListRecipeList
    {
        public string UserId { get; set; } = default!;

        [JsonIgnore]
        public ICollection<RecipeItem> RecipeList { get; set; } = new List<RecipeItem>();
    }
}
