using Infrastructure.Contexts;

namespace Api.Middlewares;

public class TransactionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, DtpDbContext dbContext)
    {
        if (context.Request.Method == HttpMethods.Post ||
            context.Request.Method == HttpMethods.Put ||
            context.Request.Method == HttpMethods.Delete)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                await next(context);

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        else
        {
            await next(context);
        }
    }
}