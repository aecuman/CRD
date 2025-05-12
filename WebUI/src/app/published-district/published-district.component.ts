import { Component, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ExportService } from '../export.service';
import { ModerationReportViewModel, ModeratedPlantRateViewModel, OptionViewModel, APIService, OptionsListViewModel } from '../api.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-published-district',
  standalone: false,
  templateUrl: './published-district.component.html',
  styleUrl: './published-district.component.css'
})
export class PublishedDistrictComponent {
  activeTab: 'plants' | 'structures' = 'plants';

  report?: ModerationReportViewModel;
  PlantData: ModeratedPlantRateViewModel[]=[];

  plantRateSearch:string ='';
  structureRateSearch:string ='';
  growth_list: OptionViewModel[]=[];
  structureRatesFlat: { categoryName: string; items: any[]; }[]=[];

  loading=false;
  /**
   *
   */
  constructor(private api:APIService, private route:ActivatedRoute, public exportService:ExportService, private router:Router, public location:Location) {
    this.getAllOptions();
    this.route.params.subscribe(params => {

      if (params['id']) {        
        
          this.getReport(params['id']);
        
      }
    });
  }

/*   getGroupStageName(id:number){
    return this.growth_list?.find(x => x.id === id)?.name;
  }
  getStageName(id: number) {
    return this.growth_list.find(x => x.id === id)?.name ?? '';
  //  return this.plants.find(x => x.id === plantId)?.growthStages?.find(x => x.growthStageId === id)?.name ?? '';
    
  }
  hasNonGoodQuality(name: string): boolean {
    return this.groupedPlantRates.some(rate =>
      (rate.groupName === name || rate.plantName === name) &&
      rate.quality !== 'Good'
    );
  } */
   getReport(id:number){
    this.loading = true;
      this.api.getModerationReport(id).subscribe(data => {
        this.report=data;
        this.groupRows();
        this.groupRatesByStructure();
        this.exportService.SetData(this.report,this.PlantData,this.structureRatesFlat,this.growth_list,'Approved');
        this.loading = false;
      }, error => {
        console.error('Error fetching report:', error);
        this.loading = false;
      });
    }
        getAllOptions() {
          this.api.optionsGET().subscribe({
            next: (value: OptionsListViewModel) => {
             
              this.growth_list =  value.growthStages || [];
            }
          });
        }
    groupRows() {
      // First group by a unified key: either plantName or groupName
      const groupedByKey = this.groupBy(this.report?.plantRates??[], item =>
        item.groupedPlantId ? item.groupName ?? '' : item.plantName ?? ''
      );
    
      this.PlantData = [];
    
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
    
            this.PlantData.push(item);
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
      return this.PlantData.some(rate =>
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
    groupRatesByStructure() {
      const grouped: { [key: string]: any[] } = {};
    
      for (let rate of this.report?.structureRates??[]) {
        const categoryName = rate.structure?.category?.name || 'Uncategorized';
        if (!grouped[categoryName]) {
          grouped[categoryName] = [];
        }
        grouped[categoryName].push(rate);
      }
    
      this.structureRatesFlat = Object.entries(grouped).map(([categoryName, items]) => ({
        categoryName,
        items
      }));
    }
    GetAttributeNames(attribute: any): string {
      return attribute.map((opt:any) => opt.name).join(', ')
     }
}

