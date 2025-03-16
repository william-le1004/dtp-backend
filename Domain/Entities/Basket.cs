namespace Domain.Entities;

public class Basket
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; }
    private readonly List<TourBasketItem> items = new();
    public IReadOnlyCollection<TourBasketItem> Items => items.AsReadOnly();

    public void AddItem(Guid tourScheduleId, Guid ticketTypeId, int units = 1)
    {
        var existedItem = items.SingleOrDefault(x => x.TourScheduleId == tourScheduleId
                                                     && x.TicketTypeId == ticketTypeId);
        if (existedItem is not null)
        {
            existedItem.AddUnits(units, ticketTypeId);
        }
        else
        {
            items.Add(new TourBasketItem(tourScheduleId, ticketTypeId, units));
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