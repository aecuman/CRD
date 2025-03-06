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
    public class UpdateStructureAttributeCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Options { get; set; }
    }

    public class UpdateStructureAttributeCommandHandler : IRequestHandler<UpdateStructureAttributeCommand, bool>
    {
        private readonly IRepository<StructureAttribute> _attributeRepository;
        private readonly IRepository<StructureOption> _optionRepository;

        public UpdateStructureAttributeCommandHandler(
            IRepository<StructureAttribute> attributeRepository,
            IRepository<StructureOption> optionRepository)
        {
            _attributeRepository = attributeRepository;
            _optionRepository = optionRepository;
        }

        public async Task<bool> Handle(UpdateStructureAttributeCommand request, CancellationToken cancellationToken)
        {
            var attribute = await _attributeRepository.GetByIdAsync(request.Id);
            if (attribute == null) return false;

            attribute.Name = request.Name;
            _attributeRepository.Update(attribute);

            // Remove old options
            var existingOptions = await _optionRepository.GetAllAsync();
            foreach (var option in existingOptions)
            {
                if (option.AttributeId == attribute.Id)
                {
                    _optionRepository.Remove(option.Id);
                }
            }

            // Add new options
            foreach (var optionName in request.Options)
            {
                var option = new StructureOption
                {
                    AttributeId = attribute.Id,
                    Name = optionName
                };
                _optionRepository.Add(option);
            }
            await _attributeRepository.SaveAsync();
            await _optionRepository.SaveAsync();

            return true;
        }
    }
}
