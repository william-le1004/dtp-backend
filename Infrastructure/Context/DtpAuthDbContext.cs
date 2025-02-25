using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class DtpAuthDbContext: IdentityDbContext
{
    public DtpAuthDbContext()
    {
    }

    public DtpAuthDbContext(DbContextOptions<DtpAuthDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}