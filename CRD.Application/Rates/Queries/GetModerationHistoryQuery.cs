using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Rates.Queries
{
    public class GetModerationHistoryQuery : IRequest<List<CompensationRateModerationViewModel>>
    {
        public int RateId { get; set; }
    }
    public class CompensationRateModerationViewModel
    {
        public string Moderator { get; set; }
        public string ModerationNotes { get; set; }
        public ModerationStatus Status { get; set; }
        public DateTime ModerationDate { get; set; }
        public decimal? OldRate { get; set; }
        public decimal? NewRate { get; set; }
        public bool IsDeferred { get; set; }
        public string? DeferredReason { get; set; }
    }
    public class GetModerationHistoryHandler : IRequestHandler<GetModerationHistoryQuery, List<CompensationRateModerationViewModel>>
    {
        private readonly IRepository<CompensationRateModeration> _context;

        public GetModerationHistoryHandler(IRepository<CompensationRateModeration> context)
        {
            _context = context;
        }

        public async Task<List<CompensationRateModerationViewModel>> Handle(GetModerationHistoryQuery request, CancellationToken cancellationToken)
        {
            return await _context.GetAll().AsQueryable()
                .Where(m => m.Id == request.RateId)
                .Select(m => new CompensationRateModerationViewModel
                {
                    Moderator = m.Moderator,
                    ModerationNotes = m.ModerationNotes,
                    Status = m.Status,
                    ModerationDate = m.ModerationDate,
                    OldRate = m.OldRate,
                    NewRate = m.NewRate,
                    IsDeferred = m.IsDeferred,
                    DeferredReason = m.DeferredReason
                })
                .ToListAsync(cancellationToken);
        }
    }


}
