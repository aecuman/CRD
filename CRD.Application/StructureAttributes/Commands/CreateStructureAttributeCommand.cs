using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.StructureAttributes.Commands
{
    public class CreateStructureAttributeCommand : IRequest<int>
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public List<string> Options { get; set; }
    }

    public class CreateStructureAttributeCommandHandler : IRequestHandler<CreateStructureAttributeCommand, int>
    {
        private readonly IRepository<StructureAttribute> _attributeRepository;
        private readonly IRepository<StructureOption> _optionRepository;

        public CreateStructureAttributeCommandHandler(
            IRepository<StructureAttribute> attributeRepository,
            IRepository<StructureOption> optionRepository)
        {
            _attributeRepository = attributeRepository;
            _optionRepository = optionRepository;
        }

        public async Task<int> Handle(CreateStructureAttributeCommand request, CancellationToken cancellationToken)
        {
            var list = new List<StructureOption>();
            if (request.Options != null)
            {
                foreach (var optionName in request.Options)
                {
                    var option = new StructureOption
                    {
                        Name = optionName
                    };
                    list.Add(option);
                   // _optionRepository.Add(option);
                }
            }
            var attribute = new StructureAttribute
            {
                CategoryId = request.CategoryId,
                Name = request.Name,
                Options = list
            };

            _attributeRepository.Add(attribute);
           return await _attributeRepository.SaveAsync();

           /* if (request.Options != null)
            {
                foreach (var optionName in request.Options)
                {
                    var option = new StructureOption
                    {
                        AttributeId = createdAttribute.Id,
                        Name = optionName
                    };
                    _optionRepository.Add(option);
                }
            }

            return createdAttribute.Id;*/
        }
    }
}
