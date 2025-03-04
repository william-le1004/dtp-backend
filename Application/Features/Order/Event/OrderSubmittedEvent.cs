namespace Application.Features.Order.Event;

public record OrderSubmittedEvent(string UserId, Guid TourScheduleId);