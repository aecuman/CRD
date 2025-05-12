using CRD.Application.Common;
using CRD.Application.Plants.Commands;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Plants.Queries
{
    public class GetPlantListQuery:IRequest<List<PlantListViewModel>>
    {
        public int Take { get; set; } = 10;
        public int Skip { get; set; } = 0;
        public string? Filter { get; set; }
        public string? Order { get; set; }
    }

    public class GetPlantListQueryHandler : IRequestHandler<GetPlantListQuery, List<PlantListViewModel>>
    {
        private readonly ILogger<CreateCropCommand> _logger;
        private readonly IRepository<Crop> _cropRepo;
        private readonly IRepository<Tree> _treeRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Languange> _languangeRepo;
        private readonly IRepository<GrowthStage> _growthStageRepo;
        private readonly IRepository<CategoryInfo> _categoryInfoRepo;

        public GetPlantListQueryHandler(ILogger<CreateCropCommand> logger, IRepository<Crop> cropRepo, IRepository<Tree> treeRepo, IRepository<Category> categoryRepo, IRepository<Languange> languangeRepo, IRepository<GrowthStage> growthStageRepo, IRepository<CategoryInfo> categoryInfoRepo)
        {
            _logger = logger;
            _cropRepo = cropRepo;
            _treeRepo = treeRepo;
            _categoryRepo = categoryRepo;
            _languangeRepo = languangeRepo;
            _growthStageRepo = growthStageRepo;
            _categoryInfoRepo = categoryInfoRepo;
        }
        public Task<List<PlantListViewModel>> Handle(GetPlantListQuery request, CancellationToken cancellationToken)
        {
            var list = new List<PlantListViewModel>();

            var crop_list = _cropRepo.IncludeMultiple(_cropRepo.GetAll().AsQueryable(),x=>x.Translations,x=>x.CategoryInform);
            var vm_crop_list = crop_list.ToList().Select(c => new PlantListViewModel()
            {
                CommonName = c.CommonName,
                Aez = c.Aez,
                BotanicalName = c.BotanicalName,
                CropType = c.CropType,
                Id = c.Id,
                Info = c.Info,
                PlantType = "crop",
                Categories = GetCategories(c.Categories),
                InfoCategories = GetCategoriesInfo(c.CategoryInform),
                GrowthStages = GetGrowthStages(c.GrowthStages),
                Translations = GetTranslations(c.Translations)
            }).ToList();
            list.AddRange(vm_crop_list);
            _logger.LogInformation("{0} {1}", vm_crop_list.Count, "crops added");
            var tree_list = _treeRepo.IncludeMultiple(_treeRepo.GetAll().AsQueryable(),x => x.Translations,x=>x.CategoryInform);
            var vm_tree_list = tree_list.ToList().Select(c => new PlantListViewModel()
            {
                CommonName = c.CommonName,
                BotanicalName = c.BotanicalName,
                Id = c.Id,
                Info = c.Info,
                PlantType = "tree",
                Categories = GetCategories(c.Categories),
                InfoCategories = GetCategoriesInfo(c.CategoryInform),
                GrowthStages = GetGrowthStages(c.GrowthStages),
                Translations = GetTranslations(c.Translations),
                Othernames = c.Othernames
            }).ToList();
            _logger.LogInformation("{0} {1}", vm_tree_list.Count, "trees added");
            list.AddRange(vm_tree_list);

            return Task.FromResult(list);
        }

        List<TranslationViewModel> GetTranslations(List<Translation> translations)
        {
            var _translations = new List<TranslationViewModel>();
            translations.ForEach(x =>
            {
                _translations.Add(new TranslationViewModel() {TranslationId=x.Id, languangeId=x.LanguageId,LanguangeName=_languangeRepo.GetById(x.LanguageId).Name,Translated=x.Translated });
            });
            return _translations;
        }

        List<GrowthStageViewModel> GetGrowthStages(List<int> growthStages)
        {
            var _growthStages = new List<GrowthStageViewModel>();
           growthStages.ForEach(x =>
            {
                _growthStages.Add(new GrowthStageViewModel() { GrowthStageId = x, Name = _growthStageRepo.GetById(x).Name });
            });
            return _growthStages;
        }

        private List<CategoryViewModel> GetCategories(List<int> categories)
        {
            var _categories = new List<CategoryViewModel>();
            categories.ForEach(x =>
            {
                _categories.Add(new CategoryViewModel() {Id=x, Name = _categoryRepo.GetById(x).Name });
            });
            return _categories;
        }
        private List<CategoryInfoViewModel> GetCategoriesInfo(List<CategoryInfo> categories)
        {
            var _categories = new List<CategoryInfoViewModel>();
            categories.ForEach(x =>
            {
                var info = _categoryInfoRepo.GetById(x.Id);
                _categories.Add(new CategoryInfoViewModel() { Id = x.Id, CategoryId = x.CategoryId, Info= x.Info, Name = _categoryRepo.GetById(x.CategoryId).Name });
            });
            return _categories;
        }
    }

    public class PlantListViewModel
    {
        public PlantListViewModel() {
            Translations = new List<TranslationViewModel>();
            GrowthStages = new List<GrowthStageViewModel>();
            Categories = new List<CategoryViewModel>();
            Othernames = new List<string>();
        }
        public int Id { get; set; }
        public string PlantType { get; set; }
        public string CommonName { get; set; }
        public string BotanicalName { get; set; }
        public string CropType { get; set; }
        public List<TranslationViewModel> Translations { get; set; } = new List<TranslationViewModel>();
        public List<GrowthStageViewModel> GrowthStages { get; set; } = new List<GrowthStageViewModel>();
        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        public List<CategoryInfoViewModel> InfoCategories { get; set; }= new List<CategoryInfoViewModel>();
        public List<string> Othernames { get; set; }
        public string Info { get; set; }
        public string Aez { get; set; }
    }

    public class TranslationViewModel
    {
        public int languangeId { get; set; }
        public int TranslationId { get; set; }
        public string LanguangeName { get; set; }
        public string Translated {  get; set; }
    }

    public class GrowthStageViewModel
    {
        public int GrowthStageId { get; set; }
        public string Name {  get; set; }
    }
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
        public class CategoryInfoViewModel
    {
        public int Id { get; set; } 
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string[] Info { get; set; }
    }
}
