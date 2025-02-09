using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Moksha_App.Controllers;
using Moksha_App.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Get the current environment
var env = builder.Environment;

// Determine the data protection keys storage path
string keysPath = env.IsDevelopment()
    ? Path.Combine(env.ContentRootPath, "DataProtection-Keys") // Local folder in development
    : "/var/data-protection-keys";                              // Use persistent storage on Render (or production)

// Ensure the keys folder exists
if (!Directory.Exists(keysPath))
{
    Directory.CreateDirectory(keysPath);
    Console.WriteLine($"[INFO] Created Data Protection Keys directory: {keysPath}");
}

// Configure Data Protection without certificate encryption
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("Moksha_App");

// Register your KeepAliveService (make sure this service is implemented in your project)
builder.Services.AddHostedService<KeepAliveService>();

// Configure authentication using cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Configure JWT using settings from configuration
var jwtConfig = builder.Configuration.GetSection("Jwt");
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new GlobalTokenAuthorizationFilter(
        jwtConfig["Key"] ?? throw new ArgumentNullException("Jwt:Key"),
        jwtConfig["Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer"),
        jwtConfig["Audience"] ?? throw new ArgumentNullException("Jwt:Audience")
    ));
});

// Configure CORS for both production and development
builder.Services.AddCors(options =>
{
    options.AddPolicy("RenderPolicy", policy =>
    {
        policy.WithOrigins("https://your-render-service.onrender.com", "http://localhost")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure middleware based on environment
if (app.Environment.IsProduction())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.All
    });
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
}

// Configure static file serving with caching headers
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = false,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
    }
});

app.UseRouting();
app.UseCors("RenderPolicy");
app.UseAuthentication();
app.UseAuthorization();

// Set up the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}");

// Bind to the Render-assigned port if in production
if (!app.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Urls.Clear();
    app.Urls.Add($"http://0.0.0.0:{port}");
    Console.WriteLine($"[INFO] Running on port {port}");
}

app.Run();
