using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Districts.Queries
{
    public class GetAllDistrictsQuery : IRequest<List<District>>;
    public class GetAllDistrictsHandler : IRequestHandler<GetAllDistrictsQuery, List<District>>
    {
        private readonly IRepository<District> _context;
        public GetAllDistrictsHandler(IRepository<District> context) => _context = context;
        public async Task<List<District>> Handle(GetAllDistrictsQuery request, CancellationToken cancellationToken)
            => await _context.GetAllAsync();
    }
}
