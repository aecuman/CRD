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
    public record UpdateDistrictRateCommand(int Id, string DistrictId, int Year, string Status) : IRequest<bool>;
public class UpdateDistrictRateHandler : IRequestHandler<UpdateDistrictRateCommand, bool>
    {
        private readonly IRepository<DistrictRate> _context;
        public UpdateDistrictRateHandler(IRepository<DistrictRate> context) => _context = context;
        public async Task<bool> Handle(UpdateDistrictRateCommand request, CancellationToken cancellationToken)
        {
            var districtRate = await _context.SelectAsync(x=>x.Id==request.Id);
            if (districtRate == null) return false;
            districtRate.DistrictId = request.DistrictId;
            districtRate.Year = request.Year;
            districtRate.Status = request.Status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
