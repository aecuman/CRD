using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Rates.Commands
{
 
    public class ModerateCompensationRateCommand : IRequest<bool>
    {
        public int RateId { get; set; }
        public RateType RateType { get; set; } // ✅ New

        public bool? NoRate { get; set; }

        public decimal? NewRate { get; set; }
        public string? NewDiscretionInfo { get; set; }

        public string Moderator { get; set; }
        public string ModerationNotes { get; set; }

        public ModerationStatus Status { get; set; }

        public bool IsDeferred { get; set; } = false;
        public string? DeferredReason { get; set; }

    }
    public class ModerateCompensationRateHandler : IRequestHandler<ModerateCompensationRateCommand, bool>
    {
        private readonly IRepository<PlantRate> _plantRateRepo;
        private readonly IRepository<StructureRate> _structureRateRepo;

        public ModerateCompensationRateHandler(
            IRepository<PlantRate> plantRateRepo,
            IRepository<StructureRate> structureRateRepo)
        {
            _plantRateRepo = plantRateRepo;
            _structureRateRepo = structureRateRepo;
        }

        public async Task<bool> Handle(ModerateCompensationRateCommand request, CancellationToken cancellationToken)
        {
            switch (request.RateType)
            {
                case RateType.Plant:
                    return await HandlePlantRateModeration(request, cancellationToken);

                case RateType.Structure:
                    return await HandleStructureRateModeration(request, cancellationToken);

                default:
                    throw new ArgumentOutOfRangeException(nameof(request.RateType));
            }
        }

        private async Task<bool> HandlePlantRateModeration(ModerateCompensationRateCommand request, CancellationToken cancellationToken)
        {
            var rate = await _plantRateRepo.GetAll().AsQueryable()
                .Include(r => r.ModerationHistory)
                .FirstOrDefaultAsync(r => r.Id == request.RateId, cancellationToken);

            if (rate == null) return false;

            var moderation = new CompensationRateModeration
            {
                PlantRateId = rate.Id,
                OldRate = rate.Rate,
                DiscretionInfo = rate.DiscretionInfo,
                NewRate = request.NewRate,
                NewDiscretionInfo = request.NewDiscretionInfo,
                Moderator = request.Moderator,
                ModerationNotes = request.ModerationNotes,
                Status = request.Status,
                IsDeferred = request.IsDeferred,
                DeferredReason = request.IsDeferred ? request.DeferredReason : null
            };

            if (!request.IsDeferred)
            {
                rate.Rate = request.NewRate;
                rate.DiscretionInfo = request.NewDiscretionInfo;
            }

            rate.Status = request.Status;
            rate.ModerationHistory.Add(moderation);
            await _plantRateRepo.Update(rate);
            return true;
        }

        private async Task<bool> HandleStructureRateModeration(ModerateCompensationRateCommand request, CancellationToken cancellationToken)
        {
            var rate = await _structureRateRepo.GetAll().AsQueryable()
                .Include(r => r.ModerationHistory)
                .FirstOrDefaultAsync(r => r.Id == request.RateId, cancellationToken);

            if (rate == null) return false;

            var moderation = new CompensationRateModeration
            {
                StructureRateId = rate.Id,
                OldRate = rate.Rate,
                DiscretionInfo = rate.DiscretionInfo,
                NewRate = request.NewRate,
                NewDiscretionInfo = request.NewDiscretionInfo,
                Moderator = request.Moderator,
                ModerationNotes = request.ModerationNotes,
                Status = request.Status,
                IsDeferred = request.IsDeferred,
                DeferredReason = request.IsDeferred ? request.DeferredReason : null
            };

            if (!request.IsDeferred)
            {
                rate.Rate = request.NewRate;
                rate.DiscretionInfo = request.NewDiscretionInfo;
            }

            rate.Status = request.Status;
            rate.ModerationHistory.Add(moderation);
            await _structureRateRepo.Update(rate);
            return true;
        }
    }


}
