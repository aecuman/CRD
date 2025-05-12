import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantRateMatrixComponent } from './plant-rate-matrix.component';

describe('PlantRateMatrixComponent', () => {
  let component: PlantRateMatrixComponent;
  let fixture: ComponentFixture<PlantRateMatrixComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlantRateMatrixComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlantRateMatrixComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
