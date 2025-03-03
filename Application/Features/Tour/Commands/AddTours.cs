using Application.Contracts.Persistence;
using Application.Common;
using Application.Dtos;
using Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Commands;

// Command: chứa dữ liệu đầu vào để tạo Tour mới, trả về ApiResponse<TourResponse>
public record AddTours(string Title, Guid? CompanyId, Guid? Category, string? Description)
    : IRequest<ApiResponse<TourResponse>>;

public class AddToursHandler : IRequestHandler<AddTours, ApiResponse<TourResponse>>
{
    private readonly IDtpDbContext _context;

    public AddToursHandler(IDtpDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<TourResponse>> Handle(AddTours request, CancellationToken cancellationToken)
    {
        // Sử dụng constructor công khai của Tour để khởi tạo đối tượng
        var tour = new Domain.Entities.Tour(request.Title, request.CompanyId, request.Category, request.Description);

        _context.Tours.Add(tour);
        await _context.SaveChangesAsync(cancellationToken);

        // Tạo DTO response dựa trên dữ liệu của tour vừa tạo
        var tourResponse = new TourResponse(tour.Id, tour.Title, tour.CompanyId, tour.Category, tour.Description);

        return ApiResponse<TourResponse>.SuccessResult(tourResponse, "Tour created successfully");
    }
}
