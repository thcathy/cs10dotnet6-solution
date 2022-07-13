using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using My.Shared;
using Northwind.Common;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly NorthwindContext db;

    private readonly IHttpClientFactory clientFactory;

    public HomeController(ILogger<HomeController> logger, NorthwindContext db, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        this.db = db;
        this.clientFactory = httpClientFactory;
    }

    [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Index()
    {
        try {
            HttpClient client = clientFactory.CreateClient("Minimal.WebApi");
            HttpRequestMessage request = new(HttpMethod.Get, "api/weather");
            HttpResponseMessage response = await client.SendAsync(request);
            ViewData["weather"] = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
        } catch (Exception e) {
            _logger.LogWarning($"The API service error: {e.Message}");
            ViewData["weather"] = Enumerable.Empty<WeatherForecast>().ToArray();
        }

        HomeIndexViewModel model = new(
            VisitorCount: (new Random()).Next(1, 1001),
        
            Categories: await db.Categories.ToListAsync(),
            Products: await db.Products.ToListAsync()
        );

        return View(model);
    }

    [Route("private")]
    [Authorize(Roles = "Administrators")]
    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> ProductDetail(int? id) {
        if (!id.HasValue) {
            return BadRequest("You must pass a product ID in the route, for e.g., /Home/ProductDetail/21");
        }
        Product? model = await db.Products.SingleOrDefaultAsync(p => p.ProductId == id);
        if (model == null) {
            return NotFound($"ProductId {id} not found");
        }
        return View(model);
    }

    public IActionResult ModelBinding() {
        return View();
    }

    [HttpPost]
    public IActionResult ModelBinding(Thing thing) {
        HomeModelBindingViewModel model = new(
            thing,
            !ModelState.IsValid,
            ModelState.Values
                .SelectMany(s => s.Errors)
                .Select(e => e.ErrorMessage)
        );
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult ProductsThatCostMoreThan(decimal? price) {
        if (!price.HasValue) {
            return BadRequest("require price. e.g. /Home/ProductsThatCostMoreThan?price=50");
        }
        IEnumerable<Product> model = db.Products
                                        .Include(p => p.Category)
                                        .Include(p => p.Supplier)
                                        .Where(p => p.UnitPrice > price);
        if (!model.Any()) {
            return NotFound($"No product price > ${price:C}");
        }
        ViewData["MaxPrice"] = price.Value.ToString("C");
        return View(model);
    }

    public async Task<IActionResult> Customers(string country) {
        string uri;
        if (string.IsNullOrEmpty(country)) {
            ViewData["Title"] = "All Customers";
            uri = "api/customers";
        } else {
            ViewData["Title"] = $"Customers in {country}";
            uri = $"api/customers?country={country}";
        }

        HttpClient client = clientFactory.CreateClient(name: "Northwind.WebApi");
        HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: uri);
        HttpResponseMessage response = await client.SendAsync(request);
        IEnumerable<Customer>? model = await response.Content.ReadFromJsonAsync<IEnumerable<Customer>>();
        return View(model);
    }

    public async Task<IActionResult> Services() {
        try {
            HttpClient client = clientFactory.CreateClient("Northwind.GraphQL");
            HttpRequestMessage request = new(HttpMethod.Post, "graphql");
            request.Content = new StringContent(content: @"
                {
                    products (categoryId: 8) {
                        productId
                        productName
                        unitsInStock
                    }
                }",
                encoding: Encoding.UTF8,
                mediaType: "application/graphql");

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode) {                
                ViewData["seafoodProducts"] = (await response.Content.ReadFromJsonAsync<GraphQLProducts>())?.Data?.Products;
            } else {                
                ViewData["seafoodProducts"] = Enumerable.Empty<Product>().ToArray();
            }
        } catch (Exception e) {
            _logger.LogWarning($"exception when calling graphql {e.Message}");
        }
        return View();
    }

    public IActionResult Chat() {
        return View();
    }
}
