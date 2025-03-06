import { Component } from '@angular/core';
import { APIService } from '../api.service';

@Component({
  selector: 'app-workflow-management',
  templateUrl: './workflow-management.component.html',
  styleUrls: ['./workflow-management.component.css']
})
export class WorkflowManagementComponent {
  workflows: any[] = [];

  constructor(private api: APIService) {}

  ngOnInit() {
    //this.loadWorkflows();
  }

/*   loadWorkflows() {
    this.api.work('/api/workflow/get-all-workflows').subscribe((data: any) => {
      this.workflows = data;
    });
  } */
}
