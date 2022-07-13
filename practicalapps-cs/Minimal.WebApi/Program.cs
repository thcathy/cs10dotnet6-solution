using Northwind.Common;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://localhost:5003");
builder.Services.AddCors();

var app = builder.Build();
app.UseCors(configurePolicy: options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
});

app.MapGet("/api/weather", () => {
    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
        Date = DateTime.Now.AddDays(index),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = WeatherForecast.Summaries[Random.Shared.Next(WeatherForecast.Summaries.Length)]
    }).ToArray();
});

app.Run();
