using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

using Moksha_App.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Disable Data Protection (use this if your app doesn't need it)
builder.Services.AddDataProtection()
    .DisableAutomaticKeyGeneration();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.LogoutPath = "/Auth/Logout";
                options.AccessDeniedPath = "/Auth/AccessDenied";
            });
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new GlobalTokenAuthorizationFilter(
        builder.Configuration["Jwt:Key"],
        builder.Configuration["Jwt:Issuer"],
        builder.Configuration["Jwt:Audience"]
    ));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();
app.UseCors("AllowAllOrigins"); // Enable CORS

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}");
app.Run();
