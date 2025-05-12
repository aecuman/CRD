import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StructureRateMatrixComponent } from './structure-rate-matrix.component';

describe('StructureRateMatrixComponent', () => {
  let component: StructureRateMatrixComponent;
  let fixture: ComponentFixture<StructureRateMatrixComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StructureRateMatrixComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StructureRateMatrixComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
