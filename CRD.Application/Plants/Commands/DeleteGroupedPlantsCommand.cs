using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRD.Application.Plants.Commands
{
    public class DeleteGroupedPlantsCommand: IRequest<bool>
    {
        public int Id { get; set; }
    }
    public class DeleteGroupedPlantsCommandHandler : IRequestHandler<DeleteGroupedPlantsCommand, bool>
    {
        private readonly IRepository<GroupedPlants> _context;

        public DeleteGroupedPlantsCommandHandler(IRepository<GroupedPlants> context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteGroupedPlantsCommand request, CancellationToken cancellationToken)
        {
            var group = await _context.GetAll().AsQueryable()
                .Include(g => g.GroupedPlantItems)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);
            if (group == null) return false;

            _context.Remove(group.Id);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
