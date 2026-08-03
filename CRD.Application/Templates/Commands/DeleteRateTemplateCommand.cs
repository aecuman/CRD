using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;

namespace CRD.Application.Templates.Commands
{
    public class DeleteRateTemplateCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteRateTemplateHandler : IRequestHandler<DeleteRateTemplateCommand, bool>
    {
        private readonly IRepository<RateTemplate> _repo;

        public DeleteRateTemplateHandler(IRepository<RateTemplate> repo) => _repo = repo;

        public async Task<bool> Handle(DeleteRateTemplateCommand command, CancellationToken cancellationToken)
        {
            var template = await _repo.GetByIdAsync(command.Id);
            if (template == null) return false;
            _repo.Remove(template.Id);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
