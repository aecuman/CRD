import { ChangeDetectorRef, Component } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { APIService, StructureCategoryViewModel, StructureViewDto } from '../api.service';

@Component({
  selector: 'app-structures',
  templateUrl: './structures.component.html',
  styleUrls: ['./structures.component.css']
})
export class StructuresComponent {
    // Pagination & Sorting
    currentPage = 1;
    pageSize = 5;
    totalRecords = 0;
    searchQuery = '';
    sortBy = '';
    sortDirection = 'asc';

    editingId: number | null = null;

  structureForm: FormGroup;
  categories: StructureCategoryViewModel[] = [];
  structures: StructureViewDto[] = [];
  groupedStructures: { categoryName: string; items: StructureViewDto[]; }[]=[]; 

  selectedCategory?: StructureCategoryViewModel|null=null;
  structureTypes=[
    {id:1,name:'Permanent'},
    {id:2, name:'Semi-Permanent'}
  ];
Math: any=Math
isModalOpen=false;

  constructor(
    private fb: FormBuilder,
    private api: APIService,
    private cdr: ChangeDetectorRef
  ) {
    this.structureForm = this.fb.group({
      id:[''],
      name: ['', [Validators.required, Validators.minLength(3)]],
      structureTypeId: [1, Validators.required],
      categoryId: [null, Validators.required],
      attributeSelections: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.loadCategories();
    this.loadStructures();
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

  loadCategories(): void {
    this.api.structureCategoriesAll().subscribe(data => {
      this.categories = data;
    });
  }

  loadStructures(): void {
    this.api.structuresAll().subscribe(data => {
      this.structures = data;
      this.groupStructuresByCategory();
    });
  }

  onCategoryChange(): void {
    this.structureForm.get('categoryId')?.setValue(this.selectedCategory?.id);
   
    this.resetAttributes();
  }

  get attributeSelections(): FormArray {
    return this.structureForm.get('attributeSelections') as FormArray;
  }

  resetAttributes(): void {
    this.attributeSelections.clear();
    this.selectedCategory?.attributes?.forEach(attr => {
      this.attributeSelections.push(this.fb.group({
        attributeId: [attr.id, Validators.required],
        optionIds: [[], Validators.required]
      }));
    });
  }

  submitForm(): void {
    if (this.structureForm.invalid) return;

    if (this.editingId === null) {
      this.api.structuresPOST({structure:this.structureForm.value}).subscribe(() => {
        this.loadStructures();
        this.structureForm.reset();
      });
    } else {
      this.api.structuresPUT(this.structureForm.value).subscribe(() => {
        this.loadStructures();
        this.structureForm.reset();
        this.editingId = null;
      });
    }
  }


  editStructure(structure: StructureViewDto): void {
    this.editingId = structure.id!;
    this.selectedCategory = this.categories.find(c => c.id === structure.category?.id)!;
    
    this.onCategoryChange();
    this.resetAttributes(); // should clear the FormArray properly
    
    this.structureForm.patchValue({
      id: structure.id,
      name: structure.name,
      structureTypeId: structure.structureType?.id,
      categoryId: structure.category?.id,
     // attributeSelections: [] // important to reset this
    });
    
    // Clear existing FormArray properly
    this.attributeSelections.clear();
    
    // Now re-populate
  /*  structure.attributeSelections?.forEach((s, i) => {
      const group = this.fb.group({
        attributeId: [s.attributeId, Validators.required],
        optionIds: [[], Validators.required]
      });
    
      this.attributeSelections.push(group);
    */
      const attributeSelections = this.fb.array(
        (structure.attributeSelections??[]).map(s => this.fb.group({
          attributeId: [s.attributeId, Validators.required],
          optionIds: [s.selectedOptions?.map(x => x.id) || [], Validators.required]
        }))
      );
      this.structureForm.setControl('attributeSelections', attributeSelections);
     console.log(this.attributeSelections.value);
    
   //  this.cdr.detectChanges();
    this.isModalOpen=true;
  }

  deleteStructure(id: number): void {
    if (confirm("Are you sure you want to delete this structure?")) {
      this.api.structuresDELETE(id).subscribe(() => {
        this.loadStructures();
      });
    }
  }

  changePage(page: number): void {
    this.currentPage = page;
    this.loadStructures();
  }

  sort(column: string): void {
    if (this.sortBy === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = column;
      this.sortDirection = 'asc';
    }
    this.loadStructures();
  }

  openModal(structure?:any){
this.isModalOpen=true;
  }
  closeModal(){
    this.selectedCategory=null;
    this.isModalOpen=false;
  }
}