using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Blazored.LocalStorage;

//Builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage();


// Helper function to add JWT token to HttpClient if available
static async Task<HttpClient> CreateAuthorizedClient(IServiceProvider sp)
{
    var client = new HttpClient();
    var localStorage = sp.GetRequiredService<Blazored.LocalStorage.ISyncLocalStorageService>();
    var token = localStorage.GetItem<string>("ThisIsASuperSecureSecretKey32Char");
    if (!string.IsNullOrEmpty(token))
    {
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    return client;
}

// Registering the HttpClient with JWT token
var services = builder.Services;
services.AddScoped<HttpClient>(sp => CreateAuthorizedClient(sp).Result);


// Bypass SSL validation
services.AddTransient(sp => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});


// Configure HttpClient with the handler for bypassing SSL
services.AddHttpClient("NoSSL", c =>
    {
        c.BaseAddress = new Uri("https://localhost:7115"); // adjust if needed
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

//Build
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Appls
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();