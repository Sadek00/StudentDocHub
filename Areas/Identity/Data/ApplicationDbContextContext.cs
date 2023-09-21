using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentDocVault.Areas.Identity.Data;
using StudentDocVault.Models;

namespace StudentDocVault.Data;

public class ApplicationDbContextContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContextContext(DbContextOptions<ApplicationDbContextContext> options)
        : base(options)
    {
    }
    public DbSet<Document> Document { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>()
            .HasIndex(e => e.StudentId)
            .IsUnique();
    }
}
