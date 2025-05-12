import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantsModerationComponent } from './plants-moderation.component';

describe('PlantsModerationComponent', () => {
  let component: PlantsModerationComponent;
  let fixture: ComponentFixture<PlantsModerationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlantsModerationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlantsModerationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
