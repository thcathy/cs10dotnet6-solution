using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Console;

namespace My.Shared;

public class Northwind : DbContext {
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Product>? Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Category>()
            .Property(category => category.CategoryName)
            .IsRequired()
            .HasMaxLength(15);
        
        if (ProjectConstants.DatabaseProvider == "SQLite") {
            modelBuilder.Entity<Product>()
                .HasQueryFilter(p => !p.Discontinued)
                .Property(product => product.Cost)
                .HasConversion<double>();                
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseLazyLoadingProxies();

        if (ProjectConstants.DatabaseProvider == "SQLite") {
            string path = Path.Combine(Environment.CurrentDirectory, "Northwind.db");
            WriteLine($"Using {path} db file");
            optionsBuilder.UseSqlite($"Filename={path}");
        } else {
            string connection = "Data Source=.;Initial Catalog=Northwind;Integrated Security=true;MultipleActiveResultSets=true;";
            optionsBuilder.UseSqlServer(connection);
        }
    }
}