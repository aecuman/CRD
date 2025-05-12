using CRD.Application.Common;
using CRD.Application.Common.Exceptions;
using CRD.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Options.Commands
{
    public class CreateOptionCommand:IRequest<int>
    {
        public string Option { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public int? StructureDescriptionNameId { get; set; }
        public StructureCategoryDescriptionDto[]? Options { get; set; }
    }
    public class StructureCategoryDescriptionDto
    {
        public string Name { get; set; }
        public string[] Options { get; set; }
    }
    public class CreateOptionCommandHandler : IRequestHandler<CreateOptionCommand, int>
    {
       /* private readonly IRepository<StructureCategory> _structureCategoryRepo;
        private readonly IRepository<StructureType> _structureTypeRepo;
        private readonly IRepository<StructureDescriptionOption> _structureDescriptionOptionRepo;
        private readonly IRepository<StructureDescriptionName> _structureDescriptionNameRepo;*/
        private readonly IRepository<Category> _categoryCropRepo;
        private readonly IRepository<Languange> _languangeRepo;
        private readonly IRepository<GrowthStage> _growthStageRepo;

        public CreateOptionCommandHandler(/*IRepository<StructureCategory> structureCategoryRepo, IRepository<StructureType> structureTypeRepo,/* IRepository<StructureDescriptionOption> structureDescriptionOptionRepo, IRepository<StructureDescriptionName> structureDescriptionNameRepo,*/ IRepository<Category> categoryCropRepo, IRepository<Languange> languangeRepo, IRepository<GrowthStage> growthStageRepo)
        {
         /*   _structureCategoryRepo = structureCategoryRepo;
            _structureTypeRepo = structureTypeRepo;
            _structureDescriptionOptionRepo = structureDescriptionOptionRepo;
            _structureDescriptionNameRepo = structureDescriptionNameRepo;*/
            _categoryCropRepo = categoryCropRepo;
            _languangeRepo = languangeRepo;
            _growthStageRepo = growthStageRepo;
        }

        public async Task<int> Handle(CreateOptionCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new NullReferenceException();
            switch (request.Option)
            {
                /*case OptionName.StructureCategoryOptionName:
                    var descNames = new List<StructureDescriptionName>();
                    if (request.Options?.Length > 0)
                    {
                        foreach (var name in request.Options)
                        {
                            var ops = new List<StructureDescriptionOption>();
                            foreach (var option in name.Options)
                            {
                                ops.Add(new StructureDescriptionOption() { Name =  option });
                            }
                            descNames.Add(new StructureDescriptionName() { Name = name.Name, StructureDescriptionOptions= ops });
                        }
                    }
                    _structureCategoryRepo.Add(new StructureCategory() { Name = request.Name,StructureCategoryOptions=descNames });
                  return  await _structureCategoryRepo.SaveAsync();                 
                case OptionName.StructureDescriptionOptionName:
                    _structureDescriptionOptionRepo.Add(new StructureDescriptionOption() { Name = request.Name, StructureDescriptionNameId=request.StructureDescriptionNameId.HasValue?request.StructureDescriptionNameId.Value:int.MinValue}); 
                    return await _structureDescriptionOptionRepo.SaveAsync();
                case OptionName.StructureDescriptionNameOptionName:
                    _structureDescriptionNameRepo.Add(new StructureDescriptionName() { Name = request.Name,CategoryId=request.CategoryId.HasValue?request.CategoryId.Value:int.MinValue });
                    return await _structureDescriptionNameRepo.SaveAsync();
                case OptionName.StructureTypeOptionName:
                    _structureTypeRepo.Add(new StructureType() { Name = request.Name });
                  return await _structureTypeRepo.SaveAsync();*/
                case OptionName.PlantCategoryOptionName:
                    _categoryCropRepo.AddWithoutSaving(new Category() { Name = request.Name });
                    return await _categoryCropRepo.SaveAsync();
                case OptionName.LanguangeOptionName:
                    _languangeRepo.AddWithoutSaving(new Languange() { Name = request.Name });
                    return await _languangeRepo.SaveAsync();
                case OptionName.GrowthStageOptionName:
                    _growthStageRepo.AddWithoutSaving(new GrowthStage() { Name = request.Name });
                   return await _growthStageRepo.SaveAsync();
                default:
                    throw new BadRequestException(request.Option+" does not exist as a option");
            }
            
           
        }
    }

}
