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

    public async Task<UserListStoreList> GetUserListWithStoreListNoTracking(string userId)
    {
        try
        {
            var UserListStoreList = await _userListDbContext
                .UserList.Select(u => new UserListStoreList
                {
                    UserId = u.UserId,
                    StoreList = u.StoreList
                })
                .SingleAsync(UserListStoreList => UserListStoreList.UserId == userId);
            return UserListStoreList!;
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
            var UserListStoreList = await GetUserListWithStoreListNoTracking(userId);
            if (UserListStoreList != null)
            {
                return UserListStoreList
                    .StoreList.AsEnumerable()
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

    public async Task AddStoreItemData(string userId, InputStorageInfo InputStorageInfo)
    {
        try
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
                    UserId = userId
                }
            );

            await _userListDbContext.SaveChangesAsync();
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
            var UserListStoreList = await GetUserListWithStoreListNoTracking(userId);
            if (UserListStoreList != null)
            {
                StoreItem? UpdateItem = UserListStoreList.StoreList.Single(item =>
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
            var UserListStoreList = await GetUserListWithStoreListNoTracking(userId);
            if (UserListStoreList != null)
            {
                StoreItem? RemoveItem = UserListStoreList.StoreList.Single(item =>
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

    public class UserListStoreList
    {
        public string UserId { get; set; } = default!;

        [JsonIgnore]
        public ICollection<StoreItem> StoreList { get; set; } = new List<StoreItem>();
    }
}
