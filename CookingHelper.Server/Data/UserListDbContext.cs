using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;

namespace CookingHelper.Data;

public class UserListDbContext : DbContext
{
    public UserListDbContext(DbContextOptions<UserListDbContext> options)
        : base(options) { }

    public DbSet<UserList> UserList { get; set; }

    public DbSet<StoreItem> StoreItem { get; set; }
    public DbSet<StoreList> StoreList { get; set; }
    public DbSet<RecipeItem> RecipeItem { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserList>(entityBuilder =>
        {
            entityBuilder.Property(e => e.UserId);
            entityBuilder.Property(e => e.ShoppingListText).HasMaxLength(50);
        });

        modelBuilder
            .Entity<UserList>()
            .HasOne(UserList => UserList.StoreList)
            .WithOne(StoreList => StoreList.UserList)
            .HasForeignKey<StoreList>(StoreList => StoreList.UserId);

        modelBuilder.Entity<StoreList>().HasKey(StoreList => StoreList.StoreListId);
        modelBuilder
            .Entity<StoreList>()
            .Property(StoreList => StoreList.StoreListId)
            .ValueGeneratedOnAdd();

        modelBuilder
            .Entity<StoreList>()
            .HasMany(StoreList => StoreList.StoreItemList)
            .WithOne(StoreItem => StoreItem.StoreList)
            .HasForeignKey(StoreItem => StoreItem.StoreListId);

        modelBuilder.Entity<StoreItem>().HasKey(StoreItem => StoreItem.StoreItemId);

        modelBuilder
            .Entity<StoreItem>()
            .Property(StoreItem => StoreItem.StoreItemId)
            .ValueGeneratedOnAdd();

        modelBuilder
            .Entity<UserList>()
            .HasMany(UserList => UserList.RecipeList)
            .WithOne(RecipeItem => RecipeItem.UserList)
            .HasForeignKey(RecipeItem => RecipeItem.UserId);

        modelBuilder.Entity<RecipeItem>().HasKey(RecipeItem => RecipeItem.RecipeItemId);

        modelBuilder
            .Entity<RecipeItem>()
            .Property(RecipeItem => RecipeItem.RecipeItemId)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<UserList>().ToTable("UserList");

        modelBuilder.Entity<StoreList>().ToTable("StoreList");

        modelBuilder.Entity<StoreItem>().ToTable("StoreItem");

        modelBuilder.Entity<RecipeItem>().ToTable("RecipeItem");
    }
}
