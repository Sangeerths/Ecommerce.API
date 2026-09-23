using ECommerce.UI.Menu;
using ECommerce.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

void ConfigureApiClient(HttpClient client)
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiSettings:BaseUrl"]!);
}
builder.Services.AddHttpClient<ProductApiService>(ConfigureApiClient).RemoveAllLoggers(); ;
builder.Services.AddHttpClient<CategoryApiService>(ConfigureApiClient).RemoveAllLoggers(); ;
builder.Services.AddHttpClient<SaleApiService>(ConfigureApiClient).RemoveAllLoggers(); ;
builder.Services.AddTransient<ConsoleHelper>();


builder.Services.AddScoped<MenuUI>();
using var app = builder.Build();

var menu = app.Services.GetRequiredService<MenuUI>();

await menu.OnStart(); 