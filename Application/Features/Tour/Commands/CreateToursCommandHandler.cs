using Application.Common;
using Application.Contracts;
using Application.Contracts.Persistence;
using Application.Dtos;
using Domain.DataModel;
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
        List<DestinationActivityToAdd>? DestinationActivities,
        TimeSpan? StartTime,
        TimeSpan? EndTime,
        int? SortOrder = null,
        int? SortOrderByDate = null,
        List<string>? Img = null
    );

    public record TicketToAdd(
        decimal DefaultNetCost,
        int MinimumPurchaseQuantity,
        int TicketKind
    );

    public record DestinationActivityToAdd(
        string? Name,
        TimeSpan? StartTime,
        TimeSpan? EndTime,
        int? SortOrder = null
    );

    public record CreateTourCommand(
        string Title,
        Guid? Categoryid,
        string? Description,
        List<DestinationToAdd>? Destinations,
        List<TicketToAdd>? Tickets,
        DateTime OpenDay,
        DateTime CloseDay,
        string ScheduleFrequency,
        List<string>? Img,
        string About,
        string? Include = null,
        string? Pickinfor = null
    ) : IRequest<ApiResponse<TourResponse>>;

    public class CreateTourHandler : IRequestHandler<CreateTourCommand, ApiResponse<TourResponse>>
    {
        private readonly IDtpDbContext _context;
        private readonly IUserContextService _userContextService;

        public CreateTourHandler(IDtpDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        private async Task<string> GenerateTourCode(Guid? companyId, CancellationToken cancellationToken)
        {
            if (!companyId.HasValue || companyId.Value == Guid.Empty)
                throw new Exception("CompanyId is null or empty. Ensure the user has an associated company.");

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == companyId, cancellationToken);
            if (company == null)
                throw new Exception($"No company found with Id: {companyId}");

            string abbreviation = company.Name.Length >= 2
                ? company.Name.Substring(company.Name.Length - 2).ToUpper()
                : company.Name.ToUpper();
            string year = DateTime.Now.Year.ToString();

            var count = await _context.Tours.CountAsync(t => t.CompanyId == companyId, cancellationToken);
            int sequenceNumber = count + 1;
            string sequenceStr = sequenceNumber.ToString("D4");

            return $"{abbreviation}{year}{sequenceStr}";
        }

        public async Task<ApiResponse<TourResponse>> Handle(CreateTourCommand request, CancellationToken cancellationToken)
        {
            var companyId = _userContextService.GetCompanyId();
            string tourCode = await GenerateTourCode(companyId, cancellationToken);

            // Tạo Tour mới và gán Code
            var tour = new Domain.Entities.Tour(request.Title, companyId, request.Categoryid, request.Description, tourCode, request.About, request.Include, request.Pickinfor);
            foreach ( var img in request.Img)
            {
                _context.ImageUrls.Add(new ImageUrl(tour.Id,img));
            }
           
            if (request.Destinations is not null)
            {
                foreach (var dest in request.Destinations)
                {
                    // Sử dụng null-coalescing để ép giá trị của StartTime và EndTime nếu null (có thể thay TimeSpan.Zero thành giá trị mặc định phù hợp)
                    var tourDestination = new TourDestination(
                        tour.Id,
                        dest.DestinationId,
                        dest.StartTime ?? TimeSpan.Zero,
                        dest.EndTime ?? TimeSpan.Zero,
                        dest.SortOrder,
                        dest.SortOrderByDate
                    );
                    tour.TourDestinations.Add(tourDestination);
                    foreach (var img in dest.Img)
                    {
                        _context.ImageUrls.Add(new ImageUrl(tourDestination.Id, img));
                    }

                    if (dest.DestinationActivities is not null)
                    {
                        foreach (var activity in dest.DestinationActivities)
                        {
                            var destinationActivity = new DestinationActivity(
                                tourDestination.Id,
                                activity.Name,
                                activity.StartTime ?? TimeSpan.Zero,
                                activity.EndTime ?? TimeSpan.Zero,
                                activity.SortOrder
                            );
                            tourDestination.DestinationActivities.Add(destinationActivity);
                        }
                    }
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

            var dbContext = _context as DbContext;
            if (dbContext == null)
            {
                throw new Exception("The IDtpDbContext instance is not a DbContext. Ensure your context implements Microsoft.EntityFrameworkCore.DbContext.");
            }

            DateTime currentDay = request.OpenDay.Date;
            Func<DateTime, DateTime> stepFunc = request.ScheduleFrequency.ToLower() switch
            {
                "weekly" => d => d.AddDays(7),
                "monthly" => d => d.AddMonths(1),
                _ => d => d.AddDays(1)
            };
            var largestSortOrder = tour.TourDestinations.Any() ? tour.TourDestinations.Max(d => d.SortOrderByDate) ?? 0 : 0;
            while (currentDay <= request.CloseDay.Date)
            {
                var schedule = new TourSchedule();
                dbContext.Entry(schedule).Property("TourId").CurrentValue = tour.Id;
                dbContext.Entry(schedule).Property("OpenDate").CurrentValue = currentDay;
                dbContext.Entry(schedule).Property("CloseDate").CurrentValue = currentDay.AddDays(largestSortOrder);

                foreach (var ticketType in tour.Tickets)
                {
                    var scheduleTicket = new TourScheduleTicket(ticketType.DefaultNetCost, 100, ticketType.Id, schedule.Id);
                    schedule.AddTicket(scheduleTicket);
                }

                tour.TourSchedules.Add(schedule);
                currentDay = stepFunc(currentDay);
            }

            _context.Tours.Add(tour);
            await _context.SaveChangesAsync(cancellationToken);

            var tourResponse = new TourResponse(tour.Id, tour.Title, tour.CompanyId, tour.CategoryId, tour.Description, tour.About, tour.Include, tour.Pickinfor, tour.IsDeleted);
            return ApiResponse<TourResponse>.SuccessResult(tourResponse, "Tour created successfully with destinations, tickets, schedules and code: " + tourCode);
        }
    }
}
