import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PublishedDistrictComponent } from './published-district.component';

describe('PublishedDistrictComponent', () => {
  let component: PublishedDistrictComponent;
  let fixture: ComponentFixture<PublishedDistrictComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PublishedDistrictComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PublishedDistrictComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
