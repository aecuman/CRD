import { Component } from '@angular/core';
import { APIService, CreateOptionCommand, DeleteOptionCommand, OptionsListViewModel,OptionViewModel,/*, StructureCategoryListViewModel, StructureDescriptionNameListViewModel, StructureDescriptionOptionListViewModel*/ 
StructureCategoryViewModel} from '../api.service';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { StructureCategory } from '../app.model';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent {

  option_language:any;
  option_growth_stage:any;
  option_crop_category:any;
  option_structure_type:any;
  lists?:OptionsListViewModel;
  option_category_form_description:any;
  stop_edit=false;
  option_category_form_description_edit:any;
  option_category_form_description_option_edit:any;
  StructureCategoryForm?:FormGroup;
  StructureDescriptionOptionForm?:FormGroup;
  StructureDescriptionForm?:FormGroup;
  struct_editor:any[][]=[[]];
  struct_desc_editor:any[][]=[[]];
  expandedRows: { [key: number]: boolean } = {}; // Track expanded rows
  /**
   *
   */
  constructor(private api:APIService,private fb:FormBuilder) {
    
    this.getAll();
    this.AddNewStructureCategory();

    this.loadStructureCategories();

    ///Structures
    this.categoryForm = this.fb.group({
      id:[null],
      name: ['', [Validators.required, Validators.minLength(3)]],
      attributes: this.fb.array([])
    });

  }
  ngAfterViewInit(){
    this.api.optionsGET().subscribe({
      next:(value:OptionsListViewModel)=>{
      
        /* value.structureCategoryOptions?.forEach(x=>{
         // this.getTableSpans(x)
        }) */
      },
    })
  }
  getAll(){
    this.api.optionsGET().subscribe({
      next:(value:OptionsListViewModel)=>{
        this.lists=value;
/*         this.struct_editor= Object.assign([],value.structureCategoryOptions);
        this.struct_desc_editor = [... this.struct_editor]//Object.assign([],this.struct_editor); */
      },
    })
  }
  AddOption(name:any,opt:any){
    let command :CreateOptionCommand={name:name,option:opt}
    this.api.optionsPOST(command).subscribe({
      next:()=>{
        this.getAll();
        this.ResetForms();
      }
    })
  }
/*   getTableSpans(data:StructureCategoryListViewModel){
    const tbody = document.querySelector("#tree-table tbody");
console.log(data)
            let level1RowSpan = data?.structureCategories?.reduce((total, child) => total + (<StructureDescriptionOptionListViewModel[]>child.options).length, 0);

            let level1Cell = `<td class="p-2.5 border border-black" rowspan="${level1RowSpan}">${data.name}</td>`;
            
            data.structureCategories?.forEach((child, childIndex) => {
                let level2RowSpan = (<StructureDescriptionOptionListViewModel[]>child.options).length;

                child.options?.forEach((leaf, leafIndex) => {
                    let row = document.createElement("tr");

                    if (childIndex === 0 && leafIndex === 0) {
                        row.innerHTML += level1Cell;
                    }

                    if (leafIndex === 0) {
                        row.innerHTML += `<td class="p-2.5 border border-black" rowspan="${level2RowSpan}">${child.name}</td>`;
                    }

                    row.innerHTML += `<td class="p-2.5 border border-black">${leaf.name}</td>`;
                    tbody?.appendChild(row);
                });
            });
  }
  GetL1RS(data:StructureCategoryListViewModel){
return data?.structureCategories?.reduce((total, child) => total + (<StructureDescriptionOptionListViewModel[]>child.options).length, 0);
  }
  GetLevel2RowSpan(child: StructureDescriptionNameListViewModel): number {
    return (<StructureDescriptionOptionListViewModel[]>child.options).length;
  } */
  ResetForms(){
    this.option_language=null;
    this.option_growth_stage=null;
    this.option_crop_category=null;
    this.option_structure_type=null;
    this.option_category_form_description=null;
    this.StructureDescriptionOptionForm=null as any;
    this.StructureDescriptionForm=null as any;
    this.option_category_form_description_edit=null;
  }
  AddNewStructureCategory(){
    this.StructureCategoryForm=this.InitStructureCategories();
  }
  InitStructureCategories(){
    var cmd:CreateOptionCommand={
      name:'building',
      option:'structure_category',
      options:[
        {name:'roof',options:['grass thatched','galvanised corrugated ironsheets']},

      ],
      structureDescriptionNameId:0
    };
    return this.fb.group({
      name:['',Validators.required],
      option:['structure_category'],
      options:this.fb.array([this.InitStructureDescriptionOptionCategory()])
    })
/*     */
  }
  SaveCategory(){
let cmd:CreateOptionCommand=this.StructureCategoryForm?.value;
     this.api.optionsPOST(cmd).subscribe({
      next:()=>{
        this.getAll();
        this.ResetForms();
      }
    })
  }
 get StructureCategoryFormDescription():FormArray{
return <FormArray> this.StructureCategoryForm?.get('options')
  }
 StructureCategoryFormDescriptionOptions(pos:number):FormArray{
    return <FormArray> this.StructureCategoryFormDescription.at(pos)?.get('options')
      }
  InitStructureDescriptionOptionCategory(){
return  this.fb.group({
  name:['',Validators.required],
  options:[[]]
})
  }
  AddNewDescription(){
    this.StructureCategoryFormDescription.push(this.InitStructureDescriptionOptionCategory())
  }
  PushOption(pos:number, value:any){
let v:any[] =this.StructureCategoryFormDescriptionOptions(pos).value;
v.push(value);
this.StructureCategoryFormDescriptionOptions(pos).setValue(v);
this.ResetForms();
  }
  InitStructureDescriptionOption(structuredescriptionid:any,name:string){
    this.StructureDescriptionOptionForm =this.fb.group({
      structureDescriptionNameId:[structuredescriptionid],
      name:[name],
      option:['structure_description_option']
    })
  }
  InitStructureDescription(categoryid:any,name:string){
    this.StructureDescriptionForm= this.fb.group({
      categoryId:[categoryid],
      name:[name],
      option:['structure_description_name']
    })
  }
  SaveOption(id:any,name:string){
this.StructureDescriptionOptionForm?.patchValue({structureDescriptionNameId:id,name:name})
let command :CreateOptionCommand=this.StructureDescriptionOptionForm?.value
this.api.optionsPOST(command).subscribe({
  next:()=>{
    this.getAll();
    this.ResetForms();
  }
})
  }
  SaveDescription(id:any,name:string){
    this.StructureDescriptionForm?.patchValue({categoryId:id,name:name})
    let command :CreateOptionCommand=this.StructureDescriptionForm?.value
    this.api.optionsPOST(command).subscribe({
      next:()=>{
        this.getAll();
        this.ResetForms();
      }
    })
      }
  ShowAlert(text:any){
    alert(text)
  }

  //// Structures
  categoryForm: FormGroup;
  categories: StructureCategoryViewModel[] = [];
  editingIndex: number | null = null;
  submitted = false;

  loadStructureCategories(): void {
    this.api.structureCategoriesAll().subscribe(data => {
      this.categories = data;
    });
  }
  get attributes(): FormArray {
    return this.categoryForm.get('attributes') as FormArray;
  }

  addAttribute(): void {
    this.attributes.push(this.fb.group({
      id:[null],
      name: ['', [Validators.required, Validators.minLength(3)]],
      options: this.fb.array([this.fb.group({name:['', [Validators.required]]})])
    }));
  }

  removeAttribute(index: number): void {
    this.attributes.removeAt(index);
  }

  getOptions(attributeIndex: number): FormArray {
    return this.attributes.at(attributeIndex).get('options') as FormArray;
  }

  addOption(attributeIndex: number): void {
    this.getOptions(attributeIndex).push(this.fb.group({name:['', [Validators.required]]}));
  }

  removeOption(attributeIndex: number, optionIndex: number): void {
    this.getOptions(attributeIndex).removeAt(optionIndex);
  }

  toggleExpandRow(index: number): void {
    this.expandedRows[index] = !this.expandedRows[index];
  }

  submitForm(): void {
    this.submitted = true;
    if (this.categoryForm.invalid) return;

    if (this.editingIndex === null) {
      this.api.structureCategoriesPOST(this.categoryForm.value).subscribe(() => {
        this.loadStructureCategories();
        this.resetForm();
      });
    } else {
      const updatedCategory = { ...this.categoryForm.value, id: this.categories[this.editingIndex].id };
      this.api.structureCategoriesPUT(updatedCategory).subscribe(() => {
        this.loadStructureCategories();
        this.resetForm();
        this.editingIndex = null;
      });
    }
  }

  editCategory(index: number): void {
    this.editingIndex = index;
    const category = this.categories[index];
    this.categoryForm.patchValue(category);
    this.attributes.clear();
    //if(category.attributes)
    category.attributes?.forEach(attr => {
      const attributeForm = this.fb.group({
        id: [attr.id],
        name: [attr.name, [Validators.required, Validators.minLength(3)]],
        options: attr.options? this.fb.array(attr.options.map(opt => this.fb.group({id:[opt?.id],name:[opt.name, [Validators.required]]}))):[this.fb.array([]),[Validators.required]]
      });
      this.attributes.push(attributeForm);
    });
  }

  deleteCategory(id: number): void {
    this.api.structureCategoriesDELETE(id).subscribe(() => {
      this.loadStructureCategories();
    });
  }

  deleteGrowthStageOption(option: OptionViewModel): void {
    if (confirm(`Are you sure you want to delete the Growth Stage "${option.name}"?`)) {
    let cmd: DeleteOptionCommand = { id: option.id, option: 'plant_growthstage' };
this.api.deleteSettingOption(cmd).subscribe({
      next: () => {
        this.api.optionsGET().subscribe({
          next: (value: OptionsListViewModel) => {
            this.lists = value;
          },
        });
      }
    });
  }
  }
  resetForm(): void {
    this.submitted = false;
    this.categoryForm.reset({ id:null,name: '' });
    this.attributes.clear();
  }

}
