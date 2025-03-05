
using Application.Contracts.Persistence;
using Application.Common;
using Application.Dtos;
using Domain.Entities;
using Domain.Enum;
using MediatR;


namespace Application.Features.Tour.Commands;


public record DestinationToAdd(
    Guid DestinationId,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int? SortOrder = null,
    int? SortOrderByDate = null
);

public record TicketToAdd(
    decimal DefaultNetCost,
    double DefaultTax,
    int MinimumPurchaseQuantity,
    int TicketKind 
);

public record CreateTourCommand(
    string Title,
    Guid? CompanyId,
    Guid? Category,
    string? Description,
    List<DestinationToAdd>? Destinations,
    List<TicketToAdd>? Tickets
) : IRequest<ApiResponse<TourResponse>>;

public class CreateTourHandler : IRequestHandler<CreateTourCommand, ApiResponse<TourResponse>>
{
    private readonly IDtpDbContext _context;

    public CreateTourHandler(IDtpDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<TourResponse>> Handle(CreateTourCommand request, CancellationToken cancellationToken)
    {
        var tour = new Domain.Entities.Tour(request.Title, request.CompanyId, request.Category, request.Description);

        if (request.Destinations is not null)
        {
            foreach (var dest in request.Destinations)
            {
                var tourDestination = new TourDestination(tour.Id, dest.DestinationId, dest.StartTime, dest.EndTime, dest.SortOrder,dest.SortOrderByDate);
                tour.TourDestinations.Add(tourDestination);
            }
        }

        if (request.Tickets is not null)
        {
            foreach (var ticket in request.Tickets)
            {
                
                var ticketKind = (TicketKind)ticket.TicketKind;
                var ticketType = new TicketType(ticket.DefaultNetCost, ticket.DefaultTax, ticket.MinimumPurchaseQuantity, ticketKind, tour.Id);
                tour.Tickets.Add(ticketType);
            }
        }

      
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync(cancellationToken);

        var tourResponse = new TourResponse(tour.Id, tour.Title, tour.CompanyId, tour.Category, tour.Description);
        return ApiResponse<TourResponse>.SuccessResult(tourResponse, "Tour created successfully with destinations and tickets");
    }
}