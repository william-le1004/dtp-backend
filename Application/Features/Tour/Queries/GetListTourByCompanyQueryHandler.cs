using Application.Contracts;
using Application.Contracts.Persistence;
using Application.Dtos;
using MediatR;


namespace Application.Features.Tour.Queries
{
    public record GetListTourByCompanyQuery() : IRequest<IQueryable<TourByCompanyResponse>>;

    public class
        GetListTourByCompanyQueryHandler : IRequestHandler<GetListTourByCompanyQuery, IQueryable<TourByCompanyResponse>>
    {
        private readonly IDtpDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetListTourByCompanyQueryHandler(IDtpDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public Task<IQueryable<TourByCompanyResponse>> Handle(GetListTourByCompanyQuery request,
            CancellationToken cancellationToken)
        {
            var companyId = _userContextService.GetCompanyId();
            var tours = _context.Tours
                .Where(t => t.CompanyId == companyId);

            var tourResponses = tours
                .Select(t => new TourByCompanyResponse()
                {
                    Id = t.Id,
                    CompanyId = t.CompanyId,
                    Title = t.Title,
                    CategoryId = t.CategoryId,
                    Description = t.Description,
                    About = t.About,
                    Pickinfor = t.Pickinfor,
                    Include = t.Include,
                    IsDeleted = t.IsDeleted,
                });

            return Task.FromResult(tourResponses);
        }
    }
}