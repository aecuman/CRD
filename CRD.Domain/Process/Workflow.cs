using CRD.Domain.Entities;
using CRD.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Process
{
    public class Workflow : AuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<WorkflowStep> Steps { get; set; } = new();
        public List<DistrictWorkflow> DistrictWorkflows { get; set; } = new();
    }

    public class WorkflowStep : AuditableEntity
    {
        public string Name { get; set; }
        public List<string> AssignedToRoles { get; set; } = new();
        public List<WorkflowSubStep> SubSteps { get; set; } = new();
        public int WorkflowId { get; set; }
        public Workflow Workflow { get; set; }
        public int? PreviousStepId { get; set; }
        public int? NextStepId { get; set; }
    }

    public class WorkflowSubStep : AuditableEntity
    {
        public string Name { get; set; }
        public List<string> AssignedToRoles { get; set; } = new();
        public int WorkflowStepId { get; set; }
        public WorkflowStep WorkflowStep { get; set; }
    }

    public class DistrictWorkflow : AuditableEntity
    {
        public int? DistrictRateId { get; set; }
        public DistrictRate? DistrictRate { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; }
        public int WorkflowId { get; set; }
        public Workflow Workflow { get; set; }
        public List<DistrictWorkflowStep> DistrictSteps { get; set; } = new();
        public WorkflowStatus Status { get; set; } = WorkflowStatus.Received;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }

    public class DistrictWorkflowStep : AuditableEntity
    {
        public int WorkflowStepId { get; set; }
        public WorkflowStep WorkflowStep { get; set; }

        public int DistrictWorkflowId { get; set; }
        public DistrictWorkflow DistrictWorkflow { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime? CompletedOn { get; set; }
        public List<DistrictWorkflowSubStep> DistrictSubSteps { get; set; } = new();
        public List<Document> Documents { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
    }

    public class DistrictWorkflowSubStep : AuditableEntity
    {
        public int WorkflowSubStepId { get; set; }
        public WorkflowSubStep WorkflowSubStep { get; set; }

        public int DistrictWorkflowStepId { get; set; }
        public DistrictWorkflowStep DistrictWorkflowStep { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime? CompletedOn { get; set; }
        public List<Document> Documents { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
    }

    public class Document : AuditableEntity
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public int? DistrictWorkflowStepId { get; set; }
        public DistrictWorkflowStep DistrictWorkflowStep { get; set; }

        public int? DistrictWorkflowSubStepId { get; set; }
        public DistrictWorkflowSubStep DistrictWorkflowSubStep { get; set; }
    }

    public class Comment : AuditableEntity
    {
        public string Text { get; set; }
        public string CreatedBy { get; set; }

        public int? DistrictWorkflowStepId { get; set; }
        public DistrictWorkflowStep DistrictWorkflowStep { get; set; }

        public int? DistrictWorkflowSubStepId { get; set; }
        public DistrictWorkflowSubStep DistrictWorkflowSubStep { get; set; }

    }
    public enum WorkflowStatus
    {
        Received = 0,
        Uploaded = 1,
        PendingModeration = 2,
        Moderated = 3,
        Approved = 4,
        Published = 5
    }

}
