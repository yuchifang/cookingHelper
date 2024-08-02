using CookingHelper.Data;
using CookingHelper.LineDtoService;
using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;
using static CookingHelper.Utils;

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

    public async Task<IQueryable<StoreItem>> GetSearchedStorageList(
        StorageInfo UserInputStorageInfo,
        string userId
    )
    {
        try
        {
            var StoreListData = await GetStoreListData(userId);
            if (StoreListData != null)
            {
                return StoreListData
                    .StoreItemList.Where(item => ClassMatch(item, UserInputStorageInfo))
                    .AsQueryable();
            }
            else
            {
                return Enumerable.Empty<StoreItem>().AsQueryable();
                ;
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

    public async Task DeleteStorageInfo(StoreItem StoreItem, string userId)
    {
        try
        {
            var StoreListData = await GetStoreListData(userId);
            if (StoreListData != null)
            {
                StoreItem? RemoveItem = StoreListData.StoreItemList.FirstOrDefault(item =>
                    item.StoreItemId == StoreItem.StoreItemId
                );
                if (RemoveItem != null)
                {
                    _userListDbContext.StoreItem.Remove(RemoveItem);
                    await _userListDbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Error");
                }
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
