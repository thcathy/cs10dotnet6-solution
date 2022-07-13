using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using My.Shared;
using static System.Console;

internal class Program
{
    private static void Main(string[] args)
    {
        //FilterAndSort();
        //JoinCategoriesAndProducts();
        //GroupJoinCategoriesAndProducts();
        //AggregateProducts();
        //CustomExtensionMethods();
        //OutputProductAsXml();
        ProcessSettingsXml();
    }

    static void ProcessSettingsXml() {
        XDocument doc = XDocument.Load("settings.xml");
        var appSettings = doc.Descendants("appSettings")
                            .Descendants("add")
                            .Select(node => new
                            {
                                Key = node.Attribute("key")?.Value,
                                Value = node.Attribute("value")?.Value
                            }).ToArray();
        foreach (var item in appSettings) {
            WriteLine($"{item.Key}={item.Value}");
        }
    }

    static void OutputProductAsXml() {
        using (Northwind db = new()) {
            Product[] productsArray = db.Products.ToArray();
            XElement xml = new("products",
                from p in productsArray
                select new XElement("product", new XAttribute("id", p.ProductId), new XAttribute("price", p.UnitPrice), new XElement("name", p.ProductName))
            );
            WriteLine(xml.ToString());
        }
    }

    static void CustomExtensionMethods() {
        using (Northwind db = new()) {
            WriteLine("Mean units in stock: {0:N0}", db.Products.Average(p => p.UnitsInStock));
            WriteLine("Mean units price: {0:$#,##0.00}", db.Products.Average(p => p.UnitPrice));
            WriteLine("Median units in stock: {0:N0}", db.Products.Median(p => p.UnitsInStock));
            WriteLine("Median units price: {0:$#,##0.00}", db.Products.Median(p => p.UnitPrice));
            WriteLine("Mode units in stock: {0:N0}", db.Products.Mode(p => p.UnitsInStock));
            WriteLine("Mode units price: {0:$#,##0.00}", db.Products.Mode(p => p.UnitPrice));
        }
    }

    static void AggregateProducts() {
        using (Northwind db=new()) {
            WriteLine("{0,-25} {1,10}", "Product count:", db.Products.Count());
            WriteLine("{0,-25} {1,10:$#,##0.00}", "Highest product price:", db.Products.Max(p => p.UnitPrice));
            WriteLine("{0,-25} {1,10:N0}", "Sum of units in stock:", db.Products.Sum(p => p.UnitsInStock));
            WriteLine("{0,-25} {1,10:N0}", "Sum of units on order:", db.Products.Sum(p => p.UnitsOnOrder));
        }
    }

    static void GroupJoinCategoriesAndProducts() {
        using (Northwind db = new()) {
            var queryGroupJoin = db.Categories.AsEnumerable().GroupJoin(
                inner: db.Products,
                outerKeySelector: c => c.CategoryId,
                innerKeySelector: p => p.CategoryId,
                resultSelector: (c, matchingProducts) => new { c.CategoryName, Products = matchingProducts.OrderBy(p => p.ProductName) }
            ).OrderBy(cp => cp.CategoryName);
            foreach (var item in queryGroupJoin) {
                WriteLine($"{item.CategoryName}: {String.Join(", ", item.Products.Select(p => p.ProductName))}");
            }
        }
    }

    static void JoinCategoriesAndProducts() {
        using (Northwind db = new()) {
            var queryJoin = db.Categories.Join(
                inner: db.Products,
                outerKeySelector: c => c.CategoryId,
                innerKeySelector: p => p.CategoryId,
                resultSelector: (c, p) => new { c.CategoryName, p.ProductName, p.ProductId }
            ).OrderBy(cp => cp.CategoryName);
            foreach (var item in queryJoin) {
                WriteLine($"{item.ProductId}: {item.ProductName} is in {item.CategoryName}");
            }
        }
    }

    static void FilterAndSort() {
        using (Northwind db = new()) {
            DbSet<Product>? allProducts = db.Products;
            if (allProducts is null) {
                WriteLine("no product wor");
                return;
            }
            IQueryable<Product> processedProducts = allProducts.ProcessSequence();            
            IQueryable<Product> filteredProducts = processedProducts.Where(p => p.UnitPrice < 10M);
            IOrderedQueryable<Product> sortedAndFilteredProducts = filteredProducts.OrderByDescending(p => p.UnitPrice);
            var projectedProducts = sortedAndFilteredProducts.Select(p => new
            {
                p.ProductId,
                p.ProductName,
                p.UnitPrice
            });

            WriteLine("Products that cost less than $10:");
            foreach (var p in projectedProducts) {
                WriteLine($"{p.ProductId}: {p.ProductName} costs {p.UnitPrice:$#,##0.00}");
            }
            WriteLine();
        }
    }
}
