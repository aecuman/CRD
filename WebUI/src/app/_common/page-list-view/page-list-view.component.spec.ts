import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PageListViewComponent } from './page-list-view.component';

describe('PageListViewComponent', () => {
  let component: PageListViewComponent;
  let fixture: ComponentFixture<PageListViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PageListViewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PageListViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
