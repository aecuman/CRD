import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { APIService, CRDFileDto, District, DistrictRateDto, FileParameter } from '../api.service';
import type { ColDef } from "ag-grid-community";

@Component({
  selector: 'app-status',
  templateUrl: './status.component.html',
  styleUrls: ['./status.component.css']
})
export class StatusComponent {
districtRates: DistrictRateDto[] = [];
  form: FormGroup;
  isModalOpen = false;
  editMode = false;
  selectedRateId: number | null = null;
  all_districts:District[] = [];
  uploadedFiles: CRDFileDto[] = [];
  saving = false;
  uploading = false;

  columnDefs: ColDef[] = [
    { field: 'districtName', headerName: 'District', sortable: true, filter: true},
    { field: 'year', headerName: 'Year', sortable: true, filter: true },
    { field: 'status', headerName: 'Status', sortable: true, filter: true },
    {
      field: 'actions',
      headerName: 'Actions',
      cellRenderer: (params: any) => {
        return `
          <button class="bg-green-500 text-white px-2 py-1 rounded mr-2" routerLink="/portal/rates/${params.data.id}">Edit</button>
          <button class="bg-red-500 text-white px-2 py-1 rounded" onclick="deleteRate(${params.data.id})">Delete</button>
        `;
      }
    }
  ];

  constructor(private fb: FormBuilder, private api: APIService) {
    this.loadDistricts();
    this.form = this.fb.group({
      districtId: ['', Validators.required],
      year: ['', [Validators.required, Validators.min(2000)]],
      uploadsIds: [[]]
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
      this.uploadedFiles = rate.uploads || [];
    } else {
      this.form.reset();
      this.uploadedFiles = [];
      this.selectedRateId = null;
    }
  }

  closeModal() {
    this.isModalOpen = false;
  }

  onFileSelected(event: any) {

    this.uploading = true;
    const files: FileList = event.target.files;
    console.log(files);
    if (files.length > 0) {
      for (let i = 0; i < files.length; i++) {
        const formData = new FormData();
        formData.append('file', files[i] as Blob, files[i].name);
        formData.append('refId', this.selectedRateId ? this.selectedRateId.toString() : '0');

        this.api.upload({data:files[i],fileName:files[i].name},this.selectedRateId ? this.selectedRateId :0)
          .subscribe((response:any) => {
            this.uploading = false;
            console.log('File uploaded successfully:', response);
            
            this.uploadedFiles.push({ id: response, name: files[i].name });
            this.form.patchValue({ uploadsIds: this.uploadedFiles.map(f => f.id) });
          }, error => {
            this.uploading = false;
            console.error('Error uploading file:', error);
            alert('Error uploading file. Please try again.');
          } 
          );
      }
    }
  }

  submitForm() {
    console.log(this.form.value)
    this.saving = true;
    if (this.form.valid) {
      if (this.editMode && this.selectedRateId) {
        this.api.districtRatesPUT(this.selectedRateId, this.form.value)
          .subscribe(() => {
            this.saving = false;
            this.loadDistrictRates();
            this.closeModal();
          },error=>{
            console.error('Error updating rate:', error);
            alert('Error updating rate. Please try again.');
          });
      } else {
        this.api.districtRatesPOST(this.form.value)
          .subscribe(() => {
            this.saving = false;
            this.loadDistrictRates();
            this.closeModal();
          },error=>{
            console.error('Error creating rate:', error);
            alert('Error creating rate. Please try again.');
          }
          );
      }
    }
  }

  deleteRate(id: number) {
    this.api.districtRatesDELETE(id).subscribe(() => {
      this.loadDistrictRates();
    });
  }
  startWorkflow(districtId: number,districtRateId:any): void {
    const workflowId = 5; // You can make this dynamic or select based on UI input

    this.api.startWorkflow({ districtId,districtRateId, workflowId}).subscribe(() => {
      alert("Workflow started for district.");
      this.ngOnInit(); // Refresh the table
    });
  }
}
