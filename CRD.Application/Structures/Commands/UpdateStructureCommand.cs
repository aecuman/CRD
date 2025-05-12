using CRD.Application.Common;
using CRD.Application.Common.Exceptions;
using CRD.Application.Plants.Commands;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Structures.Commands
{
    public class UpdateStructureCommand:IRequest
    {
        public UpdateStructureCommand()
        {
            Description = new List<StructureDescriptionDto>();
        }
        public int Id {  get; set; }
        public string Name { get; set; }
        public int StructureTypeId { get; set; }
        public int CategoryId { get; set; }
        public List<StructureDescriptionDto> Description { get; set; }
    }
    public class UpdateStructureCommandHandler : IRequestHandler<UpdateStructureCommand>
    {
        private readonly ILogger<UpdateStructureCommandHandler> _logger;
        private readonly IRepository<Structure> _structureRepo;

        public UpdateStructureCommandHandler(ILogger<UpdateStructureCommandHandler> logger, IRepository<Structure> structureRepo)
        {
            _logger = logger;
            _structureRepo = structureRepo;
        }

        public async Task Handle(UpdateStructureCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new NullReferenceException();
            var entity = await _structureRepo.GetAll().AsQueryable().Include(e=>e.AttributeSelections).FirstOrDefaultAsync(r=> r.Id==request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Crop), request.Id);
            }
            entity.Name = request.Name;
            entity.StructureTypeId = request.StructureTypeId;
            entity.CategoryId = request.CategoryId;           
        
          await  _structureRepo.Update(entity);
            //  await _structureRepo.SaveAsync();
            // Manage attributes
           /* foreach (var attr in request.Description)
            {
                var attribute = await _repoStructureAttribute.GetByConditionAsync(a => a.CategoryId == id && a.Name == attr.Name);

                if (attribute == null)
                {
                    // If attribute does not exist, create it
                    attribute = new StructureAttribute
                    {
                        Name = attr.
                        CategoryId = category.Id
                    };
                    _repoStructureAttribute.AddWithoutSaving(attribute);
                }
                else
                {
                    // Update existing attribute
                    attribute.Name = attr.Name;
                    _repoStructureAttribute.UpdateWithoutSaving(attribute);
                }
                await _repoStructureAttribute.SaveAsync(); // Persist changes

                // Manage options
                foreach (var optionName in attr.Options)
                {
                    var option = await _repoStructureOption.GetByConditionAsync(o => o.AttributeId == attribute.Id && o.Name == optionName.Name);

                    if (option == null)
                    {
                        // Create new option if it doesn’t exist
                        option = new StructureOption
                        {
                            Name = optionName.Name,
                            AttributeId = attribute.Id
                        };
                        _repoStructureOption.AddWithoutSaving(option);
                    }
                    else
                    {
                        // Update existing option
                        option.Name = optionName.Name;
                        _repoStructureOption.UpdateWithoutSaving(option);
                    }
                }
                await _repoStructureOption.SaveAsync(); // Save options after iteration
            }

            return true; // Successful update
        } */
    }
    
    }
}
