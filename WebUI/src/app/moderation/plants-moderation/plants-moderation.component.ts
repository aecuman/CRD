import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { APIService, DistrictRateDto, ModerateCompensationRateCommand, ModeratedPlantRateViewModel, OptionsListViewModel, OptionViewModel, PlantRateViewModel, StructureRatesListViewModel } from '../../api.service';
import { FormBuilder } from '@angular/forms';
import { RateRow } from '../../app.model';

@Component({
  selector: 'app-plants-moderation',
  standalone: false,
  templateUrl: './plants-moderation.component.html',
  styleUrl: './plants-moderation.component.css'
})
export class PlantsModerationComponent {
 currentDistrict: DistrictRateDto|null = null;
  districtPlantRates: DistrictRateDto[] = [];
  growth_list: OptionViewModel[]=[];
  plant_district_rates: PlantRateViewModel[]|ModeratedPlantRateViewModel[]=[];
  structureRates: StructureRatesListViewModel[] = [];

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
  allComparablePlantRates: PlantRateViewModel[] = [];
  allComparableStructureRates: StructureRatesListViewModel[] = [];
  comparablesLoading: boolean = false;

  constructor(private fb: FormBuilder,private api:APIService, private route:ActivatedRoute) {
 
    this.route.params.subscribe(params => {
      if (params['id']) {        
        this.api.districtRatesGET(params['id']).subscribe(data => {
          this.currentDistrict=data;
          this.loadComparables();
          this.loadDistrictPlantRates();
          this.getAllOptions();
         });
      }
    });
  }
    getAllOptions() {
      this.api.optionsGET().subscribe({
        next: (value: OptionsListViewModel) => {
         
          this.growth_list =  value.growthStages || [];
        }
      });
    }
    loadDistrictPlantRates(plantId?:any,closeModal?:boolean){
      this.api.getModeratedPlantRates(this.currentDistrict?.id as number,true).subscribe({
        next:(response)=>{
          if(!plantId){
           this.plant_district_rates = response 
          }
          if(closeModal){
           // this.showMatrixModal = false;
          }
  this.groupRows()
        }
      })
    }
    groupRows() {
      // First group by a unified key: either plantName or groupName
      const groupedByKey = this.groupBy((this.plant_district_rates as PlantRateViewModel[]), item =>
        item.groupedPlantId ? item.groupName ?? '' : item.plantName ?? ''
      );
    
      this.tableData = [];
    
      for (const plantKey in groupedByKey) {
        const plantGroup = groupedByKey[plantKey];
        const groupedByUnit = this.groupBy(plantGroup, 'unit');
    
        const plantRowspan = Object.values(groupedByUnit)
          .reduce((sum, unitGroup) => sum + unitGroup.length, 0);
    
        let plantRendered = false;
    
        for (const unitKey in groupedByUnit) {
          const unitGroup = groupedByUnit[unitKey];
          const unitRowspan = unitGroup.length;
    
          let unitRendered = false;
    
          unitGroup.forEach((item:any, index) => {
            item.showPlant = !plantRendered;
            item.plantRowspan = item.showPlant ? plantRowspan : 0;
            plantRendered = true;
    
            item.showUnit = !unitRendered;
            item.unitRowspan = item.showUnit ? unitRowspan : 0;
            unitRendered = true;
    
            this.tableData.push(item);
          });
        }
      }
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
    keepCurrentRate(row: RateRow) {
      row.rate = row.existingRate;
      row.useCurrent = true;
    }
    
    toggleNoRate(row: RateRow) {
      if (row.noRate) {
        row.rate = null as any;
        row.useCurrent = false;
      }
    }

    ////Moderation Launch
    startModerationAt(index: number): void {
      this.currentIndex = index;
      this.currentRate = this.plant_district_rates[index];
      this.showPopup = true;
     // this.showModerationPopup = true;
    }
    
    startModerationFromFirstUnmoderated(): void {
      const index = this.plant_district_rates.findIndex((r:any) => !r.isModerated);
      if (index !== -1) {
        this.currentIndex = index;
        this.currentRate = this.plant_district_rates[index];
        this.startModerationAt(index);
      } else {
        alert('All rates have already been moderated.');
      }
    }

    startModeration(context: 'Plant' | 'Structure') {
      this.isPlantContext = context === 'Plant';
      const index = this.plant_district_rates.findIndex((r:ModeratedPlantRateViewModel) => !r.isModerated);
      if (index !== -1) {
       this.currentIndex = index;  
       this.currentRate = this.plant_district_rates[index];     
        //this.startModerationAt(index);
      } else {
        alert('All rates have already been moderated.');
      }

      this.showPopup = true;
    }
  
    currentRate: any = {};
    getCurrentRate(): PlantRateViewModel | StructureRatesListViewModel {
      return this.isPlantContext
        ? this.plant_district_rates[this.currentIndex??0] as any
        :[] as any// this.structureRates[this.currentIndex];
    }
    loadComparables(){
      this.comparablesLoading = true;
      this.api.getComparablesByDistrictRateId(this.currentDistrict?.id??0,'plant').subscribe(data => {
        this.allComparablePlantRates = data.plantRates??[];
        this.allComparableStructureRates = data.structureRates ?? [];
        this.comparablesLoading = false;
      });
      
    }
    getComparisonRates(): any[] {
      const current:any = this.getCurrentRate();
      const source = this.isPlantContext
      ? this.allComparablePlantRates
      : this.allComparableStructureRates;
  
    if (!current || !source) return [];
  
    return source
      .filter((r:any) =>
        this.isPlantContext
          ?  r?.plantId === current?.plantId &&
          r.growthStage === current.growthStage &&
          r.unit === current.unit &&
          (r.quality || 'Good') === (current.quality || 'Good')
        : r.structureId === current.structureId &&
          r.unit === current.unit
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
    
      this.api.getModeratedPlantRates(this.currentDistrict?.id as number,true).subscribe({
        next:(response)=>{
          this.plant_district_rates = response;
          this.groupRows();
      this.showPopup = false;
      if(this.editMode)this.editMode=false
        }
      });
    
    }
  
    onSaveAndNext(cmd: ModerateCompensationRateCommand) {
      console.log('Submit & Next:', cmd);
      this.api.getModeratedPlantRates(this.currentDistrict?.id as number,true).subscribe({
        next:(response)=>{
          this.plant_district_rates = response;
          this.groupRows();
          if (this.currentIndex + 1 < (this.isPlantContext ? this.plant_district_rates.length : this.structureRates.length)) {
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
}
