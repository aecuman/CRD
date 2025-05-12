using CRD.Application.Common;
using CRD.Domain.Entities;
using CRD.Domain.Process;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Plants.Commands
{
    public class CreateCropCommand : IRequest<int>
    {
        public CreateCropCommand()
        {
            Translations = new List<TranslationDto>();
            GrowthStages = new List<int>();
            Categories = new List<int>();
            Othernames = new List<string>();
        }
        public string PlantType { get; set; }
        public string? CommonName { get; set; }
        public string BotanicalName { get; set; }
        public string? CropType { get; set; }
        public List<TranslationDto> Translations { get; set; }
        public List<int> GrowthStages { get; set; }
        public List<int> Categories { get; set; }
        public List<string>? Othernames { get; set; }
        public string? Aez { get; set; }
        public string? Info { get; set; }
        public List<PlantFileDto> Images { get; set; } = new List<PlantFileDto>();
        public List<CategoryInfoDto> CategoryInfos { get; set; } = new List<CategoryInfoDto>();

    }

    public class TranslationDto
    {
        public int? Id { get; set; }
        public int LanguangeId { get; set; }
        public string Translated { get; set; }
    }

    public class CategoryInfoDto
    {
        public int CategoryId { get; set; }
        public string[] CategoryInfo { get; set; }

        public class CreateCropCommandHandler : IRequestHandler<CreateCropCommand, int>
        {
            private readonly ILogger<CreateCropCommand> _logger;
            private readonly IRepository<Crop> _cropRepo;
            private readonly IRepository<Tree> _treeRepo;
            private readonly IRepository<Translation> _translationRepo;
            private readonly IRepository<CategoryInfo> _categoryInfoRepo;

            public CreateCropCommandHandler(ILogger<CreateCropCommand> logger, IRepository<Crop> cropRepo, IRepository<Tree> treeRepo, IRepository<Translation> translationRepo, IRepository<CategoryInfo> categoryInfoRepo)
            {
                _logger = logger;
                _cropRepo = cropRepo;
                _treeRepo = treeRepo;
                _translationRepo = translationRepo;
                _categoryInfoRepo = categoryInfoRepo;
            }

            public async Task<int> Handle(CreateCropCommand request, CancellationToken cancellationToken)
            {
                if (request == null) throw new NullReferenceException();
                // Create translations and add them to the repository
                var _translation = request.Translations
                    .Select(t => new Translation { LanguageId = t.LanguangeId, Translated = t.Translated })
                    .ToList();
                await _translationRepo.AddMany(_translation);

                // Create category information
                var _cats = request.CategoryInfos
                    .Select(item => new CategoryInfo { CategoryId = item.CategoryId, Info = item.CategoryInfo })
                    .ToList();

                if (request.PlantType == "crop")
                {
                    _cropRepo.AddWithoutSaving(new Crop
                    {
                        Aez = request.Aez,
                        BotanicalName = request.BotanicalName,
                        GrowthStages = request.GrowthStages,
                        Categories = request.Categories,
                        CropType = request.CropType,
                        CommonName = request.CommonName,
                        Info = request.Info,
                        Translations = _translation,
                        CategoryInform = _cats,
                        PlantType = request.PlantType
                    });
                    return await _cropRepo.SaveAsync();
                }
                else if (request.PlantType == "tree")
                {
                    _treeRepo.AddWithoutSaving(new Tree
                    {
                        Translations = _translation,
                        Info = request.Info,
                        BotanicalName = request.BotanicalName,
                        CommonName = request.CommonName,
                        Categories = request.Categories,
                        GrowthStages = request.GrowthStages,
                        CategoryInform = _cats,
                        PlantType = request.PlantType
                    });
                    return await _treeRepo.SaveAsync();
                }

                throw new ArgumentException("Invalid PlantType specified.");

            }
        }
    }

}
