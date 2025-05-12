import { Component } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { APIService } from '../api.service';

@Component({
  selector: 'app-workflow-step-management',
  templateUrl: './workflow-step-management.component.html',
  styleUrls: ['./workflow-step-management.component.css']
})
export class WorkflowStepManagementComponent {
  workflowForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.workflowForm = this.fb.group({
      name: [''],
      steps: this.fb.array([])
    });
  }

  ngOnInit() {
    this.loadWorkflowFromJson();
  }

  get steps(): FormArray {
    return this.workflowForm.get('steps') as FormArray;
  }

  getSubSteps(stepIndex: number): FormArray {
    return this.steps.at(stepIndex).get('subSteps') as FormArray;
  }

  addStep() {
    this.steps.push(this.fb.group({
      name: [''],
      assignedToRoles: [''],
      subSteps: this.fb.array([])
    }));
  }

  addSubStep(stepIndex: number) {
    this.getSubSteps(stepIndex).push(this.fb.group({
      name: [''],
      assignedToRoles: ['']
    }));
  }

  submitWorkflow() {
    const payload = this.workflowForm.value;
    console.log('Submitting Workflow:', payload);
    // send payload to backend via HTTP POST
  }
  loadWorkflowFromJson() {
    const json = {
      "name": "Compensation Rates Database Workflow",
      "steps": [
        {
          "name": "Document Submission",
          "assignedToRoles": "Registry",
          "subSteps": [
            {
              "name": "Receive proposed district compensation rates (hardcopy)",
              "assignedToRoles": "Registry"
            },
            {
              "name": "Scan and upload to Compensation Rates Database (CRD)",
              "assignedToRoles": "Registry"
            },
            {
              "name": "Submit electronic copy to Chief Government Valuer",
              "assignedToRoles": "Registry"
            },
            {
              "name": "Forward to Chair – Moderation Committee",
              "assignedToRoles": "Registry"
            }
          ]
        },
        {
          "name": "Data Input",
          "assignedToRoles": "ChairModerationCommittee,DataInputClerk",
          "subSteps": [
            {
              "name": "Allocate file for input into digital format",
              "assignedToRoles": "ChairModerationCommittee"
            },
            {
              "name": "Migrate/input into database",
              "assignedToRoles": "DataInputClerk"
            }
          ]
        },
        {
          "name": "Initial Review & Scheduling",
          "assignedToRoles": "ChairModerationCommittee",
          "subSteps": [
            {
              "name": "Review digital format for moderation",
              "assignedToRoles": "ChairModerationCommittee"
            },
            {
              "name": "Approve or defer for moderation",
              "assignedToRoles": "ChairModerationCommittee"
            },
            {
              "name": "Schedule moderation session",
              "assignedToRoles": "ChairModerationCommittee"
            }
          ]
        },
        {
          "name": "Moderation Session",
          "assignedToRoles": "ModerationCommitteeSecretary",
          "subSteps": [
            {
              "name": "Conduct moderation (adjust or approve item by item)",
              "assignedToRoles": "ModerationCommitteeSecretary"
            },
            {
              "name": "Generate moderation report and adjustment list",
              "assignedToRoles": "ModerationCommitteeSecretary"
            },
            {
              "name": "Draft communication to regional officer",
              "assignedToRoles": "ModerationCommitteeSecretary"
            }
          ]
        },
        {
          "name": "District Notification & Adjustment Upload",
          "assignedToRoles": "RegionalOfficer,Registry,ModerationCommitteeSecretary",
          "subSteps": [
            {
              "name": "Send moderation outcome to district (email/print)",
              "assignedToRoles": "RegionalOfficer"
            },
            {
              "name": "Receive adjusted report",
              "assignedToRoles": "Registry"
            },
            {
              "name": "Scan and upload adjusted report to CRD",
              "assignedToRoles": "Registry"
            },
            {
              "name": "Pass/defer the adjusted schedule",
              "assignedToRoles": "ModerationCommitteeSecretary"
            },
            {
              "name": "Generate report of adjusted schedule",
              "assignedToRoles": "ModerationCommitteeSecretary"
            },
            {
              "name": "Submit for publishing communication",
              "assignedToRoles": "ModerationCommitteeSecretary"
            }
          ]
        },
        {
          "name": "Final Publishing & Approval",
          "assignedToRoles": "RegionalOfficer,Registry,ChairModerationCommittee",
          "subSteps": [
            {
              "name": "Send approved report to district (email/print)",
              "assignedToRoles": "RegionalOfficer"
            },
            {
              "name": "Receive published copy from district",
              "assignedToRoles": "Registry"
            },
            {
              "name": "Upload published report to CRD",
              "assignedToRoles": "Registry"
            },
            {
              "name": "Approve as current official schedule",
              "assignedToRoles": "ChairModerationCommittee"
            }
          ]
        }
      ]
    };
    this.workflowForm.patchValue({ name: json.name });
  
    json.steps.forEach(step => {
      const stepGroup = this.fb.group({
        name: [step.name],
        assignedToRoles: [step.assignedToRoles],
        subSteps: this.fb.array([])
      });
  
      const subStepArray = stepGroup.get('subSteps') as FormArray;
      step.subSteps.forEach(sub => {
        subStepArray.push(this.fb.group({
          name: [sub.name],
          assignedToRoles: [sub.assignedToRoles]
        }));
      });
  
      this.steps.push(stepGroup);
    });
  }
  /*workflowSteps: any[] = [];
  worflowId: any;
  stepForm: FormGroup;
  subStepForm: FormGroup;

  constructor(private api: APIService, private fb: FormBuilder) {
    this.stepForm = this.fb.group({ name: [''] });
    this.subStepForm = this.fb.group({ name: [''] });
  }

  ngOnInit() {
    this.loadSteps();
  }


  }

  addStep() {
    if (this.stepForm.valid) {
      this.api.addStep(this.worflowId, this.stepForm.value).subscribe(() => {
        this.loadSteps();
        this.stepForm.reset();
      });
    }
  }

  addSubStep(id:any) {
    if (this.subStepForm.valid) {
      this.api.addSubstep(id,this.subStepForm.value).subscribe(() => {
        this.loadSteps();
        this.subStepForm.reset();
      });
    }
  }*/
}
