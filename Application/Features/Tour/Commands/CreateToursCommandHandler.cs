using Application.Common;
using Application.Contracts.Persistence;
using Application.Dtos;
using MediatR;

namespace Application.Features.Tour.Commands;

public record CreateToursCommand(string Title, Guid? CompanyId, Guid? Category, string? Description)
    : IRequest<ApiResponse<TourResponse>>;

public class CreateToursCommandHandler : IRequestHandler<CreateToursCommand, ApiResponse<TourResponse>>
{
    private readonly IDtpDbContext _context;

    public CreateToursCommandHandler(IDtpDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<TourResponse>> Handle(CreateToursCommand request, CancellationToken cancellationToken)
    {
        var tour = new Domain.Entities.Tour(request.Title, request.CompanyId, request.Category, request.Description);

        _context.Tours.Add(tour);
        await _context.SaveChangesAsync(cancellationToken);
        
        var tourResponse = new TourResponse(tour.Id, tour.Title, tour.CompanyId, tour.Category, tour.Description);

        return ApiResponse<TourResponse>.SuccessResult(tourResponse, "Tour created successfully");
    }
}