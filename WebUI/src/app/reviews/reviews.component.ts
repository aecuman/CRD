import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { APIService, District } from '../api.service';
import type { ColDef } from "ag-grid-community";
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-reviews',
  templateUrl: './reviews.component.html',
  styleUrls: ['./reviews.component.css']
})
export class ReviewsComponent {
  districtRates: any[] = [];
  form: FormGroup;
  isModalOpen = false;
  editMode = false;
  selectedRateId: number | null = null;
  all_districts:District[] = [];

  columnDefs: ColDef[] = [
    { field: 'districtId', headerName: 'District ID', sortable: true, filter: true },
    { field: 'year', headerName: 'Year', sortable: true, filter: true },
    { field: 'status', headerName: 'Status', sortable: true, filter: true },
    {
      field: 'actions',
      headerName: 'Actions',
      cellRenderer: (params: any) => {
        return `
          <button class="bg-green-500 text-white px-2 py-1 rounded mr-2" onclick="editRate(${params.data.id})">Edit</button>
          <button class="bg-red-500 text-white px-2 py-1 rounded" onclick="deleteRate(${params.data.id})">Delete</button>
        `;
      }
    }
  ];

  constructor(private fb: FormBuilder, private api: APIService,private auth:AuthService) {
    this.loadDistricts();
    this.form = this.fb.group({
      districtId: ['', Validators.required],
      year: ['', [Validators.required, Validators.min(2000)]],
      status: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadDistrictRates();
  }
  loadDistricts() {
    this.api.all().subscribe((data:District[])=> {
      this.all_districts = data;
    });
  }

  loadDistrictRates() {
    this.api.districtRatesAll().subscribe(data => {
      this.districtRates = data;
    });
  }

  openModal(editMode: boolean = false, rate?: any) {
    this.isModalOpen = true;
    this.editMode = editMode;
    if (editMode && rate) {
      this.selectedRateId = rate.id;
      this.form.patchValue(rate);
    } else {
      this.form.reset();
      this.selectedRateId = null;
    }
  }

  closeModal() {
    this.isModalOpen = false;
  }

  submitForm() {
    if (this.form.valid) {
      if (this.editMode && this.selectedRateId) {
        this.api.districtRatesPUT(this.selectedRateId,this.form.value)
          .subscribe(() => {
            this.loadDistrictRates();
            this.closeModal();
          });
      } else {
        this.api.districtRatesPOST(this.form.value)
          .subscribe(() => {
            this.loadDistrictRates();
            this.closeModal();
          });
      }
    }
  }

  deleteRate(id: number) {
    this.api.districtRatesDELETE(id).subscribe(() => {
      this.loadDistrictRates();
    });
  }
  get canManage(){
    return this.auth.userValue?.roles?.includes('admin') || this.auth.userValue?.roles?.includes('superadmin');
  }
}
