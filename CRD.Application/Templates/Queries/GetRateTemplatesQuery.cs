using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using System.Text.Json;

namespace CRD.Application.Templates.Queries
{
    public class GetRateTemplatesQuery : IRequest<List<RateTemplateViewModel>> { }

    public class GetRateTemplatesHandler : IRequestHandler<GetRateTemplatesQuery, List<RateTemplateViewModel>>
    {
        private readonly IRepository<RateTemplate> _repo;

        public GetRateTemplatesHandler(IRepository<RateTemplate> repo) => _repo = repo;

        public async Task<List<RateTemplateViewModel>> Handle(GetRateTemplatesQuery request, CancellationToken cancellationToken)
        {
            var templates = await _repo.GetAllAsync();

            return templates
                .OrderByDescending(t => t.CreatedAt)
                .Select(t =>
                {
                    var config = new RateTemplateConfig();
                    try { config = JsonSerializer.Deserialize<RateTemplateConfig>(t.ConfigJson) ?? config; }
                    catch { /* malformed json — return empty config */ }

                    return new RateTemplateViewModel
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        CreatedAt = t.CreatedAt,
                        Config = config,
                        PlantCount = config.PlantItems.Count,
                        StructureCount = config.StructureItems.Count
                    };
                })
                .ToList();
        }
    }
}
