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