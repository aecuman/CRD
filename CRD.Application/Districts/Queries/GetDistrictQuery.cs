using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Districts.Queries
{
    public record GetDistrictRatesQuery() : IRequest<List<DistrictRate>>;
    public class GetDistrictRatesHandler : IRequestHandler<GetDistrictRatesQuery, List<DistrictRate>>
    {
        private readonly IRepository<DistrictRate> _context;
        public GetDistrictRatesHandler(IRepository<DistrictRate> context) => _context = context;
        public async Task<List<DistrictRate>> Handle(GetDistrictRatesQuery request, CancellationToken cancellationToken)
            => await _context.GetAll().AsQueryable().Include(dr => dr.Uploads).Include(dr => dr.District).ToListAsync();
    }
}
