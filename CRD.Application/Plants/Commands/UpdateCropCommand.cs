using CRD.Application.Common;
using CRD.Application.Common.Exceptions;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Plants.Commands
{
    public class UpdatePlantCommand:IRequest
    {
        public UpdatePlantCommand()
        {
            Translations = new List<TranslationDto>();
            GrowthStages = new List<int>();
            Categories = new List<int>();
            Othernames = new List<string>();
            Images = new List<PlantFileDto>();
        }
        public int Id { get; set; }
        public string PlantType { get; set; }
        public string CommonName { get; set; }
        public string BotanicalName { get; set; }
        public string CropType { get; set; }
        public List<TranslationDto> Translations { get; set; }
        public List<int> GrowthStages { get; set; }
        public List<int> Categories { get; set; }
        public List<string> Othernames { get; set; }
        public string Info { get; set; }
        public string Aez { get; set; }
        public List<PlantFileDto> Images { get; set; }
    }
    public class UpdatePlantCommandHandler : IRequestHandler<UpdatePlantCommand>
    {
        private readonly ILogger<UpdatePlantCommandHandler> _logger;
        private readonly IRepository<Crop> _cropRepo;
        private readonly IRepository<Tree> _treeRepo;
        private readonly IRepository<CRDFile> _fileRepo;

        public UpdatePlantCommandHandler(ILogger<UpdatePlantCommandHandler> logger, IRepository<Crop> cropRepo, IRepository<Tree> treeRepo, IRepository<CRDFile> fileRepo)
        {
            _logger = logger;
            _cropRepo = cropRepo;
            _treeRepo = treeRepo;
            _fileRepo = fileRepo;
        }
        public async Task Handle(UpdatePlantCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new NullReferenceException();
            if (request.PlantType == "crop")
            {
                var entity = await _cropRepo.GetByIdAsync(request.Id);

                if (entity == null)
                {
                    throw new NotFoundException(nameof(Crop), request.Id);
                }
                entity.Info = request.Info;
                entity.Aez = request.Aez;
                var _translation = new List<Translation>();
                request.Translations.ForEach(t =>
                {
                    _translation.Add(new Translation() { Id = t.LanguangeId, Translated = t.Translated });
                });
                entity.Translations=_translation;
                entity.Categories = request.Categories;
                entity.BotanicalName = request.BotanicalName;
                entity.CommonName = request.CommonName;
                entity.CropType = request.CropType;
                entity.GrowthStages = request.GrowthStages;
                //entity.Images = request.Images.Where(x=>x.Id.HasValue).ToList()
                var  files = new List<CRDFile>();
                foreach (var f in request.Images.Where(x=>(!x.Id.HasValue)))
                {
                    files.Add(new CRDFile() { Name = f.Name, Url = f.Url, FileClass = nameof(Crop), RefId = entity.Id});
                }
                if(files.Count>0) _fileRepo.AddMany(files);
                //request.Images.Where(x => (x.Id.HasValue)).Select(s => new CRDFile() { });
               
                // entity.Images=request.Images;

                _cropRepo.Update(entity);
               await _cropRepo.SaveAsync();

            }
            else if (request.PlantType == "tree")
            {
                var entity = await _treeRepo.GetByIdAsync(request.Id);

                if (entity == null)
                {
                    throw new NotFoundException(nameof(Tree), request.Id);
                }
                entity.Info = request.Info;
                var _translation = new List<Translation>();
                request.Translations.ForEach(t =>
                {
                    _translation.Add(new Translation() { Id = t.LanguangeId, Translated = t.Translated });
                });
                entity.Translations = _translation;
                entity.Categories = request.Categories;
                entity.BotanicalName = request.BotanicalName;
                entity.CommonName = request.CommonName;
                entity.Othernames = request.Othernames;
                entity.GrowthStages = request.GrowthStages;

                _treeRepo.Update(entity);
                await _cropRepo.SaveAsync();
            }
                
        }
    }
}
