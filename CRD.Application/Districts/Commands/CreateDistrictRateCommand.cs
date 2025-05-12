using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Districts.Commands
{
    public record CreateDistrictRateCommand(int DistrictId, int Year, List<int> UploadsIds) : IRequest<int>;
    public class CreateDistrictRateHandler : IRequestHandler<CreateDistrictRateCommand, int>
    {
        private readonly IRepository<DistrictRate> _context;
        public CreateDistrictRateHandler(IRepository<DistrictRate> context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateDistrictRateCommand request, CancellationToken cancellationToken)
        {
            var districtRate = new DistrictRate
            {
                DistrictId = request.DistrictId,
                Year = request.Year,
                Status = DistrictRateStatus.Pending,
                UploadIds = request.UploadsIds
               // Uploads = _context_files.GetAll().Where(f => request.UploadsIds.Contains(f.Id)).ToList()
            };
            _context.AddWithoutSaving(districtRate);
            await _context.SaveAsync();
            return districtRate.Id;
        }
    }
}
