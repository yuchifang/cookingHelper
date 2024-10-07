using CookingHelper.Data;
using CookingHelper.LineDto;
using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CookingHelper.Server.Test;

public class CookingHelperServerTest
{
    private RecipeListDatabaseService _recipeListDatabaseService;

    private UserListDbContext _userListDbContext;

    private DbContextOptions<UserListDbContext> _userListDbContextOptions;

    private IConfiguration _configuration;

    [SetUp]
    public void Setup()
    {
        _userListDbContextOptions = new DbContextOptionsBuilder<UserListDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _userListDbContext = new UserListDbContext(_userListDbContextOptions);

        var inMemorySettings = new Dictionary<string, string> { { "SomeSetting", "SomeValue" } };

        // 使用 ConfigurationBuilder 建立 IConfiguration 實例
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings) // 使用內存中的 key-value 集合
            .Build();
        _userListDbContext.UserList.AddRange(
            new List<UserList>
            {
                new UserList
                {
                    UserId = "1",
                    ShoppingListText = "測試1",
                    StoreList =
                    {
                        new StoreItem
                        {
                            StoreItemId = 1,
                            UserId = "1",
                            Place = "冰箱",
                            Name = "蘋果",
                            Location = "第二層",
                            Amount = "5",
                        }
                    },
                    RecipeList =
                    {
                        new RecipeItem
                        {
                            RecipeItemId = 1,
                            UserId = "1",
                            Name = "番茄炒蛋",
                            Step = { "前", "後", "左" },
                            Ingredients = "蘋果 冬瓜"
                        }
                    }
                }
            }
        );
        _userListDbContext.SaveChangesAsync();
        _recipeListDatabaseService = new RecipeListDatabaseService(
            _userListDbContext,
            _configuration
        );
    }

    [TearDown]
    public void TearDown()
    {
        _userListDbContext.Database.EnsureDeleted();
        _userListDbContext.Dispose();
    }

    [TestCase("1")]
    public async Task TestGetRecipeListDatabase(string id)
    {
        var result = await _recipeListDatabaseService.GetRecipeList(id);
        var recipeList = result.RecipeList.ToList();

        Assert.That(recipeList[0].Name, Is.EqualTo("番茄炒蛋"));
    }

    [TestCaseSource(nameof(WebhookSample))]
    public async Task TestGetRecipeList(WebhookEventDto WebHookEventDto)
    {
        var RecipeListService = new RecipeListService(_recipeListDatabaseService);
        await RecipeListService.GetRecipeList(WebHookEventDto);

        Assert.That(RecipeListService._ReplyMessageListStatic[0].AltText, Is.EqualTo("食譜"));

        Assert.That(
            RecipeListService
                ._ReplyMessageListStatic[0]
                .Contents
                .Contents[0]
                .Body
                .Contents[0]
                .Contents[1]
                .Text,
            Is.EqualTo("番茄炒蛋")
        );
    }

    public static object[] WebhookSample =
    {
        new object[]
        {
            new WebhookEventDto
            {
                Message = new MessageEventDto { Text = "" },
                Source = new SourceDto { UserId = "1" }
            },
        },
        new object[]
        {
            new WebhookEventDto
            {
                Postback = new Postback { Data = "你好" },
                Source = new SourceDto { UserId = "1" }
            }
        }
    };
}
