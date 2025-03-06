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
    public record DeleteDistrictRateCommand(int Id) : IRequest<bool>;
    public class DeleteDistrictRateHandler : IRequestHandler<DeleteDistrictRateCommand, bool>
    {
        private readonly IRepository<DistrictRate> _context;
        public DeleteDistrictRateHandler(IRepository<DistrictRate> context) => _context = context;
        public async Task<bool> Handle(DeleteDistrictRateCommand request, CancellationToken cancellationToken)
        {
            var districtRate = await _context.SelectAsync(x => x.Id == request.Id);
            if (districtRate == null) return false;
            _context.Remove(districtRate.Id);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
