import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { 
  DistrictWorkflowStatusDto,
  DistrictWorkflowStepStatusDto,
  DistrictWorkflowSubStepStatusDto,
  CommentDto,
  APIService,
  DocumentDto,
  DistrictRateDto
} from '../api.service';
import { AuthService } from '../auth.service';
import { environment } from 'src/environment/environment';

@Component({
  selector: 'app-district-workflow-detail',
  standalone: false,
  templateUrl: './district-workflow-detail.component.html',
  styleUrl: './district-workflow-detail.component.css'
})
export class DistrictWorkflowDetailComponent {
  status: DistrictWorkflowStatusDto[] = [];
  districtId!: number;
  districtRateId!:number;
  currentUser = 'admin'; // replace with real user service in production
  selectedFiles: { [key: number]: File } = {};
  uploadStatus: { [key: number]: string } = {};
  currentStepIndex: number = 0;
  reportsSectionEnabled: boolean = false;
  districtRate:DistrictRateDto|any;
  showModal: boolean = false;
  baseUrl= environment.baseUrl;

  constructor(
    private auth:AuthService,
    private route: ActivatedRoute,
    private api: APIService,
    private router: Router
  ) {
    this.currentUser = this.auth.getUser?.fullName || 'admin'; // replace with real user service in production
  }

  ngOnInit(): void {
    this.districtId = +this.route.snapshot.paramMap.get('districtId')!;
    this.districtRateId = +this.route.snapshot.paramMap.get('districtRateId')!;
    this.loadDistrictRatedetails(this.districtRateId);
    this.api.getDistrictWorkflowStatus(this.districtId,this.districtRateId).subscribe(data => {
      this.status = data;
          // Find index of the first incomplete step
    const steps = this.status[0]?.steps ?? [];
    this.currentStepIndex = steps.findIndex(s => !s.isCompleted);
    if (this.currentStepIndex === -1) this.currentStepIndex = steps.length; // All completed

    // ✅ Enable reports section if step 4 exists and its first substep is completed
    const step4 = steps[3];
    const step4Substep1 = step4?.subSteps?.[0];

    this.reportsSectionEnabled = !!(step4Substep1 && step4Substep1.isCompleted);
    
    });
  }
loadDistrictRatedetails(districtId: number) {
  this.api.districtRatesGET(districtId).subscribe(data => {
    this.districtRate=data;
  });
}
  markSubStepComplete(subStepId: number) {
    confirm('Complete this step?') &&
    this.api.completeSubStep({districtSubStepId:subStepId}).subscribe(() => this.ngOnInit());
  }

  addCommentToStep(step: DistrictWorkflowStepStatusDto, text: string) {
    this.api.addComment({
      districtStepId: step?.stepId,
      text,
      createdBy: this.currentUser
    }).subscribe(() => this.ngOnInit());
  }

  addCommentToSubStep(sub: DistrictWorkflowSubStepStatusDto, text: string) {
    this.api.addComment({
      districtSubStepId: sub.subStepId,
      text,
      createdBy: this.currentUser
    }).subscribe(() => this.ngOnInit());
  }

  deleteComment(commentId: number) {
   // this.api.deleteComment(commentId).subscribe(() => this.ngOnInit());
  }
 /*  isCurrentStep(step: DistrictWorkflowStepStatusDto): boolean {
    return !step.isCompleted && this.status&&
           !(this.status[0]?.steps ?? []).some(s => !s.isCompleted && (s.stepId ?? 0) < (step.stepId ?? 0));
  }
  
  isCurrentSubStep(sub: DistrictWorkflowSubStepStatusDto): boolean {
    return !sub.isCompleted;
  }
  
  canGoBack(step: DistrictWorkflowStepStatusDto): boolean {
    return step.isCompleted??false; // Only allow reverting from completed steps
  }
  
 */  revertToStep(stepId: number) {
   // this.workflowService.revertStep(stepId).subscribe(() => this.ngOnInit());
  }
  isCurrentStep(workflow: DistrictWorkflowStatusDto, index: number): boolean {
    return !workflow.steps![index].isCompleted &&
           workflow.steps?.findIndex(s => !s.isCompleted) === index;
  }
  
  isCurrentSubStep(step: DistrictWorkflowStepStatusDto, sub: DistrictWorkflowSubStepStatusDto): boolean {
    return step?.subSteps?.find(s => !s.isCompleted)?.subStepId === sub.subStepId;
  }
  
  allSubStepsComplete(step: DistrictWorkflowStepStatusDto): boolean {
    return step.subSteps?.every(s => s.isCompleted)??false;
  }
  
  completeStep(stepId: number): void {
    if (confirm('Complete this step and all its substeps?')) {
      this.api.ompleteStep({districtStepId:stepId}).subscribe(() => this.ngOnInit());
    }
    //this.api.ompleteStep({districtStepId:stepId}).subscribe(() => this.ngOnInit());
  }
  handleComparables(event:any){
this.loadDistrictRatedetails(this.districtRateId);
  }

onFileSelected(event: Event, subStepId: number): void {
  const input = event.target as HTMLInputElement;
  if (input.files?.length) {
    this.selectedFiles[subStepId] = input.files[0];
  }
}

uploadFile(subStepId: number): void {
  const file = this.selectedFiles[subStepId];
  if (!file) return;

  const formData = new FormData();
  formData.append('file', file);
  formData.append('districtWorkflowSubStepId', subStepId.toString());

  this.api.uploadWorkflowDocument(subStepId,{data:file,fileName:file.name}).subscribe({
    next: () => {
      this.uploadStatus[subStepId] = 'Upload successful';
      this.selectedFiles[subStepId] = undefined as any;
      this.ngOnInit(); // Refresh data
    },
    error: () => {
      this.uploadStatus[subStepId] = 'Upload failed';
    }
  });
}
getProgressPercent(workflow: DistrictWorkflowStatusDto): number {
  const total = workflow?.steps?.length;
  const done = workflow.steps?.filter(s => s.isCompleted).length;
  return total ? Math.round(((done??0) / total) * 100) : 0;
}

getAllUploadedDocs(workflow: DistrictWorkflowStatusDto): DocumentDto[]|any {
  return workflow.steps?.flatMap(s => [
    ...(s?.subSteps?.flatMap(sub => sub.documents || []) ?? [])
  ]);
}

goBackToDashboard() {
 // this.router.navigate(['/dashboard']);
}

triggerForceRevert() {
  // Trigger backend action or show confirmation modal
}
ViewReport(){
  this.router.navigate(['/portal/rates/moderation/report/',this.districtRateId,{ outlets: { print: ['final-report-download'] } }], {
    queryParams: { type: 'pdf', preview: 'true' }
  });
  
}
revertSubStep(subStepId: number): void {
  if (confirm('Revert this substep?')) {
    this.api.revertSubStep(subStepId).subscribe(() => this.ngOnInit());
  }
}

revertStep(stepId: number): void {
  if (confirm('Revert this entire step and all its substeps?')) {
    this.api.revertStep(stepId).subscribe(() => this.ngOnInit());
  }
}
get preselectedRates(){
 return this.districtRate?.comparableDistrictRates?.map((c:any) => c.id)
}
get canManage(){
  return this.auth.userValue?.roles?.includes('admin') || this.auth.userValue?.roles?.includes('superadmin');
}
}
