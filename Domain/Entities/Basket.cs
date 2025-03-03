namespace Domain.Entities;

public class Basket
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    private readonly List<TourBasketItem> items = new();
    public IReadOnlyCollection<TourBasketItem> Items => items.AsReadOnly();

    public void AddItem(Guid tourScheduleId, Guid tourScheduleTicketId, int units = 1)
    {
        var existedItem = items.SingleOrDefault(x => x.TourScheduleId == tourScheduleId
                                                     && x.TourScheduleTicketId == tourScheduleTicketId);
        if (existedItem is not null)
        {
            existedItem.AddUnits(units, tourScheduleTicketId);
        }
        else
        {
            items.Add(new TourBasketItem(tourScheduleId, tourScheduleTicketId, units));
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