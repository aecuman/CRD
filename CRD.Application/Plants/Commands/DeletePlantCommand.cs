using CRD.Application.Common;
using CRD.Application.Common.Exceptions;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Plants.Commands
{
    public class DeletePlantCommand:IRequest<bool>
    {
        public int Id { get; set; }
        public string PlantType { get; set; }
    }
    public class DeletePlantCommandHandler : IRequestHandler<DeletePlantCommand,bool>
    {
        private readonly ILogger<CreateCropCommand> _logger;
        private readonly IRepository<Crop> _cropRepo;
        private readonly IRepository<Tree> _treeRepo;

        public DeletePlantCommandHandler(ILogger<CreateCropCommand> logger, IRepository<Crop> cropRepo, IRepository<Tree> treeRepo)
        {
            _logger = logger;
            _cropRepo = cropRepo;
            _treeRepo = treeRepo;
        }

        public async Task<bool> Handle(DeletePlantCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new NullReferenceException();
            if (request.PlantType == "crop")
            {
                var entity = await _cropRepo.GetByIdAsync(request.Id);

                if (entity == null)
                {
                    throw new NotFoundException(nameof(Crop), request.Id);
                }
               return _cropRepo.Remove(request.Id);
            }
            else
            {
                var entity = await _treeRepo.GetByIdAsync(request.Id);

                if (entity == null)
                {
                    throw new NotFoundException(nameof(Tree), request.Id);
                }
               return _treeRepo.Remove(request.Id);
            }


        }
    }
}