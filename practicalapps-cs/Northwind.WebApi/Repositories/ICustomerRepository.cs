using My.Shared;

namespace Northwind.WebApi.Repositories;

public interface ICustomerRepository {
    public Task<Customer?> CreateAsync(Customer c);
    public Task<IEnumerable<Customer>> RetrieveAllAsync();
    public Task<Customer?> RetrieveAsync(string id);
    public Task<Customer?> UpdateAsync(string id, Customer c);
    public Task<bool?> DeleteAsync(string id);
}