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
  defaultWorkflowId: number = 1; // Default, will be overridden by API call

  columnDefs: ColDef[] = [
    { field: 'districtId', headerName: 'District ID', sortable: true, filter: true },
    { field: 'year', headerName: 'Year', sortable: true, filter: true },
    { field: 'status', headerName: 'Status', sortable: true, filter: true },
    {
      field: 'id',
      headerName: 'Actions',
      cellRenderer: (params: any) => {
        const editBtn = document.createElement('button');
        editBtn.className = 'bg-blue-500 hover:bg-blue-600 text-white px-2 py-1 rounded mr-2';
        editBtn.textContent = 'Edit';
        editBtn.addEventListener('click', () => this.onEditRate(params.data));

        const reviewBtn = document.createElement('button');
        reviewBtn.className = 'bg-green-500 hover:bg-green-600 text-white px-2 py-1 rounded mr-2';
        reviewBtn.textContent = 'Start Review';
        reviewBtn.addEventListener('click', () => this.onStartReview(params.data));

        const deleteBtn = document.createElement('button');
        deleteBtn.className = 'bg-red-500 hover:bg-red-600 text-white px-2 py-1 rounded';
        deleteBtn.textContent = 'Delete';
        deleteBtn.addEventListener('click', () => this.deleteRate(params.data.id));

        const container = document.createElement('div');
        container.style.display = 'flex';
        container.style.gap = '4px';
        container.appendChild(editBtn);
        container.appendChild(reviewBtn);
        container.appendChild(deleteBtn);
        return container;
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
    this.loadDefaultWorkflow();
  }

  loadDefaultWorkflow() {
    // Get the first workflow from the API to use as default
    this.api.getWorkflows().subscribe(
      (workflows: any[]) => {
        if (workflows && workflows.length > 0) {
          this.defaultWorkflowId = workflows[0].id;
        }
      },
      (error) => {
        console.error('Error loading workflows:', error);
        // Keep default workflowId = 1
      }
    );
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

  onEditRate(rate: any) {
    this.openModal(true, rate);
  }

  onStartReview(rate: any) {
    // Start workflow review for this district rate
    const workflowId = this.defaultWorkflowId;
    const districtId = rate.districtId;
    const districtRateId = rate.id;

    if (!districtId || !districtRateId) {
      alert('Missing district information');
      return;
    }

    this.api.startWorkflow({ districtId, districtRateId, workflowId }).subscribe(
      () => {
        alert('Workflow started successfully!');
        this.loadDistrictRates(); // Refresh the table
      },
      (error) => {
        console.error('Error starting workflow:', error);
        alert('Failed to start workflow. Please check the console for details.');
      }
    );
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
    return this.auth.isOperationalUser;
  }
}
