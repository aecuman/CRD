using CRD.Domain.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Persistence
{
    public class WorkflowSeeder
    {
        private readonly ApplicationDbContext _context;

        public WorkflowSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedDefaultWorkflowAsync()
        {
            if (_context.Workflows.Any(w => w.Name == "Compensation Rates Database Workflow")) return;

            var workflow = new Workflow
            {
                Name = "Compensation Rates Database Workflow",
                Description = "Workflow for managing the Compensation Rates Database process."
            };
                var stepNames = new[]
        {
            "Document Submission",
            "Data Input",
            "Initial Review & Scheduling",
            "Moderation Session",
            "District Notification & Adjustment Upload",
            "Final Publishing & Approval"
        };

                var roleAssignments = new[]
        {
            "Registry",
            "ChairModerationCommittee,DataInputClerk",
            "ChairModerationCommittee",
            "ModerationCommitteeSecretary",
            "RegionalOfficer,Registry,ModerationCommitteeSecretary",
            "RegionalOfficer,Registry,ChairModerationCommittee"
        };

                var substepData = new[]
        {
            new[] {
                ("Receive proposed district compensation rates (hardcopy)", "Registry"),
                ("Scan and upload to Compensation Rates Database (CRD)", "Registry"),
                ("Submit electronic copy to Chief Government Valuer", "Registry"),
                ("Forward to Chair – Moderation Committee", "Registry")
            },
            new[] {
                ("Allocate file for input into digital format", "ChairModerationCommittee"),
                ("Migrate/input into database", "DataInputClerk")
            },
            new[] {
                ("Review digital format for moderation", "ChairModerationCommittee"),
                ("Approve or defer for moderation", "ChairModerationCommittee"),
                ("Schedule moderation session", "ChairModerationCommittee")
            },
            new[] {
                ("Conduct moderation (adjust or approve item by item)", "ModerationCommitteeSecretary"),
                ("Generate moderation report and adjustment list", "ModerationCommitteeSecretary"),
                ("Draft communication to regional officer", "ModerationCommitteeSecretary")
            },
            new[] {
                ("Send moderation outcome to district (email/print)", "RegionalOfficer"),
                ("Receive adjusted report", "Registry"),
                ("Scan and upload adjusted report to CRD", "Registry"),
                ("Pass/defer the adjusted schedule", "ModerationCommitteeSecretary"),
                ("Generate report of adjusted schedule", "ModerationCommitteeSecretary"),
                ("Submit for publishing communication", "ModerationCommitteeSecretary")
            },
            new[] {
                ("Send approved report to district (email/print)", "RegionalOfficer"),
                ("Receive published copy from district", "Registry"),
                ("Upload published report to CRD", "Registry"),
                ("Approve as current official schedule", "ChairModerationCommittee")
            }
        };

                var steps = new List<WorkflowStep>();

        // Create Steps and SubSteps first
        for (int i = 0; i < stepNames.Length; i++)
            {
                var step = new WorkflowStep
                {
                    Name = stepNames[i],
                    AssignedToRoles = roleAssignments[i].Split(',').ToList(),
                    SubSteps = substepData[i].Select(sub =>
                        new WorkflowSubStep
                        {
                            Name = sub.Item1,
                            AssignedToRoles = sub.Item2.Split(',').ToList()
                        }).ToList()
                };
                steps.Add(step);
            }

            // Link previous and next steps
            for (int i = 0; i < steps.Count; i++)
            {
                if (i > 0) steps[i].PreviousStepId = i; // previous is index-based +1 due to EF-generated Ids
                if (i < steps.Count - 1) steps[i].NextStepId = i + 2;
            }

            workflow.Steps = steps;
            _context.Workflows.Add(workflow);
            await _context.SaveChangesAsync();
        }
    }

}
