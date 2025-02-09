using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moksha_App.Areas.Identity.Data;
using Moksha_App.Controllers;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DBContextIdentityConnection") ?? throw new InvalidOperationException("Connection string 'DBContextIdentityConnection' not found.");

builder.Services.AddDbContext<DBContextIdentity>(options => options.UseSqlServer(connectionString));

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

pu
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
