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
        public List<CategoryInfoDto> CategoryInfos { get; set; } = new List<CategoryInfoDto>();
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
                
                    // Fetch the entity
                    var entity = await _cropRepo.GetAll().AsQueryable()
                    .Include(p => p.Translations)                    
    .Include(p => p.CategoryInform)
    .Include(p => p.Images)
    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                if (entity == null)
                    {
                        throw new NotFoundException(nameof(Crop), request.Id);
                    }

                    // Update fields
                    entity.Info = request.Info;
                    entity.Aez = request.Aez;
                    entity.BotanicalName = request.BotanicalName;
                    entity.CommonName = request.CommonName;
                    entity.CropType = request.CropType;
                    entity.Categories = request.Categories;
                    var existingCategoryInforms = entity.CategoryInform
    .Where(ci => request.CategoryInfos.Select(c => c.CategoryId).Contains(ci.CategoryId))
    .ToList();

                   /*    */ foreach (var newCategoryInfo in request.CategoryInfos)
                    {
                        var existingEntry = existingCategoryInforms.FirstOrDefault(ci => ci.CategoryId == newCategoryInfo.CategoryId);

                        if (existingEntry != null)
                        {
                            // Update existing entry if Info has changed
                            if (existingEntry.Info != newCategoryInfo.CategoryInfo)
                            {
                                existingEntry.Info = newCategoryInfo.CategoryInfo;
                                //context.CategoryInforms.Update(existingEntry);
                            }
                        }
                        else
                        {
                            // Add new entry if it doesn't exist
                            entity.CategoryInform.Add(new CategoryInfo
                            {
                                CategoryId = newCategoryInfo.CategoryId,
                                Info = newCategoryInfo.CategoryInfo
                            });
                        }
                    }
               
                    
                    entity.GrowthStages = request.GrowthStages;

                    // Handle Translations
                    var existingTranslations = entity.Translations;
                    var updatedTranslations = request.Translations;
             
             /* */   foreach (var t in updatedTranslations)
                    {
                        var existing = existingTranslations.FirstOrDefault(e => e.LanguageId == t.LanguangeId);
                        if (existing != null)
                        {
                            existing.Translated = t.Translated;
                        }
                        else
                        {
                            entity.Translations.Add(new Translation
                            {
                                //d = t.LanguangeId,
                                LanguageId = t.LanguangeId,
                                Translated = t.Translated,
                                CropId = entity.Id
                            });
                        }
                    }
   
                    var toRemove = existingTranslations
                        .Where(e => !updatedTranslations.Any(t => t.LanguangeId == e.LanguageId))
                        .ToList();
                    toRemove.ForEach(t => entity.Translations.Remove(t));
         
                    // Handle Images (commented out section with improvement)
                   /* var filesToAdd = request.Images
                        .Where(x => !x.Id.HasValue)
                        .Select(f => new CRDFile
                        {
                            Name = f.Name,
                            Url = f.Url,
                            FileClass = nameof(Crop),
                            RefId = entity.Id
                        }).ToList();

                    if (filesToAdd.Any())
                    {
                        await _fileRepo.AddManyAsync(filesToAdd);
                    }*/

                    // Update the entity
                    await _cropRepo.Update(entity);
               // await _cropRepo.SaveAsync();
                

            }
            else if (request.PlantType == "tree")
            {
                var entity = await _treeRepo.GetAll().AsQueryable()
                    .Include(p => p.Translations)
    .Include(p => p.CategoryInform)
    .Include(p => p.Images)
    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

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

                await _treeRepo.Update(entity);
                await _cropRepo.SaveAsync();
            }
                
        }
    }
}
