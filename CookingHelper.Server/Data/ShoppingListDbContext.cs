using Microsoft.EntityFrameworkCore;

namespace CookingHelper.Data;
public class ShoppingListDbContext : DbContext
{
    // DI 注入
    public ShoppingListDbContext(DbContextOptions<ShoppingListDbContext> options) : base(options)
    {

    }
    public DbSet<ShoppingListModel> ShoppingListModel { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShoppingListModel>(entityBuilder =>
        {
            entityBuilder.Property(e => e.Id).UseMySqlIdentityColumn();
            entityBuilder.Property(e => e.ShoppingListText).HasMaxLength(50);
        });
    }
}