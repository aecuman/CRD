using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRD.Application.Districts.Queries
{
    public class GetLatestDistrictRatesQuery : IRequest<List<DistrictRateDto>>
    {
        public int ExcludeId { get; set; }
        public string? Name { get; set; }
        public string? Year { get; set; }
    }

    public class GetLatestDistrictRatesHandler : IRequestHandler<GetLatestDistrictRatesQuery, List<DistrictRateDto>>
    {
        private readonly IRepository<DistrictRate> _context;

        public GetLatestDistrictRatesHandler(IRepository<DistrictRate> context)
        {
            _context = context;
        }

        public async Task<List<DistrictRateDto>> Handle(GetLatestDistrictRatesQuery request, CancellationToken cancellationToken)
        {
            var baseQuery = _context.GetAll().AsQueryable()
                .Include(dr => dr.District)
                .Where(dr => dr.Id != request.ExcludeId);

            // Get latest year per district
            var latestRates = await baseQuery
                .GroupBy(dr => dr.DistrictId)
                .Select(g => g.OrderByDescending(dr => dr.Year).First())
                .ToListAsync(cancellationToken);

            // Apply optional filters
            var filtered = latestRates
                .Where(dr =>
                    (string.IsNullOrWhiteSpace(request.Name) || dr.District.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(request.Year) || dr.Year.ToString() == request.Year))
                .Select(dr => new DistrictRateDto
                {
                    Id = dr.Id,
                    DistrictName = dr.District.Name,
                    Year = dr.Year
                })
                .ToList();

            return filtered;
        }
    }


}
