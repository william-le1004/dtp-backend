namespace Application.Dtos;

public record TourResponse(Guid Id, string Title, Guid? CompanyId, Guid? Category, string? Description,string?About);