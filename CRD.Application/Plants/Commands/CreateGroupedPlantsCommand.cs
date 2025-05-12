using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;

namespace CRD.Application.Plants.Commands
{
    public class CreateGroupedPlantsCommand:IRequest<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> CropIds { get; set; } = new();
        public List<int> TreeIds { get; set; } = new();
        public List<int> GrowthStages { get; set; } = new();
    }
    public class CreateGroupedPlantsCommandHandler : IRequestHandler<CreateGroupedPlantsCommand, int>
    {
        private readonly IRepository<GroupedPlants> _context;

        public CreateGroupedPlantsCommandHandler(IRepository<GroupedPlants> context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateGroupedPlantsCommand request, CancellationToken cancellationToken)
        {
            var group = new GroupedPlants
            {
                Name = request.Name,
                Description = request.Description,
                GrowthStages = request.GrowthStages,
                GroupedPlantItems = new List<GroupedPlantItem>()
            };

            request.CropIds.ForEach(cropId =>
            {
                group.GroupedPlantItems.Add(new GroupedPlantItem { CropId = cropId });
            });

            request.TreeIds.ForEach(treeId =>
            {
                group.GroupedPlantItems.Add(new GroupedPlantItem { TreeId = treeId });
            });

            _context.AddWithoutSaving(group);
              await _context.SaveAsync();

            return group.Id;
        }
    }
}
