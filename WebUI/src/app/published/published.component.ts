import { Component } from '@angular/core';
import { APIService, PublishedRateSummaryDto } from '../api.service';

@Component({
  selector: 'app-published',
  standalone: false,
  templateUrl: './published.component.html',
  styleUrl: './published.component.css'
})
export class PublishedComponent {
  rates: PublishedRateSummaryDto[] = [];
  filter: string = 'valid';
  loading = false;

  constructor(private api: APIService) {}

  ngOnInit(): void {
    this.loadRates();
  }

  loadRates(): void {
    this.loading = true;
    this.api.getPublishedRates(this.filter).subscribe({
      next: data => {
        this.rates = data;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  setFilter(f: string): void {
    this.filter = f;
    this.loadRates();
  }

  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'published': return 'bg-green-100 text-green-800';
      case 'expired': return 'bg-yellow-100 text-yellow-800';
      case 'not published': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }
}
