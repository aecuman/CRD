import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DistrictWorkflowDetailComponent } from './district-workflow-detail.component';

describe('DistrictWorkflowDetailComponent', () => {
  let component: DistrictWorkflowDetailComponent;
  let fixture: ComponentFixture<DistrictWorkflowDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DistrictWorkflowDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DistrictWorkflowDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
