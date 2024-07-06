using CookingHelper.Data;
using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;

namespace CookingHelper.DatabaseService;

public class StorageManagementDatabaseService
{
    private readonly UserListDbContext _userListDbContext;

    public StorageManagementDatabaseService(UserListDbContext UserListDbContext)
    {
        _userListDbContext = UserListDbContext;
    }

    // 新增空資料至 StoreList
    public async Task AddEmptyStorageData(string userId)
    {
        try
        {
            var UserData = await GetStoreListData(userId);
            if (UserData == null)
            {
                await _userListDbContext.StoreList.AddAsync(new StoreList { UserId = userId, });
                await _userListDbContext.SaveChangesAsync();
                // var UserDataAdded = await GetStoreListData(userId);

                // if (UserDataAdded != null)
                // {
                //     var StoreItemData = await _userListDbContext
                //         .StoreItem.AsNoTracking()
                //         .FirstOrDefaultAsync(StoreItem =>
                //             StoreItem.StoreListId == UserDataAdded.StoreListId
                //         );
                //     if (StoreItemData == null)
                //     {
                //         await _userListDbContext.StoreItem.AddAsync(
                //             new StoreItem
                //             {
                //                 Name = "",
                //                 Place = "",
                //                 StoreListId = UserDataAdded.StoreListId
                //             }
                //         );
                //         await _userListDbContext.SaveChangesAsync();
                //         var StoreItemDataAdded =
                //             await _userListDbContext.StoreItem.FirstOrDefaultAsync(StoreItem =>
                //                 StoreItem.StoreListId == UserDataAdded.StoreListId
                //             );
                //         if (StoreItemDataAdded != null)
                //         {
                //             UserDataAdded.StoreItemList.Add(StoreItemDataAdded);
                //             await _userListDbContext.SaveChangesAsync();
                //         }
                //         else
                //         {
                //             throw new Exception("StoreItemDataAdded not Found");
                //         }
                //     }
                //     else
                //     {
                //         throw new Exception("StoreItemGroupData not Found");
                //     }
                // }
                // else
                // {
                //     throw new Exception("UserDataAdded not Found");
                // }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex?.InnerException?.Message);
            throw new Exception(nameof(ex));
        }
    }

    //? 新增一個 GetStoreItemGroup 的function 或 改GetStoreListData

    public async Task<StoreList> GetStoreListData(string userId)
    {
        try
        {
            var StoreListData = await _userListDbContext
                .StoreList.AsNoTracking()
                .FirstOrDefaultAsync(StoreList => StoreList.UserId == userId);
            return StoreListData!;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex?.InnerException?.Message);
            throw new Exception(nameof(ex));
        }
    }
}
