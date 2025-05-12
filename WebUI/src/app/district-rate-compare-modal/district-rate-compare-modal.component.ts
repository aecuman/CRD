import { Component, EventEmitter, Input, Output } from '@angular/core';
import { APIService } from '../api.service';

@Component({
  selector: 'app-district-rate-compare-modal',
  standalone: false,
  templateUrl: './district-rate-compare-modal.component.html',
  styleUrl: './district-rate-compare-modal.component.css'
})
export class DistrictRateCompareModalComponent {
  @Input() currentDistrictRateId!: number;
  @Input() isOpen: boolean = false;
  @Output() close = new EventEmitter<void>();
  @Input() preselectedIds: number[] = [];
  @Output() selectedComparableRates = new EventEmitter<number[]>();

  searchName: string = '';
  searchYear: string = '';
  districtRates: any[] = [];
  selectedIds = new Set<number>();

  constructor(private api: APIService) {}

  ngOnInit(): void {
    this.fetchDistrictRates();
  }

  fetchDistrictRates() {
    this.api.getLatestDistrictRates(this.currentDistrictRateId).subscribe(data => {
      this.districtRates = data as any as any[];
         // Pre-select those already in preselectedIds
    this.preselectedIds.forEach(id => this.selectedIds.add(id));
    });
  }

  toggleSelection(id: number) {
    if (this.selectedIds.has(id)) {
      this.selectedIds.delete(id);
    } else {
      this.selectedIds.add(id);
    }
  }

  applySelection() {
    const ids = Array.from(this.selectedIds);
    this.api.updateComparables(this.currentDistrictRateId, ids).subscribe(() => {
      this.selectedComparableRates.emit(ids);
      this.close.emit();
    });
  }

  filteredRates() {
    return this.districtRates?.filter(r =>
      (this.searchName ? r.districtName.toLowerCase().includes(this.searchName.toLowerCase()) : true) &&
      (this.searchYear ? r.year.toString().includes(this.searchYear) : true)
    );
  }
}
