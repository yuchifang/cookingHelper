using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;

namespace CookingHelper.Data;

public class UserListDbContext : DbContext
{
    // DI 注入
    public UserListDbContext(DbContextOptions<UserListDbContext> options)
        : base(options) { }

    public DbSet<UserList> UserList { get; set; }
    public DbSet<FeedbackGroup> FeedbackGroup { get; set; }
    public DbSet<FeedbackPost> FeedbackPost { get; set; }

    public DbSet<QuestionReply> QuestionReply { get; set; }

    public DbSet<OtherSuggestion> OtherSuggestion { get; set; }

    public DbSet<SystemSuggestion> SystemSuggestion { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserList>(entityBuilder =>
        {
            entityBuilder.Property(e => e.UserId);
            entityBuilder.Property(e => e.ShoppingListText).HasMaxLength(50);
        });

        modelBuilder
            .Entity<UserList>()
            .HasOne(UserList => UserList.FeedbackGroup)
            .WithOne(FeedbackGroup => FeedbackGroup.UserList)
            .HasForeignKey<FeedbackGroup>(FeedbackGroup => FeedbackGroup.UserId);

        modelBuilder.Entity<FeedbackGroup>().HasKey(FeedbackGroup => FeedbackGroup.FeedbackGroupId);
        modelBuilder
            .Entity<FeedbackGroup>()
            .Property(FeedbackGroup => FeedbackGroup.FeedbackGroupId)
            .ValueGeneratedOnAdd();

        modelBuilder
            .Entity<FeedbackGroup>()
            .HasOne(FeedbackGroup => FeedbackGroup.QuestionReply)
            .WithOne(QuestionReply => QuestionReply.FeedbackGroup)
            .HasForeignKey<QuestionReply>(QuestionReply => QuestionReply.FeedbackGroupId);

        modelBuilder.Entity<QuestionReply>().HasKey(QuestionReply => QuestionReply.QuestionReplyId);
        modelBuilder
            .Entity<QuestionReply>()
            .Property(QuestionReply => QuestionReply.QuestionReplyId)
            .ValueGeneratedOnAdd();

        modelBuilder
            .Entity<FeedbackGroup>()
            .HasOne(FeedbackGroup => FeedbackGroup.SystemSuggestion)
            .WithOne(SystemSuggestion => SystemSuggestion.FeedbackGroup)
            .HasForeignKey<SystemSuggestion>(SystemSuggestion => SystemSuggestion.FeedbackGroupId);

        modelBuilder
            .Entity<SystemSuggestion>()
            .HasKey(SystemSuggestion => SystemSuggestion.SystemSuggestionId);
        modelBuilder
            .Entity<SystemSuggestion>()
            .Property(SystemSuggestion => SystemSuggestion.SystemSuggestionId)
            .ValueGeneratedOnAdd();

        // modelBuilder
        //     .Entity<FeedbackGroup>()
        //     .HasOne(FeedbackGroup => FeedbackGroup.OtherSuggestion)
        //     .WithOne(OtherSuggestion => OtherSuggestion.FeedbackGroup)
        //     .HasForeignKey<OtherSuggestion>(OtherSuggestion => OtherSuggestion.FeedbackGroupId);

        // modelBuilder
        //     .Entity<OtherSuggestion>()
        //     .HasMany(OtherSuggestion => OtherSuggestion.PostList)
        //     .WithOne(FeedbackPost => FeedbackPost.OtherSuggestion)
        //     .HasForeignKey(FeedbackPost => FeedbackPost.OtherSuggestionId);

        modelBuilder
            .Entity<SystemSuggestion>()
            .HasMany(SystemSuggestion => SystemSuggestion.PostList)
            .WithOne(FeedbackPost => FeedbackPost.SystemSuggestion)
            .HasForeignKey(FeedbackPost => FeedbackPost.SystemSuggestionId);

        modelBuilder
            .Entity<QuestionReply>()
            .HasMany(QuestionReply => QuestionReply.PostList)
            .WithOne(FeedbackPost => FeedbackPost.QuestionReply)
            .HasForeignKey(FeedbackPost => FeedbackPost.QuestionReplyId);

        modelBuilder.Entity<FeedbackPost>().HasKey(FeedbackPost => FeedbackPost.Id);
        modelBuilder
            .Entity<FeedbackPost>()
            .Property(FeedbackPost => FeedbackPost.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<FeedbackGroup>().ToTable("FeedbackGroup");
        modelBuilder.Entity<FeedbackPost>().ToTable("FeedbackPost");
        modelBuilder.Entity<UserList>().ToTable("UserList");

        modelBuilder.Entity<SystemSuggestion>().ToTable("SystemSuggestion");
        modelBuilder.Entity<OtherSuggestion>().ToTable("OtherSuggestion");
        modelBuilder.Entity<QuestionReply>().ToTable("QuestionReply");
    }
}
