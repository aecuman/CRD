import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantRatesComponent } from './plant-rates.component';

describe('PlantRatesComponent', () => {
  let component: PlantRatesComponent;
  let fixture: ComponentFixture<PlantRatesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlantRatesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlantRatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
