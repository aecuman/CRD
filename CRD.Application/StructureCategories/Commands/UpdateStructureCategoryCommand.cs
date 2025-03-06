using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.StructureCategories.Commands
{
    public class UpdateStructureCategoryCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UpdateStructureAttributeDto> Attributes { get; set; } = new List<UpdateStructureAttributeDto>();
    }

    public class UpdateStructureCategoryCommandHandler : IRequestHandler<UpdateStructureCategoryCommand, bool>
    {
        private readonly IRepository<StructureCategory> _repository;
        private readonly IRepository<StructureAttribute> _repoStructureAttribute;

        public UpdateStructureCategoryCommandHandler(IRepository<StructureCategory> repository, IRepository<StructureAttribute> repoStructureAttribute)
        {
            _repository = repository;
            _repoStructureAttribute = repoStructureAttribute;
        }

        public async Task<bool> Handle(UpdateStructureCategoryCommand request, CancellationToken cancellationToken)
        {
            /*  var category = await _repository.GetByIdAsync(request.Id);
            if (category == null) return false;

           category.Name = request.Name;
              _repository.Update(category);
              await _repository.SaveAsync();
             return true;*/
            var category = await _repository.GetAll().AsQueryable()
             .Include(c => c.Attributes)
                 .ThenInclude(a => a.Options)
             .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (category == null)
                return false;

            category.Name = request.Name;
            var attributeNames = new HashSet<string>();

            // Handle attribute updates
            foreach (var attrDto in request.Attributes)
            {
                if (!attributeNames.Add(attrDto.Name))
                    throw new ValidationException($"Duplicate attribute name '{attrDto.Name}' is not allowed.");
                var existingAttr = category.Attributes.FirstOrDefault(a => a.Id == attrDto.Id);

                if (existingAttr != null)
                {
                    existingAttr.Name = attrDto.Name;
                    var optionNames = new HashSet<string>();

                    // Handle option updates
                    foreach (var optDto in attrDto.Options)
                    {
                        if (!optionNames.Add(optDto.Name))
                            throw new ValidationException($"Duplicate option name '{optDto.Name}' in attribute '{attrDto.Name}' is not allowed.");
                        var existingOpt = existingAttr.Options.FirstOrDefault(o => o.Id == optDto.Id);

                        if (existingOpt != null)
                        {
                            existingOpt.Name = optDto.Name;
                        }
                        else
                        {
                            existingAttr.Options.Add(new StructureOption
                            {
                                Name = optDto.Name
                            });
                        }
                    }

                    // Remove options that were not sent in the request
                    existingAttr.Options.ToList().RemoveAll(opt => !attrDto.Options.Any(o => o.Id == opt.Id));
                }
                else
                {
                    // Add new attribute
                    var newAttr = new StructureAttribute
                    {
                        Name = attrDto.Name,
                        CategoryId = category.Id,
                        Options = attrDto.Options.Select(o => new StructureOption { Name = o.Name }).ToList()
                    };
                    _repoStructureAttribute.AddWithoutSaving(newAttr);
                }
            }

            // Remove attributes that were not sent in the request
            category.Attributes.ToList().RemoveAll(attr => !request.Attributes.Any(a => a.Id == attr.Id));

            await _repository.SaveChangesAsync();
            return true;
        }
    }
    public class UpdateStructureCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UpdateStructureAttributeDto> Attributes { get; set; } = new List<UpdateStructureAttributeDto>();
    }

    public class UpdateStructureAttributeDto
    {
        public int? Id { get; set; } // Nullable for new attributes
        public string Name { get; set; }
        public List<UpdateStructureOptionDto> Options { get; set; } = new List<UpdateStructureOptionDto>();
    }

    public class UpdateStructureOptionDto
    {
        public int? Id { get; set; } // Nullable for new options
        public string Name { get; set; }
    }

}
