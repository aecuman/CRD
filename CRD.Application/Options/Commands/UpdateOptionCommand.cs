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
    public class UpdateOptionCommand:IRequest
    {
        public string Option { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class UpdateOptionCommandHandler : IRequestHandler<UpdateOptionCommand>
    {
        /*private readonly IRepository<StructureCategory> _structureCategoryRepo;
        private readonly IRepository<StructureType> _structureTypeRepo;
        private readonly IRepository<StructureDescriptionOption> _structureDescriptionOptionRepo;
        private readonly IRepository<StructureDescriptionName> _structureDescriptionNameRepo;*/
        private readonly IRepository<Category> _categoryCropRepo;
        private readonly IRepository<Languange> _languangeRepo;
        private readonly IRepository<GrowthStage> _growthStageRepo;

        public UpdateOptionCommandHandler(/*IRepository<StructureCategory> structureCategoryRepo, IRepository<StructureType> structureTypeRepo, IRepository<StructureDescriptionOption> structureDescriptionOptionRepo, IRepository<StructureDescriptionName> structureDescriptionNameRepo,*/ IRepository<Category> categoryCropRepo, IRepository<Languange> languangeRepo, IRepository<GrowthStage> growthStageRepo)
        {
           /* _structureCategoryRepo = structureCategoryRepo;
            _structureTypeRepo = structureTypeRepo;
            _structureDescriptionOptionRepo = structureDescriptionOptionRepo;
            _structureDescriptionNameRepo = structureDescriptionNameRepo;*/
            _categoryCropRepo = categoryCropRepo;
            _languangeRepo = languangeRepo;
            _growthStageRepo = growthStageRepo;
        }

        public async Task Handle(UpdateOptionCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new NullReferenceException();
            switch (request.Option)
            {
               /* case OptionName.StructureCategoryOptionName:
                    var entity = await _structureCategoryRepo.GetByIdAsync(request.Id);
                    entity.Name= request.Name;
                    _structureCategoryRepo.Update(entity);
                    _structureCategoryRepo.Save();
                    break;
                case OptionName.StructureDescriptionOptionName:
                    var entity_s_d = await _structureDescriptionOptionRepo.GetByIdAsync(request.Id);
                    entity_s_d.Name = request.Name;
                    _structureDescriptionOptionRepo.Update(entity_s_d);
                    _structureDescriptionOptionRepo.Save();
                    break;
                case OptionName.StructureDescriptionNameOptionName:
                    var entity_s_n = await _structureDescriptionNameRepo.GetByIdAsync(request.Id);
                    entity_s_n.Name = request.Name;
                    _structureDescriptionNameRepo.Update(entity_s_n);
                    _structureDescriptionNameRepo.Save();
                    break;
                   
                case OptionName.StructureTypeOptionName:
                    var entity_s_t = await _structureTypeRepo.GetByIdAsync(request.Id);
                    entity_s_t.Name = request.Name;
                    _structureTypeRepo.Update(entity_s_t);
                    _structureTypeRepo.Save();
                    break;           */        
                case OptionName.PlantCategoryOptionName:
                    var entity_p_c = await _categoryCropRepo.GetByIdAsync(request.Id);
                    entity_p_c.Name = request.Name;
                    _categoryCropRepo.Update(entity_p_c);
                    _categoryCropRepo.Save();
                    break;                    
                case OptionName.LanguangeOptionName:
                    var entity_l = await _languangeRepo.GetByIdAsync(request.Id);
                    entity_l.Name = request.Name;
                    _languangeRepo.Update(entity_l);
                    _languangeRepo.Save();
                    break;
                case OptionName.GrowthStageOptionName:
                    var entity_g_s = await _growthStageRepo.GetByIdAsync(request.Id);
                    entity_g_s.Name = request.Name;
                    _growthStageRepo.Update(entity_g_s);
                    _growthStageRepo.Save();
                    break;                   
                default:
                    throw new BadRequestException(request.Option + " provided does not exist as a option");
            }
        }
    }
}
