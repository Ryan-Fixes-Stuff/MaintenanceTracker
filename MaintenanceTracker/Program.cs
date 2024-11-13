using MaintenanceTracker.Models;
using MaintenanceTracker.ViewModels;
using MaintenanceTracker.Views;
using Radzen;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", false, true)
           .Build();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
// keep instance of IDataModel accessible for Blazor Components in case of injection need
builder.Services.AddScoped<IDataModel, DataModel>();
// pass a DataModel constructor for the ViewModel
builder.Services.AddScoped(provider => new HomePageViewModel(new DataModel(config)));
// configure notification/modal services
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<DialogService>();
// configure Serilog/SQLite logging
builder.Services.AddRadzenComponents();
builder.Host.UseSerilog((context, services, configuration) => configuration
                .MinimumLevel.Information().WriteTo.SQLite(Environment.CurrentDirectory + @"\Data\local.db"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(" / Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
