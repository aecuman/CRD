using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CRD.Application.StructureTypes.Queries
{
    public class GetAllStructureTypesQuery : IRequest<IEnumerable<StructureTypeViewModel>>
    {
    }

    public class GetAllStructureTypesQueryHandler : IRequestHandler<GetAllStructureTypesQuery, IEnumerable<StructureTypeViewModel>>
    {
        private readonly IRepository<StructureType> _repository;

        public GetAllStructureTypesQueryHandler(IRepository<StructureType> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<StructureTypeViewModel>> Handle(GetAllStructureTypesQuery request, CancellationToken cancellationToken)
        {
            var types = await _repository.GetAllAsync();
            return types.Select(t => new StructureTypeViewModel
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();
        }
    }

    public class StructureTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
