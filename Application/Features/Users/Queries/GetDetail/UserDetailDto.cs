namespace Application.Features.Users.Queries.GetDetail;

public class UserDetailDto
{
    public string Id { get; set; }
    public string? UserName { get; set; }
    public decimal Balance { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? CompanyName { get; set; }
}