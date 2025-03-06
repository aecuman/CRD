import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-page-list-view',
  templateUrl: './page-list-view.component.html',
  styleUrls: ['./page-list-view.component.css']
})
export class PageListViewComponent {
@Input() pageTitle:any;
@Input() newButtonText:any;
}
