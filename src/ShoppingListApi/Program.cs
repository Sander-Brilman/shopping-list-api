using Microsoft.EntityFrameworkCore;
using ShoppingListApi.ShoppingItem;
using ShoppingListApi.ShoppingList;
using ShoppingListApi;
using ShoppingListApi.Data;
using ShoppingListApi.Health;

var builder = WebApplication.CreateBuilder(args);

string host = builder.Configuration.GetValue<string>("POSTGRES_HOST") ?? throw new Exception("enviorment variable POSTGRES_HOST not found");
string port = builder.Configuration.GetValue<string>("POSTGRES_PORT") ?? throw new Exception("enviorment variable POSTGRES_PORT not found");
string db = builder.Configuration.GetValue<string>("POSTGRES_DB") ?? throw new Exception("enviorment variable POSTGRES_DB not found");
string user = builder.Configuration.GetValue<string>("POSTGRES_USER") ?? throw new Exception("enviorment variable POSTGRES_USER not found");
string password = File.ReadAllText(builder.Configuration.GetValue<string>("POSTGRES_PASSWORD_FILE") ?? throw new Exception("enviorment variable POSTGRES_PASSWORD_FILE not found"));



string connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={password}";
builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});


builder.Services.AddSignalR();
builder.Services.AddMemoryCache();

builder.Services.AddEndpointsApiExplorer(); // Enables discovery of minimal API endpoints
builder.Services.AddSwaggerGen();           // Adds Swagger generation

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseMiddleware<CorsMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => Results.Redirect("/swagger/"));

app.UseExceptionHandler("/Error", createScopeForErrors: true);

var api = app.MapGroup("/api");
api.MapGroup("List").MapShoppingListEndpoints();
api.MapGroup("Item").MapShoppingItemEndpoints();
app.MapHealthEndpoint();


app.MapHub<ShoppingItemHub>("/ShoppingItemHub", options => { });
app.MapHub<ShoppingListHub>("/ShoppingListHub", options => { });

app.Run();
