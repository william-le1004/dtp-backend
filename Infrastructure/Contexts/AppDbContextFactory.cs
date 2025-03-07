using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Contexts;

public class AppDbContextFactory : IDesignTimeDbContextFactory<DtpDbContext>
{
    public DtpDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DtpDbContext>();
        optionsBuilder.UseMySQL("Server=localhost;Database=dtp;Uid=root;Pwd=123456");

        return new DtpDbContext(optionsBuilder.Options);
    }
}