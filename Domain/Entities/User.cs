using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public sealed class User : IdentityUser
{
    private readonly List<Feedback> _feedbacks = new();
    private readonly List<Rating> _ratings = new();

    public User(string userName, string email, string name, string address, string phoneNumber)
    {
        UserName = userName;
        Email = email;
        IsActive = true;
        Name = name;
        Address = address;
        PhoneNumber = phoneNumber;
        Basket = new Basket();
        CreatedAt = DateTime.UtcNow;
    }
    
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; } = "System";
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    
    public string Name { get; private set; }
    public string Address { get; private set; }
    public bool IsActive { get; set; }
    public Guid? CompanyId { get; private set; }
    public Company? Company { get; private set; }
    public IReadOnlyCollection<Feedback> Feedbacks => _feedbacks.AsReadOnly();
    public IReadOnlyCollection<Rating> Ratings => _ratings.AsReadOnly();
    public Wallet Wallet { get; private set; }
    public Basket Basket { get; private set; }

    public void UpdateProfile(string name, string address, string phoneNumber, string email, string userName)
    {
        Name = name;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        UserName = userName;
    }

    public void AddFeedback(Feedback feedback)
    {
        if (feedback == null) throw new ArgumentNullException(nameof(feedback));
        _feedbacks.Add(feedback);
    }

    public void AddRating(Rating rating)
    {
        if (rating == null) throw new ArgumentNullException(nameof(rating));
        _ratings.Add(rating);
    }

    public void AssignCompany(Company company)
    {
        Company = company ?? throw new ArgumentNullException(nameof(company));
        CompanyId = company.Id;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    
    public void InitializeWallet()
    {
        Wallet = new Wallet(Id);
    }
}
