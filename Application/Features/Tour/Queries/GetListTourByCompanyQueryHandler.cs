using Application.Contracts.Persistence;
using Application.Common;
using Application.Dtos; // Giả sử TourResponse được định nghĩa, ví dụ: record TourResponse(Guid Id, string Title, Guid? CompanyId, Guid? Category, string? Description);
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Queries
{
    // Query nhận CompanyId và trả về danh sách TourResponse được bọc trong ApiResponse
    public record GetListTourByCompanyQuery(Guid CompanyId) : IRequest<ApiResponse<List<TourResponse>>>;

    public class GetListTourByCompanyQueryHandler : IRequestHandler<GetListTourByCompanyQuery, ApiResponse<List<TourResponse>>>
    {
        private readonly IDtpDbContext _context;

        public GetListTourByCompanyQueryHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<TourResponse>>> Handle(GetListTourByCompanyQuery request, CancellationToken cancellationToken)
        {
            // Truy vấn danh sách Tour có CompanyId trùng với giá trị truyền vào
            var tours = await _context.Tours
                .Where(t => t.CompanyId == request.CompanyId)
                .ToListAsync(cancellationToken);

            var tourResponses = tours.Select(t => new TourResponse(t.Id, t.Title, t.CompanyId, t.Category, t.Description))
                                     .ToList();

            return ApiResponse<List<TourResponse>>.SuccessResult(tourResponses, "Tours retrieved successfully");
        }
    }
}
