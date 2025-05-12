using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRD.Application.Plants.Commands
{
    public class UpdateGroupedPlantsCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> CropIds { get; set; } = new();
        public List<int> TreeIds { get; set; } = new();
        public List<int> GrowthStages { get; set; } = new();
    }
    public class UpdateGroupedPlantsCommandHandler : IRequestHandler<UpdateGroupedPlantsCommand, bool>
    {
        private readonly IRepository<GroupedPlants> _context;

        public UpdateGroupedPlantsCommandHandler(IRepository<GroupedPlants> context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateGroupedPlantsCommand request, CancellationToken cancellationToken)
        {
            var group = await _context.GetAll().AsQueryable()
                .Include(g => g.GroupedPlantItems)
                .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

            if (group == null) return false;

            group.Name = request.Name;
            group.Description = request.Description;
            group.GrowthStages = request.GrowthStages;

            group.GroupedPlantItems.Clear();

            request.CropIds.ForEach(cropId =>
            {
                group.GroupedPlantItems.Add(new GroupedPlantItem { CropId = cropId });
            });

            request.TreeIds.ForEach(treeId =>
            {
                group.GroupedPlantItems.Add(new GroupedPlantItem { TreeId = treeId });
            });



            await _context.Update(group);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
