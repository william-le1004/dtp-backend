namespace Application.Dtos;

public record TourResponse(Guid Id, string Title, Guid? CompanyId, Guid? Category, string? Description, string? about, string? include, string? pickinfor,bool isclose);

public class TourByCompanyResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } 
    public Guid? CompanyId { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Description { get; set; }
    public string? About { get; set; }
    public string? Include { get; set; }
    public string? Pickinfor { get; set; }
    public bool IsDeleted { get; set; }
}