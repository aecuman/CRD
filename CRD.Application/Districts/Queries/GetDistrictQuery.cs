using CRD.Application.Common;
using CRD.Domain.Entities;
using CRD.Domain.Process;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Districts.Queries
{
    public class GetDistrictRatesQuery : IRequest<List<DistrictRateDto>>
    {
        public int? Year { get; set; }
        public string? Status { get; set; }
        public int? DistrictId { get; set; }
    }

    public class DistrictRateDto
    {
        public int Id { get; set; }
        public string DistrictName { get; set; }
        public int DistrictId { get; set; }
        public int Year { get; set; }
        public string Status { get; set; }
        public List<CRDFileDto> Uploads { get; set; } = new List<CRDFileDto>();
        public string WorkflowName { get; set; }
        public string ActiveStep { get; set; }
        public List<string> AssignedTo { get; set; }
        public int ProgressPercent { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public List<SimpleComparableDto> ComparableDistrictRates { get; set; } = new();

    }
    public class SimpleComparableDto
    {
        public int Id { get; set; }
        public string DistrictName { get; set; }
        public int Year { get; set; }
    }
    public class CRDFileDto
    {
        public int Id { get; set; }
        public string FileClass { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }


    public class GetAllDistrictRatesHandler : IRequestHandler<GetDistrictRatesQuery, List<DistrictRateDto>>
    {
        private readonly IRepository<DistrictRate> _context;
        private readonly IRepository<CRDFile> _contextFiles;
        private readonly IRepository<DistrictWorkflow> _workflowContext;

        public GetAllDistrictRatesHandler(IRepository<DistrictRate> context, IRepository<CRDFile> contextFiles, IRepository<DistrictWorkflow> workflowContext)
        {
            _context = context;
            _contextFiles = contextFiles;
            _workflowContext = workflowContext;
        }

        public async Task<List<DistrictRateDto>> Handle(GetDistrictRatesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.GetAll().AsQueryable()
                .Include(dr => dr.District)
                 //.Include(dr => dr.Uploads) // Fetch the related files
                .AsQueryable();

            // Apply filters if provided
            if (request.Year.HasValue)
            {
                query = query.Where(dr => dr.Year == request.Year.Value);
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(dr => dr.Status.ToString() == request.Status);
            }

            if (request.DistrictId.HasValue)
            {
                query = query.Where(dr => dr.DistrictId == request.DistrictId.Value);
            }
            var districtRates = await query.ToListAsync(cancellationToken);
            var districtIds = districtRates.Select(dr => dr.DistrictId).Distinct().ToList();

            // Pull district workflow data
            var workflows = await _workflowContext.GetAll().AsQueryable()
                .Where(dw => districtIds.Contains(dw.DistrictId))
                .Include(dw => dw.Workflow)
                .Include(dw=>dw.Workflow.Steps)
                    .ThenInclude(ws => ws.SubSteps)
                .Include(dw => dw.DistrictSteps)
                    .ThenInclude(ds => ds.WorkflowStep)
                .Include(dw=>dw.DistrictSteps)
                .ThenInclude(dw=>dw.DistrictSubSteps)
                .ToListAsync(cancellationToken);

            // Group by district
            var workflowMap = workflows
                .GroupBy(w => w.DistrictId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(w => w.ValidFrom).FirstOrDefault());
            var results = new List<DistrictRateDto>();

            foreach (var dr in districtRates)
            {
                var files = dr.UploadIds?.Any() == true
                    ? _contextFiles.GetAll()
                        .Where(x => dr.UploadIds.Contains(x.Id))
                        .Select(file => new CRDFileDto
                        {
                            Id = file.Id,
                            FileClass = file.FileClass,
                            Name = file.Name,
                            Url = file.Url
                        }).ToList()
                    : new List<CRDFileDto>();

                var dto = new DistrictRateDto
                {
                    Id = dr.Id,
                    DistrictName = dr.District.Name,
                    DistrictId = dr.District.Id,
                    Year = dr.Year,
                    //Status = dr.Status.ToString(),
                    Created = dr.CreatedAt,
                    Updated = dr.UpdatedAt,
                    Uploads = files
                };

                if (workflowMap.TryGetValue(dr.DistrictId, out var districtWorkflow))
                {
                    var steps = districtWorkflow.DistrictSteps
                        .OrderBy(s => s.WorkflowStep.PreviousStepId ?? 0)
                        .ToList();

                    var completed = steps.Count(s => s.IsCompleted);
                    var total = steps.Count;
                    dto.Status= districtWorkflow.Status.ToString();
                    dto.WorkflowName = districtWorkflow.Workflow.Name;
                    dto.ActiveStep = steps.FirstOrDefault(s => !s.IsCompleted)?.WorkflowStep?.Name ?? "Completed";
                    dto.AssignedTo = steps
    .FirstOrDefault(s => !s.IsCompleted)?
    .DistrictSubSteps
    .FirstOrDefault(ss => !ss.IsCompleted)?
    .WorkflowSubStep?
    .AssignedToRoles; ;
                    dto.ProgressPercent = total > 0 ? (int)Math.Round((double)(completed * 100) / total) : 0;
                }

                results.Add(dto);
            }

            return results;
        }

   
    }

 


}
