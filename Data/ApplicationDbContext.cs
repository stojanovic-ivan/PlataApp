using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PlataApp.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)  { }

    public DbSet<Radnik> Radnici { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Radnik>(entity =>
        {
            // define NetoPlata as decimal with 18 digits of which 2 are decimals
            entity.Property(e => e.NetoPlata)
                .HasColumnType("decimal(18, 2)");
        });
    }

}
