import { Component } from '@angular/core';
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
/**
 *
 */
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
}
