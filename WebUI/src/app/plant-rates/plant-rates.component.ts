import { Component, Input } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { APIService, CompensationRateEntryDto,  CreatePlantRatesCommand,  DistrictRateDto, GroupedPlantListViewModel, OptionsListViewModel, OptionViewModel, PlantListViewModel, PlantRateViewModel, UpdatePlantRatesCommand} from '../api.service';
import { PlantRateDto, UnitOfMeasure  } from '../app.model';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-plant-rates',
  standalone: false,
  templateUrl: './plant-rates.component.html',
  styleUrl: './plant-rates.component.css'
})
export class PlantRatesComponent {
  plant_district_rates: PlantRateViewModel[]=[];
  currentDistrict?: DistrictRateDto;
  plants: PlantListViewModel[]=[];
  groupedRates: PlantRateViewModel|any;
  tableData: any[]=[];
activeTab='plants'
  groupedPlants: GroupedPlantListViewModel[]=[];
  selectedEntityType: 'plant' | 'group' | null = null;
selectedEntity: PlantListViewModel | GroupedPlantListViewModel | null = null;
  
isGroupedMatrix: boolean=false;
  groupName: string='';
  groupId: number | null = null;
  groupedPlantIds: number[] = [];
  selectedGroupedPlant: GroupedPlantListViewModel|null = null;
  growth_list: OptionViewModel[]=[];

plantSearch:string ='';
rateSearch:string ='';
  existingMatrixRates: PlantRateDto[]=[];

  constructor(private fb: FormBuilder,private api:APIService, private route:ActivatedRoute, private auth:AuthService) {
 
    this.route.params.subscribe(params => {
      if (params['id']) {        
        this.api.districtRatesGET(params['id']).subscribe(data => {
          this.currentDistrict=data;
          this.loadDistrictPlantRates();
          this.getAllOptions();
         });
      }
    });
  }

  ngOnInit() {
    this.loadPlants();
    this.loadGroupedPlants();
  }

  loadPlants(reload?:true) {
    // Replace with API call
    this.api.plantsAll().subscribe({
      next: (value: PlantListViewModel[]) => {
        this.plants = value;
        /* if(!reload){
          this.currentPlant = this.plants[this.currentPlantIndex];
        } */
      }
    });
    
  }
  hasNonGoodQuality(name: string): boolean {
    return this.tableData.some(rate =>
      (rate.groupName === name || rate.plantName === name) &&
      rate.quality !== 'Good'
    );
  }
  hasRateForCategoryInfoOption(plant: PlantListViewModel, categoryId: number, categoryInfoId: number,categoryInfoIndex:number): boolean {
   
    return this.tableData.some(rate =>
      rate.category === ((plant.infoCategories??[]).find(x=>x.id==categoryInfoId)as any)?.info[categoryInfoIndex??0]
    );
  }
  getAllOptions() {
    this.api.optionsGET().subscribe({
      next: (value: OptionsListViewModel) => {
       
        this.growth_list =  value.growthStages || [];
      }
    });
  }
  loadDistrictPlantRates(plantId?:any,closeModal?:boolean){
    this.api.getPlantsRatesByDistrict(this.currentDistrict?.id as number).subscribe({
      next:(response)=>{
        if(!plantId){
         this.plant_district_rates = response 
        }
        if(closeModal){
          this.showMatrixModal = false;
        }
this.groupRows()
      }
    })
  }
  loadGroupedPlants() {
    this.api.getGroupedPlants().subscribe(groups => {
      this.groupedPlants = groups;
    });
  }
  getGroupStageName(id:number){
    return this.growth_list?.find(x => x.id === id)?.name;
  }
  showMatrixModal = false;
  selectedPlant: any = {}; // assign from your list if 
  selcetdPlantIndex:number=0;
  
 openMatrixModal(type: 'plant' | 'group', entity: any,index:number) {
  if (type === 'plant') {
    this.selectedPlant = entity as PlantListViewModel;
    this.selcetdPlantIndex = index;
    this.isGroupedMatrix = false;
    this.groupName = '';
    this.groupId = null;
    this.groupedPlantIds = [];
    this.selectedGroupedPlant=null;
  } 
  else if (type === 'group') {
    this.selectedPlant = null;
    this.selcetdPlantIndex = 0;
    let SelectedEntity = entity as GroupedPlantListViewModel;
    this.selectedEntityType = 'group';
    this.selectedEntity = SelectedEntity;

    this.isGroupedMatrix = true;
    this.groupName = SelectedEntity.name??''
    this.groupId = SelectedEntity.id??0;
    this.groupedPlantIds = SelectedEntity.plants?.map(x=>x.id??0) || []; // assuming entity has this structure
    this.selectedGroupedPlant = SelectedEntity;
  }
  this.existingMatrixRates = this.getExistingRates(entity, type === 'group');
  this.showMatrixModal = true;
 
  } 
  plantHasRates(plantId: any): boolean {
    return this.plant_district_rates.some(rate => rate.plantId === plantId);
  }

  handleMatrixSave(matrix: PlantRateDto[]) {
   // this.savedRates = [...this.savedRates, ...matrix];
   this.loadDistrictPlantRates(null,true);
    
  }
  getStageName(plantId:number,id: number) {
    return this.plants.find(x => x.id === plantId)?.growthStages?.find(x => x.growthStageId === id)?.name ?? '';
    
  }
  groupPlantRates() {
    const groupedMap = new Map();

  

  // Sort by plantName → growthStage → unit
  const sortedRates = this.plant_district_rates.sort((a, b) => 
    (a.plantName ?? '').localeCompare(b.plantName ?? '') || 
    (a.growthStage??0) - (b.growthStage??0) || 
    (a.unit??'').localeCompare(b.unit??'')
  );

  this.plant_district_rates.forEach(rate => {
    const plantKey = rate.plantName;
    const growthKey = `${rate.plantName}-${rate.growthStage}`;
    const unitKey = `${rate.plantName}-${rate.growthStage}-${rate.unit}`;

    if (!groupedMap.has(plantKey)) {
      groupedMap.set(plantKey, { ...rate, plantRowSpan: 0 });
    }
    groupedMap.get(plantKey).plantRowSpan++;

    if (!groupedMap.has(growthKey)) {
      groupedMap.set(growthKey, { ...rate, growthRowSpan: 0 });
    }
    groupedMap.get(growthKey).growthRowSpan++;

    if (!groupedMap.has(unitKey)) {
      groupedMap.set(unitKey, { ...rate, unitRowSpan: 1 });
    }
  });



  this.groupedRates = Array.from(groupedMap.values());


  
}
groupRows() {
  // First group by a unified key: either plantName or groupName
  const groupedByKey = this.groupBy(this.plant_district_rates, item =>
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
getExistingRates(selectedEntity: any, isGrouped: boolean): PlantRateDto[] {
  if (!selectedEntity || !this.tableData) return [];

  if (isGrouped) {
    // For Grouped Plants
    return this.tableData.filter(rate =>
      rate.groupedPlantId === selectedEntity.id
    ).map(rate => ({
      ...rate,
      groupedPlantIds: selectedEntity.plantIds, // optional for context
      groupName: selectedEntity.name
    }));
  } else {
    // For Single Plant
    return this.tableData.filter(rate =>
      rate.plantId === selectedEntity.id
    );
  }
}

DeleteRate(id:number){
  if (confirm("Are you sure you want to delete this rate?")) {
  this.api.deletePlantRate(id).subscribe({
   next:()=>{
      this.loadDistrictPlantRates();
    }
  });
  }
}

/*groupRows() {
  // First group by plantName
  const groupedByPlant = this.groupBy(this.plant_district_rates, 'plantName');

  this.tableData = [];

  for (const plantName in groupedByPlant) {
    const plantGroup = groupedByPlant[plantName];
    const plantRowspan = plantGroup.length;

    // Group by unit within each plantName group
    const groupedByUnit = this.groupBy(plantGroup, 'unit');

    for (const unit in groupedByUnit) {
      const unitGroup = groupedByUnit[unit];
      const unitRowspan = unitGroup.length;

      unitGroup.forEach((item, index) => {
        item.showPlant = index === 0 && plantGroup.indexOf(item) === 0;
        item.plantRowspan = item.showPlant ? plantRowspan : 0;

        item.showUnit = index === 0;
        item.unitRowspan = item.showUnit ? unitRowspan : 0;

        this.tableData.push(item);
      });
    }
  }
}

groupBy(array: any[], key: string): { [key: string]: any[] } {
  return array.reduce((acc, curr) => {
    const k = curr[key];
    if (!acc[k]) acc[k] = [];
    acc[k].push(curr);
    return acc;
  }, {} as { [key: string]: any[] });
}*/
editIndex: number | null = null;

editRate(index: number) {
  this.editIndex = index;
}
  units = Object.entries(UnitOfMeasure).filter(([key, value]) => !isNaN(Number(value))) // Get only numeric values
  .map(([key, value]) => ({ key, value: Number(value) }));
saveRate(rate: PlantRateViewModel, index: number) {
  var cmd:CreatePlantRatesCommand={
    districtRateId: this.currentDistrict?.id as number,
    rates: [{
      id: rate.id,
      rate: rate.rate,
      quality:rate.quality
    }
    ]

  }
  this.api.updateSingleRate(rate.id,{id:rate.id,rate:rate.rate}).subscribe({
    next: () => {
      this.editIndex = null;
      this.loadDistrictPlantRates(); // Refresh
    },
    error: () => alert('Failed to update rate.')
  });
}

confirmDelete(rate: PlantRateViewModel, index: number) {
  if (!confirm('Are you sure you want to delete this rate?')) return;

/*   this.api.deletePlantRate(rate.id!).subscribe({
    next: () => {
      this.plant_district_rates.splice(index, 1);
    },
    error: () => alert('Failed to delete rate.')
  }); */
}

groupHasRates(groupId: number): boolean {
  return this.plant_district_rates.some(rate => rate.groupedPlantId === groupId);
 // return this.plant_district_rates.some(rate => rate.groupName === this.groupedPlants.find(g => g.id === groupId)?.name);
}
/*     this.groupedRates = this.plant_district_rates.reduce((acc:any, rate) => {
      const key:any = `${rate.plantName}-${rate.growthStage}-${rate.unit}`;
      if (!acc[key]) {
        acc[key] = { ...rate, totalRate: rate.rate };
      } else {
        acc[key].totalRate += rate.rate;
      }
      return acc;
    }, {}); */

    get canManage(){
      return this.auth.userValue?.roles?.includes('admin') || this.auth.userValue?.roles?.includes('superadmin');
    }
  }

  


