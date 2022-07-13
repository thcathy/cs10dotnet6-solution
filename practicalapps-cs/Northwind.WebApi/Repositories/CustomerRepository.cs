using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using My.Shared;

namespace Northwind.WebApi.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private static ConcurrentDictionary<string, Customer>? customerCache;
    private NorthwindContext db;

    public CustomerRepository(NorthwindContext db) {
        this.db = db;
        if (customerCache is null) {
            customerCache = new ConcurrentDictionary<string, Customer>(db.Customers.ToDictionary(c => c.CustomerId));
        }
    }

    public  async Task<Customer?> CreateAsync(Customer c)
    {
        c.CustomerId = c.CustomerId.ToUpper();
        EntityEntry<Customer> added = await db.Customers.AddAsync(c);
        int affected = await db.SaveChangesAsync();
        if (affected == 1) {
            if (customerCache is null) return c;
            return customerCache.AddOrUpdate(c.CustomerId, c, UpdateCache);
        } else {
            return null;
        }
    }

    private Customer UpdateCache(string id, Customer c) {
        Customer? old;
        if (customerCache is not null) {
            if (customerCache.TryGetValue(id, out old)) {
                if (customerCache.TryUpdate(id, c, old)) {
                    return c;
                }
            }
        }
        return null!;
    }

    public async Task<bool?> DeleteAsync(string id)
    {
        id = id.ToUpper();
        Customer? c = db.Customers.Find(id);
        if (c is null) return null;
        db.Customers.Remove(c);
        int affected = await db.SaveChangesAsync();
        if (affected == 1) {
            if (customerCache is null) return null;
            return customerCache.TryRemove(id, out c);
        } else {
            return null;
        }
    }

    public Task<IEnumerable<Customer>> RetrieveAllAsync()
    {
        return Task.FromResult(customerCache is null ? Enumerable.Empty<Customer>() : customerCache.Values);
    }

    public Task<Customer?> RetrieveAsync(string id)
    {
        id = id.ToUpper();
        if (customerCache is null) return null;
        customerCache.TryGetValue(id, out Customer? c);
        return Task.FromResult(c);
    }

    public async Task<Customer?> UpdateAsync(string id, Customer c)
    {
        id = id.ToUpper();
        c.CustomerId = c.CustomerId.ToUpper();
        db.Customers.Update(c);
        int affected = await db.SaveChangesAsync();
        if (affected == 1) {
            return UpdateCache(id, c);
        }
        return null;
    }
}