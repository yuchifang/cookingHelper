using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;

namespace CookingHelper.Data;

public class UserListDbContext : DbContext
{
    // DI 注入
    public UserListDbContext(DbContextOptions<UserListDbContext> options)
        : base(options) { }

    public DbSet<UserList> UserList { get; set; }

    public DbSet<StoreItem> StoreItem { get; set; }
    public DbSet<StoreList> StoreList { get; set; }

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
            .WithOne(StoreItemGroup => StoreItemGroup.StoreList)
            .HasForeignKey(StoreItemGroup => StoreItemGroup.StoreItemGroupId);

        modelBuilder.Entity<StoreItem>().HasKey(StoreItemGroup => StoreItemGroup.StoreItemGroupId);
        modelBuilder
            .Entity<StoreItem>()
            .Property(StoreItemGroup => StoreItemGroup.StoreItemGroupId)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<UserList>().ToTable("UserList");

        modelBuilder.Entity<StoreList>().ToTable("StoreList");

        modelBuilder.Entity<StoreItem>().ToTable("StoreItemGroup");
    }
}
