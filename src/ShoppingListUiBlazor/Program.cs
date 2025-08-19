using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShoppingListUIBlazor;
using ShoppingListUIBlazor.Services;
using Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// add the file wwwroot/appsettings.Production.json to set the api url when pushing to production  
string apiUrl = builder.Configuration.GetValue<string>("ApiUrl") ?? throw new Exception("no ApiUrl value in appsettings.json");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });
builder.Services.AddScoped<ItemRepository>();
builder.Services.AddScoped<LiveUpdateService>();
builder.Services.AddScoped<AlertBoxService>();
builder.Services.AddScoped<ShoppingListRepository>();

var app = builder.Build();

var liveUpdateService = app.Services.GetRequiredService<LiveUpdateService>();
await liveUpdateService.ConnectTo(apiUrl);

await app.RunAsync();



await builder.Build().RunAsync();
