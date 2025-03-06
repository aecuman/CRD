using CRD.Application.Common;
using CRD.Application.Common.Exceptions;
using CRD.Application.Plants.Commands;
using CRD.Domain.Entities;
using MediatR;
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

        public DeleteStructureCommandHandler(ILogger<DeleteStructureCommandHandler> logger, IRepository<Structure> structureRepo)
        {
            _logger = logger;
            _structureRepo = structureRepo;
        }
        public async Task<bool> Handle(DeleteStructureCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new NullReferenceException();
           
                var entity = await _structureRepo.GetByIdAsync(request.Id);

                if (entity == null)
                {
                    throw new NotFoundException(nameof(Crop), request.Id);
                }
                return _structureRepo.Remove(request.Id);
            }
    }
}
