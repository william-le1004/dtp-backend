using Application.Contracts.Persistence;
using Functional.Option;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Voucher.Commands;

public record DeleteVoucherCommand(Guid Id) : IRequest<Option<Guid>>;

public class DeleteVoucherCommandHandler(IDtpDbContext context) : IRequestHandler<DeleteVoucherCommand, Option<Guid>>
{
    public async Task<Option<Guid>> Handle(DeleteVoucherCommand request, CancellationToken cancellationToken)
    {
        var persistenceVoucher = await context.Voucher
            .FirstOrDefaultAsync(x=> x.Id == request.Id, cancellationToken);
        if (persistenceVoucher is not null)
        {
            persistenceVoucher.IsDeleted = true;
            await context.SaveChangesAsync(cancellationToken);
            return Option<Guid>.Some(persistenceVoucher.Id);
        }
        return Option.None;
    }
}
