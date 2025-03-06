using CRD.Domain.Process;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Workflows
{
    using MediatR;
    using System.Collections.Generic;

    // DTOs and View Models
    public class WorkflowDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<WorkflowStepDto> Steps { get; set; } = new List<WorkflowStepDto>();
    }

    public class WorkflowStepDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AssignedToRole { get; set; }
        public bool IsCompleted { get; set; }
        public List<WorkflowSubStepDto> SubSteps { get; set; } = new List<WorkflowSubStepDto>();
    }

    public class WorkflowSubStepDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AssignedToRole { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string CreatedBy { get; set; }
    }
    public class GetDistrictStatusViewModel
    {
        public string District {  get; set; }
                  public string Workflow {  get; set; }
                    public string CurrentStep { get; set; }
                    public string Status { get; set; }
    }

    // CQRS Commands and Queries with Attributes
    public record AssignWorkflowCommand(int DistrictId, int WorkflowId) : IRequest<DistrictWorkflow>;
    public record ConfirmStepCommand(int StepId) : IRequest<WorkflowStep>;
    public record GetDistrictStatusesQuery() : IRequest<List<GetDistrictStatusViewModel>>;
    public record AddCommentCommand(int WorkflowStepId, string Text, string CreatedBy) : IRequest<Comment>;
    public record CreateWorkflowCommand(string Name,string Description) : IRequest<Workflow>;
    public record UpdateWorkflowCommand(int WorkflowId, string Name, string Description) : IRequest<Workflow>;
    public record DeleteWorkflowCommand(int WorkflowId) : IRequest<bool>;
    public record GetAllWorkflowsQuery() : IRequest<List<WorkflowDto>>;
    public record AddStepCommand(int WorkflowId, string Name, List<string> AssignedToRoles) : IRequest<WorkflowStep>;
    public record DeleteStepCommand(int StepId) : IRequest<bool>;
    public record AddSubStepCommand(int WorkflowStepId, string Name, List<string> AssignedToRoles) : IRequest<WorkflowSubStep>;
    public record DeleteSubStepCommand(int SubStepId) : IRequest<bool>;


}
