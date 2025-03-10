using Application.Common;
using Application.Contracts.Persistence;
using MediatR;

namespace Application.Features.Company.Commands;

public record DeleteCompanyCommand(Guid Id) : IRequest<ApiResponse<bool>>;

public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, ApiResponse<bool>>
{
    private readonly IDtpDbContext _context;

    public DeleteCompanyCommandHandler(IDtpDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.FindAsync(request.Id);

        if (company is null)
            return ApiResponse<bool>.Failure("Company not found");

        company.Delete();

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResult(true);
    }
}