using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using CustomAuthentication;

//Builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage();

//Auth
builder.Services.AddAuthorizationCore();

// Add Authentication State Provider services
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// Register the custom message handler
builder.Services.AddTransient<JwtTokenHandler>();

// Configure HttpClient with the handler for bypassing SSL and setting the JWT token
builder.Services.AddHttpClient("NoSSL", c =>
    {
        c.BaseAddress = new Uri("https://localhost:7115"); // adjust if needed
    })
    .ConfigurePrimaryHttpMessageHandler(sp => 
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        var tokenHandler = sp.GetRequiredService<JwtTokenHandler>();
        tokenHandler.InnerHandler = handler; // Set the inner handler
        return tokenHandler;
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