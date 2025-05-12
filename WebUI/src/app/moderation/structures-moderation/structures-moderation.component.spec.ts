import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StructuresModerationComponent } from './structures-moderation.component';

describe('StructuresModerationComponent', () => {
  let component: StructuresModerationComponent;
  let fixture: ComponentFixture<StructuresModerationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StructuresModerationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StructuresModerationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
