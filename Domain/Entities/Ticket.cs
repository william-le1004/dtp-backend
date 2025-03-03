using Domain.Extensions;

namespace Domain.Entities;

public class Ticket
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public Guid TourScheduleTicketId { get; private set; }
    public TourScheduleTicket TourScheduleTicket { get; private set; }
    public int Quantity { get; private set; }
    public decimal GrossCost { get; private set; }
    public Guid TourBookingId { get; private set; }
    public virtual TourBooking TourBooking { get; private set; }

    public Ticket(Guid tourScheduleTicketId, int quantity, decimal grossCost)
    {
        Code = (DateTimeOffset.Now.ToString("ffffff")
                + tourScheduleTicketId.ToString("N").Substring(0, 8)).Random();
        TourScheduleTicketId = tourScheduleTicketId;
        Quantity = quantity;
        GrossCost = grossCost;
    }

    public void AddUnit(int quantity)
    {
        Quantity += quantity;
    }
}