using CookingHelper.Data;
using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;

namespace CookingHelper.DatabaseService;

public class ShoppingListDatabaseService
{
    private readonly UserListDbContext _userListDbContext;

    public ShoppingListDatabaseService(UserListDbContext UserListDbContext)
    {
        _userListDbContext = UserListDbContext;
    }

    public async Task<UserList> GetUserList(string userId)
    {
        try
        {
            var UserList = await _userListDbContext
                .UserList.AsNoTracking()
                .FirstOrDefaultAsync(user => user.UserId == userId);
            return UserList!;
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
            var UserList = await GetUserList(userId);
            if (UserList == null)
            {
                await _userListDbContext.UserList.AddAsync(
                    new UserList { UserId = userId, ShoppingListText = "" }
                );
                await _userListDbContext.SaveChangesAsync();
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
                var UserData = await GetUserList(userId!);
                if (UserData != null)
                {
                    UserData.ShoppingListText = UpdateShoppingText;
                    _userListDbContext.UserList.Update(UserData);
                    await _userListDbContext.SaveChangesAsync();
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
                _userListDbContext.UserList.Update(UserDataInput);
                await _userListDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred:  {ex.Message}");
            }
        }
    }
}
