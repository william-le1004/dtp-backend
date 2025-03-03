namespace Domain.Entities;

public class TourBasketItem(Guid tourScheduleId, Guid tourScheduleTicketId, int quantity)
{
    public Guid BasketId { get; private set; }
    public Guid TourScheduleId { get; private set; } = tourScheduleId;

    public TourSchedule TourSchedule { get; private set; }
    public Guid TourScheduleTicketId { get; private set; } = tourScheduleTicketId;
    public int Quantity { get; private set; } = quantity;
    public virtual Basket Basket { get; private set; }

    public void AddUnits(int quantity, Guid tourScheduleTicketId)
    {
        if (quantity < 0)
        {
            throw new AggregateException("Invalid units");
        }

        if (!TourSchedule.HasAvailableTicket(quantity, tourScheduleTicketId) || TourSchedule.IsStarted())
        {
            throw new AggregateException("Tour schedule is not available");
        }

        Quantity += quantity;
    }

    public void RemoveUnits(int quantity)
    {
        if (quantity < 0)
        {
            throw new AggregateException("Invalid units");
        }

        Quantity -= quantity;
    }
}