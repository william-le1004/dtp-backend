using Application.Contracts.Persistence;
using Application.Common;
using Application.Dtos;
using Domain.Entities;
using Domain.Enum;
using MediatR;


namespace Application.Features.Tour.Commands;

// DTO cho Destination đã được cập nhật với TimeOnly cho thời gian
public record DestinationToAdd(
    Guid DestinationId,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int? SortOrder = null
);

// DTO cho Ticket cần thêm vào Tour
public record TicketToAdd(
    decimal DefaultNetCost,
    double DefaultTax,
    int MinimumPurchaseQuantity,
    int TicketKind // Map sang enum TicketKind
);

// Command để tạo Tour kèm danh sách Destination và Ticket
public record CreateTourCommand(
    string Title,
    Guid? CompanyId,
    Guid? Category,
    string? Description,
    List<DestinationToAdd>? Destinations,
    List<TicketToAdd>? Tickets
) : IRequest<ApiResponse<TourResponse>>;

// Handler xử lý logic tạo Tour
public class CreateTourHandler : IRequestHandler<CreateTourCommand, ApiResponse<TourResponse>>
{
    private readonly IDtpDbContext _context;

    public CreateTourHandler(IDtpDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<TourResponse>> Handle(CreateTourCommand request, CancellationToken cancellationToken)
    {
        // Tạo Tour mới (sử dụng constructor công khai của Tour)
        var tour = new Domain.Entities.Tour (request.Title, request.CompanyId, request.Category, request.Description);

        // Thêm danh sách Destination vào Tour nếu có
        if (request.Destinations is not null)
        {
            foreach (var dest in request.Destinations)
            {
                // Giả sử TourDestination có constructor tương thích với TimeOnly
                var tourDestination = new TourDestination(tour.Id, dest.DestinationId, dest.StartTime, dest.EndTime, dest.SortOrder);
                tour.TourDestinations.Add(tourDestination);
            }
        }

        // Thêm danh sách Ticket vào Tour nếu có
        if (request.Tickets is not null)
        {
            foreach (var ticket in request.Tickets)
            {
                // Chuyển đổi TicketKind từ int sang enum (giả sử enum TicketKind có sẵn)
                var ticketKind = (TicketKind)ticket.TicketKind;
                var ticketType = new TicketType(ticket.DefaultNetCost, ticket.DefaultTax, ticket.MinimumPurchaseQuantity, ticketKind, tour.Id);
                tour.Tickets.Add(ticketType);
            }
        }

        // Thêm Tour vào DbContext và lưu thay đổi
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync(cancellationToken);

        // Tạo DTO response
        var tourResponse = new TourResponse(tour.Id, tour.Title, tour.CompanyId, tour.Category, tour.Description);
        return ApiResponse<TourResponse>.SuccessResult(tourResponse, "Tour created successfully with destinations and tickets");
    }
}
