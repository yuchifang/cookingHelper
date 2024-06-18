using CookingHelper.Data;
using Microsoft.EntityFrameworkCore;

namespace CookingHelper.DatabaseService;
public class ShoppingListService
{
    private readonly UserListDbContext _userListContext;
    public ShoppingListService(UserListDbContext UserListDbContext)
    {
        _userListContext = UserListDbContext;
    }

    public async Task<UserList> GetUserData(string userId)
    {
        var GetUserData = await _userListContext.UserList.AsNoTracking().FirstOrDefaultAsync(user => user.UserId == userId);

        return GetUserData!;
    }
}