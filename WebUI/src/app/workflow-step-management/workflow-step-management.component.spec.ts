import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkflowStepManagementComponent } from './workflow-step-management.component';

describe('WorkflowStepManagementComponent', () => {
  let component: WorkflowStepManagementComponent;
  let fixture: ComponentFixture<WorkflowStepManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WorkflowStepManagementComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkflowStepManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
