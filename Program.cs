using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Moksha_App.Controllers;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// ✅ Ensure directory exists for Data Protection keys
var keysPath = Path.Combine("/app", "DataProtection-Keys");
if (!Directory.Exists(keysPath))
{
    Directory.CreateDirectory(keysPath);
    Console.WriteLine($"[INFO] Created Data Protection Keys directory: {keysPath}");
}

// ✅ Configure Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("Moksha_App");

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

// 🔴 REMOVE Redis Configuration (Handled by another API)

// 🔥 Build application
var app = builder.Build();

// ✅ Middleware Configuration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ✅ Force HTTPS Redirect in Production
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();

    // ✅ Configure headers for reverse proxy (Required for Render)
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
}

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

Console.WriteLine("[INFO] Application is starting...");
app.Run();
