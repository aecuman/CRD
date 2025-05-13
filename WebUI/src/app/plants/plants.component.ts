/*import { Component } from '@angular/core';
import { APIService, CategoryInfoDto, CreateCropCommand, OptionsListViewModel, OptionViewModel, PlantListViewModel, TranslationDto } from '../api.service';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { IDropdownSettings } from '../app.model';

@Component({
  selector: 'app-plants',
  templateUrl: './plants.component.html',
  styleUrls: ['./plants.component.css']
})
export class PlantsComponent {
  list?:PlantListViewModel[]=[];
  PlantForm?:FormGroup;
  message='';
  isSubmitted=false;
  options_list?: OptionsListViewModel;
  dropdownList:any[] = [];
  selectedItems:any[] = [];  
  dropdownSettings:IDropdownSettings={};
  growth_list: OptionViewModel[] | undefined;
  plant_category_list:any[]=[];
  zones=[
  {id:1,name:'Lake Albert Crescent'},
{id:2,name:'Western Highlands'},
{id:3,name:'Southern Highlands'},
{id:4,name:'Southern Drylands'},
{id:5,name:'Lake Victoria Crescent'},
{id:6,name:'South Eastern'},
{id:7,name:'Eastern'},
{id:8,name:'Karamoja drylands'},
{id:9,name:'Mid Northern'},
{id:10,name:'West Nile'}
  ]

constructor(private api:APIService, private fb:FormBuilder) {  
  this.PlantForm=this.initPlant();
  this.CategoryChanges()
  this.getAll();
 this.getAllOptions();
}
ngAfterContentChecked(){
  
  this.dropdownSettings = {
    singleSelection: false,
    idField: 'id',
    textField: 'text',
    selectAllText: 'Select All',
    unSelectAllText: 'UnSelect All',
    itemsShowLimit: 3,
    allowSearchFilter: false
  };
// this.dropdownList= this.options_list?.growthStages?.map(x=>{return{id:x.id,text:x.name}})
  //this.getAllOptions();
}
getAllOptions(){
  this.api.optionsGET().subscribe({
    next:(value:OptionsListViewModel)=>{
      this.options_list=value;
      this.growth_list=value.growthStages;
      this.options_list?.growthStages?.forEach(x=>{
        this.dropdownList.push({id:x.id,text:x.name})
      })
      value.plantCategories?.forEach(x=>{
        this.plant_category_list.push({id:x.id,text:x.name})
      })
    },
  })
}

initPlant(){
  return this.fb.group({
    plantType:[''],
    aez:[''],
    botanicalName:[''],
    categories:[],
    cats:[],
    commonName:[''],
    cropType:[''],
    growthStages:[],
    stages:[''],
    info:[''],
    othernames:[],
    translations:this.fb.array([this.InitTranslation()]),
    categoryInfos:this.fb.array([])
  })
}
CategoryChanges(){
  this.PlantForm?.get('cats')?.valueChanges.subscribe((x:{id:number,text:string}[])=>{
    x.forEach(cx=>{
      this.AddCatInfo(cx.id,cx.text);
    })
  })
}
getAll(){
  this.api.plantsAll().subscribe({
    next:(value:PlantListViewModel[])=> {
      this.list=value;
    },
  })
}
CreatePlant(){
  let command:CreateCropCommand={
    aez:'',
    botanicalName:'',
    categories:[],
    commonName:'',
    cropType:'',
    growthStages:[],
    //images:[],
    info:'',
    othernames:[],
    plantType:'',
    translations:[]
  }
}
InitCatInfo(id:number,name:string){
  let c : CategoryInfoDto={
    categoryId:id,
    categoryInfo:[]
  }
  return this.fb.group({
    categoryId:[id],
    categoryInfo:[[],Validators.required],
    name:[name],
    info:['']
  })
}
get CategoryInfo():FormArray{
  return <FormArray>this.PlantForm?.get('categoryInfos')
}
AddCatInfo(id:number,name:string){
this.CategoryInfo.push(this.InitCatInfo(id,name))
}
AddOption(t:AbstractControl){
let val:any[] = t.get('categoryInfo')?.value;
val.push(t.get('info')?.value);
t.get('categoryInfo')?.setValue(val);
t.get('info')?.setValue('')
}
InitTranslation(){
  var l:TranslationDto={
    languangeId:1,
    translated:''
  }
  return this.fb.group({
    languangeId:[null,Validators.required],
    translated:['',Validators.required]
  })
}
get Translations():FormArray{
  return <FormArray>this.PlantForm?.get('translations')
}
RemoveLanguange(i:number){
this.Translations.removeAt(i)
}
AddLanguange(){
  this.Translations.push(this.InitTranslation());
}
SavePlant(){
  let body:CreateCropCommand=this.PlantForm?.value;
  let tDTO:{id:number,text:string}[]= this.PlantForm?.get('stages')?.value;
  body.growthStages=tDTO.map(x=>{return x.id})
  let catDTO:{id:number,text:string}[]= this.PlantForm?.get('cats')?.value;
  body.categories=catDTO.map(x=>{return x.id})
 // body.growthStages=this.PlantForm?.get()
  this.api.plantsPOST(body).subscribe({
    next:()=>{
      this.PlantForm=this.initPlant();
      this.getAll();
    }
  })
}

onItemSelect(item: any) {
console.log(item);
}
onSelectAll(items: any) {
console.log(items);
}
}*/

import { Component } from '@angular/core';
import { 
  APIService, CategoryInfoDto, CategoryViewModel, CreateCropCommand, 
  GroupedPlantListViewModel, 
  GrowthStageViewModel, 
  OptionsListViewModel, OptionViewModel, PlantListViewModel, 
  TranslationDto, 
  UpdateGroupedPlantsCommand
} from '../api.service';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { IDropdownSettings } from '../app.model';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-plants',
  templateUrl: './plants.component.html',
  styleUrls: ['./plants.component.css']
})
export class PlantsComponent {
  list: PlantListViewModel[] = [];
  PlantForm: FormGroup;
  message = '';
  isSubmitted = false;
  isModalOpen = false;
  editMode = false;
  selectedTab = 0;

  options_list?: OptionsListViewModel;
  dropdownList: any[] = [];
  selectedItems: any[] = [];
  dropdownSettings: IDropdownSettings = {};
  growth_list: OptionViewModel[] | undefined;
  plant_category_list: any[] = [];

  pageSize: number = 5;
currentPage: number = 1;

  tabs = ["Basic Details", "Categories & Options", "Growth Stages", "Translations"];


  zones = [
    { id: 1, name: 'Lake Albert Crescent' },
    { id: 2, name: 'Western Highlands' },
    { id: 3, name: 'Southern Highlands' },
    { id: 4, name: 'Southern Drylands' },
    { id: 5, name: 'Lake Victoria Crescent' },
    { id: 6, name: 'South Eastern' },
    { id: 7, name: 'Eastern' },
    { id: 8, name: 'Karamoja Drylands' },
    { id: 9, name: 'Mid Northern' },
    { id: 10, name: 'West Nile' }
  ];
  selectedPlant?: PlantListViewModel|undefined;

  groupedPlantList: GroupedPlantListViewModel[] = [];

  searchPlantText = '';
  isGroupModalOpen = false;
  selectedPlants: PlantListViewModel[] = [];
  selectedGroup: GroupedPlantListViewModel | null = null;
  groupForm = { name: '', description: '', growthStages:new Array<any>() };
  activeTab='plants';

  constructor(private api: APIService, private fb: FormBuilder,private auth: AuthService) {  
    this.PlantForm = this.initPlant();
    this.CategoryChanges();
    this.getAll();
    this.refreshGroups();
    this.getAllOptions();
    this.translationsForm = this.fb.group({
      translations: this.fb.array([])
    });

    this.newTranslationForm = this.fb.group({
      languangeId: ['', Validators.required],
      translated: ['', Validators.required]
    });
  }

  ngAfterContentChecked() {
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'text',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 3,
      allowSearchFilter: false
    };
  }

  getAllOptions() {
    this.api.optionsGET().subscribe({
      next: (value: OptionsListViewModel) => {
        this.options_list = value;
        this.growth_list = value.growthStages;
        this.dropdownList = value.growthStages?.map(x => ({ id: x.id, text: x.name })) || [];
        this.plant_category_list = value.plantCategories?.map(x => ({ id: x.id, text: x.name })) || [];
      }
    });
  }

  initPlant() {
    return this.fb.group({
      id:[''],
      plantType: ['', Validators.required],
      aez: ['n/A'],
      botanicalName: ['', Validators.required],
      categories: [[]],
      cats: [[]],
      commonName: ['', Validators.required],
      cropType: [''],
      growthStages: [[]],
      stages: [[]],
      info: ['N/A'],
      othernames: this.fb.array([]),
      translations: this.fb.array([this.initTranslation()]),
      categoryInfos: this.fb.array([])
    });
  }

  CategoryChanges() {
    this.PlantForm.get('cats')?.valueChanges.subscribe((selectedCategories: { id: number, text: string }[]) => {
     // this.CategoryInfo.clear();
      console.log(selectedCategories);
      selectedCategories.forEach((cat, index) => {
        if (this.CategoryInfo.at(index)) {
          this.CategoryInfo.at(index).patchValue({ name: cat.text });
        } else {
          this.addCatInfo(cat.id, cat.text);
        }
      });
    });
  }

  getAll() {
    this.api.plantsAll().subscribe({
      next: (value: PlantListViewModel[]) => {
        this.list = value;
      }
    });
  }
  refreshGroups() {
    this.api.getGroupedPlants().subscribe(data => {
      this.groupedPlantList = data;
    });
  }
  searchText: string = '';

  filteredList() {
    if (!this.searchText) {
      return this.list;
    }
    
    const lowerSearch = this.searchText.toLowerCase();
    
    return this.list.filter(p =>
      (p.commonName?.toLowerCase().includes(lowerSearch) || '') ||
      p.botanicalName?.toLowerCase().includes(lowerSearch) || 
      p.cropType?.toLowerCase().includes(lowerSearch) || 
      p.aez?.toLowerCase().includes(lowerSearch) || 
      p.translations?.some(t => t.translated?.toLowerCase().includes(lowerSearch)) || false
    );
  }
  createPlant() {
    let command: CreateCropCommand = this.PlantForm.value;

    // Extract selected categories
    command.categories = this.PlantForm.get('cats')?.value.map((cat: { id: number }) => cat.id) || [];

    // Extract growth stages
    command.growthStages = this.PlantForm.get('stages')?.value.map((stage: { id: number }) => stage.id) || [];

    this.api.plantsPOST(command).subscribe({
      next: () => {
        this.closeModal();
        this.getAll();
      }
    });
  }

  initCatInfo(id: number, name: string) {
    return this.fb.group({
      categoryId: [id],
      categoryInfo: [[], Validators.required],
      name: [name],
      info: ['']
    });
  }

  get CategoryInfo(): FormArray {
    return this.PlantForm.get('categoryInfos') as FormArray;
  }

  addCatInfo(id: number, name: string) {
    this.CategoryInfo.push(this.initCatInfo(id, name));
  }

  addOption(category: AbstractControl) {
    let val: any[] = category.get('categoryInfo')?.value;
    val.push(category.get('info')?.value);
    category.get('categoryInfo')?.setValue(val);
    category.get('info')?.setValue('');
  }

  initTranslation() {
    return this.fb.group({
      languangeName: [''],
      languangeId: [null, Validators.required],
      Id:[''],
      translated: ['', Validators.required]
    });
  }

  get Translations(): FormArray {
    return this.PlantForm.get('translations') as FormArray;
  }

/*   removeTranslation(index: number) {
    this.Translations.removeAt(index);
  }

  addTranslation() {
    this.Translations.push(this.initTranslation());
  } */

  savePlant() {
    let body: CreateCropCommand = this.PlantForm.value;

    // Extract IDs for dropdown selections
    body.growthStages = this.PlantForm.get('stages')?.value.map((x: { id: number }) => x.id) || [];
    body.categories = this.PlantForm.get('cats')?.value.map((x: { id: number }) => x.id) || [];

    if (this.editMode) {
      this.api.plantsPUT(body).subscribe({
        next: () => {
          this.closeModal();
          this.getAll();
        }
      });
    } else {
      this.api.plantsPOST(body).subscribe({
        next: () => {
          this.closeModal();
          this.getAll();
        }
      });
    }
  }

/*   // Modal & Tab Functions
  openModal() {
    this.isModalOpen = true;
    this.selectedTab = 0;
    this.PlantForm.reset();
  } */

  closeModal() {
    this.isModalOpen = false;
    this.selectedPlant = null as any;
    this.Translations.clear();
  }

  prevTab() {
    if (this.selectedTab > 0) {
      this.selectedTab--;
    }
  }

  nextTab() {
    if (this.selectedTab < this.tabs.length - 1) {
      this.selectedTab++;
    }
  }

  onItemSelect(item: any) {
    console.log(item);
  }
  onItemGroupSelect(event:any){
    console.log(event);
  }

  onSelectAll(items: any) {
    console.log(items);
  }
  // Pagination functions
paginatedList() {
  const filtered = this.filteredList();
  const totalPages = this.totalPages();
  
  // Ensure currentPage is within bounds
  if (this.currentPage > totalPages) {
    this.currentPage = 1; // Reset to page 1 if out of range
  }

  const startIndex = (this.currentPage - 1) * this.pageSize;
  return filtered.slice(startIndex, startIndex + this.pageSize);
}

totalPages() {
  return Math.max(1, Math.ceil(this.filteredList().length / this.pageSize));
}

// Handle next page click
nextPage() {
  if (this.currentPage < this.totalPages()) {
    this.currentPage++;
  }
}

// Handle previous page click
prevPage() {
  if (this.currentPage > 1) {
    this.currentPage--;
  }
}

  // Open modal for adding a new plant
  openModal() {
    this.isModalOpen = true;
    this.editMode = false;
    this.selectedTab = 0;
    this.PlantForm.reset();
  }

  editPlant(plant: PlantListViewModel) {
    this.isModalOpen = true;
    this.editMode = true;
    this.selectedTab = 0;
  
    // Reset the form before patching new values
    this.PlantForm.reset();
  
    // Patch existing plant data into the form
    this.PlantForm.patchValue({
      id: plant.id,
      plantType: plant.plantType,
      commonName: plant.commonName,
      botanicalName: plant.botanicalName,
      cropType: plant.cropType,
      aez: plant.aez,
      info: plant.info || 'N/A'
    });
  
    // Pre-fill Growth Stages (Convert IDs to Selected Options)
    const selectedGrowthStages = this.dropdownList.filter((stage: { id: number; text: any }) =>
      plant?.growthStages?.some((growthStage: GrowthStageViewModel) => growthStage.growthStageId === stage.id)
    );
    
    // Set the selected IDs in the form
    this.PlantForm.get('stages')?.setValue(selectedGrowthStages.map((stage) => {return {id:stage.id,text:stage.text}}));
    
    
  
    // Pre-fill Translations (Clear existing then add new)
    this.Translations.clear();
    plant.translations?.forEach(translation => {
      this.Translations.push(this.fb.group({
        Id: [translation.translationId],
        languangeName: [translation.languangeName],
        languangeId: [translation.languangeId, Validators.required],
        translated: [translation.translated, Validators.required]
      }));
    });
  
    // Pre-fill Categories
    const selectedCategories = this.plant_category_list.filter((category:{id:number,name:any}) => 
      plant.categories?.some((cat:CategoryViewModel)=> cat.id===category.id)
    );
    this.PlantForm.get('cats')?.setValue(selectedCategories.map((cat)=> {return {id: cat.id,text:cat.text}}));
   
    // Pre-fill Category Info (Clear and then add)
    this.CategoryInfo.clear();
    plant.infoCategories?.forEach(cat => {
      const categoryGroup = this.fb.group({
        categoryId: [cat.categoryId],
        categoryInfo: [cat?.info || [], Validators.required],
        name: [cat.name],
        info: ['']
      });
      this.CategoryInfo.push(categoryGroup);
    });
  }
  
  viewDetails(plant: PlantListViewModel) {
    this.selectedPlant = plant;
  }


  ///

  translationsForm: FormGroup;
  addingNewTranslation: boolean = false;
  editingRow: number | null = null; // Tracks the row being edited
  newTranslationForm: FormGroup;

  addTranslation(): void {
    this.addingNewTranslation = true;
    this.newTranslationForm?.reset();
  }

  saveNewTranslation(): void {
    if (this.newTranslationForm?.valid) {
      const selectedLang = this.options_list?.languanges?.find(
        (lang) => lang.id === this.newTranslationForm?.value.languangeId
      );

      this.Translations.push(
        this.fb.group({
          languangeId: [this.newTranslationForm.value.languangeId],
          Id: [null],
          languangeName: [selectedLang?.name || ''],
          translated: [this.newTranslationForm.value.translated]
        })
      );
      this.addingNewTranslation = false;
    }
  }

  cancelNewTranslation(): void {
    this.addingNewTranslation = false;
  }

  editTranslation(index: number): void {
    this.editingRow = index;
  }

  saveTranslation(index: number): void {
    const selectedLang = this.options_list?.languanges?.find(
      (lang) => lang.id === this.Translations.at(index).value.languangeId
    );
    this.Translations.at(index).patchValue({
      languangeName: selectedLang?.name || ''
    });
    this.editingRow = null;
  }

  cancelEdit(): void {
    this.editingRow = null;
  }

  removeTranslation(index: number): void {
    this.Translations.removeAt(index);
  }

  /**Grouped Plants */

  isInGroup(p:any){
return this.groupedPlantList.some(g => (g.plants||[]).some(pl => pl.id === p.id && pl.plantType === p.plantType))
  }
  openGroupModal() {
    this.isGroupModalOpen = true;
    this.selectedGroup = null;
    this.selectedPlants = [];
    this.groupForm = { name: '', description: '',growthStages:[] };
  }

  closeGroupModal() {
    this.isGroupModalOpen = false;
  }
 getGrowthStage(id:number){
  return this.dropdownList.find(x => x.id === id)
 }
  editGroup(group: GroupedPlantListViewModel) {
    this.selectedGroup = group;
    this.isGroupModalOpen = true;
    let selectedGrowthStages:any[] = []
group.growthStages?.forEach(gs=>{
 selectedGrowthStages.push(this.dropdownList.find(x => x.id === gs))
})
    
    this.groupForm = { name: group.name || '', description: group.description || '',growthStages:selectedGrowthStages };
    this.selectedPlants = group.plants ? [...group.plants] : [];
  }

  isSelected(plant: PlantListViewModel): boolean {
    return this.selectedPlants.some(p => p.id === plant.id && p.plantType === plant.plantType);
  }

  addToGroup(plant: PlantListViewModel) {
    if (!this.isSelected(plant)) {
      this.selectedPlants.push(plant);
    }
  }

  removeFromGroup(plant: PlantListViewModel) {
    this.selectedPlants = this.selectedPlants.filter(p => p.id !== plant.id || p.plantType !== plant.plantType);
  }

  filteredPlants(): PlantListViewModel[] {
    const text = this.searchPlantText.toLowerCase();
    return this.list.filter(p =>
      (p.commonName ?? '').toLowerCase().includes(text) ||
      (p.botanicalName??'').toLowerCase().includes(text)
    );
  }

  submitGroupedPlant() {
    const cropIds = this.selectedPlants.filter(p => p.plantType === 'crop').map(p => p.id);
    const treeIds = this.selectedPlants.filter(p => p.plantType === 'tree').map(p => p.id);

    if (this.selectedGroup) {
      const command:UpdateGroupedPlantsCommand = {
        id: this.selectedGroup.id,
        name: this.groupForm.name,
        description: this.groupForm.description,
        growthStages: this.groupForm.growthStages.map(x=>x.id),
        cropIds: cropIds as number[] ?? [],
        treeIds: treeIds as number[]??[]
      };

      this.api.updateGroupedPlant(this.selectedGroup.id??0, command as any).subscribe(() => {
        alert('Group updated!');
        this.closeGroupModal();
        this.refreshGroups();
      });
    } else {
      const command = {
        name: this.groupForm.name,
        description: this.groupForm.description,
        growthStages: this.groupForm.growthStages.map(x=>x.id),
        cropIds,
        treeIds
      };

      this.api.createGroupedPlant(command as any).subscribe(() => {
        alert('Group created!');
        this.closeGroupModal();
        this.refreshGroups();
      });
    }
  }

  deleteGroupedPlant() {
    if (!this.selectedGroup) return;

    if (confirm(`Are you sure you want to delete the group "${this.selectedGroup.name}"?`)) {
      if(this.selectedGroup.id){
             this.api.deleteGroupedPlant(this.selectedGroup.id).subscribe(() => {
        alert('Group deleted');
        this.closeGroupModal();
        this.refreshGroups();
      });
      }
 
    }
  }
  get canManage(){
    return this.auth.userValue?.roles?.includes('Admin') || this.auth.userValue?.roles?.includes('superadmin');
  }

/*     savePlant() {
    let body: CreateCropCommand = this.PlantForm.value;

    // Extract IDs for dropdown selections
    body.growthStages = this.PlantForm.get('stages')?.value.map((x: { id: number }) => x.id) || [];
    body.categories = this.PlantForm.get('cats')?.value.map((x: { id: number }) => x.id) || [];

    if (this.editMode) {
      this.api.plantsPOST(body).subscribe({
        next: () => {
          this.closeModal();
          this.getAll();
        }
      });
    } else {
      this.api.plantsPOST(body).subscribe({
        next: () => {
          this.closeModal();
          this.getAll();
        }
      });
    }
  }

  initCatInfo(id: number, name: string) {
    return this.fb.group({
      categoryId: [id],
      categoryInfo: [[], Validators.required],
      name: [name],
      info: ['']
    });
  }

get CategoryInfo(): FormArray {
    return this.PlantForm.get('categoryInfos') as FormArray;
  }

  addCatInfo(id: number, name: string) {
    this.CategoryInfo.push(this.initCatInfo(id, name));
  }

  initTranslation() {
    return this.fb.group({
      languangeId: [null, Validators.required],
      translated: ['', Validators.required]
    });
  }

  get Translations(): FormArray {
    return this.PlantForm.get('translations') as FormArray;
  }

  removeTranslation(index: number) {
    this.Translations.removeAt(index);
  }

  addTranslation() {
    this.Translations.push(this.initTranslation());
  } */
}

