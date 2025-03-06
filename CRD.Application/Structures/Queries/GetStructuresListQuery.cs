using CRD.Application.Common;
using CRD.Application.Common.Dtos;
using CRD.Application.Common.ViewModels;
using CRD.Application.Structures.Commands;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Structures.Queries
{
    public class GetStructuresListQuery : IRequest<IEnumerable<StructureViewDto>>
    {
    }

    public class GetStructuresListQueryHandler : IRequestHandler<GetStructuresListQuery, IEnumerable<StructureViewDto>>
    {
        private readonly ILogger<GetStructuresListQueryHandler> _logger;
        private readonly IRepository<Structure> _structureRepo;
        //private readonly IRepository<StructureCategory> _structureCategoryRepo;
        //private readonly IRepository<StructureType> _structureTypeRepo;
        //private readonly IRepository<StructureDescriptionOption> _structureDescriptionOptionRepo;
        //private readonly IRepository<StructureDescriptionName> _structureDescriptionNameRepo;


        public GetStructuresListQueryHandler(ILogger<GetStructuresListQueryHandler> logger, IRepository<Structure> structureRepo/*, IRepository<StructureCategory> structureCategoryRepo, IRepository<StructureType> structureTypeRepo, IRepository<StructureDescriptionOption> structureDescriptionOptionRepo, IRepository<StructureDescriptionName> structureDescriptionNameRepo*/)
        {
            _logger = logger;
            _structureRepo = structureRepo;
            // _structureCategoryRepo = structureCategoryRepo;
            // _structureTypeRepo = structureTypeRepo;
            // _structureDescriptionOptionRepo = structureDescriptionOptionRepo;
            //_structureDescriptionNameRepo = structureDescriptionNameRepo;
        }

        public async Task<IEnumerable<StructureViewDto>> Handle(GetStructuresListQuery request, CancellationToken cancellationToken)
        {
            var structures = _structureRepo.GetAll().AsQueryable()
                    .Include(s => s.Category)
        .Include(s => s.StructureType)
        .Include(s => s.AttributeSelections) // Load AttributeSelections
            .ThenInclude(a => a.Attribute) // Ensure Attribute is loaded
        .Include(s => s.AttributeSelections) // Load Options for each Attribute
            .ThenInclude(a => a.OptionSelections)
            .ThenInclude(o => o.Option)
                .ToList();

            return await Task.FromResult(structures.Select(s => new StructureViewDto
            {
                Id = s.Id,
                Name = s.Name,
                Category = new StructureCategoryDto { Id = s.Category.Id, Name = s.Category.Name },
                StructureType = new StructureTypeDto { Id = s.StructureType.Id, Name = s.StructureType.Name },
                AttributeSelections = s.AttributeSelections?.Select(a => new StructureAttributeSelectionDto
                {
                    AttributeId = a.Attribute?.Id ?? 0,  // Handle possible null reference
                    AttributeName = a.Attribute?.Name ?? "Unknown",
                    SelectedOptions = a.OptionSelections?.Select(o => new StructureOptionDto
                    {
                        Id = o.Option?.Id ?? 0,  // Handle possible null reference
                        Name = o.Option?.Name ?? "Unknown"
                    }).ToList() ?? new List<StructureOptionDto>() // Ensure no null collection
                }).ToList() ?? new List<StructureAttributeSelectionDto>() // Ensure no null collection
            }).ToList());
            // var list = await _structureRepo.GetAllAsync();

            /*return list.Select(x => new GetStructuresListViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                CategoryId = x.CategoryId,
                CategoryName = _structureCategoryRepo.GetById(x.CategoryId).Name,
                StructureTypeId = x.StructureTypeId,
                StructureTypeName = _structureTypeRepo.GetById(x.StructureTypeId).Name,
                Description = GetDescriptionDetail(x.Description)
            }).ToList();
           
            return structures.Select(s => new StructureViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Category = new StructureCategoryViewModel
                {
                    Id = s.Category.Id,
                    Name = s.Category.Name
                },
                Type = new StructureTypeViewModel
                {
                    Id = s.StructureType.Id,
                    Name = s.StructureType.Name
                },
                Attributes = s.Category.Attributes.Select(a => new StructureAttributeViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Options = a.Options.Select(o => new StructureOptionViewModel
                    {
                        Id = o.Id,
                        Name = o.Name
                    }).ToList()
                }).ToList()
            }).ToList();
 */
        }
    }

        /*
        private List<DescriptionViewModel> GetDescriptionDetail(List<StructureDescription> description)
        {
            var list = new List<DescriptionViewModel>();
            description.ForEach(d => 
            {
                list.Add(new DescriptionViewModel()
                {
                    DescreptionNameId = d.StructureDescriptionNameId,
                    DescreptionName = _structureDescriptionNameRepo.GetById(d.StructureDescriptionNameId).Name,
                    DescriptionOptionId = d.DescriptionOptionValue,
                    DescriptionOptionName = _structureDescriptionOptionRepo.GetById(d.DescriptionOptionValue).Name              

                });
            });
            return list;
        }
    }
    */
       
    }
    public class GetStructuresListViewModel
        {
            public GetStructuresListViewModel()
            {
                Description = new List<DescriptionViewModel>();
            }
            public int Id { get; set; }
            public string Name { get; set; }
            public int StructureTypeId { get; set; }
            public string StructureTypeName { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public List<DescriptionViewModel> Description { get; set; }

        }

        public class DescriptionViewModel
        {
            public int DescreptionNameId { get; set; }
            public string DescreptionName { get; set; }
            public int DescriptionOptionId { get; set; }
            public string DescriptionOptionName { get; set; }
        }
   
