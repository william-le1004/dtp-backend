namespace Domain.Entities;

public partial class Company : AuditEntity
{
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;
    public string Address { get; set; } = null!;

    public string TaxCode { get; set; } = null!;
    
    public double CommissionRate { get; set; } = 0.1;

    public bool Licensed { get; set; } = false;

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
    public virtual ICollection<User> Staffs { get; set; } = new List<User>();

    private Company()
    {
    }

    public Company(string name, string email, string phone, string taxCode, string address, double commissionRate)
    {
        Name = name;
        Email = email;
        Phone = phone;
        TaxCode = taxCode;
        Address = address;
        Licensed = false;
        CommissionRate = commissionRate;
    }

    public void AcceptLicense() => Licensed = true;

    public void Delete() => IsDeleted = true;

    public void UpdateDetails(string name, string email, string phone, string taxCode, double commissionRate)
    {
        Name = name;
        Email = email;
        Phone = phone;
        TaxCode = taxCode;
        CommissionRate = commissionRate;
    }

    public void AddStaff(User staff) => Staffs.Add(staff);

    public int StaffCount() => Staffs.Count;

    public int TourCount() => Tours.Count;
}