namespace Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public TicketType Type { get; set; }
    public Guid TourScheduleId { get; set; }
}

public enum TicketType
{
    Adult,
    Child,
    PerGroupOfThree,
    PerGroupOfFive,
    PerGroupOfSeven,
    PerGroupOfTen,
}