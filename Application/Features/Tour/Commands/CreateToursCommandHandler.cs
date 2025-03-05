using Application.Contracts.Persistence;
using Application.Common;
using Application.Dtos;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Commands
{
    public record DestinationToAdd(
        Guid DestinationId,
        TimeSpan StartTime,
        TimeSpan EndTime,
        int? SortOrder = null,
        int? SortOrderByDate = null
    );

    public record TicketToAdd(
        decimal DefaultNetCost,
        int MinimumPurchaseQuantity,
        int TicketKind
    );

    public record CreateTourCommand(
        string Title,
        Guid? CompanyId,
        Guid? Category,
        string? Description,
        List<DestinationToAdd>? Destinations,
        List<TicketToAdd>? Tickets,
        DateTime OpenDay,
        DateTime CloseDay
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
                    var tourDestination = new TourDestination(tour.Id, dest.DestinationId, dest.StartTime, dest.EndTime, dest.SortOrder, dest.SortOrderByDate);
                    tour.TourDestinations.Add(tourDestination);
                }
            }

            if (request.Tickets is not null)
            {
                foreach (var ticket in request.Tickets)
                {
                    var ticketKind = (TicketKind)ticket.TicketKind;
                    var ticketType = new TicketType(ticket.DefaultNetCost, ticket.MinimumPurchaseQuantity, ticketKind, tour.Id);
                    tour.Tickets.Add(ticketType);
                }
            }

            for (DateTime day = request.OpenDay.Date; day <= request.CloseDay.Date; day = day.AddDays(1))
            {
                var schedule = new TourSchedule();
                var dbContext = _context as DbContext;
                if (dbContext == null)
                {
                    throw new Exception("The IDtpDbContext instance is not a DbContext. Ensure your context implements Microsoft.EntityFrameworkCore.DbContext.");
                }
                dbContext.Entry(schedule).Property("TourId").CurrentValue = tour.Id;
                dbContext.Entry(schedule).Property("StartDate").CurrentValue = day;
                dbContext.Entry(schedule).Property("EndDate").CurrentValue = day;
                foreach (var ticketType in tour.Tickets)
                {
                    var scheduleTicket = new TourScheduleTicket(
                        Guid.NewGuid(),
                        ticketType.DefaultNetCost,
                        ticketType.DefaultTax,
                        100,
                        ticketType.Id,
                        schedule.Id
                    );
                    schedule.AddTicket(scheduleTicket);
                }
                tour.TourSchedules.Add(schedule);
            }

            _context.Tours.Add(tour);
            await _context.SaveChangesAsync(cancellationToken);

            var tourResponse = new TourResponse(tour.Id, tour.Title, tour.CompanyId, tour.Category, tour.Description);
            return ApiResponse<TourResponse>.SuccessResult(tourResponse, "Tour created successfully with destinations, tickets, and daily schedules");
        }
    }
}
