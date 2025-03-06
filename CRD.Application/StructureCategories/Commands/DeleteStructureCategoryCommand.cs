using CRD.Application.Common;
using CRD.Application.Common.Exceptions;
using CRD.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.StructureCategories.Commands
{
    public class DeleteStructureCategoryCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteStructureCategoryCommandHandler : IRequestHandler<DeleteStructureCategoryCommand, bool>
    {
        private readonly IRepository<StructureCategory> _repository;

        public DeleteStructureCategoryCommandHandler(IRepository<StructureCategory> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteStructureCategoryCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new NullReferenceException();

            var entity = await _repository.GetByIdAsync(request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Crop), request.Id);
            }
            return _repository.Remove(request.Id);
        }
    }
}
