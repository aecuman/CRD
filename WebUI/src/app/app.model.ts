

export interface LoginResponse {
    succeed: boolean
    message: string
    token: string
    user: CurrentUser
  }
  
  export interface CurrentUser {
    id: number
    fullName: string
    email: string
    roles: string[]
  }
  export interface IDropdownSettings {
    singleSelection?: boolean;
    idField?: string;
    textField?: string;
    disabledField?: string;
    enableCheckAll?: boolean;
    selectAllText?: string;
    unSelectAllText?: string;
    allowSearchFilter?: boolean;
    clearSearchFilter?: boolean;
    maxHeight?: number;
    itemsShowLimit?: number;
    limitSelection?: number;
    searchPlaceholderText?: string;
    noDataAvailablePlaceholderText?: string;
    noFilteredDataAvailablePlaceholderText?: string;
    closeDropDownOnSelection?: boolean;
    showSelectedItemsAtTop?: boolean;
    defaultOpen?: boolean;
    allowRemoteDataSearch?: boolean;
  }
  
  export class ListItem {
    id!: string | number;
    text!: string | number 
    isDisabled?: boolean;
  
    public constructor(source: any) {
      if (typeof source === 'string' || typeof source === 'number') {
        this.id = this.text = source;
        this.isDisabled = false;
      }
      if (typeof source === 'object') {
        this.id = source.id;
        this.text = source.text;
        this.isDisabled = source.isDisabled;
      }
    }
  }
  export interface StructureOption {
    id?: number;
    name: string;
  }
  
  export interface StructureAttribute {
    id?: number;
    name: string;
    options: StructureOption[];
  }
  
  export interface StructureCategory {
    id?: number;
    name: string;
    attributes: StructureAttribute[];
  }
  export class PlantRateDto {
    id?:number;
    districtRateId: number;
    plantId?: number;
    plantType?:string;
    groupedPlantIds?: number[];
    groupName?: string;
    groupedPlantId?: number;
    growthStageId: number;
    categoryId?: number;
    unit: UnitOfMeasure;
    unitName:string;
    rate?: number;
    assumptions?: string;
    discretionInfo?: string;
    showDetails?: boolean;
    noRate?: boolean;
    showAssumptions?: boolean;
    categoryInfoId?: number;
    categoryInfoOption?: number;
    categoryInfoOptionName?: string;
    quality?:string
  
    constructor(
      districtRateId: number,
      growthStageId: number,
      unit: UnitOfMeasure,
      plantId?: number,
      groupedPlantIds?: number[],
      groupName?: string,
      groupedPlantId?: number,
      plantType?:string,
      categoryId?: number,
      rate?: number,
      assumptions?: string,
      discretionInfo?: string,
      id?:number,
      categoryInfoId?: number,
      categoryInfoOptionName?: string,
    categoryInfoOption?: number,

    quality?:string
    ) {
      this.districtRateId = districtRateId;
      this.growthStageId = growthStageId;
      this.unit = unit;
      this.plantId = plantId;
      this.groupedPlantIds = groupedPlantIds;
      this.groupName = groupName;
      this.categoryId = categoryId;
      this.rate = rate;
      this.assumptions = assumptions;
      this.discretionInfo = discretionInfo;
      this.showDetails = false;
      this.noRate = false;
      this.showAssumptions = false;
      this.unitName=unit?Object.values(unit).find(([key, val]) => val === this.unit)?.[0]:'' as any;
      this.plantType=plantType;
      this.groupedPlantId=groupedPlantId;
      this.id=id;
      this.categoryInfoId =   categoryInfoId;
      this.categoryInfoOption = categoryInfoOption;
      this.categoryInfoOptionName = categoryInfoOptionName;
      this.quality = quality;

    }
  }
  
  export enum UnitOfMeasure {
    'Per Square Metre' = 0,
    'Per Tree' = 1,
    'Per Plant' = 2,
    'Per Acre' = 3,
    'Per Clump' = 4
  }
  export enum StructuresUnitOfMeasure {
    PerSquareMetre=0,
    PerMetre=1,
    PerUnit=2,
    PerFoot=3,
    PerCubicMetre=4
  }
  
  export const StructureUnitDescriptions: { [key in StructuresUnitOfMeasure]: string } = {
    [StructuresUnitOfMeasure.PerSquareMetre]: 'Per Square Metre',
    [StructuresUnitOfMeasure.PerMetre]: 'Per Metre',
    [StructuresUnitOfMeasure.PerUnit]: 'Per Unit',
    [StructuresUnitOfMeasure.PerFoot]: 'Per Foot',
    [StructuresUnitOfMeasure.PerCubicMetre]: 'Per Cubic Metre'
  };
  export enum ModerationStatus {
   // Pending = 0,
    Approved = 1,
   // Rejected = 2,
    Revised = 3,
    Deferred = 4,
    Deleted = 5,
    NewEntry = 6
  }
  export const ModerationStatusDescriptions: { [key in ModerationStatus]: string } = {
   // [ModerationStatus.Pending]: 'Pending',
    [ModerationStatus.Approved]: 'Approved',
   // [ModerationStatus.Rejected]: 'Rejected',
    [ModerationStatus.Revised]: 'Revised',
    [ModerationStatus.Deferred]: 'Deferred',
    [ModerationStatus.Deleted]: 'Deleted',
    [ModerationStatus.NewEntry]: 'New Entry'
  };
  export const ModerationStatusActions: { [key in ModerationStatus]: string } = {
    [ModerationStatus.Approved]: 'Approve',
    [ModerationStatus.Revised]: 'Revise',
    [ModerationStatus.Deferred]: 'Defer',
    [ModerationStatus.Deleted]: 'Delete',
    [ModerationStatus.NewEntry]: 'Mark New Entry'
  };
 /**
 * Finds the enum key (number) corresponding to a given label in a description map.
 * 
 * @param descriptionMap A mapping from enum values to labels (e.g. ModerationStatusDescriptions)
 * @param label The human-readable label (e.g. "Approved")
 * @returns The enum key (as number) or undefined if not found
 */
export function findEnumKeyByValue<T extends string | number | symbol>(descriptionMap: { [key in T]: string }, label: string): T | undefined {
  const entry = Object.entries(descriptionMap).find(([_, value]) => value === label);
  return entry ? (Number(entry[0]) as T) : undefined;
}
 
  
  export interface RateRow {
    id: number;
    existingRate?: number;
    rate?: number;
    useCurrent?: boolean;
  
    growthStageId?: number;
    growthStageName?: string;
  
    structureName?: string;
    unit?: string;
    quality?: string;
  
    noRate?: boolean;
    discretionInfo?: string;
  
    assumptions?: string;
    showAssumptions?: boolean;
  
    status: ModerationStatus;
    moderationNotes?: string;
    deferredReason?: string;
  }
  