import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModerationReportComponent } from './moderation-report.component';

describe('ModerationReportComponent', () => {
  let component: ModerationReportComponent;
  let fixture: ComponentFixture<ModerationReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ModerationReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ModerationReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
