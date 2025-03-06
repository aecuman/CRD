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
    public class DeleteOptionCommand:IRequest<bool>
    {
        public string Option { get; set; }
        public int Id { get; set; }
    }

    public class DeleteOptionCommandHandler : IRequestHandler<DeleteOptionCommand, bool>
    {
        /*private readonly IRepository<StructureCategory> _structureCategoryRepo;
        private readonly IRepository<StructureType> _structureTypeRepo;
        private readonly IRepository<StructureDescriptionOption> _structureDescriptionOptionRepo;
        private readonly IRepository<StructureDescriptionName> _structureDescriptionNameRepo;*/
        private readonly IRepository<Category> _categoryCropRepo;
        private readonly IRepository<Languange> _languangeRepo;
        private readonly IRepository<GrowthStage> _growthStageRepo;

        public DeleteOptionCommandHandler(/*IRepository<StructureCategory> structureCategoryRepo, IRepository<StructureType> structureTypeRepo, IRepository<StructureDescriptionOption> structureDescriptionOptionRepo, IRepository<StructureDescriptionName> structureDescriptionNameRepo,*/ IRepository<Category> categoryCropRepo, IRepository<Languange> languangeRepo, IRepository<GrowthStage> growthStageRepo)
        {
            /*_structureCategoryRepo = structureCategoryRepo;
            _structureTypeRepo = structureTypeRepo;
            _structureDescriptionOptionRepo = structureDescriptionOptionRepo;
            _structureDescriptionNameRepo = structureDescriptionNameRepo;*/
            _categoryCropRepo = categoryCropRepo;
            _languangeRepo = languangeRepo;
            _growthStageRepo = growthStageRepo;
        }

        public async Task<bool> Handle(DeleteOptionCommand request, CancellationToken cancellationToken)
        {

            if (request == null) throw new NullReferenceException();
            switch (request.Option)
            {
               /* case OptionName.StructureCategoryOptionName:
                    var entity = await _structureCategoryRepo.GetByIdAsync(request.Id);
                    if (entity == null)
                    {
                        throw new NotFoundException(nameof(StructureCategory), request.Id);
                    }
                    return _structureCategoryRepo.Remove(request.Id);
                case OptionName.StructureDescriptionOptionName:
                    var entity_s_d = await _structureDescriptionOptionRepo.GetByIdAsync(request.Id);
                    if (entity_s_d == null)
                    {
                        throw new NotFoundException(nameof(StructureDescriptionOption), request.Id);
                    }
                    return _structureDescriptionOptionRepo.Remove(request.Id);
                case OptionName.StructureDescriptionNameOptionName:
                    var entity_s_n = await _structureDescriptionNameRepo.GetByIdAsync(request.Id);
                    if (entity_s_n == null)
                    {
                        throw new NotFoundException(nameof(StructureDescriptionName), request.Id);
                    }
                    return _structureDescriptionNameRepo.Remove(request.Id);

                case OptionName.StructureTypeOptionName:
                    var entity_s_t = await _structureTypeRepo.GetByIdAsync(request.Id);
                    if (entity_s_t == null)
                    {
                        throw new NotFoundException(nameof(StructureType), request.Id);
                    }
                    return _structureTypeRepo.Remove(request.Id);*/
                case OptionName.PlantCategoryOptionName:
                    var entity_p_c = await _categoryCropRepo.GetByIdAsync(request.Id);
                    if (entity_p_c == null)
                    {
                        throw new NotFoundException(nameof(Category), request.Id);
                    }
                    return _categoryCropRepo.Remove(request.Id);
                case OptionName.LanguangeOptionName:
                    var entity_l = await _languangeRepo.GetByIdAsync(request.Id);
                    if (entity_l == null)
                    {
                        throw new NotFoundException(nameof(Languange), request.Id);
                    }
                    return _languangeRepo.Remove(request.Id);
                case OptionName.GrowthStageOptionName:
                    var entity_g_s = await _growthStageRepo.GetByIdAsync(request.Id);
                    if (entity_g_s == null)
                    {
                        throw new NotFoundException(nameof(GrowthStage), request.Id);
                    }
                    return _growthStageRepo.Remove(request.Id);
                default:
                    throw new BadRequestException(request.Option + " provided does not exist as a option");
            }
        }
    }
}
