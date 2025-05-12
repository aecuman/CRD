import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FinalReportExportComponent } from './final-report-export.component';

describe('FinalReportExportComponent', () => {
  let component: FinalReportExportComponent;
  let fixture: ComponentFixture<FinalReportExportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FinalReportExportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FinalReportExportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
