using System.Text.Json.Serialization;
using CookingHelper.Data;
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

    public class UserListRecipeList
    {
        public string UserId { get; set; } = default!;

        [JsonIgnore]
        public ICollection<RecipeItem> RecipeList { get; set; } = new List<RecipeItem>();
    }
}
