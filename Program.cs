using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Moksha_App.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

// Global Authorization Filter
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new GlobalTokenAuthorizationFilter(
        builder.Configuration["Jwt:Key"],
        builder.Configuration["Jwt:Issuer"],
        builder.Configuration["Jwt:Audience"]
    ));
});

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// 🔴 REMOVE Redis Configuration because it's handled by another API

// Build application
var app = builder.Build();

// Middleware Configuration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Configure headers for reverse proxy (Required for Render)
if (app.Environment.IsProduction())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
}

// Middleware Pipeline
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();

// Default Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}");

app.Run();
