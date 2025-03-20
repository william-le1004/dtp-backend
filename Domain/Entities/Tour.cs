using Microsoft.VisualBasic;

namespace Domain.Entities;

public partial class Tour : AuditEntity
{
    public string Title { get; private set; } = null!;

    public Guid? CompanyId { get; private set; }

    public Guid? CategoryId { get; private set; }
    public Category Category { get; private set; }
    public string? Description { get; private set; }
    public string? Code { get; private set; }
    public List<TicketType> Tickets { get; private set; } = new();
    public virtual Company Company { get; private set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; private set; } = new List<Feedback>();

    public virtual ICollection<Rating> Ratings { get; private set; } = new List<Rating>();

    public virtual ICollection<TourDestination> TourDestinations { get; private set; } = new List<TourDestination>();

    public virtual ICollection<TourSchedule> TourSchedules { get; private set; } = new List<TourSchedule>();

    public Tour()
    {
    }

    public Tour(string title, Guid? companyId, Guid? category, string? description,string? code )
    {
        Id = Guid.NewGuid();
        Code= code;
        Title = title;
        CompanyId = companyId;
        CategoryId = category;
        Description = description;
    }

    public void Update(string title, Guid? category, string? description)
    {
        Title = title;
        CategoryId = category;
        Description = description;
    }

    public decimal OnlyFromCost()
    {
        return Tickets.Select(x => x.DefaultNetCost).Min();
    }

}