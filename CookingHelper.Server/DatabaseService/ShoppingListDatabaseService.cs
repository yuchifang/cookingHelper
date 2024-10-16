using CookingHelper.Data;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;

namespace CookingHelper.DatabaseService;

public class ShoppingListDatabaseService
{
    private readonly UserListDbContext _userListDbContext;

    private readonly IServiceProvider _ServiceProvider;

    public ShoppingListDatabaseService(
        UserListDbContext UserListDbContext,
        IServiceProvider ServiceProvider
    )
    {
        _userListDbContext = UserListDbContext;
        _ServiceProvider = ServiceProvider;
    }

    public async Task<UserList> GetUserList(string userId, WebhookEventDto WebHookEventDto)
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
            await _ServiceProvider
                .GetService<LineBotService>()!
                .ErrorHandler($"${userId} GetUserList Error", WebHookEventDto);
            throw new Exception(nameof(ex));
        }
    }

    public async Task AddEmptyShoppingListText(string userId, WebhookEventDto WebHookEventDto)
    {
        try
        {
            var UserList = await GetUserList(userId, WebHookEventDto);
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
            await _ServiceProvider
                .GetService<LineBotService>()!
                .ErrorHandler($"${userId} AddEmptyShoppingListText Error", WebHookEventDto);
            throw new Exception("AddEmptyShoppingListText Error");
        }
    }

    public async Task EmptyShoppingText(string? userId, WebhookEventDto WebHookEventDto)
    {
        try
        {
            var UserData = await GetUserList(userId!, WebHookEventDto);
            if (UserData != null)
            {
                UserData.ShoppingListText = "";
                _userListDbContext.UserList.Update(UserData);
                await _userListDbContext.SaveChangesAsync();
            }
            else
            {
                await _ServiceProvider
                    .GetService<LineBotService>()!
                    .ErrorHandler($"${userId} EmptyShoppingText Error", WebHookEventDto);
                throw new Exception("EmptyShoppingText Error");
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
        UserList? UserDataInput,
        WebhookEventDto WebHookEventDto
    )
    {
        if (UserDataInput == null)
        {
            try
            {
                var UserData = await GetUserList(userId!, WebHookEventDto);
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
                await _ServiceProvider
                    .GetService<LineBotService>()!
                    .ErrorHandler($"${userId} UpdateUserShoppingText Error", WebHookEventDto);
                throw new Exception("UpdateUserShoppingText Error");
            }
        }
        else
        {
            try
            {
                UserDataInput.ShoppingListText =
                    UserDataInput.ShoppingListText + " " + UpdateShoppingText;
                _userListDbContext.UserList.Update(UserDataInput);
                await _userListDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred:  {ex.Message}");
                await _ServiceProvider
                    .GetService<LineBotService>()!
                    .ErrorHandler($"${userId} UpdateUserShoppingText Error", WebHookEventDto);
                throw new Exception("UpdateUserShoppingText Error");
            }
        }
    }
}
