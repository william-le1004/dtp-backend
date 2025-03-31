using Domain.Enum;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public sealed class User : IdentityUser
{
    private readonly List<Feedback> _feedbacks = new();
    private readonly List<Rating> _ratings = new();

    public User()
    {
    }

    public User(string userName, string email, string name, string address, string phoneNumber)
    {
        UserName = userName;
        Email = email;
        IsActive = true;
        Name = name;
        Address = address;
        PhoneNumber = phoneNumber;
        Basket = new Basket();
        Wallet = new Wallet(Id);
        CreatedAt = DateTime.UtcNow;
    }

    public DateTime CreatedAt { get; init; }
    public string? CreatedBy { get; set; } = "System";
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    public string Name { get; set; }
    public string Address { get; set; }
    public bool IsActive { get; set; }
    public string? OtpKey { get; set; }
    public Guid? CompanyId { get; private set; }
    public Company? Company { get; private set; }
    public IReadOnlyCollection<Feedback> Feedbacks => _feedbacks.AsReadOnly();
    public IReadOnlyCollection<Rating> Ratings => _ratings.AsReadOnly();
    public Wallet Wallet { get; private set; }
    public Basket Basket { get; private set; }
    
    private readonly List<ExternalTransaction> _transactions = new();
    public IReadOnlyCollection<ExternalTransaction> ExternalTransactions => _transactions.AsReadOnly();


    public ExternalTransaction RequestWithdraw(string transactionCode,
        decimal amount, string description)
    {
        var externalTransaction = new ExternalTransaction(
            transactionCode, amount, ExternalTransactionType.Withdraw, Id, description);
        _transactions.Add(externalTransaction);
        return externalTransaction;
    }
    
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

    public void AssignCompany(Guid companyId)
    {
        CompanyId = companyId;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}