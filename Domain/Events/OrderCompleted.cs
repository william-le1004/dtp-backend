using MediatR;

namespace Domain.Events;

public record OrderCompleted(Guid TourScheduleId) : INotification;