import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModerationPopupComponent } from './moderation-popup.component';

describe('ModerationPopupComponent', () => {
  let component: ModerationPopupComponent;
  let fixture: ComponentFixture<ModerationPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ModerationPopupComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ModerationPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
