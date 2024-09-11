using System.Text.Json.Serialization;
using CookingHelper.Data;
using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;
using static CookingHelper.Utils;

public class RecipeListDatabaseService
{
    private readonly UserListDbContext _userListDbContext;
    private readonly IConfiguration _configuration;

    public RecipeListDatabaseService(
        UserListDbContext UserListDbContext,
        IConfiguration configuration
    )
    {
        _userListDbContext = UserListDbContext;
        _configuration = configuration;
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

    public async Task AddRecipe(RecipeItem RecipeItem, string userId)
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
            throw new Exception(nameof(ex));
        }
    }

    public async Task DeleteRecipeItem(RecipeItem RecipeItem, string userId)
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
                    throw new Exception("Error");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("********");
            Console.WriteLine(ex?.InnerException?.Message);
            Console.WriteLine("********");
            throw new Exception(nameof(ex));
        }
    }

    public async Task<IEnumerable<RecipeItem>> GetSearchedRecipeItem(RecipeInfo RecipeInfo)
    {
        IEnumerable<RecipeItem> RecipeItem = Enumerable.Empty<RecipeItem>();
        if (RecipeInfo.Ingredients != null && RecipeInfo.Name != null)
        {
            RecipeItem = _userListDbContext
                .RecipeItem.Where(item => item.Name.IndexOf(RecipeInfo.Name) != -1)
                .AsEnumerable()
                .Where(RecipeItem =>
                    RecipeInfo.Ingredients.All(item => RecipeItem.Ingredients.Contains(item))
                );
        }
        else if (RecipeInfo.Name != null)
        {
            RecipeItem = _userListDbContext.RecipeItem.Where(item =>
                item.Name.IndexOf(RecipeInfo.Name) != -1
            );
        }
        else if (RecipeInfo.Ingredients != null)
        {
            RecipeItem = _userListDbContext
                .RecipeItem.AsEnumerable()
                .Where(RecipeItem =>
                    RecipeInfo.Ingredients.All(item => RecipeItem.Ingredients.Contains(item))
                );
        }
        return RecipeItem;
    }

    public class UserListRecipeList
    {
        public string UserId { get; set; } = default!;

        [JsonIgnore]
        public ICollection<RecipeItem> RecipeList { get; set; } = new List<RecipeItem>();
    }
}
