using Application.Contracts.Persistence;
using Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.DomainEvents;

public class OrderCompletedHandler(IDtpDbContext context, IUserRepository repository) : INotificationHandler<OrderCompleted>
{
    public async Task Handle(OrderCompleted notification, CancellationToken cancellationToken)
    {
        var tourSchedule = await context.TourSchedules
            .Include(x=> x.Tour)
            .Include(x=> x.TourBookings)
            .ThenInclude(x=> x.Tickets)
            .AsNoTracking()
            .FirstOrDefaultAsync(x=> x.Id == notification.TourScheduleId, cancellationToken);

        if (tourSchedule is not null)
        {
            var user = await repository.GetOperatorByCompanyId(tourSchedule.Tour.CompanyId ?? Guid.Empty);
            var admin = await repository.GetAdmin();
            
            var receiveWallet = user.Wallet;
            var poolFund = admin.Wallet;
            
            context.Wallets.AttachRange(receiveWallet, poolFund);

            var amount = (tourSchedule.GrossSettlementCost() * (decimal)user.Company.CommissionRate) / 100m;
            var description =
                $"Tất toán tour {tourSchedule.Tour.Code} ngày {tourSchedule.OpenDate} Touschedule Id : {tourSchedule.Id}";
            
            
            poolFund.Transfer(receiveWallet, amount,description);
            
            context.Wallets.UpdateRange(receiveWallet, poolFund);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}