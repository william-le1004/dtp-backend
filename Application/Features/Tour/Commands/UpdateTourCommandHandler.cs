using Application.Contracts.Persistence;
using Application.Common;
using Application.Dtos;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Features.Tour.Commands;

// Command PUT: cập nhật toàn bộ thông tin Tour, bao gồm các Destination và Ticket
public record PutTourCommand(
    Guid TourId,
    string Title,
    Guid? CompanyId,
    Guid? Category,
    string? Description,
    List<DestinationToAdd>? Destinations,
    List<TicketToAdd>? Tickets
) : IRequest<ApiResponse<TourResponse>>;

public class PutTourHandler : IRequestHandler<PutTourCommand, ApiResponse<TourResponse>>
{
    private readonly IDtpDbContext _context;

    public PutTourHandler(IDtpDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<TourResponse>> Handle(PutTourCommand request, CancellationToken cancellationToken)
    {
        // Lấy Tour kèm theo danh sách Destination và Ticket
        var tour = await _context.Tours
            .Include(t => t.TourDestinations)
            .Include(t => t.Tickets)
            .FirstOrDefaultAsync(t => t.Id == request.TourId, cancellationToken);

        if (tour == null)
        {
            return ApiResponse<TourResponse>.Failure("Tour not found", 404);
        }

        // Cập nhật các trường cơ bản
        tour.Update(request.Title, request.CompanyId, request.Category, request.Description);

        // Cập nhật danh sách Destination:
        // Xóa toàn bộ Destination cũ và thêm mới theo yêu cầu
        tour.TourDestinations.Clear();
        if (request.Destinations is not null)
        {
            foreach (var dest in request.Destinations)
            {
                // Giả sử TourDestination có constructor:
                // TourDestination(Guid tourId, Guid destinationId, TimeOnly startTime, TimeOnly endTime, int? sortOrder)
                var tourDestination = new TourDestination(tour.Id, dest.DestinationId, dest.StartTime, dest.EndTime, dest.SortOrder);
                tour.TourDestinations.Add(tourDestination);
            }
        }

        // Cập nhật danh sách Ticket:
        // Xóa toàn bộ Ticket cũ và thêm mới theo yêu cầu
        tour.Tickets.Clear();
        if (request.Tickets is not null)
        {
            foreach (var ticket in request.Tickets)
            {
                // Chuyển đổi int sang enum TicketKind
                var ticketKind = (TicketKind)ticket.TicketKind;
                // Giả sử TicketType có constructor:
                // TicketType(decimal defaultNetCost, double defaultTax, int minimumPurchaseQuantity, TicketKind ticketKind, Guid tourId)
                var ticketType = new TicketType(ticket.DefaultNetCost, ticket.DefaultTax, ticket.MinimumPurchaseQuantity, ticketKind, tour.Id);
                tour.Tickets.Add(ticketType);
            }
        }

        _context.Tours.Update(tour);
        await _context.SaveChangesAsync(cancellationToken);

        // Tạo DTO response
        var tourResponse = new TourResponse(tour.Id, tour.Title, tour.CompanyId, tour.Category, tour.Description);
        return ApiResponse<TourResponse>.SuccessResult(tourResponse, "Tour updated successfully with all destinations and tickets");
    }
}
