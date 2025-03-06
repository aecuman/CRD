using CRD.Application.Common;
using CRD.Application.Common.ViewModels;
using CRD.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.StructureAttributes.Queries
{
    public class GetStructureAttributesQuery : IRequest<IEnumerable<StructureAttributeViewModel>> { }


    public class GetStructureAttributesQueryHandler : IRequestHandler<GetStructureAttributesQuery, IEnumerable<StructureAttributeViewModel>>
    {
        private readonly IRepository<StructureAttribute> _repository;

        public GetStructureAttributesQueryHandler(IRepository<StructureAttribute> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<StructureAttributeViewModel>> Handle(GetStructureAttributesQuery request, CancellationToken cancellationToken)
        {
            var attributes = await _repository.GetAllAsync();
            return attributes.Select(a => new StructureAttributeViewModel
            {
                Id = a.Id,
                Name = a.Name,
                CategoryId = a.CategoryId
            }).ToList();
        }
    }

}
