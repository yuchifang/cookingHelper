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
        try
        {
            var UserData = await _userListContext.UserList.AsNoTracking().FirstOrDefaultAsync(user => user.UserId == userId);
            return UserData!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw new Exception(nameof(ex));
        }
    }


    public async Task UserAddedFriend(string userId)
    {
        try
        {
            var UserData = await _userListContext.UserList.AsNoTracking().FirstOrDefaultAsync(user => user.UserId == userId);
            if (UserData == null)
            {
                _userListContext.UserList.Add(new UserList
                {
                    UserId = userId,
                    ShoppingListText = ""

                });
                await _userListContext.SaveChangesAsync();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred:  {ex.Message}");
        }
    }
}