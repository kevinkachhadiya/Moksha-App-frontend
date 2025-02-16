using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Moksha_App.Controllers;
using Moksha_App.Models;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------------------
// 1. Load Environment Variables (explicit, though by default they load)
// --------------------------------------------------------------------
builder.Configuration.AddEnvironmentVariables();
var env = builder.Environment;

// --------------------------------------------------------------------
// 1. KeepAlive Service Configuration
// --------------------------------------------------------------------
builder.Services.AddHostedService<KeepAliveService>();

// --------------------------------------------------------------------
// 2. Cookie Authentication Configuration
// --------------------------------------------------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Ensure that your AuthController's Login action is decorated with [AllowAnonymous]

        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.SameSite = SameSiteMode.None; // Needed for cross-site cookies
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;

    });
// --------------------------------------------------------------------
// 3. JWT & Global Filter Configuration
// --------------------------------------------------------------------
var jwtConfig = builder.Configuration.GetSection("Jwt");

// Fetch Backend URL from Environment Variable (for Render) or appsettings.json (for local)
var backendUrl = Environment.GetEnvironmentVariable("backend_url") 
                 ?? builder.Configuration["BackendUrl"]  
                 ?? throw new ArgumentNullException("BackendUrl is not set.");

// Store in Configuration for Global Access
// Store in Configuration (in-memory, not modifying appsettings.json)
builder.Configuration["BackendUrl"] = backendUrl;


// Log Backend URL for debugging
Console.WriteLine($"Using Backend URL: {builder.Configuration["BackendUrl"]}");

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new GlobalTokenAuthorizationFilter(backendUrl));
});


// --------------------------------------------------------------------
// 6. CORS Configuration
// --------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("RenderPolicy", policy =>
    {
        // Ensure that your production frontend URL is included.
        policy.WithOrigins("https://moksha-app-frontend.onrender.com", "http://localhost", "https://moksha-app-backend.onrender.com/api")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(origin => true); // ✅ Add this to handle credentials correctly
    });
});

builder.Logging.AddConsole();
// Set the default culture
var supportedCultures = new[] { new CultureInfo("en-US") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

var app = builder.Build();

app.UseRequestLocalization(localizationOptions);

// --------------------------------------------------------------------
// 7. Middleware Pipeline Configuration
// --------------------------------------------------------------------
if (app.Environment.IsProduction())
{
    // Process forwarded headers from the reverse proxy (e.g. X-Forwarded-For, X-Forwarded-Proto)
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        RequireHeaderSymmetry = false,
        ForwardLimit = null
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
    app.UseCors("RenderPolicy");
    // Render provides a PORT environment variable.
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.Configuration["Kestrel:Endpoints:Http:Url"] = $"http://0.0.0.0:{port}";

    app.Urls.Add($"http://0.0.0.0:{port}");
    Console.WriteLine($"[INFO] Running on port {port}");
}

app.Run();
