﻿namespace Application.Contracts.Job;

public interface IOrderJobService
{
    void ScheduleCancelOrder(Guid bookingId);
    Task CancelOrder(Guid bookingId);
    void PaidCheck(string jobId);
    Task MarkToursAsCompleted();
    void ScheduleCompleteOrder(Guid bookingId);
    Task MarkOrderCompleted(Guid bookingId);
}