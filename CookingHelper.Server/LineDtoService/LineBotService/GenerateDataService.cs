using CookingHelper.Data;
using CookingHelper.LineDto;
using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;

namespace CookingHelper.LineDtoService;

public class GenerateDataService
{
    private readonly UserListDbContext _UserListDbContext;

    public GenerateDataService(UserListDbContext UserListDbContext)
    {
        _UserListDbContext = UserListDbContext;
    }

    public async Task<bool> GenerateData(WebhookEventDto WebHookEventDto)
    {
        Console.WriteLine(_UserListDbContext.RecipeItem.Any());
        Console.WriteLine(_UserListDbContext.StoreItem.Any());

        if (_UserListDbContext.RecipeItem.Any() || _UserListDbContext.StoreItem.Any())
        {
            return false;
        }

        var User = await _UserListDbContext.UserList.FirstOrDefaultAsync(item =>
            item.UserId == WebHookEventDto!.Source!.UserId!
        );
        Console.WriteLine(User == null);
        if (User == null)
        {
            return false;
        }
        User.ShoppingListText = "冬瓜 蘋果 地瓜 馬鈴薯";
        User.StoreList = new List<StoreItem>
        {
            new StoreItem
            {
                Place = "冰箱",
                Name = "蘋果",
                Location = "第二層",
                Amount = "6",
            },
            new StoreItem
            {
                Place = "冰箱",
                Name = "蘋果",
                Location = "第一層",
                Amount = "6"
            },
            new StoreItem
            {
                Place = "冰箱",
                Name = "香蕉",
                Location = "第二層",
                Amount = "4"
            },
            new StoreItem
            {
                Place = "冰箱",
                Name = "葡萄",
                Location = "第三層",
                Amount = "10"
            },
            new StoreItem
            {
                Place = "冰箱",
                Name = "橙子",
                Location = "第一層",
                Amount = "8"
            },
            new StoreItem
            {
                Place = "冰箱",
                Name = "西瓜",
                Location = "第二層",
                Amount = "1"
            },
            new StoreItem
            {
                Place = "冰箱",
                Name = "芒果",
                Location = "第三層",
                Amount = "3"
            },
            new StoreItem
            {
                Place = "冰箱",
                Name = "梨",
                Location = "第一層",
                Amount = "7"
            },
            new StoreItem
            {
                Place = "冰箱",
                Name = "草莓",
                Location = "第二層",
                Amount = "20"
            },
            new StoreItem
            {
                Place = "冰箱",
                Name = "藍莓",
                Location = "第三層",
                Amount = "15"
            },
            new StoreItem
            {
                Place = "冰箱",
                Name = "柚子",
                Location = "第一層",
                Amount = "2"
            },
            new StoreItem
            {
                Place = "冰箱",
                Name = "櫻桃",
                Location = "第二層",
                Amount = "30"
            },
            new StoreItem
            {
                Place = "冰箱",
                Name = "鳳梨",
                Location = "第三層",
                Amount = "1"
            },
            new StoreItem
            {
                Place = "冰箱",
                Name = "李子",
                Location = "第一層",
                Amount = "5"
            }
        };
        User.RecipeList = new List<RecipeItem>()
        {
            new RecipeItem
            {
                Name = "水果沙拉",
                Ingredients = "蘋果1顆, 香蕉1顆, 優格",
                Step = new List<string> { "水果切塊", "加入優格攪拌均勻" }
            },
            new RecipeItem
            {
                Name = "蔥油拌麵",
                Ingredients = "麵條1份, 蔥, 醬油",
                Step = new List<string> { "煮麵條", "熱鍋爆香蔥段", "加入醬油拌匀" }
            },
            new RecipeItem
            {
                Name = "蒜香蝦",
                Ingredients = "蝦200克, 蒜2瓣, 鹽",
                Step = new List<string> { "蒜切碎", "熱鍋炒香蒜", "加入蝦炒熟" }
            },
            new RecipeItem
            {
                Name = "炒青菜",
                Ingredients = "青菜300克, 蒜, 鹽",
                Step = new List<string> { "青菜洗淨", "熱鍋爆香蒜末", "加入青菜翻炒" }
            },
            new RecipeItem
            {
                Name = "牛奶燕麥",
                Ingredients = "燕麥30克, 牛奶200ml, 蜂蜜",
                Step = new List<string> { "將燕麥加入牛奶煮熟", "加入蜂蜜調味" }
            },
            new RecipeItem
            {
                Name = "玉米濃湯",
                Ingredients = "玉米1罐, 牛奶200ml, 鹽",
                Step = new List<string> { "將玉米和牛奶煮熱", "加入鹽調味" }
            },
        };

        _UserListDbContext.UserList.Update(User);
        await _UserListDbContext.SaveChangesAsync();
        return true;
    }
}
