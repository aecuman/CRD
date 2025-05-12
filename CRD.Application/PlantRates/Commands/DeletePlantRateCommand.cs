using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRD.Application.PlantRates.Commands
{
    public class DeletePlantRateCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
    public class DeletePlantRateHandler : IRequestHandler<DeletePlantRateCommand, bool>
    {
        private readonly IRepository<PlantRate> _context;
        private readonly IRepository<PlantRateGroup> _plantRateGroupContext;

        public DeletePlantRateHandler(IRepository<PlantRate> context, IRepository<PlantRateGroup> plantRateGroupContext)
        {
            _context = context;
            _plantRateGroupContext = plantRateGroupContext;
        }

        public async Task<bool> Handle(DeletePlantRateCommand request, CancellationToken cancellationToken)
        {
            var rate = await _context.GetAll().AsQueryable()
                .Include(pr => pr.PlantRateGroups)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (rate == null)
                return false;

            if(rate.PlantRateGroups == null || !rate.PlantRateGroups.Any())
            {
                _context.Remove(rate.Id);
                return true;
            }
            // Remove related junction table records
            var result =  _plantRateGroupContext.RemoveRange(rate.PlantRateGroups);
            if (result)
            {                 // Remove the PlantRate record
                _context.Remove(rate.Id);
            }
            else
            {
                return false;
            }
           // _context.Remove(rate.Id);

          //  await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

}
