using Microsoft.EntityFrameworkCore;

namespace CookingHelper.Data;
public class UserListDbContext : DbContext
{
    // DI 注入
    public UserListDbContext(DbContextOptions<UserListDbContext> options) : base(options)
    {

    }
    public DbSet<UserList> UserList { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserList>(entityBuilder =>
        {
            entityBuilder.Property(e => e.UserId);
            entityBuilder.Property(e => e.ShoppingListText).HasMaxLength(50);
        });
    }
}