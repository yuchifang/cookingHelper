using CookingHelper.Data;
using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;

namespace CookingHelper.DatabaseService;

public class UserListDatabaseService
{
    private readonly UserListDbContext _userListContext;

    public UserListDatabaseService(UserListDbContext UserListDbContext)
    {
        _userListContext = UserListDbContext;
    }

    public async Task<UserList> GetUserData(string userId)
    {
        try
        {
            var UserData = await _userListContext
                .UserList.AsNoTracking()
                .FirstOrDefaultAsync(user => user.UserId == userId);
            return UserData!;
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
            var UserData = await _userListContext
                .UserList.AsNoTracking()
                .FirstOrDefaultAsync(user => user.UserId == userId);
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
                var UserData = await _userListContext
                    .UserList.AsNoTracking()
                    .FirstOrDefaultAsync(user => user.UserId == userId);
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
