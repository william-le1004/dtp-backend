namespace Domain.Entities;

public class TourBasketItem
{
    public Guid BasketId { get; set; }
    public Guid TourScheduleId { get; set; }

    public TourSchedule TourSchedule { get; set; }
    public Guid TicketTypeId { get; set; }
    public virtual TicketType TicketType { get; set; }
    public int Quantity { get; set; }
    public virtual Basket Basket { get; set; }

    public void AddUnits(int quantity)
    {
        if (quantity < 0)
        {
            throw new AggregateException("Invalid units");
        }

        if (!TourSchedule.HasAvailableTicket(quantity) || TourSchedule.IsStarted())
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