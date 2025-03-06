import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { APIService } from '../api.service';

@Component({
  selector: 'app-workflow-step-management',
  templateUrl: './workflow-step-management.component.html',
  styleUrls: ['./workflow-step-management.component.css']
})
export class WorkflowStepManagementComponent {
  workflowSteps: any[] = [];
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

  loadSteps() {
/*     this.api.('/api/workflow').subscribe((data: any) => {
      this.workflowSteps = data;
    }); */
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
  }
}
