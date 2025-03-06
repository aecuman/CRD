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
        public string CommonName { get; set; }
        public string BotanicalName { get; set; }
        public string CropType { get; set; }
        public List<TranslationDto> Translations { get; set; }
        public List<int> GrowthStages { get; set; }
        public List<int> Categories { get; set; }
        public List<string>? Othernames { get; set; }
        public string Aez { get; set; }
        public string Info { get; set; }
        public List<PlantFileDto> Images { get; set; } = new List<PlantFileDto>();
        public List<CategoryInfoDto> CategoryInfos { get; set; } = new List<CategoryInfoDto>();

    }

    public class TranslationDto
    {
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
                var _translation = new List<Translation>();
                request.Translations.ForEach(t =>
                {
                    _translation.Add(new Translation() { LanguageId = t.LanguangeId, Translated = t.Translated });
                });
                _translationRepo.AddMany(_translation);
                var _cats = new List<CategoryInfo>();
                foreach (var item in request.CategoryInfos)
                {
                    _cats.Add(new CategoryInfo() { CategoryId = item.CategoryId, Info = item.CategoryInfo });
                }
                /*var files = new List<CRDFile>();
                foreach (var f in request.Images) {
                    files.Add(new CRDFile() { Name = f.Name, Url=f.Url, FileClass=nameof(Crop), });
                */

                if (request.PlantType == "crop")
                {

                    _cropRepo.Add(new Crop()
                    {
                        Aez = request.Aez,
                        BotanicalName = request.BotanicalName,
                        GrowthStages = request.GrowthStages,
                        Categories = request.Categories,
                        CropType = request.CropType,
                        CommonName = request.CommonName,
                        //  Images=request.Images,
                        Info = request.Info,
                        Translations = _translation,
                        CategoryInform = _cats

                    });
                }
                else if (request.PlantType == "tree")
                {

                    _treeRepo.Add(new Tree()
                    {
                        Translations = _translation,
                        Info = request.Info,
                        //  Images=request.Images,
                        BotanicalName = request.BotanicalName,
                        CommonName = request.CommonName,
                        Categories = request.Categories,
                        GrowthStages = request.GrowthStages,
                        CategoryInform = _cats

                    });
                }

                return await _cropRepo.SaveAsync();

            }
        }
    }
}
