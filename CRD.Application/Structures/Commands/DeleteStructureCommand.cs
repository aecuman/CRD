using CRD.Application.Common;
using CRD.Application.Common.Exceptions;
using CRD.Application.Plants.Commands;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Structures.Commands
{
    public class DeleteStructureCommand:IRequest<bool>
    {
        public int Id { get; set; }

    }
    public class DeleteStructureCommandHandler : IRequestHandler<DeleteStructureCommand, bool>
    {
        private readonly ILogger<DeleteStructureCommandHandler> _logger;
        private readonly IRepository<Structure> _structureRepo;
        private readonly IRepository<StructureOptionSelection> _structureOptionSelectionsRepo;

        public DeleteStructureCommandHandler(ILogger<DeleteStructureCommandHandler> logger, IRepository<Structure> structureRepo, IRepository<StructureOptionSelection> structureOptionSelectionsRepo)
        {
            _logger = logger;
            _structureRepo = structureRepo;
            _structureOptionSelectionsRepo = structureOptionSelectionsRepo;
        }
        public async Task<bool> Handle(DeleteStructureCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new NullReferenceException();
           
                var entity = await _structureRepo.GetAll().AsQueryable()
        .Include(s => s.AttributeSelections)
            .ThenInclude(a => a.OptionSelections)
        .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (entity == null) return false;

            // 1. Delete options first (because no cascade)
            foreach (var attr in entity.AttributeSelections)
            {
                _structureOptionSelectionsRepo.RemoveRange(attr.OptionSelections.ToList());
            }

            if (entity == null)
                {
                    throw new NotFoundException(nameof(Crop), request.Id);
                }
                return _structureRepo.Remove(request.Id);
            }
    }
}
