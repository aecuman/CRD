using CRD.Application.Common;
using CRD.Application.Common.ViewModels;
using CRD.Application.Structures.Queries;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.StructureCategories.Queries
{
    public class GetAllStructureCategoriesQuery : IRequest<IEnumerable<StructureCategoryViewModel>> { }
   
    public class GetAllStructureCategoriesQueryHandler : IRequestHandler<GetAllStructureCategoriesQuery, IEnumerable<StructureCategoryViewModel>>
    {
        private readonly IRepository<StructureCategory> _repository;

        public GetAllStructureCategoriesQueryHandler(IRepository<StructureCategory> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<StructureCategoryViewModel>> Handle(GetAllStructureCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _repository.GetAll().AsQueryable().Include(sc => sc.Attributes)
                .ThenInclude(a => a.Options)
            .ToListAsync();
            return categories.Select(c => new StructureCategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Attributes = c.Attributes.Select(a => new StructureAttributeViewModel
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
        }
    }

}
