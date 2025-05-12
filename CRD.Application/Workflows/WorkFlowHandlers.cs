using CRD.Application.Common;
using CRD.Domain.Process;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Workflows
{// Handlers for Workflow Commands and Queries
   /* public class WorkflowHandler :
        IRequestHandler<AssignWorkflowCommand, DistrictWorkflow>,
        IRequestHandler<ConfirmStepCommand, WorkflowStep>,
        IRequestHandler<GetDistrictStatusesQuery, List<GetDistrictStatusViewModel>>,
        IRequestHandler<AddCommentCommand, Comment>,
        IRequestHandler<CreateWorkflowCommand, Workflow>,
        IRequestHandler<UpdateWorkflowCommand, Workflow>,
        IRequestHandler<DeleteWorkflowCommand, bool>,
        IRequestHandler<GetAllWorkflowsQuery, List<WorkflowDto>>,
        IRequestHandler<AddStepCommand, WorkflowStep>,
        IRequestHandler<DeleteStepCommand, bool>,
        IRequestHandler<AddSubStepCommand, WorkflowSubStep>,
        IRequestHandler<DeleteSubStepCommand, bool>
    {
        private readonly IRepository<Workflow> _workflowRepository;
        private readonly IRepository<DistrictWorkflow> _districtWorkflowRepository;
        private readonly IRepository<WorkflowStep> _workflowStepRepository;
        private readonly IRepository<WorkflowSubStep> _workflowSubStepRepository;
        private readonly IRepository<Comment> _commentRepository;

        public WorkflowHandler(IRepository<Workflow> workflowRepository, IRepository<DistrictWorkflow> districtWorkflowRepository, IRepository<WorkflowStep> workflowStepRepository, IRepository<WorkflowSubStep> workflowSubStepRepository, IRepository<Comment> commentRepository)
        {
            _workflowRepository = workflowRepository;
            _districtWorkflowRepository = districtWorkflowRepository;
            _workflowStepRepository = workflowStepRepository;
            _workflowSubStepRepository = workflowSubStepRepository;
            _commentRepository = commentRepository;
        }
        
        public async Task<DistrictWorkflow> Handle(AssignWorkflowCommand request, CancellationToken cancellationToken)
        {
            var districtWorkflow = new DistrictWorkflow { DistrictId = request.DistrictId, WorkflowId = request.WorkflowId };
            _districtWorkflowRepository.Add(districtWorkflow);
            await _districtWorkflowRepository.SaveChangesAsync();
            return districtWorkflow;
        }

        public async Task<WorkflowStep> Handle(ConfirmStepCommand request, CancellationToken cancellationToken)
        {
            var step = await _workflowStepRepository.GetByIdAsync(request.StepId);
            if (step == null) return null;
            step.IsCompleted = true;
            await _workflowStepRepository.SaveChangesAsync();
            return step;
        }

        public async Task<List<GetDistrictStatusViewModel>> Handle(GetDistrictStatusesQuery request, CancellationToken cancellationToken)
        {
            var list = _districtWorkflowRepository.GetAll().AsQueryable().Include(dw => dw.District)
                .Include(dw => dw.Workflow)
                .ThenInclude(w => w.Steps)
                .Select(dw => new GetDistrictStatusViewModel
                {
                    District = dw.District.Name,
                    Workflow = dw.Workflow.Name,
                    CurrentStep = dw.Workflow.Steps.Where(s => !s.IsCompleted).OrderBy(s => s.Id).FirstOrDefault().Name ?? "Completed",
                    Status = dw.Workflow.Steps.All(s => s.IsCompleted) ? "Completed" : "In Progress"
                }).ToList();
            return await Task.FromResult(list);
        }

        public async Task<Comment> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var step = await _workflowStepRepository.GetByIdAsync(request.WorkflowStepId);
            if (step == null) return null;

            var comment = new Comment { WorkflowStepId = request.WorkflowStepId, Text = request.Text, CreatedBy = request.CreatedBy };
            _commentRepository.Add(comment);
            await _commentRepository.SaveChangesAsync();
            return comment;
        }

        public async Task<Workflow> Handle(CreateWorkflowCommand request, CancellationToken cancellationToken)
        {
            var workflow = new Workflow { Name = request.Name ,Description = request.Description};
            _workflowRepository.Add(workflow);
            await _workflowRepository.SaveChangesAsync();
            return workflow;
        }

        public async Task<Workflow> Handle(UpdateWorkflowCommand request, CancellationToken cancellationToken)
        {
            var workflow = await _workflowRepository.GetByIdAsync(request.WorkflowId);
            if (workflow == null) return null;
            workflow.Name = request.Name;
            workflow.Description = request.Description;
            await _workflowRepository.SaveChangesAsync();
            return workflow;
        }

        public async Task<bool> Handle(DeleteWorkflowCommand request, CancellationToken cancellationToken)
        {
            var workflow = await _workflowRepository.GetByIdAsync(request.WorkflowId);
            if (workflow == null) return false;
            _workflowRepository.Remove(workflow.Id);
            await _workflowRepository.SaveChangesAsync();
            return true;
        }

        public async Task<WorkflowStep> Handle(AddStepCommand request, CancellationToken cancellationToken)
        {
          //  var step = new WorkflowStep {  WorkflowId = request.WorkflowId, Name = request.Name/*, AssignedToRole = request.AssignedToRole};
            _workflowStepRepository.Add(step);
            await _workflowStepRepository.SaveChangesAsync();
            return step;
        }

        public async Task<bool> Handle(DeleteStepCommand request, CancellationToken cancellationToken)
        {
            var step = await _workflowStepRepository.GetByIdAsync(request.StepId);
            if (step == null) return false;
            _workflowStepRepository.Remove(step.Id);
            await _workflowStepRepository.SaveChangesAsync();
            return true;
        }

        public async Task<WorkflowSubStep> Handle(AddSubStepCommand request, CancellationToken cancellationToken)
        {
            var subStep = new WorkflowSubStep { WorkflowStepId = request.WorkflowStepId, Name = request.Name, AssignedToRoles = request.AssignedToRoles };
            _workflowSubStepRepository.Add(subStep);
            await _workflowSubStepRepository.SaveChangesAsync();
            return subStep;
        }

        public async Task<bool> Handle(DeleteSubStepCommand request, CancellationToken cancellationToken)
        {
            var subStep = await _workflowSubStepRepository.GetByIdAsync(request.SubStepId);
            if (subStep == null) return false;
            _workflowSubStepRepository.Remove(subStep.Id);
            await _workflowSubStepRepository.SaveChangesAsync();
            return true;
        }

        public Task<List<WorkflowDto>> Handle(GetAllWorkflowsQuery request, CancellationToken cancellationToken)
        {
            var list = _workflowRepository.GetAll();
            var dto = list.Select(x=>new WorkflowDto() { Id = x.Id, Name = x.Name, Description = x.Description,
            Steps = x.Steps.Select (s=> new WorkflowStepDto() { Id = s.Id, Name= x.Name}).ToList()
            }).ToList();
            var workflows = _workflowRepository.GetAll().AsQueryable()
            .Include(w => w.Steps)
            .ThenInclude(s => s.SubSteps)
            .ToList();

            return Task.FromResult(
                workflows.Select(w => new WorkflowDto
            {
                Id = w.Id,
                Name = w.Name,
                Steps = w.Steps.Select(s => new WorkflowStepDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    AssignedToRole = string.Join(", ", s.AssignedToRoles),
                    IsCompleted = s.IsCompleted,
                    SubSteps = s.SubSteps.Select(ss => new WorkflowSubStepDto
                    {
                        Id = ss.Id,
                        Name = ss.Name,
                        AssignedToRole = string.Join(", ", ss.AssignedToRoles),
                        IsCompleted = ss.IsCompleted
                    }).ToList()
                }).ToList()
            }).ToList());
           
        }
    }*/
}
