using Domain.DataModel;

namespace Domain.Entities;

public class TourBasketItem(Guid tourScheduleId, Guid ticketTypeId, int quantity)
{
    public Guid BasketId { get; private set; }
    public Guid TourScheduleId { get; private set; } = tourScheduleId;

    public TourSchedule TourSchedule { get; private set; }
    public Guid TicketTypeId { get; private set; } = ticketTypeId;
    public int Quantity { get; private set; } = quantity;
    public virtual Basket Basket { get; private set; }
    public void AddUnits(int quantity, Guid ticketTypeId)
    {
        if (quantity < 0)
        {
            throw new AggregateException("Invalid units");
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