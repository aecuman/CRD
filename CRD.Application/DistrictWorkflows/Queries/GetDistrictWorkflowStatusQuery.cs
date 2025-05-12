using CRD.Application.Common;
using CRD.Domain.Process;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.DistrictWorkflows.Queries
{

    public class DistrictWorkflowStatusDto
    {
        public string WorkflowName { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string DistrictName { get; set; }
        public List<DistrictWorkflowStepStatusDto> Steps { get; set; }
        public WorkflowStatus Status { get; set; }

    }

    public class DistrictWorkflowStepStatusDto
    {
        public int StepId { get; set; }
        public string StepName { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedOn { get; set; }
        public List<DistrictWorkflowSubStepStatusDto> SubSteps { get; set; }
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public string AssignedToRole { get; set; }
        public string CommentText { get; set; }
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
        public class DistrictWorkflowSubStepStatusDto
    {
        public int SubStepId { get; set; }
        public string SubStepName { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedOn { get; set; }
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public List<DocumentDto> Documents { get; set; } = new List<DocumentDto>();
        public List<string> AssignedToRole { get; set; }
        public string CommentText { get; set; }
    }
    public class GetDistrictWorkflowStatusQuery : IRequest<List<DistrictWorkflowStatusDto>>
    {
        public int DistrictId { get; set; }
        public int DistrictRateId {  get; set; }
    }

    public class GetDistrictWorkflowStatusHandler : IRequestHandler<GetDistrictWorkflowStatusQuery, List<DistrictWorkflowStatusDto>>
    {
        private readonly IRepository<DistrictWorkflow> _context;

        public GetDistrictWorkflowStatusHandler(IRepository<DistrictWorkflow> context)
        {
            _context = context;
        }

        public async Task<List<DistrictWorkflowStatusDto>> Handle(GetDistrictWorkflowStatusQuery request, CancellationToken cancellationToken)
        {
            var workflows = await _context.GetAll().AsQueryable()
                .Where(dw => dw.DistrictId == request.DistrictId&& dw.DistrictRateId==request.DistrictRateId)
                .Include(dw => dw.DistrictSteps)
                    .ThenInclude(ds => ds.Comments)
                .Include(dw => dw.DistrictSteps)
                    .ThenInclude(ds => ds.DistrictSubSteps)                    
                        .ThenInclude(ss => ss.Comments)
                 .Include(dd=> dd.DistrictSteps)
                    .ThenInclude(ds=>ds.Documents)
                    .Include(dd => dd.DistrictSteps)
                    .ThenInclude(ds => ds.DistrictSubSteps)
                    .ThenInclude(dd=>dd.Documents)
                .Include(dw => dw.Workflow)
                    .ThenInclude(w => w.Steps)
                        .ThenInclude(s => s.SubSteps)
                .Include(dr=>dr.DistrictRate)
                .Include(dr=>dr.District)
                .ToListAsync(cancellationToken);


            var result = new List<DistrictWorkflowStatusDto>();

            foreach (var dw in workflows)
            {
                // Match all district steps by workflow step id
                var stepDict = dw.DistrictSteps.ToDictionary(ds => ds.WorkflowStep.Id, ds => ds);

                // Instead of traversal, just use the ordered workflow step list
                var orderedWorkflowSteps = dw.Workflow.Steps.OrderBy(s => s.PreviousStepId ?? 0).ToList();

                var stepsDto = new List<DistrictWorkflowStepStatusDto>();

                foreach (var wfStep in orderedWorkflowSteps)
                {
                    if (!stepDict.TryGetValue(wfStep.Id, out var districtStep)) continue;

                    stepsDto.Add(new DistrictWorkflowStepStatusDto
                    {
                        StepId = districtStep.Id,
                        StepName = wfStep.Name,
                        IsCompleted = districtStep.IsCompleted,
                        Comments = districtStep.Comments.Select(c => new CommentDto
                        {
                            Id = c.Id,
                            Text = c.Text,
                            CreatedBy = c.CreatedBy,
                            CreatedOn = c.CreatedAt
                        }).ToList(),
                        SubSteps = districtStep.DistrictSubSteps
                            .OrderBy(ss => ss.WorkflowSubStep.Id)
                            .Select(ss => new DistrictWorkflowSubStepStatusDto
                            {
                                SubStepId = ss.Id,
                                SubStepName = ss.WorkflowSubStep.Name,
                                IsCompleted = ss.IsCompleted,
                                Comments = ss.Comments.Select(c => new CommentDto
                                {
                                    Id = c.Id,
                                    Text = c.Text,
                                    CreatedBy = c.CreatedBy,
                                    CreatedOn = c.CreatedAt
                                }).ToList(),
                                Documents = ss.Documents.Select(d => new DocumentDto
                                {
                                    Id = d.Id,
                                    FileName = d.FileName,
                                    FilePath = d.FilePath
                                }).ToList(),
                                AssignedToRole = ss.WorkflowSubStep.AssignedToRoles.ToList(),
                            }).ToList()
                    });
                }

                result.Add(new DistrictWorkflowStatusDto
                {
                    WorkflowName = dw.Workflow.Name,
                    ValidFrom = new DateTime(dw.DistrictRate.Year,1,1),
                    ValidTo = new DateTime(dw.DistrictRate.Year+1, 1, 1),
                    DistrictName = dw.District.Name,
                    Steps = stepsDto,
                    Status = dw.Status

                });
            }

            return result;
        }
    }

    public class DocumentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}


