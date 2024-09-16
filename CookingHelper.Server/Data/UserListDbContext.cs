using CookingHelper.Model;
using Microsoft.EntityFrameworkCore;

namespace CookingHelper.Data;

public class UserListDbContext : DbContext
{
    public UserListDbContext(DbContextOptions<UserListDbContext> options)
        : base(options) { }

    public DbSet<UserList> UserList { get; set; }

    public DbSet<StoreItem> StoreItem { get; set; }
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
            .HasMany(UserList => UserList.StoreList)
            .WithOne(StoreItem => StoreItem.UserList)
            .HasForeignKey(StoreItem => StoreItem.UserId);

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

        modelBuilder.Entity<StoreItem>().ToTable("StoreItem");

        modelBuilder.Entity<RecipeItem>().ToTable("RecipeItem");
    }
}
