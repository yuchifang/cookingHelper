using System.Text.Json.Serialization;
using CookingHelper.Data;
using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;

public class RecipeListDatabaseService
{
    private readonly UserListDbContext _userListDbContext;

    public RecipeListDatabaseService(UserListDbContext UserListDbContext)
    {
        _userListDbContext = UserListDbContext;
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

    public class UserListRecipeList
    {
        public string UserId { get; set; } = default!;

        [JsonIgnore]
        public ICollection<RecipeItem> RecipeList { get; set; } = new List<RecipeItem>();
    }
}
