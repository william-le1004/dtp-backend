namespace Domain.Entities;

public partial class Tour : AuditEntity
{
    public string Title { get; set; } = null!;

    public Guid? CompanyId { get; set; }

    public Guid? Category { get; set; }

    public string? Description { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<TourDestination> TourDestinations { get; set; } = new List<TourDestination>();

    public virtual ICollection<TourSchedule> TourSchedules { get; set; } = new List<TourSchedule>();
}