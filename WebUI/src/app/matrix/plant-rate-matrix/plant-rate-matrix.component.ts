import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PlantListViewModel, APIService, DistrictRateDto, GroupedPlantListViewModel, OptionsListViewModel, OptionViewModel, PlantRateViewModel } from 'src/app/api.service';
import { PlantRateDto,UnitOfMeasure } from 'src/app/app.model';

@Component({
  selector: 'app-plant-rate-matrix',
  standalone: false,
  templateUrl: './plant-rate-matrix.component.html',
  styleUrl: './plant-rate-matrix.component.css'
})
export class PlantRateMatrixComponent {
  plantRateForm: FormGroup;
  @Input() plants: PlantListViewModel[] = [];
  @Input() currentPlantIndex: number = 0;
  isConfigured: boolean = false; // Step 1: Configuration before showing the matrix

  @Input() isGrouped: boolean = false;
  @Input() groupName: string = '';
  @Input() groupId: number | null= null;
  @Input() groupedPlantIds: number[] = [];
  @Input() currentGroupedPlant: GroupedPlantListViewModel|null = null;

  units = Object.entries(UnitOfMeasure).filter(([key, value]) => !isNaN(Number(value))) // Get only numeric values
  .map(([key, value]) => ({ key, value: Number(value) }));

  matrixRows: PlantRateDto[] = [];
  allMatrixRows: PlantRateDto[] = [];

  @Input() currentPlant?: PlantListViewModel;
  @Input() currentDistrict?: DistrictRateDto;
  currencyOptions = { align: 'right', allowNegative: false, precision: 0, prefix: 'UGX ' };
  collapsedUnits: { [unit: string]: boolean } = {};
  groupedMatrix: { unit: string, rows: PlantRateDto[] }[] = [];

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<PlantRateDto[]>();


  @Input() growth_list: OptionViewModel[]=[];
  
  @Input() existingRates: PlantRateDto[] = [];  // 💡 Add this input

  isSaving:boolean = false;

  activeCategoryId: number | null = null;
  
  activeCategoryInfo: { categoryId: number, info: string, categoryInfoId: number,categoryInfoIndex:number } | null = null;

  activeTabIndex: number = 0;



  /**
   *
   */
 constructor(private fb: FormBuilder,private api:APIService) {
    this.plantRateForm = this.fb.group({
      districtRateId: ['', Validators.required],
      isGrouped: [false, Validators.required],
      groupName: [''],
      groupedPlantIds: this.fb.array([]),
      considerCategories: [false],
      selectedCategoryInfos: this.fb.array([], Validators.required),
      selectedCategories: this.fb.array([]),
      selectedGrowthStages: this.fb.array([], Validators.required),
      selectedUnits: this.fb.array([], Validators.required),
      selectedQualities:this.fb.array([],Validators.required)
    });
    this.plantRateForm.get('districtRateId')?.setValue(this.currentDistrict?.id)
    // Auto-select Growth Stages
this.selectedGrowthStages.clear();
for (let stage of this.currentPlant?.growthStages ?? []) {
  this.selectedGrowthStages.push(this.fb.control(stage.growthStageId));
}

// Auto-select Units
this.selectedUnits.clear();
for (let unit of this.units) {
  this.selectedUnits.push(this.fb.control(unit.value));
}

this.selectedQualities.clear();

// Always include "Good"
this.selectedQualities.push(this.fb.control('Good'));

// Optional others (can be pre-checked if you want)
/*const optionalQualities = ['Medium', 'Poor'];
for (let q of optionalQualities) {
  this.selectedQualities.push(this.fb.control(q));
}*/

// Auto-select all Category Info Options
this.selectedCategoryInfos.clear();
for (let cat of this.currentPlant?.infoCategories ?? []) {
  (cat.info??[]).forEach((info, index) => {
    this.selectedCategoryInfos.push(this.fb.control({
      categoryId: cat.categoryId,
      info,
      categoryInfoId: index
    }));
  });
}

    //this.getAllOptions();
  }

getCategoryNameById(id: number): string {
  return this.currentPlant?.categories?.find(c => c.id === id)?.name || `Category ${id}`;
}

  getStageName(id: number) {
    return this.currentPlant?.growthStages?.find(x => x.growthStageId === id)?.name;
  }
  getGroupStageName(id:number){
    return this.growth_list?.find(x => x.id === id)?.name;
  }

  get selectedGrowthStages(): FormArray {
    return this.plantRateForm.get('selectedGrowthStages') as FormArray;
  }

  get selectedUnits(): FormArray {
    return this.plantRateForm.get('selectedUnits') as FormArray;
  }

  get selectedCategories(): FormArray {
    return this.plantRateForm.get('selectedCategories') as FormArray;
  }
  get selectedCategoryInfos(): FormArray {
    return this.plantRateForm.get('selectedCategoryInfos') as FormArray;
  }
  
  isCategoryInfoSelected(categoryId: number, categoryInfoId: number): boolean {
    return this.selectedCategoryInfos.value.some((item: any) =>
      item.categoryId === categoryId && item.categoryInfoId === categoryInfoId
    );
  }
  
  toggleCategoryInfo(categoryId: number, info: string, categoryInfoId: number,categoryInfoIndex: number) {
    const index = this.selectedCategoryInfos.value.findIndex(
      (item: any) => item.categoryId === categoryId && item.categoryInfoId === categoryInfoId&& item.categoryInfoIndex === categoryInfoIndex
    );
  
    if (index !== -1) {
      this.selectedCategoryInfos.removeAt(index);
    } else {
      this.selectedCategoryInfos.push(
        this.fb.control({ categoryId, info, categoryInfoId, categoryInfoIndex })
      );
    }
  }
  toggleGrowthStage(stageId: any) {
    const index = this.selectedGrowthStages.value.indexOf(stageId);
    if (index === -1) {
      this.selectedGrowthStages.push(this.fb.control(stageId));
    } else {
      this.selectedGrowthStages.removeAt(index);
    }
  }
  isGrowthStageSelected(stageId: any): boolean {
    return this.selectedGrowthStages.value.includes(stageId);
  }
  

  toggleUnit(event: any,unit: any) {
   /* if (this.selectedUnits.includes(unit)) {
      this.selectedUnits = this.selectedUnits.filter(u => u !== unit);
    } else {
      this.selectedUnits.push(unit);
    }*/
    const index = this.selectedUnits.value.indexOf(unit.value);
   console.log(index,unit)

    if (index === -1) {
      this.selectedUnits.push(this.fb.control(unit.value));
    } else {
      this.selectedUnits.removeAt(index);
    }
  }
  isUnitSelected(unitValue: any): boolean {
    return this.selectedUnits.value.includes(unitValue);
  }
  
  toggleCategory(categoryId: number) {
    const index = this.selectedCategories.value.indexOf(categoryId);
    if (index === -1) {
      this.selectedCategories.push(this.fb.control(categoryId));
    } else {
      this.selectedCategories.removeAt(index);
    }
  }

  addToGroup() {
    const currentPlant = this.plants[this.currentPlantIndex];
    if (!this.groupedPlantIds.includes(currentPlant.id as number)) {
      this.groupedPlantIds.push(currentPlant.id as number);
    }
  }

  nextPlant() {
    if (this.currentPlantIndex < this.plants.length - 1) {
      this.currentPlantIndex++;
    }
  }

  prevPlant() {
    if (this.currentPlantIndex > 0) {
      this.currentPlantIndex--;
    }
  }


setActiveInfoTab(tab: { categoryId: number, info: string, categoryInfoId: number,categoryInfoIndex: number }) {
  this.activeCategoryInfo = tab;
  this.generateMatrixForActiveTab(); // 🔁 Generate matrix only for that tab
}
isRateRowValid(row: PlantRateDto): boolean {
  const hasValidRate:boolean = row.rate != null;
  const hasDiscretion:boolean = (row.noRate && row.discretionInfo && row.discretionInfo?.trim() !== '')as boolean;
  return hasValidRate || hasDiscretion;
}
isCurrentTabValid(): boolean {
  return this.matrixRows.every(row => this.isRateRowValid(row));
}

get activeTabCategoryInfo(): any {
  return this.selectedCategoryInfos.value[this.activeTabIndex];
}

nextTab() {
  if (this.activeTabIndex < this.selectedCategoryInfos.length - 1 && this.isCurrentTabValid()) {
    this.activeTabIndex++;
    this.setActiveInfoTab(this.activeTabCategoryInfo);
  }
}

previousTab() {
  if (this.activeTabIndex > 0) {
    this.activeTabIndex--;
    this.setActiveInfoTab(this.activeTabCategoryInfo);
  }
}
get hasAdditionalQualities(): boolean {
  return this.selectedQualities.value.includes('Medium') || this.selectedQualities.value.includes('Poor');
}

get visibleQualities(): string[] {
  return this.hasAdditionalQualities ? ['Good', ...this.selectedQualities.value] : ['Good'];
}
setActiveTabByIndex(i: number) {
  this.activeTabIndex = i;
  const tab = this.selectedCategoryInfos.value[i];
  this.setActiveInfoTab(tab);
}

isTabValid(categoryInfoIndex: number): boolean {
  const rows = this.allMatrixRows.filter(r => r.categoryInfoOption=== categoryInfoIndex);
  return rows.length > 0 && rows.every(this.isRateRowValid);
}
  generateMatrixForActiveTab() {
    this.matrixRows = [];
  
    const growthStages = this.selectedGrowthStages.value;
    const units = this.selectedUnits.value;
    const selectedCategoryInfos = this.selectedCategoryInfos.value;
    const districtRateId = this.plantRateForm.get('districtRateId')?.value;
    const infoCategories = this.currentPlant?.infoCategories ?? [];
    const qualities = ['Good', ...(this.selectedQualities.value as any[]).filter(q => q !== 'Good')];
  
    if (!this.activeCategoryInfo) return;
  
    const { categoryId, info, categoryInfoId, categoryInfoIndex } = this.activeCategoryInfo;

    const baseRows: PlantRateDto[] = [];
  
    for (let unit of units) {
      for (let growthStage of growthStages) {
        for (let quality of qualities) {
        /**/  const row: PlantRateDto = {
            districtRateId,
            growthStageId: growthStage,
            categoryId,
            categoryInfoOption:categoryInfoIndex,
            categoryInfoOptionName: info,
            categoryInfoId,
            unit,
            unitName: this.units.find(u => u.value == unit)?.key || '',
            quality,
            rate: undefined,
            assumptions: '',
            discretionInfo: '',
            noRate: false,
            showAssumptions: false
          };
    
          if (this.isGrouped) {
            row.groupedPlantIds = this.groupedPlantIds;
            row.groupName = this.groupName;
            row.groupedPlantId = this.groupId ?? undefined;
          } else {
            row.plantId = this.currentPlant?.id;
            row.plantType = this.currentPlant?.plantType ?? undefined;
          }
        // Check if we already had this row in allMatrixRows
        const existing = this.allMatrixRows.find(r =>
          r.categoryId === categoryId &&
          r.categoryInfoId === categoryInfoId &&
          r.categoryInfoOption === categoryInfoIndex &&
          r.growthStageId === growthStage &&
          r.unit === unit &&
          r.quality === quality
        );

        baseRows.push(existing ? { ...existing } : row);
        }
      }
    }
    // Replace only current tab's data in allMatrixRows
    this.allMatrixRows = [
      ...this.allMatrixRows.filter(r => r.categoryInfoOption !== categoryInfoIndex),
      ...baseRows
    ];
    this.matrixRows = baseRows;
    this.isConfigured = true;
   this.groupedMatrix = this.groupByUnit(baseRows);
  }
  
  generateMatrix() {
    if (this.selectedCategoryInfos.length > 0) {
      const first = this.selectedCategoryInfos.value[0];
      this.setActiveInfoTab(first);
    }else{
    this.matrixRows = [];
  
    const growthStages = this.selectedGrowthStages.value;
    const units = this.selectedUnits.value;
    const selectedCategoryInfos = this.selectedCategoryInfos.value;
    const districtRateId = this.plantRateForm.get('districtRateId')?.value;
    const infoCategories = this.currentPlant?.infoCategories ?? [];
    const qualities = ['Good', ...(this.selectedQualities.value as any[]).filter(q => q !== 'Good')];
  
    const baseRows: PlantRateDto[] = [];
  
    for (let unit of units) {
      for (let growthStage of growthStages) {
        for (let quality of qualities) {
        // 🟨 CASE 1: If plant has selected categories
        if (selectedCategoryInfos.length > 0) {
          for (let info of selectedCategoryInfos) {
              const row: PlantRateDto = {
                districtRateId,
                growthStageId: growthStage,
                categoryId: info.categoryId,
                categoryInfoOption: info.info as any,
                categoryInfoId: this.currentPlant?.infoCategories?.findIndex(c => c.name === info.info),
                unit,
                unitName: this.units.find(u => u.value == unit)?.key || '',
                rate: undefined,
                assumptions: '',
                discretionInfo: '',
                noRate: false,
                showAssumptions: false
              };
  
              if (this.isGrouped) {
                row.groupedPlantIds = this.groupedPlantIds;
                row.groupName = this.groupName;
                row.groupedPlantId = this.groupId ?? undefined;
              } else {
                row.plantId = this.currentPlant?.id;
                row.plantType = this.currentPlant?.plantType ?? undefined;
              }
  
              baseRows.push(row);
            }
          
  
        } else {
          // 🟩 CASE 2: No categories – just growth stage × unit
          const row: PlantRateDto = {
            districtRateId,
            growthStageId: growthStage,
            unit,
            unitName: this.units.find(u => u.value == unit)?.key || '',
            rate: undefined,
            quality,
            assumptions: '',
            discretionInfo: '',
            noRate: false,
            showAssumptions: false
          };
  
          if (this.isGrouped) {
            row.groupedPlantIds = this.groupedPlantIds;
            row.groupName = this.groupName;
            row.groupedPlantId = this.groupId ?? undefined;
          } else {
            row.plantId = this.currentPlant?.id;
            row.plantType = this.currentPlant?.plantType ?? undefined;
          }
  
          baseRows.push(row);
        }
      }
  
      }
    }
  
    this.matrixRows = baseRows;
    this.isConfigured = true;
    this.groupedMatrix = this.groupByUnit(baseRows);
    const groups = this.getCategoryInfoGroups();
/*if (groups.length > 0&& this.selectedCategoryInfos.length > 0) {
  // Set the first group as the active category info
  this.activeCategoryInfo = groups[0].infoOption;
}*/
  }
  }
  
  /*generateMatrix() {
    this.matrixRows = [];

  const growthStages = this.selectedGrowthStages.value;
  const units = this.selectedUnits.value;
  const categories = this.selectedCategories.value;

  const districtRateId = this.plantRateForm.get('districtRateId')?.value;

  const baseRows: PlantRateDto[] = [];

  for (let unit of units) {
    for (let growthStage of growthStages) {
      const base: PlantRateDto = {
        districtRateId: districtRateId,
        growthStageId: growthStage,
        categoryId: categories.length ? categories[0] : undefined,
        unit: unit,
        unitName: this.units.find(u => u.value == unit)?.key || '',
        rate: undefined,
        assumptions: '',
        discretionInfo: '',
        noRate: false,
        showAssumptions: false
      };

if (this.isGrouped) {
  base.groupedPlantIds = this.groupedPlantIds;
  base.groupName = this.groupName;
  base.groupedPlantId = this.groupId?? undefined;
} else {
  base.plantId = this.currentPlant?.id;
  base.plantType = this.currentPlant?.plantType ?? undefined;
}

      baseRows.push(base);
    }
  }

  this.matrixRows = baseRows;
  this.isConfigured = true;
  this.groupedMatrix = this.groupByUnit(baseRows);
   /*  this.matrixRows = [];
    const selectedGrowthStages = this.selectedGrowthStages.value;
    const selectedUnits = this.selectedUnits.value;
    const selectedCategories = this.selectedCategories.value;
    

    for (let growthStage of selectedGrowthStages) {
      for (let unit of selectedUnits) {
        this.matrixRows.push({
          districtRateId: this.plantRateForm.get('districtRateId')?.value,
          plantId: this.isGrouped ? undefined : this.currentPlant?.id,
          groupedPlantIds: this.isGrouped ? this.groupedPlantIds : undefined,
          groupName: this.isGrouped ? this.groupName : undefined,
          growthStageId: growthStage,
          categoryId: selectedCategories.length ? selectedCategories[0] : undefined,
          unit: unit,
          unitName:this.units.filter(u=>u.value==unit)[0].key as any,
          rate: undefined,
          assumptions: '',
          discretionInfo: ''
        });
    
      }
    }
    this.isConfigured = true;
    this.groupedMatrix = this.groupByUnit(this.matrixRows); */
  //}
  toggleDetails(index: number): void {
    const row = this.matrixRows[index];
    row.showDetails = !row.showDetails; // Toggle showDetails property
  }

  groupByUnit(rows: PlantRateDto[]) {
    const map = new Map<string, PlantRateDto[]>();
    for (const row of rows) {
      if (!map.has(row.unitName)) map.set(row.unitName, []);
      map.get(row.unitName)!.push(row);
    }
    return Array.from(map.entries()).map(([unit, rows]) => ({ unit, rows }));
  }
  getCategoryInfoGroups(): { infoOption: string, rows: PlantRateDto[] }[] {
    const groups: { [info: string]: PlantRateDto[] } = {};
  
    for (let row of this.matrixRows) {
      const key = row.categoryInfoOption || 'No Info';
      if (!groups[key]) groups[key] = [];
      groups[key].push(row);
    }
  
    return Object.entries(groups).map(([infoOption, rows]) => ({ infoOption, rows }));
  }

 

  handleNoRateToggle(i: number, event: any) {
    const row = this.matrixRows[i];
    row.noRate = event.target.checked;
    if (row.noRate) row.rate = undefined;
  }

  trackByIndex(index: number, _: any) {
    return index;
  }
  
  toggleUnitCollapse(unit: string) {
    this.collapsedUnits[unit] = !this.collapsedUnits[unit];
  }
  
  toggleAssumptions(i: number, event: any) {
    this.matrixRows[i].showAssumptions = event.target.checked;
  }
  qualities = ['Poor', 'Medium', 'Good'];

get selectedQualities(): FormArray {
  return this.plantRateForm.get('selectedQualities') as FormArray;
}

toggleQuality(quality: string) {
  const index = this.selectedQualities.value.indexOf(quality);

  if (index === -1) {
    this.selectedQualities.push(this.fb.control(quality));
  } else {
    const formArray = this.selectedQualities;
    const controlIndex = formArray.controls.findIndex(ctrl => ctrl.value === quality);
    if (controlIndex !== -1) formArray.removeAt(controlIndex);
  }
}

  saveGroup(unit: string) {
    const data = this.groupedMatrix.find(g => g.unit === unit)?.rows;
    // TODO: Replace with actual save logic
    console.log('Saving group:', unit, data);
    alert(`Saved ${unit} group`);
  }
  getColspan(): number {
    return 4;
  }
  getPlantNameById(id: number): string {
    const match = this.plants.find(p => p.id === id);
    return match?.commonName || match?.botanicalName || 'Unknown Plant';
  }
  getPlantTypeById(id: number): string {
    const plant = this.plants.find(p => p.id === id);
    return plant?.plantType || '';
  }
  SaveMatrix(){    
    console.log(this.matrixRows)
  /**/  this.api.matrixPOST({rates:(this.selectedCategoryInfos.length>0?this.allMatrixRows: this.matrixRows as any),districtRateId:this.currentDistrict?.id}).subscribe({
      next:()=>{
        this.save.emit(this.matrixRows);
//this.loadDistrictPlantRates();
      }
    })
  }
  ngOnChanges() {
    if (this.existingRates && this.existingRates.length > 0) {
      this.prefillMatrixFromExisting();
    }
    if (this.selectedCategories.length > 0) {
      this.activeCategoryId = this.selectedCategories.value[0];
    }
  }
  getActiveCategory(list:PlantRateDto[]){
   return this.activeCategoryId?list.filter(r => this.selectedCategories.length === 0 || r.categoryId === this.activeCategoryId):list
  }
  
  prefillMatrixFromExisting() {
    console.log(this.existingRates)
     // 🌱 1. Extract unique growthStages, units, qualities, and categoryInfo options
  const growthStageIds = Array.from(new Set(this.existingRates.map(r => r.growthStageId)));

  const unitValues = Array.from(new Set(
    this.existingRates
      .map(r => this.units.find(u => u.key === r.unit.toString())?.value)
      .filter(v => v !== undefined)
  ));

  const qualities = Array.from(new Set(
    this.existingRates.map(r => r.quality).filter(q => q !== null && q !== undefined)
  ));

  const categoryInfos = Array.from(new Set(
    this.existingRates
      .filter((r) => r.categoryInfoId != null && r.categoryInfoId != null)
      .map((r:any) => JSON.stringify({ categoryId: r.categoryId, info: r.category, categoryInfoId: r.categoryInfoId}))
  )).map(item => JSON.parse(item));

  // 🧠 2. Reset form arrays
  this.selectedGrowthStages.clear();
  this.selectedUnits.clear();
  this.selectedQualities.clear();
  this.selectedCategoryInfos.clear();

  // 🌿 3. Re-populate form controls
  growthStageIds.forEach(id => this.selectedGrowthStages.push(this.fb.control(id)));
  unitValues.forEach(val => this.selectedUnits.push(this.fb.control(val)));
  qualities.forEach(q => {
    if (q !== 'Good') this.selectedQualities.push(this.fb.control(q));
  });
  categoryInfos.forEach((ci,i)=>
    this.selectedCategoryInfos.push(this.fb.control({
      categoryId: ci.categoryId,
      info: ci.info,
      categoryInfoId: ci.categoryInfoId,
      categoryInfoIndex: i
    }))
  );

  // ✅ 4. Always include "Good" as baseline quality
  if (!this.selectedQualities.value.includes('Good')) {
    this.selectedQualities.insert(0, this.fb.control('Good'));
  }

  // 🏗️ 5. Build matrix rows
  this.allMatrixRows = this.existingRates.map((rate:any) => ({
    id: rate.id,
    districtRateId: rate.districtRateId,
    growthStageId: rate.growthStage,
    //categoryInfoOptionName: rate.categoryInfoOptionName,
    //categoryInfoIndex: rate.categoryInfoOption,
    unit: this.units.find(u => u.key === rate.unit.toString())?.value || 0,
    unitName: rate.unit.toString() || '',
    quality: rate.quality,
    rate: rate.rate,
    assumptions: rate.assumptions,
    discretionInfo: rate.discretionInfo,
    noRate: rate.rate == null,
    showAssumptions: !!rate.assumptions,

    // 🌿 Source type
    plantId: rate.plantId,
    plantType: rate.plantType,
    groupedPlantIds: rate.groupedPlantIds,
    groupName: rate.groupName,
    groupedPlantId: rate.groupedPlantId,
    categoryId: rate.categoryId,
    categoryInfoId: rate.categoryInfoId,
    categoryInfoIndex: rate.categoryInfoOption,
    categoryInfoOptionName: rate.category,/**/
  }));

  // ✅ 6. Set active tab to first available categoryInfo tab (if wizard)
  if (this.selectedCategoryInfos.length > 0) {
    this.activeTabIndex = 0;
    this.activeCategoryInfo = this.selectedCategoryInfos.value[0]
    this.matrixRows = [...this.allMatrixRows];
    this.groupedMatrix = this.groupByUnit(this.matrixRows);
    console.log(this.groupedMatrix)
    this.isConfigured = true;
    //this.setActiveInfoTab(this.selectedCategoryInfos.value[0]);
  } else {
    // If no wizard, load all into matrix directly
    this.matrixRows = [...this.allMatrixRows];
    this.groupedMatrix = this.groupByUnit(this.matrixRows);
    this.isConfigured = true;
  }
   /* if (!this.existingRates?.length) return;
  
    const first = this.existingRates[0];
      
    // 🌱 Deconstruct growth stages, units, categories
    const growthStageIds = Array.from(new Set(this.existingRates.map(r => r.growthStageId)));
    const unitValues = Array.from(new Set(this.existingRates.map(r => this.units.find(u => u.key === r.unit.toString())?.value ))); // Replace underscores with spaces
    console.log(unitValues)
    const categoryIds = Array.from(
      new Set(this.existingRates.filter(r => r.categoryId != null).map(r => r.categoryId))
    );
  
    // 🧠 Reset and patch form selections
    this.selectedGrowthStages.clear();
    this.selectedUnits.clear();
    this.selectedCategories.clear();
  
    growthStageIds.forEach(id => this.selectedGrowthStages.push(this.fb.control(id)));
    unitValues.forEach(val => this.selectedUnits.push(this.fb.control(val)));
    console.log(this.selectedUnits.value)
    categoryIds.forEach(id => this.selectedCategories.push(this.fb.control(id)));
  
    // 🏗️ Build matrixRows
    this.matrixRows = this.existingRates.map(rate => ({
      id: rate.id,
      districtRateId: rate.districtRateId,
      growthStageId: rate.growthStageId,
      categoryId: rate.categoryId,
      categoryInfoId: rate.categoryInfoId,
      categoryInfoOption: rate.categoryInfoOption,
      unit: this.units.find(u => u.key === rate.unit.toString())?.value||0 ,
      unitName: rate.unit.toString() || '',
      rate: rate.rate,
      assumptions: rate.assumptions,
      discretionInfo: rate.discretionInfo,
      noRate: rate.rate == null,
      showAssumptions: !!rate.assumptions,
  
      // 🌿 Source type
      plantId: rate.plantId,
      plantType: rate.plantType,
      groupedPlantIds: rate.groupedPlantIds,
      groupName: rate.groupName,
      groupedPlantId: rate.groupedPlantId
    }));
  
    // ✅ Final setup
    this.groupedMatrix = this.groupByUnit(this.matrixRows);
    this.isConfigured = true;*/
  }
  
  DeleteRate(id:number){
    if (confirm("Are you sure you want to delete this rate?")) {
    this.api.deletePlantRate(id).subscribe({});
    }
  }
}
