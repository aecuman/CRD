using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;

namespace CRD.Application.StructureRates.Commands
{
    public class DeleteStructureRateCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
    public class DeleteStructureRateCommandHandler : IRequestHandler<DeleteStructureRateCommand, bool>
    {
        private readonly IRepository<StructureRate> _context;

        public DeleteStructureRateCommandHandler(IRepository<StructureRate> context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteStructureRateCommand request, CancellationToken cancellationToken)
        {
            var rate = await _context.GetByIdAsync(request.Id);
            if (rate == null)
                return false;

            _context.Remove(rate.Id);
           // await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

}
