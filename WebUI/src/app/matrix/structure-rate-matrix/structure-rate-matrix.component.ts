import { Component, Input, Output, EventEmitter, OnInit, input } from '@angular/core';
import { FormBuilder, FormGroup, FormArray } from '@angular/forms';
import { APIService, DistrictRateDto, StructureViewDto } from 'src/app/api.service';
import { StructuresUnitOfMeasure, StructureUnitDescriptions } from 'src/app/app.model';
import { AuthService } from 'src/app/auth.service';

@Component({
  selector: 'app-structure-rate-matrix',
  standalone: false,
  templateUrl: './structure-rate-matrix.component.html',
  styleUrls: ['./structure-rate-matrix.component.css']
})
export class StructureRateMatrixComponent implements OnInit {
  @Input() structures: StructureViewDto[] = [];
  @Input() categoryName: string = '';
  @Input() districtRateId!: number;

  @Input() currentDistrict?: DistrictRateDto;

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<any[]>();

  matrixForm!: FormGroup;
  StructuresUnitOfMeasure = StructuresUnitOfMeasure;

  isSaving:boolean=false;

  constructor(private fb: FormBuilder, private api: APIService,private auth:AuthService) {
    this.matrixForm = this.fb.group({
      rows: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.generateMatrix();
  }

  get rows(): FormArray {
    return this.matrixForm.get('rows') as FormArray;
  }

  generateMatrix(): void {
    const matrixControls = this.structures.map(structure => this.fb.group({
      structureId: [structure.id],
      structureName: [structure.name],
      unit: [StructuresUnitOfMeasure[0]],
      rate: [null],
      noRate:[false],
      showAssumptions:[false],
      assumptions: [''],
      discretionInfo: ['']
    }));
    this.matrixForm.setControl('rows', this.fb.array(matrixControls));
  }

  handleSubmit(): void {
    if (!this.matrixForm.value.rows || !this.matrixForm.value.rows.length || !this.currentDistrict?.id) {
      alert("No matrix data to save or missing district info.");
      return;
    }
    if (this.matrixForm.valid) {
      
     // this.ap
  
    const payload = {
      districtRateId: this.currentDistrict?.id,
      rates: this.matrixForm.value.rows.map((row:any) => ({
        structureId: row.structureId,
        unit: row.unit,
        rate: row.noRate ? null : row.rate,
        assumptions: row.assumptions,
        discretionInfo: row.noRate ? row.discretionInfo : null
      }))
    };
  this.isSaving = true;
    this.api.submitStructureRateMatrix(payload).subscribe({
      next: () => {
       // alert("Structure rates matrix saved successfully!");
        this.save.emit(this.matrixForm.value.rows); // if you want to send data to parent
        this.isSaving=false;
      },
      error: (err) => {
        this.isSaving=false;
        console.error("Failed to save structure matrix", err);
        alert("Failed to save structure matrix.");
      }
    });  }
  }

  cancel(): void {
    this.close.emit();
  }
   get unitKeys():number[] {
      return Object.keys(StructuresUnitOfMeasure).filter(k => !isNaN(Number(k))).map(Number);
    }
    
    get unitDescriptions():any {
      return StructureUnitDescriptions;
    }
    trackByIndex(index: number, _: any) {
      return index;
    }
    handleNoRateToggle(i: number, event: any) {
     // const row = this.matrixRows[i];
    //  row.noRate = event.target.checked;
     // if (row.noRate) row.rate = undefined;
    }
    toggleAssumptions(i: number, event: any) {
     // this.matrixRows[i].showAssumptions = event.target.checked;
    }
    get canManage(){
      return this.auth.isOperationalUser;
    }
    
}
