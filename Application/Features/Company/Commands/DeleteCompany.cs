using Application.Common;
using Application.Contracts.Persistence;
using MediatR;

namespace Application.Features.Company.Commands;

public record DeleteCompanyCommand(Guid Id) : IRequest<ApiResponse<bool>>;

public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, ApiResponse<bool>>
{
    private readonly ICompanyRepository _companyRepository;

    public DeleteCompanyCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetCompanyAsync(request.Id);

        if (company is null)
            return ApiResponse<bool>.Failure("Company not found");

        try
        {
            company.Delete();
            await _companyRepository.UpsertCompanyAsync(company);
            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"An error occurred: {ex.Message}");
        }
    }
}