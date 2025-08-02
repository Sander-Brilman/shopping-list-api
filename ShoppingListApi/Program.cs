using Microsoft.EntityFrameworkCore;
using ShoppingListApi.ShoppingItem;
using ShoppingListApi.ShoppingList;
using ShoppingListApi;
using ShoppingListApi.Data;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("ShoppingListDB") ?? "";
builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    // options.UseNpgsql("Host=localhost;Port=5432;Database=mydatabase;Username=user;Password=password;");
    
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

System.Console.WriteLine($"CONNECTION STRING {connectionString}");

builder.Services.AddResponseCompression();
builder.Services.AddSignalR();
builder.Services.AddMemoryCache();

// if (builder.Environment.IsDevelopment())
// {
    builder.Services.AddEndpointsApiExplorer(); // Enables discovery of minimal API endpoints
    builder.Services.AddSwaggerGen();           // Adds Swagger generation
// }

var app = builder.Build();


await Task.Delay(TimeSpan.FromSeconds(5));

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseMiddleware<CorsMiddleware>();

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger/"));
// }

app.UseExceptionHandler("/Error", createScopeForErrors: true);
app.UseHttpsRedirection();

app.UseResponseCompression();

var api = app.MapGroup("/api");
api.MapGroup("List").MapShoppingListEndpoints();
api.MapGroup("Item").MapShoppingItemEndpoints();

app.MapHub<ShoppingItemHub>("/ShoppingItemHub", options => { });
app.MapHub<ShoppingListHub>("/ShoppingListHub", options => { });


app.Run();
