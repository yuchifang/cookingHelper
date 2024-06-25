using System.ComponentModel.DataAnnotations;

namespace CookingHelper.Model;

public class UserList
{
    [Key]
    public string UserId { get; set; } = default!;
    public string? ShoppingListText { get; set; }

    public FeedbackGroup FeedbackGroup { get; set; } = default!;
}
/* example
using Microsoft.EntityFrameworkCore;
using CookingHelper.Model;

namespace CookingHelper.Data
{
    public class CookingHelperContext : DbContext
    {
        public DbSet<UserList> UserLists { get; set; }
        public DbSet<FeedbackGroup> FeedbackGroups { get; set; }
        public DbSet<FeedbackPost> FeedbackPosts { get; set; }

        public CookingHelperContext(DbContextOptions<CookingHelperContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置 UserList 和 FeedbackGroup 之间的一对多关系
            modelBuilder.Entity<UserList>()
                .HasMany(u => u.FeedbackGroups)
                .WithOne(fg => fg.UserList)
                .HasForeignKey(fg => fg.UserId);

            // 配置 FeedbackGroup 和 FeedbackPost 之间的一对多关系
            modelBuilder.Entity<FeedbackGroup>()
                .HasMany(fg => fg.QuestionReplyList)
                .WithOne(fp => fp.FeedbackGroup)
                .HasForeignKey(fp => fp.FeedbackGroupId);

            modelBuilder.Entity<FeedbackGroup>()
                .HasMany(fg => fg.SystemSuggestionList)
                .WithOne(fp => fp.FeedbackGroup)
                .HasForeignKey(fp => fp.FeedbackGroupId);

            modelBuilder.Entity<FeedbackGroup>()
                .HasMany(fg => fg.OtherSuggestionList)
                .WithOne(fp => fp.FeedbackGroup)
                .HasForeignKey(fp => fp.FeedbackGroupId);

            // 可选: 配置表名（如果默认表名不符合需求）
            modelBuilder.Entity<UserList>().ToTable("UserLists");
            modelBuilder.Entity<FeedbackGroup>().ToTable("FeedbackGroups");
            modelBuilder.Entity<FeedbackPost>().ToTable("FeedbackPosts");
        }
    }
}


using CookingHelper.Data;
using CookingHelper.Model;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

public class HomeController : Controller
{
    private readonly CookingHelperContext _context;

    public HomeController(CookingHelperContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var userList = _context.UserLists
                               .Include(u => u.FeedbackGroups)
                               .ThenInclude(fg => fg.QuestionReplyList)
                               .ToList();
        return View(userList);
    }

    [HttpPost]
    public IActionResult CreateUser(UserList user)
    {
        _context.UserLists.Add(user);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}

*/
