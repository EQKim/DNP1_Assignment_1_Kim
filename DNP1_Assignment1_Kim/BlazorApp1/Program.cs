using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;

//Builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage();

//Auth
builder.Services.AddAuthorizationCore();

// Bypass SSL validation
builder.Services.AddTransient(sp => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

// Configure HttpClient with the handler for bypassing SSL
builder.Services.AddHttpClient("NoSSL", c =>
    {
        c.BaseAddress = new Uri("https://localhost:7115"); // adjust if needed
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

// Registering an HttpClient factory that takes care of JWT tokens
builder.Services.AddScoped<HttpClient>(sp =>
{
    var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("NoSSL");
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    var token = localStorage.GetItemAsync<string>("ThisIsASuperSecureSecretKey32Char").Result; 
    if (!string.IsNullOrEmpty(token))
    {
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    return client;
});

//Build
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();