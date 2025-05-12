import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CrdMapComponent } from './crd-map.component';

describe('CrdMapComponent', () => {
  let component: CrdMapComponent;
  let fixture: ComponentFixture<CrdMapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CrdMapComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CrdMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
