using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

namespace CRD.Application.PlantRates.Commands
{
    public class UpdatePlantRatesCommand:IRequest<bool>
    {
       
            public CompensationRateMatrixDto CompensationRateMatrix { get; set; }
        
    }
    public class CompensationRateMatrixDto
    {
        public int DistrictRateId { get; set; }
        public List<CompensationRateEntryDto> Rates { get; set; } = new List<CompensationRateEntryDto>();
    }


    public class UpdateCompensationRateHandler : IRequestHandler<UpdatePlantRatesCommand, bool>
    {
        private readonly IRepository<PlantRate> _context;

        public UpdateCompensationRateHandler(IRepository<PlantRate> context)
        {
            _context = context;
        }


public async Task<bool> Handle(UpdatePlantRatesCommand request, CancellationToken cancellationToken)
        {
            var rate = await _context.GetAll().AsQueryable()
                .Include(cr => cr.DistrictRate)
                .ThenInclude(dr => dr.District)
                .Include(cr => cr.CategoryInfo)
                .Include(cr => cr.Plant)
                .Include(cr => cr.GroupedPlants)
                .Include(cr => cr.PlantRateGroups)
                .ThenInclude(prg => prg.Plant)
                .FirstOrDefaultAsync(cr => cr.DistrictRateId == request.CompensationRateMatrix.DistrictRateId, cancellationToken);

            if (rate == null)
            {
                return false;
            }

            var compensationRates = request.CompensationRateMatrix.Rates.Select(rateEntry =>
            {
                var updatedRate = new PlantRate { Id = rateEntry.Id ?? 0 };

                if (rateEntry.PlantId != null) updatedRate.PlantId = rateEntry.PlantId;
                if (rateEntry.GrowthStageId != null) updatedRate.GrowthStageId = rateEntry.GrowthStageId;
                if (rateEntry.CategoryId != null) updatedRate.CategoryId = rateEntry.CategoryId;
                if (rateEntry.CategoryInfoId != null) updatedRate.CategoryInfoId = rateEntry.CategoryInfoId;
                if (!string.IsNullOrWhiteSpace(rateEntry.Unit.ToString())) updatedRate.Unit = rateEntry.Unit;
                if (rateEntry.Rate != null) updatedRate.Rate = rateEntry.Rate;
                if (!string.IsNullOrWhiteSpace(rateEntry.Assumptions)) updatedRate.Assumptions = rateEntry.Assumptions;
                if (!string.IsNullOrWhiteSpace(rateEntry.DiscretionInfo)) updatedRate.DiscretionInfo = rateEntry.DiscretionInfo;
                if (!string.IsNullOrWhiteSpace(rateEntry.Quality)) updatedRate.Quality = rateEntry.Quality;

              //  updatedRate.Status = ModerationStatus.Pending; // Always set status

                return updatedRate;
            }).ToList();

            await _context.AddManyAsync(compensationRates);

            return true;
        }


    }

}
