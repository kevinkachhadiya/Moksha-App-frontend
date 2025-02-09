using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Moksha_App.Controllers;
using Moksha_App.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// ✅ Ensure directory exists for Data Protection keys
var keysPath = Path.Combine("/app", "DataProtection-Keys");
if (!Directory.Exists(keysPath))
{
    Directory.CreateDirectory(keysPath);
    Console.WriteLine($"[INFO] Created Data Protection Keys directory: {keysPath}");
}

// ✅ Configure Data Protection (Linux-compatible)
builder.Services.AddDataProtection()
    .SetApplicationName("Moksha_App")
    .UseEphemeralDataProtectionProvider(); // ✅ Uses in-memory keys (will reset on restart)

builder.Services.AddHostedService<KeepAliveService>();

// ✅ Authentication Configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

// ✅ Global Authorization Filter
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("Missing Jwt:Key");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new Exception("Missing Jwt:Issuer");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new Exception("Missing Jwt:Audience");

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new GlobalTokenAuthorizationFilter(jwtKey, jwtIssuer, jwtAudience));
});

// ✅ CORS Policy (Allow all origins)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// 🔥 Build application
var app = builder.Build();

// ✅ Middleware Configuration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ✅ Disable HTTPS Redirection for Render (since Render already provides HTTPS)
if (!app.Environment.IsDevelopment())
{
    Console.WriteLine("[INFO] Running in production mode. Skipping UseHttpsRedirection.");

    // ✅ Explicitly bind to PORT (Fixes Render shutting down issue)
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Urls.Add($"http://0.0.0.0:{port}");
}
else
{
    app.UseHttpsRedirection();
}

// ✅ Configure headers for reverse proxy (Required for Render)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// ✅ Middleware Pipeline
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();

// ✅ Default Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}");

Console.WriteLine("[INFO] Application started successfully and is running...");

app.Run();
