using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.StructureRates.Commands
{
    public class StructureRateDto
    {
        public int? Id { get; set; }  // Optional for updates
        public int DistrictRateId { get; set; }
        public int StructureId { get; set; }
        public StructuresUnitOfMeasure Unit { get; set; }

        public decimal? Rate { get; set; }
        public string? Assumptions { get; set; }
        public string? DiscretionInfo { get; set; }
    }
    public class CreateOrUpdateStructureRateCommand : IRequest<int>
    {
        public StructureRateDto Data { get; set; }
    }

    public class CreateOrUpdateStructureRateHandler : IRequestHandler<CreateOrUpdateStructureRateCommand, int>
    {
        private readonly IRepository<StructureRate> _context;

        public CreateOrUpdateStructureRateHandler(IRepository<StructureRate> context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateOrUpdateStructureRateCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Data;
            StructureRate? rate = null;

            if (dto.Id.HasValue)
            {
                rate = await _context.GetByIdAsync(dto.Id.Value);
            }

            if (rate == null)
            {
                rate = new StructureRate();
                _context.AddWithoutSaving(rate);
            }

            // ✅ Set fields
            rate.DistrictRateId = dto.DistrictRateId;
            rate.StructureId = dto.StructureId;
            rate.Unit = dto.Unit;
            rate.Rate = dto.Rate;
            rate.Assumptions = dto.Assumptions;
            rate.DiscretionInfo = dto.DiscretionInfo;
            rate.Status = ModerationStatus.Pending;

            await _context.SaveAsync();
            return rate.Id;
        }
    }
}
