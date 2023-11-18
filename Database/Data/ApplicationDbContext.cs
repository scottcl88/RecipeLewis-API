using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
  : base(options)
    {
    }

    public DbSet<Log> Logs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        SeedCategories(modelBuilder);

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                                                v => v.ToUniversalTime(),
                                                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.IsKeyless)
            {
                continue;
            }

            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
    }

    private void SeedCategories(ModelBuilder modelBuilder)
    {
        var date = new DateTime(2022, 5, 23, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 1, Name = "Any", CreatedDateTime = date });
        modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 2, Name = "Breakfast", CreatedDateTime = date });
        modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 3, Name = "Lunch", CreatedDateTime = date });
        modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 4, Name = "Dinner", CreatedDateTime = date });
        modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 5, Name = "Snack", CreatedDateTime = date });
        modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 6, Name = "Dessert", CreatedDateTime = date });
        modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 7, Name = "Drink", CreatedDateTime = date });
    }
}