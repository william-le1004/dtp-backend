using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Contexts;

public class AppDbContextFactory : IDesignTimeDbContextFactory<DtpDbContext>
{
    public DtpDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DtpDbContext>();
        optionsBuilder.UseMySQL("Server=MYSQL1001.site4now.net;Database=db_ab3495_dtp;Uid=ab3495_dtp;Pwd=dtpct123");

        return new DtpDbContext(optionsBuilder.Options);
    }
}