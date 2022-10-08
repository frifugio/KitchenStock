using KitchenStock;
using KitchenStock.Shared.Repositories;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseAddress = builder.Configuration["BaseAddress"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(baseAddress) });

//if (builder.HostEnvironment.IsDevelopment())
//{
//    builder.Services.AddScoped<IStockRepository, MockStockRepository>();
//}
//else
//{
//    builder.Services.AddScoped<IStockRepository, StockRepository>();
//}

builder.Services.AddScoped<IStockRepository, StockRepository>();

await builder.Build().RunAsync();
