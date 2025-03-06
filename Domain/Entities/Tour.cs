namespace Domain.Entities;

public partial class Tour : AuditEntity
{
    public string Title { get; set; } = null!;

    public Guid? CompanyId { get; set; }

    public Guid? CategoryId { get; set; }
    
    public virtual Category Category { get; set; } = null!;
    
    public string? Description { get; set; }

    public List<TicketType> Tickets { get; set; } = new();
    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<TourDestination> TourDestinations { get; set; } = new List<TourDestination>();

    public virtual ICollection<TourSchedule> TourSchedules { get; set; } = new List<TourSchedule>();

  
    public decimal OnlyFromCost()
    {
        return Tickets.Select(x => x.DefaultNetCost).Min();
    }
}