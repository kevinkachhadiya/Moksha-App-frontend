using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.HttpOverrides;
using Moksha_App.Controllers;
using Moksha_App.Models;

var builder = WebApplication.CreateBuilder(args);

// Ensure environment variables are added (by default they are, but this is explicit)
builder.Configuration.AddEnvironmentVariables();

// Get the current hosting environment
var env = builder.Environment;

// ------------------------------------------
// 1. Data Protection Configuration
// ------------------------------------------
// If running in a container, consider persisting keys to a volume or using an external key store.
builder.Services.AddDataProtection()
    .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
    {
        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
    });

// ------------------------------------------
// 2. KeepAlive Service Configuration
// ------------------------------------------
builder.Services.AddHostedService<KeepAliveService>();

// ------------------------------------------
// 3. Cookie Authentication Configuration
// ------------------------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Ensure the login action is accessible anonymously (decorate your Login action with [AllowAnonymous])
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// ------------------------------------------
// 4. JWT & Global Filter Configuration
// ------------------------------------------
// These configuration values are loaded from your configuration providers (appsettings, environment variables, etc.)
var jwtConfig = builder.Configuration.GetSection("Jwt");
var backend = builder.Configuration.GetSection("connectionstrings");

// The GlobalTokenAuthorizationFilter might be rejecting calls if required parameters are missing
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new GlobalTokenAuthorizationFilter(
        jwtConfig["Key"] ?? throw new ArgumentNullException("Jwt:Key"),
        jwtConfig["Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer"),
        jwtConfig["Audience"] ?? throw new ArgumentNullException("Jwt:Audience"),
        backend["backend_url"] ?? throw new ArgumentNullException("can not find backend url")
    ));
});
// ------------------------------------------
// 5. CORS Configuration
// ------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("RenderPolicy", policy =>
    {
        // Make sure that your production frontend URL is listed here.
        policy.WithOrigins("https://moksha-app-frontend.onrender.com", "http://localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// ------------------------------------------
// 6. Middleware Pipeline Configuration
// ------------------------------------------
if (app.Environment.IsProduction())
{
    // If behind a reverse proxy, forward headers (e.g. X-Forwarded-For)
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.All
    });
    // Global exception handling & HSTS in production
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // In non-production environments, show detailed error pages
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
}

// Serve static files (css, js, images, etc.)
app.UseStaticFiles();

// IMPORTANT: Routing must come before authentication/authorization
app.UseRouting();

// Apply CORS policy
app.UseCors("RenderPolicy");

// Authenticate and authorize
app.UseAuthentication();
app.UseAuthorization();

// ------------------------------------------
// 7. Endpoint Routing Configuration
// ------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// ------------------------------------------
// 8. Production-specific Settings
// ------------------------------------------
if (!app.Environment.IsDevelopment())
{
    // Ensure that the environment provides a PORT and backend URL.
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    var backendUrl = Environment.GetEnvironmentVariable("backend_url");
    if (!string.IsNullOrEmpty(backendUrl))
    {
        builder.Configuration["connectionstrings:backend_url"] = backendUrl;
    }
    app.Urls.Add($"http://0.0.0.0:{port}");
    Console.WriteLine($"[INFO] Running on port {port}");
}

app.Run();
