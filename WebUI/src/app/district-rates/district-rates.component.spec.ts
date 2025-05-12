import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DistrictRatesComponent } from './district-rates.component';

describe('DistrictRatesComponent', () => {
  let component: DistrictRatesComponent;
  let fixture: ComponentFixture<DistrictRatesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DistrictRatesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DistrictRatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
