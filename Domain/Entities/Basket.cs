namespace Domain.Entities;

public class Basket
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<TourBasketItem> BasketItems { get; set; }
}

public class TourBasketItem
{
    public Guid BasketId { get; set; }
    public Guid TourScheduleId { get; set; }
    public Guid TicketTypeId { get; set; }
    public virtual TicketType TicketType { get; set; }
    public int Quantity { get; set; }
    public virtual Basket Basket { get; set; }
}