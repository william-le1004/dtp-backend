namespace Domain.DataModel;

public class ImageUrl 
{
    public Guid Id { get; set; }
    public Guid RefId { get; set; }
    public string Url { get; set; } = null!;
}