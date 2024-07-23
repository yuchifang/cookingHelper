using CookingHelper.Data;
using CookingHelper.LineDtoService;
using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;

namespace CookingHelper.DatabaseService;

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

public class StorageManagementDatabaseService
{
    private readonly UserListDbContext _userListDbContext;

    public StorageManagementDatabaseService(UserListDbContext UserListDbContext)
    {
        _userListDbContext = UserListDbContext;
    }

    // 新增空資料至 StoreList
    public async Task AddEmptyStorageListData(string userId)
    {
        try
        {
            var UserData = await GetStoreListNoStrackingData(userId);
            if (UserData == null)
            {
                await _userListDbContext.StoreList.AddAsync(new StoreList { UserId = userId, });
                await _userListDbContext.SaveChangesAsync();
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

    public async Task<StoreList> GetStoreListNoStrackingData(string userId)
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

    //! 這邊有沒有比較好的寫法 或許可以合併 GetStoreListNoStrackingData
    //! 新增的地方看一下
    //! 讀取的地方看一下
    //! 看看目前UI

    public async Task<StoreList> GetStoreListData(string userId)
    {
        try
        {
            var StoreListData = await _userListDbContext
                .StoreList.Include(s => s.StoreItemList)
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

    // 新增資料
    public async Task AddStoreItemData(string userId, InputStorageInfo InputStorageInfo)
    {
        var StoreListData = await GetStoreListNoStrackingData(userId);
        try
        {
            if (StoreListData != null)
            {
                await _userListDbContext.StoreItem.AddAsync(
                    new StoreItem
                    {
                        Name = InputStorageInfo.Name,
                        Place = InputStorageInfo.Place,
                        Location = InputStorageInfo.Location,
                        Amount = InputStorageInfo.Amount,
                        PurchaseDate = InputStorageInfo.PurchaseDate,
                        ExpiryDate = InputStorageInfo.ExpiryDate,
                        StoreListId = StoreListData.StoreListId,
                    }
                );

                await _userListDbContext.SaveChangesAsync();
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
}
