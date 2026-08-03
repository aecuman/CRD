using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CRD.Application.StructureTypes.Commands
{
    public class CreateStructureTypeCommand : IRequest<int>
    {
        public string Name { get; set; }
    }

    public class CreateStructureTypeCommandHandler : IRequestHandler<CreateStructureTypeCommand, int>
    {
        private readonly IRepository<StructureType> _repository;

        public CreateStructureTypeCommandHandler(IRepository<StructureType> repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateStructureTypeCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Structure Type name cannot be empty.");

            var structureType = new StructureType
            {
                Name = request.Name
            };

            _repository.AddWithoutSaving(structureType);
            await _repository.SaveAsync();

            return structureType.Id;
        }
    }
}
