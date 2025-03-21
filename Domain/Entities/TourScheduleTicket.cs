using Domain.DataModel;

namespace Domain.Entities;

public class TourScheduleTicket : SoftDeleteEntity
{
    public decimal NetCost { get; private set; }

    public int AvailableTicket { get; private set; }

    public int Capacity { get; private set; }
    public Guid TicketTypeId { get; private set; }
    public TicketType TicketType { get; private set; } = null!;
    public Guid TourScheduleId { get; private set; }
    public TourSchedule TourSchedule { get; private set; } = null!;

    public bool IsAvailable() => AvailableTicket > 0;
    public bool HasAvailableTicket(int quantity) => AvailableTicket > quantity;

    public TourScheduleTicket(decimal netCost, int capacity, 
        Guid ticketTypeId, Guid tourScheduleId, bool isDeleted = false)
    {
        NetCost = netCost;
        Capacity = capacity;
        TicketTypeId = ticketTypeId;
        TourScheduleId = tourScheduleId;
        AvailableTicket = Capacity;
        IsDeleted = isDeleted;
    }

    public void CalAvailableTicket(int orderedTickets)
    {
        AvailableTicket = Capacity - orderedTickets;
    }
}