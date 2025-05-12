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
    public class GetDistrictRateByIdQuery:IRequest<DistrictRateDto>
    {
        public int Id { get; set; }
    }

    public class GetDistrictRateByIdHandler : IRequestHandler<GetDistrictRateByIdQuery, DistrictRateDto>
    {
        private readonly IRepository<DistrictRate> _context;
        private readonly IRepository<CRDFile> _contextFiles;

        public GetDistrictRateByIdHandler(IRepository<DistrictRate> context, IRepository<CRDFile> contextFiles)
        {
            _context = context;
            _contextFiles = contextFiles;
        }

        public async Task<DistrictRateDto> Handle(GetDistrictRateByIdQuery request, CancellationToken cancellationToken)
        {
            var dr = await _context.GetAll().AsQueryable()
                   .Include(dr => dr.District)
                   .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (dr == null)
                throw new NullReferenceException("District Rate not found");

            var fileDtos = dr.UploadIds.Any()
                ? await _contextFiles.GetAll().AsQueryable()
                    .Where(x => dr.UploadIds.Contains(x.Id))
                    .Select(file => new CRDFileDto
                    {
                        Id = file.Id,
                        FileClass = file.FileClass,
                        Name = file.Name,
                        Url = file.Url
                    }).ToListAsync(cancellationToken)
                : new List<CRDFileDto>();

            var comparableDtos = (dr.ComparableDistrictRatesIds ?? new List<int>()).Any()
                ? await _context.GetAll().AsQueryable()
                    .Include(c => c.District)
                    .Where(c => dr.ComparableDistrictRatesIds.Contains(c.Id))
                    .Select(c => new SimpleComparableDto
                    {
                        Id = c.Id,
                        DistrictName = c.District.Name,
                        Year = c.Year
                    }).ToListAsync(cancellationToken)
                : new List<SimpleComparableDto>();

            return new DistrictRateDto
            {
                Id = dr.Id,
                DistrictId = dr.DistrictId,
                DistrictName = dr.District.Name,
                Year = dr.Year,
                Status = dr.Status.ToString(),
                Uploads = fileDtos,
                ComparableDistrictRates = comparableDtos
                // Fill in other fields (WorkflowName, ActiveStep, etc.) if needed
            };
        }
    }
}
