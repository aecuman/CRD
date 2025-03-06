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
    public class UpdateStructureCommand:IRequest
    {
        public UpdateStructureCommand()
        {
            Description = new List<StructureDescriptionDto>();
        }
        public int Id {  get; set; }
        public string Name { get; set; }
        public int StructureTypeId { get; set; }
        public int CategoryId { get; set; }
        public List<StructureDescriptionDto> Description { get; set; }
    }
    public class UpdateStructureCommandHandler : IRequestHandler<UpdateStructureCommand>
    {
        private readonly ILogger<UpdateStructureCommandHandler> _logger;
        private readonly IRepository<Structure> _structureRepo;

        public UpdateStructureCommandHandler(ILogger<UpdateStructureCommandHandler> logger, IRepository<Structure> structureRepo)
        {
            _logger = logger;
            _structureRepo = structureRepo;
        }

        public async Task Handle(UpdateStructureCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new NullReferenceException();
            var entity = await _structureRepo.GetByIdAsync(request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Crop), request.Id);
            }
            entity.Name = request.Name;
            entity.StructureTypeId = request.StructureTypeId;
            entity.CategoryId = request.CategoryId;           
        
            _structureRepo.Update(entity);
            await _structureRepo.SaveAsync();
        }
    }
}
