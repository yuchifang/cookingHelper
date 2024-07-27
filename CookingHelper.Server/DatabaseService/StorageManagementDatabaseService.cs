using CookingHelper.Data;
using CookingHelper.LineDtoService;
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
                await _userListDbContext.AddAsync(
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

    public async Task<List<StoreItem>> SearchStorageList(StorageInfo StorageInfo, string userId)
    {
        try
        {
            var StorageInfoValue = StorageInfo
                .GetType()
                .GetProperties()
                .Select(item => item.GetValue(StorageInfo))
                .ToHashSet();
            var StoreListData = await GetStoreListData(userId);
            if (StoreListData != null)
            {
                var SearchStoreItemData = StoreListData
                    .StoreItemList.Where(item =>
                        item.GetType()
                            .GetProperties()
                            .Select(p => p.GetValue(item))
                            .Any(value => StorageInfoValue.Contains(value))
                    )
                    .ToList();
                return SearchStoreItemData;
            }
            else
            {
                return new List<StoreItem>();
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
