using GraphQL.Server;
using My.Shared;
using Northwind.GraphQL;

var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseUrls("https://::5005/");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<NorthwindSchema>();
builder.Services.AddNorthwindContext();

builder.Services.AddGraphQL()
    .AddGraphTypes(typeof(NorthwindSchema), ServiceLifetime.Scoped)
    .AddDataLoader()
    .AddSystemTextJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseGraphQLPlayground();
}

app.UseGraphQL<NorthwindSchema>();

//app.UseHttpsRedirection();
app.UsePathBase("/cs-web");

app.UseAuthorization();

app.MapControllers();

app.Run();
