namespace Domain.Entities;

public class Basket
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    private readonly List<TourBasketItem> items = new();
    public IReadOnlyCollection<TourBasketItem> Items => items.AsReadOnly();

    public void AddItem(Guid tourScheduleId, Guid ticketTypeId, int units = 1)
    {
        var existedItem = items.SingleOrDefault(x => x.TourScheduleId == tourScheduleId
                                                     && x.TicketTypeId == ticketTypeId);
        if (existedItem is not null)
        {
            existedItem.AddUnits(units);
        }
        else
        {
            items.Add(new TourBasketItem()
            {
                Quantity = units,
                TourScheduleId = tourScheduleId,
            });
        }
    }

    public void DeleteItem(Guid tourScheduleId)
    {
        items.RemoveAll(x => x.TourScheduleId == tourScheduleId);
    }

    public void EmptyBasket()
    {
        items.Clear();
    }
}