using Microsoft.EntityFrameworkCore;
using ShoppingListApi.Data.Models;

namespace ShoppingListApi.Data;

public sealed class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<ShoppingListEntity> Lists { get; set; }

    public DbSet<ShoppingItemEntity> Items { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<ShoppingItemEntity>()
            .HasIndex(i => i.ListId);

    }
}
