using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Northwind.Mvc.Data;
using My.Shared;
using Northwind.Mvc.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddNorthwindContext();

builder.Services.AddHttpClient(name: "Northwind.WebApi",
    configureClient: options => {
        options.BaseAddress = new Uri("https://localhost:5002");
        options.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json", 1.0)
        );                
    }
).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            }
        });

builder.Services.AddHttpClient(name: "Minimal.WebApi",
    configureClient: options => {
        options.BaseAddress = new Uri("https://localhost:5003");
        options.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json", 1.0)
        );                
    }
).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            }
        });

builder.Services.AddHttpClient(name: "Northwind.GraphQL",
    configureClient: options => {
        options.BaseAddress = new Uri("https://localhost:5005");
        options.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json", 1.0)
        );                
    }
).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            }
        });

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // app.UseHsts();
}

app.UsePathBase("/cs-web");

// app.UseHttpsRedirection();
app.UseStaticFiles();

// app.UseMiddleware<SecurityHeaders>(); Not working

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapHub<ChatHub>("/chat");

app.Run();
