using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moksha_App.Areas.Identity.Data;
using Moksha_App.Models;

namespace Moksha_App.Areas.Identity.Data;

public class DBContextIdentity : IdentityDbContext<Db_User>
{
    public DBContextIdentity(DbContextOptions<DBContextIdentity> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    public DbSet<Moksha_App.Models.User> User { get; set; } = default!;

    public DbSet<Moksha_App.Models.Stock> Stock { get; set; } = default!;

    public DbSet<Moksha_App.Models.Material> Material { get; set; } = default!;

    public DbSet<Moksha_App.Models.B_Bill> B_Bill { get; set; } = default!;
}
