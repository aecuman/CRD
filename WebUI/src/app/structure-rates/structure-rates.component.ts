import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { APIService, CreateOrUpdateStructureRateCommand, DistrictRateDto, OptionsListViewModel, StructureRateDto, StructureRatesListViewModel, StructureViewDto } from '../api.service';
import { StructuresUnitOfMeasure, StructureUnitDescriptions } from '../app.model';

@Component({
  selector: 'app-structure-rates',
  standalone: false,
  templateUrl: './structure-rates.component.html',
  styleUrl: './structure-rates.component.css'
})
export class StructureRatesComponent {
  currentDistrict?: DistrictRateDto;
  structures: StructureViewDto[]=[];
structureRates: StructureRatesListViewModel[] = [];
selectedStructure: StructureViewDto|null=null;

showAssumptions = false

structureSearch=''
rateSearch=''

currencyOptions = { align: 'right', allowNegative: false, precision: 0, prefix: 'UGX ' };
      groupedStructureRates: any[] = [];
  groupedStructures: { categoryName: string; items: StructureViewDto[]; }[]=[]; 
showMatrixModal = false;
selectedStructures: StructureViewDto[] = [];
selectedCategoryName = '';
    constructor(private fb: FormBuilder,private api:APIService, private route:ActivatedRoute) {
   
      this.route.params.subscribe(params => {
        if (params['id']) {        
          this.api.districtRatesGET(params['id']).subscribe(data => {
            this.currentDistrict=data;
            this.loadStructureRates();
            this.loadStructures();
            this.getAllOptions();
           });
        }
      });
      this.RateForm = this.fb.group({
        districtRateId: [null, Validators.required],
        id: [null], // For editing existing rates
        structureId: [null, Validators.required],
        unit: [null, Validators.required],
        unitId: [null],
        rate: [null, [Validators.min(1)]],
        noRate:[false],
        showAssumptions:[false],
        assumptions: [''],
        discretionInfo: ['']
      });
    }
      getAllOptions() {
        this.api.optionsGET().subscribe({
          next: (value: OptionsListViewModel) => {
           
            //this.growth_list =  value.growthStages || [];
          }
        });
      }
      
ngOnInit(){
 
}

      loadStructureRates() {
        this.api.getStructureRates(this.currentDistrict?.id).subscribe(rates => 
          {
            this.structureRates = rates
          this.groupRatesByStructure();
          });
        
      }
      loadStructures(){
        this.api.structuresAll().subscribe(data => {
          this.structures = data;
          this.groupStructuresByCategory();
        });
      }
groupStructuresByCategory() {
  const grouped: { [key: string]: StructureViewDto[] } = {};

  for (let structure of this.structures) {
    const categoryName = structure.category?.name || 'Uncategorized';
    if (!grouped[categoryName]) {
      grouped[categoryName] = [];
    }
    grouped[categoryName].push(structure);
  }

  this.groupedStructures = Object.entries(grouped).map(([categoryName, items]) => ({
    categoryName,
    items
  }));
}

groupRatesByStructure() {
  const grouped: { [key: string]: any[] } = {};

  for (let rate of this.structureRates) {
    const categoryName = rate.structure?.category?.name || 'Uncategorized';
    if (!grouped[categoryName]) {
      grouped[categoryName] = [];
    }
    grouped[categoryName].push(rate);
  }

  this.groupedStructureRates = Object.entries(grouped).map(([categoryName, items]) => ({
    categoryName,
    items
  }));
}
      showRateModal = false;
editingRate = false;

RateForm:FormGroup;

unitOptions = ['PerUnit', 'PerSquareMeter', 'PerRoom']; // Use enum if applicable

openRateModal(structure: StructureViewDto) {
  this.selectedStructure = structure;
  this.RateForm.get("structureId")?.setValue(structure.id);
  this.showRateModal = true;
  this.editingRate = false;
}

openRateModalForCategory(category: { categoryName: string, items: StructureViewDto[] }) {
  this.selectedStructures = category.items;
  this.selectedCategoryName = category.categoryName;
  this.showMatrixModal = true;
}


editStructureRate(rate: StructureRatesListViewModel) {
 // this.selectedStructure = rate.structure;
 this.RateForm.patchValue({
   id: rate.id,
    structureId: rate.structureId,
    unit: rate.unitId,
    rate: rate.rate,
    noRate:rate.rate==null,
    assumptions: rate.assumptions,
    discretionInfo: rate.discretionInfo
  });
  this.RateForm.get("districtRateId")?.setValue(this.currentDistrict?.id);
 // this.rateForm = { ...rate };
//  this.editingRate = true;
  this.showRateModal = true;
}

closeRateModal() {
  this.showRateModal = false;
  this.RateForm.reset();
  this.editingRate = false;
  this.selectedStructure = null;
  this.showMatrixModal = false;
  this.selectedStructures = [];

//  this.rateForm = { structureId: 0, districtRateId: 0, unit: '', rate: null, assumptions: '', discretionInfo: '' };
}

saveStructureRate() {
  this.RateForm.get('districtRateId')?.setValue(this.currentDistrict?.id);
  this.api.upsertStructureRate({data:this.RateForm.value}).subscribe(() => {
    this.loadStructureRates();
    this.RateForm.reset();
    this.showRateModal = false;
  });
  // POST or PUT logic here

}
editIndex: number | null = null;

saveEditedRate(rate: StructureRatesListViewModel) {
  const payload:StructureRateDto = {
    id: rate.id,
    structureId: rate.structureId,
    districtRateId: this.currentDistrict?.id, // you must include this if required
   // unit: this.unitDescriptions[rate.unit].unit,
   // unitName: this.unitDescriptions[rate.unit],
    rate: rate.rate,
    assumptions: rate.assumptions,
    discretionInfo: rate.discretionInfo
  };

  this.api.upsertStructureRate({data:payload}).subscribe(() => {
    this.editIndex = null;
    this.loadStructureRates();
  });
}

deleteRate(id: number) {
  this.api.deleteStructureRate(id).subscribe(() => {
    this.loadStructureRates();
  });

}
confirmSave(rate: any) {
  if (confirm('Save changes to this rate?')) {
    this.saveEditedRate(rate);
    this.editIndex = null;
  }
}

confirmDelete(rate: any) {
  if (confirm('Are you sure you want to delete this rate?')) {
    this.deleteRate(rate.id);
  }
}
  get unitKeys():number[] {
    return Object.keys(StructuresUnitOfMeasure).filter(k => !isNaN(Number(k))).map(Number);
  }
  getKey(value: string): any {
    return Object.keys(StructuresUnitOfMeasure).find((key:any) => StructuresUnitOfMeasure[key] === value) || 0;
  }
  
  get unitDescriptions():any {
    return StructureUnitDescriptions;
  }
  saveRates(event:any){
this.closeRateModal();
this.loadStructureRates();
  }
  hasAnyRate(structureList: any[]): boolean {
    return structureList.some(struct =>
      this.structureRates.some(rate => rate.structureId === struct.id)
    );
  }
  
  hasAllRates(structureList: any[]): boolean {
    return structureList.every(struct =>
      this.structureRates.some(rate => rate.structureId === struct.id)
    );
  }
  hasRate(structure: StructureViewDto): boolean {
    return this.structureRates.some(rate => rate.structureId === structure.id);
  }
}
