using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.Formatters;
using My.Shared;
using Northwind.WebApi.Repositories;
using Swashbuckle.AspNetCore.SwaggerUI;
using static System.Console;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("https://localhost:5002");
// Add services to the container.
builder.Services.AddNorthwindContext();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.All;
    options.RequestBodyLogLimit = 4096;
    options.ResponseBodyLogLimit = 4096;
});

builder.Services.AddControllers(options =>
{
    WriteLine("Defualt output formatters:");
    foreach (IOutputFormatter formatter in options.OutputFormatters)
    {
        OutputFormatter? mediaFormatter = formatter as OutputFormatter;
        if (mediaFormatter == null)
        {
            WriteLine($"  {formatter.GetType().Name}");
        }
        else
        {
            WriteLine("  {0}, Media types: {1}", mediaFormatter.GetType().Name, string.Join(", ", mediaFormatter.SupportedMediaTypes));
        }
    }
})
.AddXmlDataContractSerializerFormatters()
.AddXmlSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new() { Title = "Northwind Service API", Version = "v1" });
});
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddCors();

builder.Services.AddHealthChecks().AddDbContextCheck<NorthwindContext>();

var app = builder.Build();

app.UseHttpLogging();

// app.UsePathBase("/cs-web");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Northwind Service API v1");
        c.SupportedSubmitMethods(new[] {
            SubmitMethod.Get, SubmitMethod.Put, SubmitMethod.Post, SubmitMethod.Delete
        });
    });
}

app.UseCors(configurePolicy: options => {
    options.WithMethods("GET", "POST", "PUT", "DELETE");
    options.AllowAnyOrigin();    
});

// app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHealthChecks(path: "/myhealthcheck");

app.MapControllers();

app.Run();
