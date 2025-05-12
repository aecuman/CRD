using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.PlantRates.Commands
{
    public class UpdateSinglePlantRateCommand:IRequest<bool>
    {
        public int Id { get; set; }
        public decimal Rate { get; set; }
    }
    public class UpdateSinglePlantRateHandler : IRequestHandler<UpdateSinglePlantRateCommand, bool>
    {
        private readonly IRepository<PlantRate>_repository;
        public UpdateSinglePlantRateHandler(IRepository<PlantRate> repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(UpdateSinglePlantRateCommand request, CancellationToken cancellationToken)
        {
            var plantRate = await _repository.GetAll().AsQueryable().FirstOrDefaultAsync(r=>r.Id==request.Id, cancellationToken);
            if (plantRate == null)
            {
                return false;
            }
            plantRate.Rate = request.Rate;
            await _repository.Update(plantRate);
            return true;
        }
    }
}
