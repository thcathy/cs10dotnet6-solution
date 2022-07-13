using System.Threading.Tasks;
using static System.Console;

namespace My.Shared;

public class Startup {
    public void ConfigureServices(IServiceCollection services) {
        services.AddRazorPages();
        services.AddNorthwindContext();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        app.UseDeveloperExceptionPage();
        app.UsePathBase("/cs-web");
        app.UseRouting();
        app.Use(async (HttpContext context, Func<Task> next) =>
        {
            RouteEndpoint? rep = context.GetEndpoint() as RouteEndpoint;
            if (rep is not null) {
                WriteLine($"Endpoint name: {rep.DisplayName}");
                WriteLine($"Endpoint route pattern: {rep.RoutePattern.RawText}");
            }
            if (context.Request.Path == "/bonjour") {
                await context.Response.WriteAsync("Bonjour Lor!");
                return;
            }
            await next();
        });
        app.UseDefaultFiles();
        app.UseStaticFiles();        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapGet("/hello", () => "Hello world lor");
        });
    }

}