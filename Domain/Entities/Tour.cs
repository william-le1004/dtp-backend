using Domain.Common;

namespace Domain.Entities;

public partial class Tour : AuditEntity
{
    public string Title { get; private set; } = null!;

    public Guid? CompanyId { get; private set; }

    public Guid? Category { get; private set; }
    public string? Description { get; private set; }

    public List<TicketType> Tickets { get; private set; } = new();
    public virtual Company Company { get; private set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; private set; } = new List<Feedback>();

    public virtual ICollection<Rating> Ratings { get; private set; } = new List<Rating>();

    public virtual ICollection<TourDestination> TourDestinations { get; private set; } = new List<TourDestination>();

    public virtual ICollection<TourSchedule> TourSchedules { get; private set; } = new List<TourSchedule>();
    public Tour(string title, Guid? companyId, Guid? category, string? description)
    {
        Title = title;
        CompanyId = companyId;
        Category = category;
        Description = description;
    }
}