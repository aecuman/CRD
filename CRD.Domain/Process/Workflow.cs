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
        public List<DistrictWorkflow> DistrictWorkflows { get; set; } = new List<DistrictWorkflow>();
        public List<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
    }
    public class WorkflowStep:AuditableEntity
    {
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public List<string> AssignedToRoles { get; set; }= new List<string>();
        public List<WorkflowSubStep> SubSteps { get; set; } = new List<WorkflowSubStep>();
        public List<Document> Documents { get; set; } = new List<Document>();
        public int DistrictId { get; set; }
        public DistrictWorkflow District { get; set; }
        public int WorkflowId { get; set; }
        public Workflow Workflow { get; set; }
        public List<ApplicationRole> AllowedUsers { get; set; } = new List<ApplicationRole>();
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public int? PreviousStepId { get; set; }
        public int? NextStepId { get; set; }
    }

    public class WorkflowSubStep:AuditableEntity
    {
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public List<string> AssignedToRoles { get; set; } = new List<string>();
        public int WorkflowStepId { get; set; }
        public WorkflowStep WorkflowStep { get; set; }
        public List<Document> Documents { get; set; } = new List<Document>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
    public class Document:AuditableEntity
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int? WorkflowStepId { get; set; }
        public WorkflowStep WorkflowStep { get; set; }
        public int WorkflowSubStepId { get; set; }
        public WorkflowSubStep WorkflowSubStep { get; set; }
    }

    public class DistrictWorkflow:AuditableEntity
    {
        public int DistrictId { get; set; }
        public District District { get; set; }
        public int WorkflowId { get; set; }
        public Workflow Workflow { get; set; }
    }

    public class Comment:AuditableEntity
    {
        public int? WorkflowStepId { get; set; }
        public WorkflowStep WorkflowStep { get; set; }
        public int? WorkflowSubStepId { get; set; }
        public WorkflowSubStep WorkflowSubStep { get; set; }
        public string Text { get; set; }
        public string CreatedBy { get; set; }
    }

}
