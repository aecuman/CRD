import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StructureRatesComponent } from './structure-rates.component';

describe('StructureRatesComponent', () => {
  let component: StructureRatesComponent;
  let fixture: ComponentFixture<StructureRatesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StructureRatesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StructureRatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
