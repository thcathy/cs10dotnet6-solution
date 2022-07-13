using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace My.Shared;

public static class NorthwindContextExtensions
{
    public static IServiceCollection AddNorthwindContext(this IServiceCollection services, string relativePath="..") {
        string dbPath = Path.Combine(relativePath, "Northwind.db");
        services.AddDbContext<NorthwindContext>(options => options.UseSqlite($"Data Source={dbPath}"));
        return services;
    }
}