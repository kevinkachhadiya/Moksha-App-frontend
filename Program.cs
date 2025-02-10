using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.HttpOverrides;
using Moksha_App.Controllers;
using Moksha_App.Models;
var builder = WebApplication.CreateBuilder(args);

// Get the current hosting environment
var env = builder.Environment;

builder.Services.AddDataProtection().UseCryptographicAlgorithms(
    new AuthenticatedEncryptorConfiguration
    {
        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
    });

// 2. KeepAlive Service Configuration
builder.Services.AddHostedService<KeepAliveService>();

// 3. Authentication Configuration (using Cookies)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// 4. JWT Configuration
var jwtConfig = builder.Configuration.GetSection("Jwt");
var backend = builder.Configuration.GetSection("connectionstrings");
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new GlobalTokenAuthorizationFilter(
        jwtConfig["Key"] ?? throw new ArgumentNullException("Jwt:Key"),
        jwtConfig["Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer"),
        jwtConfig["Audience"] ?? throw new ArgumentNullException("Jwt:Audience"),
        backend["backend_url"] ?? throw new ArgumentNullException("can not find backend url")
    ));
});

// 5. CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("RenderPolicy", policy =>
    {
        policy.WithOrigins("https://moksha-app-frontend.onrender.com", "http://localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// 6. Middleware Pipeline Configuration
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

app.UseStaticFiles();

app.UseRouting();
app.UseCors("RenderPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}");

// 7. Render-specific configuration: Bind to the port provided by the environment
if (!app.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    var backend_url = Environment.GetEnvironmentVariable("backend_url");
    builder.Configuration["connectionstrings:backend_url"] = backend_url;
    app.Urls.Add($"http://0.0.0.0:{port}");
    Console.WriteLine($"[INFO] Running on port {port}");
}

app.Run();


