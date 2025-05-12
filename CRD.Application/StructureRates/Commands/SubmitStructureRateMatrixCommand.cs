using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRD.Application.StructureRates.Commands
{
    public class StructureRateMatrixDto
    {
        public int? Id { get; set; }
        public int StructureId { get; set; }
        public StructuresUnitOfMeasure Unit { get; set; }
        public decimal? Rate { get; set; }
        public string? Assumptions { get; set; }
        public string? DiscretionInfo { get; set; }
    }

    public class SubmitStructureRateMatrixCommand : IRequest<bool>
    {
        public int DistrictRateId { get; set; }
        public List<StructureRateMatrixDto> Rates { get; set; } = new();
    }
    public class SubmitStructureRateMatrixHandler : IRequestHandler<SubmitStructureRateMatrixCommand, bool>
    {
        private readonly IRepository<StructureRate> _context;

        public SubmitStructureRateMatrixHandler(IRepository<StructureRate> context)
        {
            _context = context;
        }

        public async Task<bool> Handle(SubmitStructureRateMatrixCommand request, CancellationToken cancellationToken)
        {
            // Load all existing rates for the district and involved structure IDs
            var structureIds = request.Rates.Select(r => r.StructureId).ToList();

            var existingRates = await _context.GetAll().AsQueryable()
                .Where(r => r.DistrictRateId == request.DistrictRateId && structureIds.Contains(r.StructureId))
                .ToListAsync(cancellationToken);

            var existingMap = existingRates.ToDictionary(r => r.Id);

            foreach (var rateDto in request.Rates)
            {
                if (existingMap.TryGetValue(rateDto.StructureId, out var existingRate))
                {
                    // Update existing rate
                    existingRate.Unit = rateDto.Unit;
                    existingRate.Rate = rateDto.Rate;
                    existingRate.Assumptions = rateDto.Assumptions;
                    existingRate.DiscretionInfo = rateDto.DiscretionInfo;
                    existingRate.Status = ModerationStatus.Pending;
                }
                else
                {
                    // Add new rate
                    var newRate = new StructureRate
                    {
                        StructureId = rateDto.StructureId,
                        DistrictRateId = request.DistrictRateId,
                        Unit = rateDto.Unit,
                        Rate = rateDto.Rate,
                        Assumptions = rateDto.Assumptions,
                        DiscretionInfo = rateDto.DiscretionInfo,
                        Status = ModerationStatus.Pending
                    };
                    _context.AddWithoutSaving(newRate);
                }
            }

            await _context.SaveAsync();
            return true;
        }
    }

}
