using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.HttpOverrides;
using Moksha_App.Controllers;
using Moksha_App.Models;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------------------
// 0. Load Environment Variables (explicit, though by default they load)
// --------------------------------------------------------------------
builder.Configuration.AddEnvironmentVariables();
var env = builder.Environment;

// --------------------------------------------------------------------
// 1. Update Configuration from Environment Variables (Early)
// --------------------------------------------------------------------
// In production, update the backend URL from an environment variable.
// This ensures the GlobalTokenAuthorizationFilter receives the correct value.
if (!env.IsDevelopment())
{
    var backendUrl = Environment.GetEnvironmentVariable("backend_url");

    if (!string.IsNullOrWhiteSpace(backendUrl))
    {
        // Ensure the key name here matches what you use later.
        builder.Configuration["backend_url"] = backendUrl;
    }
}

// --------------------------------------------------------------------
// 2. Data Protection Configuration
// --------------------------------------------------------------------
if (!env.IsDevelopment())
{
    // In production, persist keys to a folder.
    // (Make sure that the directory is persistent. On Render, you might mount a volume at /app/keys.)
    var keysDirectory = Environment.GetEnvironmentVariable("KEYS_DIRECTORY") ?? "/app/keys";
    Directory.CreateDirectory(keysDirectory); // Ensure directory exists
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(keysDirectory))
        .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
        {
            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
        });
}
else
{
    // In development, keys are kept in memory.
    builder.Services.AddDataProtection()
        .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
        {
            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
        });
}

// --------------------------------------------------------------------
// 3. KeepAlive Service Configuration
// --------------------------------------------------------------------
builder.Services.AddHostedService<KeepAliveService>();

// --------------------------------------------------------------------
// 4. Cookie Authentication Configuration
// --------------------------------------------------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Ensure that your AuthController's Login action is decorated with [AllowAnonymous]
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        // If your app is behind a reverse proxy, you might try CookieSecurePolicy.SameAsRequest if issues occur.
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// --------------------------------------------------------------------
// 5. JWT & Global Filter Configuration
// --------------------------------------------------------------------
var jwtConfig = builder.Configuration.GetSection("Jwt");
var connectionStrings = builder.Configuration["backend_url"];

builder.Services.AddControllersWithViews(options =>
{
    // IMPORTANT: Your GlobalTokenAuthorizationFilter should check for [AllowAnonymous]
    // on endpoints to avoid intercepting public actions.
    options.Filters.Add(new GlobalTokenAuthorizationFilter(
        jwtConfig["Key"] ?? throw new ArgumentNullException("Jwt:Key"),
        jwtConfig["Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer"),
        jwtConfig["Audience"] ?? throw new ArgumentNullException("Jwt:Audience"),
        builder.Configuration["backend_url"] ?? throw new ArgumentNullException("connectionstrings:backend_url")
    ));
});

// --------------------------------------------------------------------
// 6. CORS Configuration
// --------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("RenderPolicy", policy =>
    {
        // Ensure that your production frontend URL is included.
        policy.WithOrigins("https://moksha-app-frontend.onrender.com", "http://localhost", "https://moksha-app-backend.onrender.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// --------------------------------------------------------------------
// 7. Middleware Pipeline Configuration
// --------------------------------------------------------------------
if (app.Environment.IsProduction())
{
    // Process forwarded headers from the reverse proxy (e.g. X-Forwarded-For, X-Forwarded-Proto)
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

app.UseStaticFiles();

// Routing must come before authentication/authorization
app.UseRouting();

app.UseCors("RenderPolicy");

app.UseAuthentication();
app.UseAuthorization();

// --------------------------------------------------------------------
// 8. Endpoint Routing Configuration
// --------------------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// --------------------------------------------------------------------
// 9. Production-specific Settings (Port Binding)
// --------------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    // Render provides a PORT environment variable.
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Urls.Add($"http://0.0.0.0:{port}");
    Console.WriteLine($"[INFO] Running on port {port}");
}

app.Run();
