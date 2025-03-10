namespace Domain.Entities;

public partial class Company : AuditEntity
{
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string TaxCode { get; set; } = null!;

    public bool Licensed { get; set; } = false;

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
    public virtual ICollection<User> Staffs { get; set; } = new List<User>();

    private Company()
    {
    }

    public Company(string name, string email, string phone, string taxCode)
    {
        Name = name;
        Email = email;
        Phone = phone;
        TaxCode = taxCode;
        Licensed = false;
    }

    public void AcceptLicense() => Licensed = true;

    public void Delete() => IsDeleted = true;

    public void UpdateDetails(string name, string email, string phone, string taxCode)
    {
        Name = name;
        Email = email;
        Phone = phone;
        TaxCode = taxCode;
    }

    public void AddStaff(User staff) => Staffs.Add(staff);

    public User FirstStaff()
    {
        return Staffs.FirstOrDefault();
    }

    public int StaffCount() => Staffs.Count;

    public int TourCount() => Tours.Count;
}