namespace Domain.Entities;

public class Basket
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public List<TourBasketItem> BasketItems { get; set; }
}