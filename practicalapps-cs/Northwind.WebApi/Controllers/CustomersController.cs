using Microsoft.AspNetCore.Mvc;
using My.Shared;
using Northwind.WebApi.Repositories;

namespace Northwind.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase {
    private readonly ICustomerRepository repository;

    public CustomersController(ICustomerRepository repository) {
        this.repository = repository;
    }

    // GET: api/customers
    // GET: api/customers?country=[country]
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
    public async Task<IEnumerable<Customer>> GetCustomers(string? country) {
        if (string.IsNullOrEmpty(country)) {
            return await repository.RetrieveAllAsync();
        } else {
            return (await repository.RetrieveAllAsync()).Where(c => c.Country == country);
        }
    }

    // GET: api/customers/[id]
    [HttpGet("{id}", Name = nameof(GetCustomer))]
    [ProducesResponseType(200, Type = typeof(Customer))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCustomer(string id) {
        Console.WriteLine($"GetCustomer id={id}");
        Customer? c = await repository.RetrieveAsync(id);
        if (c == null) {
            return NotFound($"Cannot find customer with id={id}");
        }
        return Ok(c);
    }

    // POST: api/customers
    // BODY: Customer (JSON, XML)
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Customer))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] Customer c) {
        if (c == null) {
            return BadRequest();
        }
        Customer? newCustomer = await repository.CreateAsync(c);
        if (newCustomer == null) {
            return BadRequest("Cannot create customer");
        } else {
            return CreatedAtRoute(
                routeName: nameof(GetCustomer),
                routeValues: new { id = newCustomer.CustomerId.ToLower() },
                value: newCustomer
            );
        }
    }

    // PUT: api/customers/[id]
    // BODY: Customer (JSON, XML)
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(string id, [FromBody] Customer c) {
        id = id.ToUpper();
        c.CustomerId = c.CustomerId.ToUpper();
        if (c == null || c.CustomerId != id) {
            return BadRequest();
        }
        Customer? existing = await repository.RetrieveAsync(id);
        if (existing == null) {
            return NotFound($"cannot find customer id={id}");
        }
        await repository.UpdateAsync(id, c);
        return new NoContentResult();
    }

    // DELETE: api/customers/[id]
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(string id) {
        if (id == "bad") {
            ProblemDetails problemDetails = new()
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://localhost:5001/customers/failed-to-delete",
                Title = $"Customer {id} found but failed to delete",
                Detail = "more detail lor",
                Instance = HttpContext.Request.Path
            };
            return BadRequest(problemDetails);
        }

        Customer? existing = await repository.RetrieveAsync(id);
        if (existing == null) {
            return NotFound();
        }
        bool? deleted = await repository.DeleteAsync(id);
        if (deleted.HasValue && deleted.Value) {
            return new NoContentResult();
        } else {
            return BadRequest($"Customer {id} found by failed to delete");
        }
    }
}