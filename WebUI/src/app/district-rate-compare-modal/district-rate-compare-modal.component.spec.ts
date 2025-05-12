import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DistrictRateCompareModalComponent } from './district-rate-compare-modal.component';

describe('DistrictRateCompareModalComponent', () => {
  let component: DistrictRateCompareModalComponent;
  let fixture: ComponentFixture<DistrictRateCompareModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DistrictRateCompareModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DistrictRateCompareModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
