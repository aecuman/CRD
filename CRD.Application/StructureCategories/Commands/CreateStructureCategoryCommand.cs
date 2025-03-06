using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.StructureCategories.Commands
{
    public class CreateStructureCategoryCommand : IRequest<int>
    {
        public string Name { get; set; }
        public List<StructureAttributeDto> Attributes { get; set; } = new List<StructureAttributeDto>();
    }

    public class StructureAttributeDto
    {
        public string Name { get; set; }
        public List<string> Options { get; set; } = new List<string>();
    }

    public class CreateStructureCategoryCommandHandler : IRequestHandler<CreateStructureCategoryCommand, int>
    {
        private readonly IRepository<StructureCategory> _repository;
        private readonly IRepository<StructureAttribute> _repoStructureAttribute;
        private readonly IRepository<StructureOption> _repoStructureOption;


        public CreateStructureCategoryCommandHandler(IRepository<StructureCategory> repository, IRepository<StructureAttribute> repoStructureAttribute, IRepository<StructureOption> repoStructureOption)
        {
            _repository = repository;
            _repoStructureAttribute = repoStructureAttribute;
            _repoStructureOption = repoStructureOption;
        }

        public async Task<int> Handle(CreateStructureCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new StructureCategory
            {
                Name = request.Name
            };

            _repository.Add(category);
            await _repository.SaveChangesAsync();

            foreach (var attr in request.Attributes)
            {
                var attribute = new StructureAttribute
                {
                    Name = attr.Name,
                    CategoryId = category.Id
                };

                _repoStructureAttribute.AddWithoutSaving(attribute);
                await _repoStructureAttribute.SaveChangesAsync();

                foreach (var optionName in attr.Options)
                {
                    var option = new StructureOption
                    {
                        Name = optionName,
                        AttributeId = attribute.Id
                    };

                    _repoStructureOption.AddWithoutSaving(option);
                }
                await _repository.SaveChangesAsync();
            }

            return category.Id;
            /* var attrs= new List<StructureAttribute>();
             foreach (var attr in request.Attributes) {
                 var list = new List<StructureOption>();
                 if (attr.Options != null)
                 {
                     foreach (var optionName in attr.Options)
                     {
                         var option = new StructureOption
                         {
                             Name = optionName
                         };
                         list.Add(option);
                         // _optionRepository.Add(option);
                     }
                 }
                 attrs.Add(new StructureAttribute() {Name=attr.Name, Options=list });
             }

             var category = new StructureCategory
             {
                 Name = request.Name,
                 Attributes = attrs
             };

             _repository.Add(category);
             return await _repository.SaveAsync();*/
        }
    }

}
