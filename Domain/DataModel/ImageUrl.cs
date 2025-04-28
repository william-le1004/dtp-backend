
namespace Domain.DataModel;

public class ImageUrl
{

    public ImageUrl(Guid refId, string url)
    {
        RefId = refId;
        Url = url;
    }

    public Guid Id { get; set; }
    public Guid RefId { get; set; }
    public string Url { get; set; } = null!;
}