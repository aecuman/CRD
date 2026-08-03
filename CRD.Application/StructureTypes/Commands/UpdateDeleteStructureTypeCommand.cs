using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CRD.Application.StructureTypes.Commands
{
    public class UpdateStructureTypeCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UpdateStructureTypeCommandHandler : IRequestHandler<UpdateStructureTypeCommand, bool>
    {
        private readonly IRepository<StructureType> _repository;

        public UpdateStructureTypeCommandHandler(IRepository<StructureType> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateStructureTypeCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Structure Type name cannot be empty.");

            var structureType = await _repository.GetByIdAsync(request.Id);
            if (structureType == null)
                throw new KeyNotFoundException($"Structure Type with ID {request.Id} not found.");

            structureType.Name = request.Name;
            await _repository.Update(structureType);

            return true;
        }
    }

    public class DeleteStructureTypeCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteStructureTypeCommandHandler : IRequestHandler<DeleteStructureTypeCommand, bool>
    {
        private readonly IRepository<StructureType> _repository;

        public DeleteStructureTypeCommandHandler(IRepository<StructureType> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteStructureTypeCommand request, CancellationToken cancellationToken)
        {
            var structureType = await _repository.GetByIdAsync(request.Id);
            if (structureType == null)
                throw new KeyNotFoundException($"Structure Type with ID {request.Id} not found.");

            _repository.Remove(request.Id);

            return true;
        }
    }
}
