using My.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static System.Console;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

internal class Program
{
    private static void Main(string[] args)
    {
        WriteLine($"Using {ProjectConstants.DatabaseProvider} db provider");
        //QueryingCategories();
        // FilterIncludes();
        // QueryingProducts();
        //QueryingWithLike();
        // if (AddProduct(6, "Bob's Burgers", 500M)) {
        //     WriteLine("success add product");
        // }
        //IncreaseProductPrice("Bob", 20M);
        int deleted = DeleteProducts("Bob");
        WriteLine($"{deleted} product deleted");
        ListProducts();
    }

    static int DeleteProducts(string productNameStartsWith) {
        using (Northwind db = new())
        {
            using (IDbContextTransaction t = db.Database.BeginTransaction())
            {
                WriteLine($"txn isolation level: {t.GetDbTransaction().IsolationLevel}");

                IQueryable<Product>? products = db.Products.Where(p => p.ProductName.StartsWith(productNameStartsWith));
                if (products is null) {
                    WriteLine("No product found");
                    return 0;                
                }
                db.Products.RemoveRange(products);
                int affected = db.SaveChanges();
                t.Commit();
                return affected;
            }          
        }
    }

    static bool IncreaseProductPrice(string productNameStartsWith, decimal amount) {
        using (Northwind db = new())
        {
            Product updateProduct = db.Products.First(p => p.ProductName.StartsWith(productNameStartsWith));
            updateProduct.Cost += amount;
            int affected = db.SaveChanges();
            return affected == 1;
        }
    }

    static bool AddProduct(int categoryId, string productName, decimal? price) { 
        using (Northwind db = new()) {
            Product p = new()
            {
                CategoryId = categoryId,
                ProductName = productName,
                Cost = price
            };
            db.Products.Add(p);
            int affected = db.SaveChanges();
            return (affected == 1);
        }
    }

    static void ListProducts() {
        using (Northwind db = new()) {
            WriteLine("{0,-3} {1, -35} {2,8} {3,5} {4}", "Id", "Product Name", "Cost", "Stock", "Disc");
            foreach (Product p in db.Products) {
                WriteLine("{0:000} {1, -35} {2,8:#,##0.00} {3,5} {4}", 
                    p.ProductId, p.ProductName, p.Cost, p.Stock, p.Discontinued);
            }
        }
    }

    static void QueryingWithLike() {
        using (Northwind db = new()) {
            ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new ConsoleLoggerProvider());
            Write("Enter part of a product name: ");
            string? input = ReadLine();
            IQueryable<Product>? products = db.Products?.Where(p => EF.Functions.Like(p.ProductName, $"%{input}%"));
            if (products is null) {
                WriteLine("No product wor");
                return;
            }
            foreach (Product p in products) {
                WriteLine($"{p.ProductName} has {p.Stock} units int stocks. Discontinued? {p.Discontinued}");
            }
        }
    }

    static void QueryingCategories() {
        using (Northwind db = new()) {
            ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new ConsoleLoggerProvider());
            WriteLine("Categories and how many products they have:");

            IQueryable<Category>? categories = db.Categories;
            db.ChangeTracker.LazyLoadingEnabled = false;
            Write("Enable eager loading? (Y/N): ");
            bool eagerloading = (ReadKey().Key == ConsoleKey.Y);
            bool explicitloading = false;
            WriteLine();
            if (eagerloading) {
                categories = db.Categories?.Include(c => c.Products);
            } else {
                categories = db.Categories;
                Write("Enable explicit loading? (Y/N): ");
                explicitloading = (ReadKey().Key == ConsoleKey.Y);
                WriteLine();
            }

            //IQueryable<Category>? categories = db.Categories?.Include(c => c.Products);
            
            if (categories is null) {
                WriteLine("No category found");
                return;
            }            
            foreach (Category c in categories) {
                if (explicitloading) {
                    Write($"Explicitly load products for {c.CategoryName}? (Y/N): ");
                    ConsoleKeyInfo key = ReadKey();
                    WriteLine();
                    if (key.Key == ConsoleKey.Y) {
                        CollectionEntry<Category, Product> products = db.Entry(c).Collection(c2 => c2.Products);
                        if (!products.IsLoaded) products.Load();
                    }
                }
                WriteLine($"{c.CategoryName}:{c.CategoryId} has {c.Products.Count} products");
            }
        }
    }

    static void QueryingProducts() {
        using (Northwind db = new()) {
            ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new ConsoleLoggerProvider());

            WriteLine("Products that cost more than a price, highest at top");
            string? input;
            decimal price;
            do
            {
                Write("Enter a product price: ");
                input = ReadLine();
            } while (!decimal.TryParse(input, out price));
            IQueryable<Product>? products = db.Products?
                .TagWith("Products filtered by price and sorted")
                .Where(p => p.Cost > price).OrderByDescending(p => p.Cost);
            if (products is null) {
                WriteLine("No products found");
                return;
            }            
            foreach (Product p in products) {
                WriteLine($"{p.ProductId}: {p.ProductName} costs {p.Cost:$#,##0.00} and has {p.Stock} in stock");
            }
        }
    }
    static void FilterIncludes() {
        using (Northwind db = new()) {
            Write("Enter a min for units in stocks:");
            string unitInStock = ReadLine() ?? "10";
            int stock = int.Parse(unitInStock);
            IQueryable<Category>? categories = db.Categories?.Include(c => c.Products.Where(p => p.Stock >= stock));
            if (categories is null) {
                WriteLine("No category found");
                return;
            }
            WriteLine($"SQL: {categories.ToQueryString()}");
            foreach (Category c in categories) {
                WriteLine($"{c.CategoryName} has {c.Products.Count} products with a min of {stock} units in stock");
                foreach (Product p in c.Products) {
                    WriteLine($"    {p.ProductName} has {p.Stock} units in stock");
                }
            }
        }
    }
}