import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { APIService, DistrictRateDto, ModerateCompensationRateCommand, ModeratedStructureRateViewModel, OptionsListViewModel, OptionViewModel, StructureRatesListViewModel, StructureViewDto } from 'src/app/api.service';
import { StructuresUnitOfMeasure, StructureUnitDescriptions } from 'src/app/app.model';
import { AuthService } from 'src/app/auth.service';

@Component({
  selector: 'app-structures-moderation',
  standalone: false,
  templateUrl: './structures-moderation.component.html',
  styleUrl: './structures-moderation.component.css'
})
export class StructuresModerationComponent {
 currentDistrict: DistrictRateDto|null = null;
  districtPlantRates: DistrictRateDto[] = [];
  growth_list: OptionViewModel[]=[];
  structureRates: StructureRatesListViewModel[]|ModeratedStructureRateViewModel[] = [];

  tableData: any[] = [];
  showMatrixModal: boolean = false;
  rateSearch:string ='';
  editIndex: number | null = null;
  currencyOptions = { align: 'right', allowNegative: false, precision: 0, prefix: 'UGX ' };

  currentIndex = 0;
  isPlantContext = true;
  showPopup = false;
  moderator = 'Moderator A';

  editMode=false;
  groupedStructureRates: { categoryName: string; items: any[]; }[]=[];
   // allComparablePlantRates: PlantRateViewModel[] = [];
    allComparableStructureRates: StructureRatesListViewModel[] = [];
    comparablesLoading: boolean = false;

  constructor(private fb: FormBuilder,private api:APIService, private route:ActivatedRoute, private auth:AuthService) {
 
    this.route.params.subscribe(params => {
      if (params['id']) {        
        this.api.districtRatesGET(params['id']).subscribe(data => {
          this.currentDistrict=data;
          this.loadStructureRates();
          this.loadComparables();
          this.getAllOptions();
         });
      }
    });
  }
  loadComparables(){
    this.comparablesLoading = true;
    this.api.getComparablesByDistrictRateId(this.currentDistrict?.id??0,'structure').subscribe(data => {
    //  this.allComparablePlantRates = data.plantRates??[];
      this.allComparableStructureRates = data.structureRates ?? [];
      this.comparablesLoading = false;
    });
    
  }
    getAllOptions() {
      this.api.optionsGET().subscribe({
        next: (value: OptionsListViewModel) => {
         
          this.growth_list =  value.growthStages || [];
        }
      });
    }
    loadStructureRates() {
      this.api.getModeratedStructureRates(this.currentDistrict?.id??0,true).subscribe(rates => 
        {
          this.structureRates = rates
        this.groupRatesByStructure();
        });
      
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
    groupBy<T>(
      array: T[],
      key: keyof T | ((item: T) => string | number)
    ): { [key: string]: T[] } {
      return array.reduce((acc, curr) => {
        const k = typeof key === 'function' ? key(curr) : curr[key];
        const keyStr = String(k); // normalize keys
        if (!acc[keyStr]) acc[keyStr] = [];
        acc[keyStr].push(curr);
        return acc;
      }, {} as { [key: string]: T[] });
    }
    hasNonGoodQuality(name: string): boolean {
      return this.tableData.some(rate =>
        (rate.groupName === name || rate.plantName === name) &&
        rate.quality !== 'Good'
      );
    }
    getGroupStageName(id:number){
      return this.growth_list?.find(x => x.id === id)?.name;
    }
    getStageName(id: number) {
      return this.growth_list.find(x => x.id === id)?.name ?? '';
    //  return this.plants.find(x => x.id === plantId)?.growthStages?.find(x => x.growthStageId === id)?.name ?? '';
      
    }


    ////Moderation Launch
    startModerationAt(index: number): void {
      this.currentIndex = index;
      this.currentRate = this.structureRates[index];
      this.showPopup = true;
     // this.showModerationPopup = true;
    }
    
    startModerationFromFirstUnmoderated(): void {
      const index = this.structureRates.findIndex((r:any) => !r.isModerated);
      if (index !== -1) {
        this.currentIndex = index;
        this.currentRate = this.structureRates[index];
        this.startModerationAt(index);
      } else {
        alert('All rates have already been moderated.');
      }
    }

    startModeration(context: 'Plant' | 'Structure') {
      this.isPlantContext = context === 'Plant';
      const index = this.structureRates.findIndex((r:ModeratedStructureRateViewModel) => !r.isModerated);
      if (index !== -1) {
       this.currentIndex = index;  
       this.currentRate = this.structureRates[index];  
      this.showPopup = true;  
        //this.startModerationAt(index);
      } else {
        alert('All rates have already been moderated.');
      }

    }
  
    currentRate: any = {};
    getCurrentRate(): StructureRatesListViewModel {
      return  this.structureRates[this.currentIndex];
    }
  
    getComparisonRates(): any[] {
      const current:any = this.getCurrentRate();
      const source = this.allComparableStructureRates;
  
    if (!current || !source) return [];
  
    return source
      .filter((r:any) =>r.structureId === current.structureId && r.unit === current.unit
      )
      .map(r => ({
        districtName: r.districtName,
        rate: r.rate,
        assumptions: r.assumptions,
        year: r.year
      }));
    }
  
    onSave(cmd: ModerateCompensationRateCommand) {
      console.log('Submit:', cmd);
      this.api.getModeratedStructureRates(this.currentDistrict?.id as number,true).subscribe({
        next:(response)=>{
          this.structureRates = response;
          this.groupRatesByStructure();
      this.showPopup = false;
      if(this.editMode)this.editMode=false
        }
      });
    
    }
  
    onSaveAndNext(cmd: ModerateCompensationRateCommand) {
      console.log('Submit & Next:', cmd);
      this.api.getModeratedStructureRates(this.currentDistrict?.id as number,true).subscribe({
        next:(response)=>{
          this.structureRates = response;
          this.groupRatesByStructure();
          if (this.currentIndex + 1 < (this.structureRates.length)) {
            this.currentIndex += 1;
            this.currentRate = null;
            this.currentRate = this.getCurrentRate();
            this.showPopup = true;
          } else {
            this.showPopup = false;
          }
        }
      })
      
    }
  
    onClose() {
      this.showPopup = false;
    }
    ngOnChange() {
      this.getCurrentRate();
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
  get canManage(){
    return this.auth.userValue?.roles?.includes('Admin') || this.auth.userValue?.roles?.includes('superadmin');
  }
}
