using System.Text.Json.Serialization;
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

    public async Task<StoreList> GetStoreListNoTrackingData(string userId)
    {
        try
        {
            var StoreListData = await _userListDbContext
                .StoreList.AsNoTracking()
                .SingleAsync(StoreList => StoreList.UserId == userId);
            return StoreListData!;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex?.InnerException?.Message);
            throw new Exception(nameof(ex));
        }
    }

    public async Task<StoreListStoreItemList> GetStoreList(string userId)
    {
        try
        {
            var StoreList = await _userListDbContext
                .StoreList.Select(s => new StoreListStoreItemList
                {
                    StoreItemList = s.StoreItemList,
                    UserId = s.UserId
                })
                .SingleAsync(StoreList => StoreList.UserId == userId);

            return StoreList!;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex?.InnerException?.Message);
            throw new Exception(nameof(ex));
        }
    }

    public async Task<IQueryable<StoreItem>> GetSearchedStoreItem(
        StorageInfo StorageInfo,
        string userId
    )
    {
        try
        {
            var StoreList = await GetStoreList(userId);
            if (StoreList != null)
            {
                return StoreList
                    .StoreItemList.AsEnumerable()
                    .Where(item => ClassMatch(item, StorageInfo))
                    .AsQueryable();
            }
            else
            {
                return Enumerable.Empty<StoreItem>().AsQueryable();
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

    // 新增空資料至 StoreList
    public async Task AddEmptyStoreList(string userId)
    {
        try
        {
            var StoreList = await GetStoreListNoTrackingData(userId);
            if (StoreList == null)
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

    public async Task AddStoreItemData(string userId, InputStorageInfo InputStorageInfo)
    {
        var StoreList = await GetStoreListNoTrackingData(userId);
        try
        {
            if (StoreList != null)
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
                        StoreListId = StoreList.StoreListId,
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

    public async Task UpdateStoreItem(StoreItem StoreItem, string userId)
    {
        try
        {
            var StoreList = await GetStoreList(userId);
            if (StoreList != null)
            {
                StoreItem? UpdateItem = StoreList.StoreItemList.Single(item =>
                    item.StoreItemId == StoreItem.StoreItemId
                );
                if (UpdateItem != null)
                {
                    UpdateItem.Name = StoreItem.Name;
                    UpdateItem.Place = StoreItem.Place;
                    UpdateItem.Location = StoreItem.Location;
                    UpdateItem.Amount = StoreItem.Amount;
                    UpdateItem.PurchaseDate = StoreItem.PurchaseDate;
                    UpdateItem.ExpiryDate = UpdateItem.ExpiryDate;

                    _userListDbContext.Entry(UpdateItem).State = EntityState.Modified;
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

    public async Task DeleteStoreItem(StoreItem StoreItem, string userId)
    {
        try
        {
            var StoreList = await GetStoreList(userId);
            if (StoreList != null)
            {
                StoreItem? RemoveItem = StoreList.StoreItemList.Single(item =>
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

    public async Task DeleteStoreItemList(IEnumerable<int> DeleteStoreItemID)
    {
        var DBDeleteStoreItem = _userListDbContext
            .StoreItem.AsEnumerable()
            .Where(item => DeleteStoreItemID.Contains(item.StoreItemId));
        _userListDbContext.StoreItem.RemoveRange(DBDeleteStoreItem);
        await _userListDbContext.SaveChangesAsync();
    }

    public class StoreListStoreItemList
    {
        public string UserId { get; set; }

        [JsonIgnore]
        public ICollection<StoreItem> StoreItemList { get; set; }
    }
}
