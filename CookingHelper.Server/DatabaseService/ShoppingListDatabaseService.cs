using CookingHelper.Data;
using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;

namespace CookingHelper.DatabaseService;

public class ShoppingListDatabaseService
{
    private readonly UserListDbContext _userListContext;

    public ShoppingListDatabaseService(UserListDbContext UserListDbContext)
    {
        _userListContext = UserListDbContext;
    }

    public async Task<UserList> GetUserListData(string userId)
    {
        try
        {
            var UserListData = await _userListContext
                .UserList.AsNoTracking()
                .FirstOrDefaultAsync(user => user.UserId == userId);
            return UserListData!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw new Exception(nameof(ex));
        }
    }

    public async Task AddEmptyShoppingListText(string userId)
    {
        try
        {
            var UserData = await GetUserListData(userId);
            if (UserData == null)
            {
                await _userListContext.UserList.AddAsync(
                    new UserList { UserId = userId, ShoppingListText = "" }
                );
                await _userListContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred:  {ex.Message}");
        }
    }

    public async Task UpdateUserShoppingText(
        string? userId,
        string UpdateShoppingText,
        UserList? UserDataInput
    )
    {
        if (UserDataInput == null)
        {
            try
            {
                var UserData = await GetUserListData(userId!);
                if (UserData != null)
                {
                    UserData.ShoppingListText = UpdateShoppingText;
                    _userListContext.UserList.Update(UserData);
                    await _userListContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred:  {ex.Message}");
            }
        }
        else
        {
            try
            {
                UserDataInput.ShoppingListText = UpdateShoppingText;
                _userListContext.UserList.Update(UserDataInput);
                await _userListContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred:  {ex.Message}");
            }
        }
    }
}
