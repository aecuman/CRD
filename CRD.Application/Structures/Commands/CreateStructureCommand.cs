using CRD.Application.Common;
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
    public class CreateStructureCommand:IRequest<int>
    {
        public CreateStructureDto Structure { get; set; }
        /*public CreateStructureCommand()
        {
            Description = new List<StructureDescriptionDto>();
        }
        public string Name { get; set; }
        public int StructureTypeId { get; set; }
        public int CategoryId { get; set; }
        public List<StructureDescriptionDto> Description { get; set; }*/
    }

    public class CreateStructureCommandHandler : IRequestHandler<CreateStructureCommand, int>
    {
        private readonly ILogger<CreateStructureCommandHandler> _logger;
        private readonly IRepository<Structure> _structureRepo;
        private readonly IRepository<StructureType> _structureTypeRepo;
        private readonly IRepository<StructureCategory> _structureCategoriesRepo;
        private readonly IRepository<StructureAttributeSelection> _structureAttributeSelectionRepo;
        private readonly IRepository<StructureOptionSelection> _structureOptionSelectionRepo;

        public CreateStructureCommandHandler(ILogger<CreateStructureCommandHandler> logger, IRepository<Structure> structureRepo, IRepository<StructureAttributeSelection> structureAttributeSelectionRepo, IRepository<StructureOptionSelection> structureOptionSelectionRepo, IRepository<StructureType> structureTypeRepo, IRepository<StructureCategory> structureCategoriesRepo)
        {
            _logger = logger;
            _structureRepo = structureRepo;
            _structureAttributeSelectionRepo = structureAttributeSelectionRepo;
            _structureOptionSelectionRepo = structureOptionSelectionRepo;
            _structureTypeRepo = structureTypeRepo;
            _structureCategoriesRepo = structureCategoriesRepo;
        }

        public async Task<int> Handle(CreateStructureCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new NullReferenceException();
            // Validate Category
            var category = await _structureCategoriesRepo.GetAll().AsQueryable()
                .Include(c => c.Attributes)
                .ThenInclude(a => a.Options)
                .FirstOrDefaultAsync(c => c.Id == request.Structure.CategoryId, cancellationToken);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {request.Structure.CategoryId} not found.");
            }

            // Validate Structure Type
            var structureType = await _structureTypeRepo.GetAll().AsQueryable()
                .FirstOrDefaultAsync(st => st.Id == request.Structure.StructureTypeId, cancellationToken);

            if (structureType == null)
            {
                throw new KeyNotFoundException($"Structure Type with ID {request.Structure.StructureTypeId} not found.");
            }

            // Create Structure
            var structure = new Structure
            {
                Name = request.Structure.Name,
                StructureTypeId = request.Structure.StructureTypeId,
                CategoryId = request.Structure.CategoryId
            };

            _structureRepo.Add(structure);
            await _structureRepo.SaveChangesAsync();

            // Store Attribute Selections
            foreach (var attrSelectionDto in request.Structure.AttributeSelections)
            {
                var attribute = category.Attributes.FirstOrDefault(a => a.Id == attrSelectionDto.AttributeId);
                if (attribute == null)
                {
                    throw new KeyNotFoundException($"Attribute with ID {attrSelectionDto.AttributeId} not found in Category {category.Name}.");
                }

                var attributeSelection = new StructureAttributeSelection
                {
                    StructureId = structure.Id,
                    AttributeId = attrSelectionDto.AttributeId
                };

                _structureAttributeSelectionRepo.AddWithoutSaving(attributeSelection);
                await _structureAttributeSelectionRepo.SaveChangesAsync();

                // Store Option Selections
                foreach (var optionId in attrSelectionDto.OptionIds)
                {
                    var option = attribute.Options.FirstOrDefault(o => o.Id == optionId);
                    if (option == null)
                    {
                        throw new KeyNotFoundException($"Option with ID {optionId} not found for Attribute {attribute.Name}.");
                    }

                    var optionSelection = new StructureOptionSelection
                    {
                        AttributeSelectionId = attributeSelection.Id,
                        OptionId = optionId
                    };

                    _structureOptionSelectionRepo.Add(optionSelection);
                }
            }

            await _structureRepo.SaveChangesAsync();
            return structure.Id;
            /*var _description = new List<StructureDescription>();
            var i = 0;
            request.Description.ForEach(x =>
            {
                _description.Add(new StructureDescription() { Id=i++,StructureDescriptionNameId= x.StructureDescriptionId,DescriptionOptionValue= x.StructureDescriptionOptionId });
            });
            _structureRepo.Add(
                new Structure()
                {
                    CategoryId = request.CategoryId,
                    StructureTypeId = request.StructureTypeId,
                    Name = request.Name,
                    Description = _description
                });
            _structureRepo.Add(
                new Structure()
                {
                    Name = request.Name,
                    StructureTypeId = request.StructureTypeId,
                    CategoryId = request.CategoryId
                });

            return _structureRepo.SaveAsync();*/
        }
    }

    public class StructureDescriptionDto
    {
        public int StructureDescriptionId { get; set; }
        public int StructureDescriptionOptionId { get; set; }
    }
    public class CreateStructureDto
    {
        public string Name { get; set; }
        public int StructureTypeId { get; set; }
        public int CategoryId { get; set; }
        public List<CreateStructureAttributeSelectionDto> AttributeSelections { get; set; } = new List<CreateStructureAttributeSelectionDto>();
    }

    public class CreateStructureAttributeSelectionDto
    {
        public int AttributeId { get; set; }
        public List<int> OptionIds { get; set; } = new List<int>();
    }
}
