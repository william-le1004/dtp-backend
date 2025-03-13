using Application.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Contexts;

public class AppDbContextFactory : IDesignTimeDbContextFactory<DtpDbContext>
{
    public DtpDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DtpDbContext>();
        optionsBuilder.UseMySQL("Server=MYSQL1001.site4now.net;Database=db_ab3495_dtp;Uid=ab3495_dtp;Pwd=dtpct123;");

        var dummyUserContext = new DummyUserContextService();

        return new DtpDbContext(optionsBuilder.Options, dummyUserContext);
    }
}

public class DummyUserContextService : IUserContextService
{
    public string GetCurrentUserId() => "system";

    public List<string> GetCurrentUserRoles()
        => new List<string> { "Admin" };

    public bool IsAdminRole()
    {
        throw new NotImplementedException();
    }

    public bool IsOperatorRole()
    {
        throw new NotImplementedException();
    }

    public string? GetAccessToken()
    {
        throw new NotImplementedException();
    }

    public Guid? GetCompanyId()
    {
        return Guid.Empty;
    }
}