namespace Domain.Entities;

public class TicketType
{
    public Guid Id { get; set; }
    public decimal NetCost { get; set; }
    public double Tax { get; set; }
    public TicketKind TicketKind { get; set; }
    public Guid TourScheduleId { get; set; }
}

public enum TicketKind
{
    Adult,
    Child,
    PerGroupOfThree,
    PerGroupOfFive,
    PerGroupOfSeven,
    PerGroupOfTen,
}