using Microsoft.EntityFrameworkCore;
using MixPlannerApi.Data.Models;

namespace MixPlannerApi.Data;

public sealed class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<TaskListDBEntity> Lists { get; set; }

    public DbSet<TaskItemDBEntity> Items { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<TaskItemDBEntity>()
            .HasIndex(i => i.ListId);

    }
}
