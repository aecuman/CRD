using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRD.Application.Plants.Queries
{
    public class GetGroupedPlantsQuery : IRequest<List<GroupedPlantListViewModel>>
    {
        public int? Id { get; set; } // Optional: if null, return all
    }
    public class GetGroupedPlantsQueryHandler : IRequestHandler<GetGroupedPlantsQuery, List<GroupedPlantListViewModel>>
    {
        private readonly IRepository<GroupedPlants> _context;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Languange> _languangeRepo;
        private readonly IRepository<GrowthStage> _growthStageRepo;

        public GetGroupedPlantsQueryHandler(IRepository<GroupedPlants> context, IRepository<Category> categoryRepo, IRepository<Languange> languangeRepo, IRepository<GrowthStage> growthStageRepo)
        {
            _context = context;
            _categoryRepo = categoryRepo;
            _languangeRepo = languangeRepo;
            _growthStageRepo = growthStageRepo;
        }

        public async Task<List<GroupedPlantListViewModel>> Handle(GetGroupedPlantsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.GetAll().AsQueryable()
                 .Include(g => g.GroupedPlantItems)
                .ThenInclude(i => i.Crop)
                    .ThenInclude(c => c.Translations)
            .Include(g => g.GroupedPlantItems)
                .ThenInclude(i => i.Crop)
                    .ThenInclude(c => c.CategoryInform)
            .Include(g => g.GroupedPlantItems)
                .ThenInclude(i => i.Tree)
                    .ThenInclude(t => t.Translations)
            .Include(g => g.GroupedPlantItems)
                .ThenInclude(i => i.Tree)
                    .ThenInclude(t => t.CategoryInform);

            if (request.Id.HasValue)
            {
                var group = await query.FirstOrDefaultAsync(g => g.Id == request.Id.Value, cancellationToken);
                if (group == null) return new List<GroupedPlantListViewModel>();
                return new List<GroupedPlantListViewModel> { MapToDto(group) };
            }

            var groups = await query.ToListAsync(cancellationToken);
            return groups.Select(MapToDto).ToList();
        }
    
     private GroupedPlantListViewModel MapToDto(GroupedPlants group)
        {
            var plants = new List<PlantListViewModel>();

            foreach (var item in group.GroupedPlantItems)
            {
                if (item.Crop != null)
                {
                    plants.Add(new PlantListViewModel
                    {
                        Id = item.Crop.Id,
                        PlantType = "crop",
                        CommonName = item.Crop.CommonName,
                        BotanicalName = item.Crop.BotanicalName,
                        Info = item.Crop.Info,
                        CropType = item.Crop.CropType,
                        Aez = item.Crop.Aez,
                      //  Othernames = item.Crop.oth,
                        GrowthStages = item.Crop.GrowthStages.Select(x => new GrowthStageViewModel
                        {
                            GrowthStageId = x,
                            Name = _growthStageRepo.GetById(x)?.Name
                        }).ToList(),
                        Categories = item.Crop.Categories.Select(x => new CategoryViewModel
                        {
                            Id = x,
                            Name = _categoryRepo.GetById(x)?.Name
                        }).ToList(),
                        InfoCategories = item.Crop.CategoryInform.Select(ci => new CategoryInfoViewModel
                        {
                            Id = ci.Id,
                            CategoryId = ci.CategoryId,
                            Info = ci.Info
                        }).ToList(),
                        Translations = item.Crop.Translations.Select(t => new TranslationViewModel
                        {
                            TranslationId = t.Id,
                            languangeId = t.LanguageId,
                            LanguangeName = _languangeRepo.Select(l => l.Id == t.LanguageId)?.Name,
                            Translated = t.Translated
                        }).ToList()
                    });
                }

                if (item.Tree != null)
                {
                    plants.Add(new PlantListViewModel
                    {
                        Id = item.Tree.Id,
                        PlantType = "tree",
                        CommonName = item.Tree.CommonName,
                        BotanicalName = item.Tree.BotanicalName,
                        Info = item.Tree.Info,
                        Othernames = item.Tree.Othernames,
                        GrowthStages = item.Tree.GrowthStages.Select(x => new GrowthStageViewModel
                        {
                            GrowthStageId = x,
                            Name = _growthStageRepo.GetById(x)?.Name
                        }).ToList(),
                        Categories = item.Tree.Categories.Select(x => new CategoryViewModel
                        {
                            Id = x,
                            Name = _categoryRepo.GetById(x)?.Name
                        }).ToList(),
                        InfoCategories = item.Tree.CategoryInform.Select(ci => new CategoryInfoViewModel
                        {
                            Id = ci.Id,
                            CategoryId = ci.CategoryId,
                            Info = ci.Info
                        }).ToList(),
                        Translations = item.Tree.Translations.Select(t => new TranslationViewModel
                        {
                            TranslationId = t.Id,
                            languangeId = t.LanguageId,
                            LanguangeName = _languangeRepo.Select(l => l.Id == t.LanguageId)?.Name,
                            Translated = t.Translated
                        }).ToList()
                    });
                }
            }

            return new GroupedPlantListViewModel
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                GrowthStages = group.GrowthStages,
                Plants = plants
            };
        }
        /*return new GroupedPlantListViewModel
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            Plants = group.Plants.Select(p => new PlantListViewModel
            {
                Id = p.Id,
                CommonName = p.CommonName,
                BotanicalName = p.BotanicalName,
                PlantType = p.PlantType,
                CropType = p is Crop crop ? crop.CropType : null,
                Info = p.Info,
                Aez = p is Crop crop2 ? crop2.Aez : null,
                Categories = p.Categories.Select(cid => new CategoryViewModel
                {
                    Id = cid,
                    Name = _categoryRepo.GetById(cid)?.Name
                }).ToList(),
                InfoCategories = p.CategoryInform.Select(ci => new CategoryInfoViewModel
                {
                    Id = ci.Id,
                    CategoryId = ci.CategoryId,
                    Info = ci.Info
                }).ToList(),
                GrowthStages = p.GrowthStages.Select(gsId => new GrowthStageViewModel
                {
                    GrowthStageId = gsId,
                    Name = _growthStageRepo.GetById(gsId)?.Name
                }).ToList(),
                Translations = p.Translations.Select(t => new TranslationViewModel
                {
                    TranslationId = t.Id,
                    languangeId = t.LanguageId,
                    LanguangeName = _languangeRepo.Select(l=>l.Id== t.LanguageId)?.Name,
                    Translated = t.Translated
                }).ToList()
            }).ToList()
        };*/
    
    }

    public class GroupedPlantListViewModel
    {
        public int Id { get; set; }                     // ID of the GroupedPlants entity
        public string Name { get; set; }                // Name of the group
        public string Description { get; set; }         // Description of the group
        public List<int> GrowthStages { get; set; } = new();
        public List<PlantListViewModel> Plants { get; set; } = new(); // List of crops/trees in this group
    }
}

