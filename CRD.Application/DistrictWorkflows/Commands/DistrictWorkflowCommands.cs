using CRD.Application.Common;
using CRD.Application.Rates.Queries;
using CRD.Domain.Process;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CRD.Application.DistrictWorkflows.Commands
{
    public class StartDistrictWorkflowCommand : IRequest<bool>
    {
        public int DistrictId { get; set; }
        public int DistrictRateId { get; set; }
        public int WorkflowId { get; set; }
    }

    public class StartDistrictWorkflowHandler : IRequestHandler<StartDistrictWorkflowCommand, bool>
    {
        private readonly IRepository<Workflow> _context;
        private readonly IRepository<DistrictWorkflow> _districtWorkflowRepository;

        public StartDistrictWorkflowHandler(IRepository<Workflow> context, IRepository<DistrictWorkflow> districtWorkflowRepository)
        {
            _context = context;
            _districtWorkflowRepository = districtWorkflowRepository;
        }

        public async Task<bool> Handle(StartDistrictWorkflowCommand request, CancellationToken cancellationToken)
        {
            var workflow = await _context.GetAll().AsQueryable()
                
                .Include(w => w.Steps)
                .ThenInclude(s => s.SubSteps)
                .FirstOrDefaultAsync(w => w.Id == request.WorkflowId, cancellationToken);

            if (workflow == null) return false;

            var districtWorkflow = new DistrictWorkflow
            {
                DistrictId = request.DistrictId,
                DistrictRateId = request.DistrictRateId,
                WorkflowId = workflow.Id,
                ValidFrom = new DateTime(DateTime.UtcNow.Year, 1, 1),
                ValidTo = new DateTime(DateTime.UtcNow.Year + 2, 1, 1),
                DistrictSteps = workflow.Steps.Select(step => new DistrictWorkflowStep
                {
                    WorkflowStepId = step.Id,
                    DistrictSubSteps = step.SubSteps.Select(sub => new DistrictWorkflowSubStep
                    {
                        WorkflowSubStepId = sub.Id
                    }).ToList()
                }).ToList()
            };

            _districtWorkflowRepository.AddWithoutSaving(districtWorkflow);
            await _districtWorkflowRepository.SaveAsync();
            return true;
        }
    }


    public class CompleteDistrictStepCommand : IRequest<bool>
    {
        public int DistrictStepId { get; set; }
    }

    public class CompleteDistrictStepHandler : IRequestHandler<CompleteDistrictStepCommand, bool>
    {
        private readonly IRepository<DistrictWorkflowStep> _context;
        private readonly IRepository<DistrictWorkflow> _districtWorkflowContext;
        private readonly IMediator _mediator;
        private readonly IPublishedRateMongoService _published;

        public CompleteDistrictStepHandler(IRepository<DistrictWorkflowStep> context, IRepository<DistrictWorkflow> districtWorkflowContext, IMediator mediator, IPublishedRateMongoService published)
        {
            _context = context;
            _districtWorkflowContext = districtWorkflowContext;
            _mediator = mediator;
            _published = published;
        }

        public async Task<bool> Handle(CompleteDistrictStepCommand request, CancellationToken cancellationToken)
        {
            var step = await _context.GetAll().AsQueryable()
                .FirstOrDefaultAsync(s => s.Id == request.DistrictStepId, cancellationToken);

            if (step == null || step.IsCompleted) return false;

            step.IsCompleted = true;
            step.CompletedOn = DateTime.UtcNow;

            await _context.Update(step);
            var workflow = await _districtWorkflowContext.GetAll().AsQueryable()
    .Include(w => w.Workflow)
    .ThenInclude(w => w.Steps)
    .FirstOrDefaultAsync(w => w.Id == step.DistrictWorkflowId, cancellationToken);

            if (workflow != null)
            {
                var orderedStepIds = workflow.Workflow.Steps
                    .OrderBy(s => s.PreviousStepId ?? 0)
                    .Select((s, index) => new { s.Id, Index = index })
                    .ToDictionary(x => x.Id, x => x.Index);

                if (orderedStepIds.TryGetValue(step.WorkflowStepId, out var idx))
                {
                    workflow.Status = (WorkflowStatus)Math.Min(idx + 1, Enum.GetValues<WorkflowStatus>().Length - 1);
                    await _districtWorkflowContext.Update(workflow);
                }
            }
            if (workflow.Status == WorkflowStatus.Published)
            {
                var districtRateId = await GetAssociatedDistrictRateId(request.DistrictStepId,cancellationToken); // Implement as needed
                if (districtRateId > 0)
                {
                    var report = await _mediator.Send(new GetModerationReportQuery
                    {
                        DistrictRateId = districtRateId,
                        Filter = "final"
                    }, cancellationToken);

                    if (report != null)
                    {
                        await _published.SaveAsync(report, cancellationToken);
                    }
                }
            }

            return true;
        }

        private async Task<int> GetAssociatedDistrictRateId(int districtStepId, CancellationToken cancellationToken)
        {
            var step = await _context.GetAll().AsQueryable()
                .Include(s => s.DistrictWorkflow)
                .ThenInclude(dw => dw.DistrictRate)
                .FirstOrDefaultAsync(s => s.Id == districtStepId, cancellationToken);
   
            // You should implement this based on your data model
            // For example, querying DistrictRate table for current year
            // Example (pseudo-code):
            // return await _districtRateRepo.GetAll().Where(x => x.DistrictId == districtId && x.Year == DateTime.Now.Year).Select(x => x.Id).FirstOrDefaultAsync();
            return step.DistrictWorkflow.DistrictRate.Id;
        }
    }
    public class CompleteDistrictSubStepCommand : IRequest<bool>
    {
        public int DistrictSubStepId { get; set; }
    }

    public class CompleteDistrictSubStepHandler : IRequestHandler<CompleteDistrictSubStepCommand, bool>
    {
        private readonly IRepository<DistrictWorkflowSubStep> _context;

        public CompleteDistrictSubStepHandler(IRepository<DistrictWorkflowSubStep> context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CompleteDistrictSubStepCommand request, CancellationToken cancellationToken)
        {
            var substep = await _context.GetAll().AsQueryable()
                .FirstOrDefaultAsync(s => s.Id == request.DistrictSubStepId, cancellationToken);

            if (substep == null || substep.IsCompleted) return false;

            substep.IsCompleted = true;
            substep.CompletedOn = DateTime.UtcNow;

            await _context.Update(substep);
            return true;
        }
    }


    public class AddCommentCommand : IRequest<bool>
    {
        public int? DistrictStepId { get; set; }
        public int? DistrictSubStepId { get; set; }
        public string Text { get; set; }
        public string CreatedBy { get; set; }
    }

    public class AddCommentHandler : IRequestHandler<AddCommentCommand, bool>
    {
        private readonly IRepository<Comment> _context;

        public AddCommentHandler(IRepository<Comment> context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = new Comment
            {
                Text = request.Text,
                CreatedBy = request.CreatedBy,
                DistrictWorkflowStepId = request.DistrictStepId,
                DistrictWorkflowSubStepId = request.DistrictSubStepId
            };

            _context.AddWithoutSaving(comment);
            await _context.SaveAsync();
            return true;
        }
    }
    public class UploadSubStepDocumentCommand : IRequest<bool>
    {
        public IFormFile File { get; set; }
        public int DistrictWorkflowSubStepId { get; set; }
    }
    public class UploadSubStepDocumentHandler : IRequestHandler<UploadSubStepDocumentCommand, bool>
    {
        private readonly IRepository<Document> _context;

        public UploadSubStepDocumentHandler(IRepository<Document> context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UploadSubStepDocumentCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                return false;
            var filePath = Path.Combine("wwwroot/uploads/Substeps", request.File.FileName);
            var directory = Path.GetDirectoryName(filePath);
            var sanitizedFileName = Path.GetFileName(request.File.FileName); // Ensures a clean file name
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
           

            var fileName = Guid.NewGuid() + Path.GetExtension(request.File.FileName);
          //  var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream, cancellationToken);
            }

            var document = new Document
            {
                FileName = sanitizedFileName,
                FilePath = Path.Combine("Uploads/Substeps", fileName),
                DistrictWorkflowSubStepId = request.DistrictWorkflowSubStepId
            };

            _context.AddWithoutSaving(document);
            await _context.SaveAsync();

            return true;
        }
    }
    public class RevertDistrictStepCommand : IRequest<bool>
    {
        public int DistrictStepId { get; set; }
    }
    public class RevertDistrictStepHandler : IRequestHandler<RevertDistrictStepCommand, bool>
    {
        private readonly IRepository<DistrictWorkflowStep> _context;
        private readonly IRepository<DistrictWorkflow> _districtWorkflowContext;
        private readonly IRepository<WorkflowStep> _workflowStepsContext;

        public RevertDistrictStepHandler(IRepository<DistrictWorkflowStep> context, IRepository<DistrictWorkflow> districtWorkflowContext, IRepository<WorkflowStep> workflowStepsContext)
        {
            _context = context;
            _districtWorkflowContext = districtWorkflowContext;
            _workflowStepsContext = workflowStepsContext;
        }

        public async Task<bool> Handle(RevertDistrictStepCommand request, CancellationToken cancellationToken)
        {
            var step = await _context.GetAll().AsQueryable()
                .Include(s => s.DistrictSubSteps)
                .FirstOrDefaultAsync(s => s.Id == request.DistrictStepId, cancellationToken);

            if (step == null) return false;

            step.IsCompleted = false;
            step.CompletedOn = null;

            foreach (var sub in step.DistrictSubSteps)
            {
                sub.IsCompleted = false;
                sub.CompletedOn = null;
            }

            await _context.Update(step);
            var workflow = await _districtWorkflowContext.GetAll().AsQueryable()
    .Include(w => w.DistrictSteps)
    .FirstOrDefaultAsync(w => w.Id == step.DistrictWorkflowId, cancellationToken);

            if (workflow != null)
            {
                var lastCompleted = workflow.DistrictSteps
                    .Where(s => s.IsCompleted)
                    .Select(s => s.WorkflowStepId)
                    .ToList();

                var orderedSteps = await _workflowStepsContext.GetAll().AsQueryable()
                    .Where(s => s.WorkflowId == workflow.WorkflowId)
                    .OrderBy(s => s.PreviousStepId ?? 0)
                    .Select((s, i) => new { s.Id, Index = i })
                    .ToListAsync();

                var latest = orderedSteps.FirstOrDefault(s => lastCompleted.Contains(s.Id));
                workflow.Status = latest != null ? (WorkflowStatus)(latest.Index + 1) : WorkflowStatus.Received;

                await _districtWorkflowContext.Update(workflow);
            }
            return true;
        }
    }
    public class RevertDistrictSubStepCommand : IRequest<bool>
    {
        public int DistrictSubStepId { get; set; }
    }
    public class RevertDistrictSubStepHandler : IRequestHandler<RevertDistrictSubStepCommand, bool>
    {
        private readonly IRepository<DistrictWorkflowStep> _context;

        public RevertDistrictSubStepHandler(IRepository<DistrictWorkflowStep> context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RevertDistrictSubStepCommand request, CancellationToken cancellationToken)
        {
            var subStep = await _context.GetAll().AsQueryable()
                .FirstOrDefaultAsync(s => s.Id == request.DistrictSubStepId, cancellationToken);

            if (subStep == null || !subStep.IsCompleted) return false;

            subStep.IsCompleted = false;
            subStep.CompletedOn = null;

            await _context.Update(subStep);
            return true;
        }
    }


}
