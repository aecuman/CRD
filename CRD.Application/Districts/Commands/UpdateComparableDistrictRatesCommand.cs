using CRD.Application.Common;
using CRD.Application.Common.Exceptions;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRD.Application.Districts.Commands
{
    public class UpdateComparableDistrictRatesCommand : IRequest
    {
        public int DistrictRateId { get; set; }
        public List<int> ComparableDistrictRateIds { get; set; } = new();
    }
    public class UpdateComparableDistrictRatesHandler : IRequestHandler<UpdateComparableDistrictRatesCommand>
    {
        private readonly IRepository<DistrictRate> _context;

        public UpdateComparableDistrictRatesHandler(IRepository<DistrictRate> context)
        {
            _context = context;
        }

        public async Task Handle(UpdateComparableDistrictRatesCommand request, CancellationToken cancellationToken)
        {
            var districtRate = await _context.GetAll().AsQueryable()
                .Include(dr => dr.ComparableStrictRates)
                .FirstOrDefaultAsync(dr => dr.Id == request.DistrictRateId, cancellationToken);

            if (districtRate == null)
                throw new NotFoundException(nameof(DistrictRate), request.DistrictRateId);

            var comparableRates = await _context.GetAll().AsQueryable()
                .Where(dr => request.ComparableDistrictRateIds.Contains(dr.Id))
                .ToListAsync(cancellationToken);

            // Clear and update both list and navigation
            districtRate.ComparableStrictRates.Clear();
            districtRate.ComparableDistrictRatesIds = request.ComparableDistrictRateIds;

            foreach (var rate in comparableRates)
            {
                districtRate.ComparableStrictRates.Add(rate);
            }

            await _context.Update(districtRate);
        }
    }

}
