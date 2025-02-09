using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Moksha_App.Controllers;
using Moksha_App.Models;
using System.IO;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Get the current hosting environment
var env = builder.Environment;

// 1. Data Protection Configuration with encryption
// Choose a keys folder based on the environment:
// - Development: use a folder within your project
// - Production: use a persistent storage folder (e.g. on Render, if available)
string keysPath = env.IsDevelopment()
    ? Path.Combine(env.ContentRootPath, "DataProtection-Keys")
    : "/var/data-protection-keys";

// Ensure the keys directory exists
if (!Directory.Exists(keysPath))
{
    Directory.CreateDirectory(keysPath);
    Console.WriteLine($"[INFO] Created Data Protection Keys directory: {keysPath}");
}

// Load the certificate for protecting the keys
var certificate = GetCertificate(builder);

// Configure Data Protection to persist keys and encrypt them using the certificate
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("Moksha_App")
    .ProtectKeysWithCertificate(certificate);
// Note: If you remove ProtectKeysWithCertificate(certificate), keys will be stored unencrypted,
// and the warning "No XML encryptor configured" will disappear—but then your keys are not protected.

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
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new GlobalTokenAuthorizationFilter(
        jwtConfig["Key"] ?? throw new ArgumentNullException("Jwt:Key"),
        jwtConfig["Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer"),
        jwtConfig["Audience"] ?? throw new ArgumentNullException("Jwt:Audience")
    ));
});

// 5. CORS Configuration
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}");

// 7. Render-specific configuration: Bind to the port provided by the environment
if (!app.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Urls.Clear();
    app.Urls.Add($"http://0.0.0.0:{port}");
    Console.WriteLine($"[INFO] Running on port {port}");
}

app.Run();


// ----------------------------------------------------------------
// Certificate loading implementation
// ----------------------------------------------------------------
static X509Certificate2 GetCertificate(WebApplicationBuilder builder)
{
    if (builder.Environment.IsDevelopment())
    {
        // In development, load a self-signed certificate from a local file.
        string certPath = Path.Combine(builder.Environment.ContentRootPath, "localhost.pfx");

        if (!File.Exists(certPath))
        {
            throw new FileNotFoundException(
                $"Development certificate not found: {certPath}. " +
                "Generate it with: dotnet dev-certs https -ep localhost.pfx -p localhost"
            );
        }

        Console.WriteLine($"[INFO] Loading development certificate from: {certPath}");
        return new X509Certificate2(
            certPath,
            "localhost", // Ensure this matches the password used with dev-certs
            X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable
        );
    }
    else
    {
        // In production, load the certificate from environment variables.
        var certContent = Environment.GetEnvironmentVariable("CERTIFICATE_CONTENT");
        var certPassword = Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD");

        if (string.IsNullOrWhiteSpace(certContent) || string.IsNullOrWhiteSpace(certPassword))
        {
            throw new InvalidOperationException(
                "Production certificate not configured. Ensure these environment variables are set:\n" +
                "CERTIFICATE_CONTENT - Base64 encoded .pfx file\n" +
                "CERTIFICATE_PASSWORD - Certificate password"
            );
        }

        try
        {
            var certBytes = Convert.FromBase64String(certContent);
            Console.WriteLine("[INFO] Successfully loaded production certificate.");
            return new X509Certificate2(
                certBytes,
                certPassword,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable
            );
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Failed to load production certificate. Verify:\n" +
                "1. CERTIFICATE_CONTENT is valid base64\n" +
                "2. CERTIFICATE_PASSWORD is correct\n" +
                "3. The certificate is not expired", ex);
        }
    }
}
