
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.HostEnvironment.IsDevelopment()
    ? "https://localhost:7156/"
    : "https://weddingapp-eugub0c9ftdnfwg5.southeastasia-01.azurewebsites.net/";
    
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

builder.Services.AddScoped<IGDriveService, GDriveService>();
builder.Services.AddSingleton<ToastService>();

await builder.Build().RunAsync();
 