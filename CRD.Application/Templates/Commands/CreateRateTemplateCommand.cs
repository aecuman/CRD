using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using System.Text.Json;

namespace CRD.Application.Templates.Commands
{
    public class CreateRateTemplateCommand : IRequest<int>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public RateTemplateConfig Config { get; set; } = new();
    }

    public class CreateRateTemplateHandler : IRequestHandler<CreateRateTemplateCommand, int>
    {
        private readonly IRepository<RateTemplate> _repo;

        public CreateRateTemplateHandler(IRepository<RateTemplate> repo) => _repo = repo;

        public async Task<int> Handle(CreateRateTemplateCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.Name))
                throw new ArgumentException("Template name is required.");

            var template = new RateTemplate
            {
                Name = command.Name.Trim(),
                Description = command.Description?.Trim(),
                CreatedAt = DateTime.UtcNow,
                ConfigJson = JsonSerializer.Serialize(command.Config)
            };

            _repo.Add(template);
            await _repo.SaveChangesAsync();
            return template.Id;
        }
    }
}
