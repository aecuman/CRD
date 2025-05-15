import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { APIService, DistrictRateDto, ModerateCompensationRateCommand,  PlantRateViewModel, RateType, StructureRatesListViewModel } from '../api.service';
import { ModerationStatusDescriptions,ModerationStatus, ModerationStatusActions } from '../app.model';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-moderation-popup',
  standalone: false,
  templateUrl: './moderation-popup.component.html',
  styleUrl: './moderation-popup.component.css'
})
export class ModerationPopupComponent {
  @Input() show = false;
  @Input() rate!: PlantRateViewModel|StructureRatesListViewModel|any;
  @Input() comparisonRates: any[] = [];
  @Input() moderator!: string;
  @Input() isPlantRate = true;
  @Input() growth_list: any[] = [];
  @Input() editMode=false;
  @Input() DistrictRateDto?:DistrictRateDto|null;

  @Output() onClose = new EventEmitter<void>();
  @Output() onSubmit = new EventEmitter<ModerateCompensationRateCommand>();
  @Output() onSubmitAndNext = new EventEmitter<ModerateCompensationRateCommand>();

//moderationStatuses = Object.values(ModerationStatus).filter(v => typeof v === 'number') as ModerationStatus[];
ModerationStatusDescriptions = ModerationStatusDescriptions; // bind to template

moderationStatuses = [
  ModerationStatus.Approved,
  ModerationStatus.Revised,
  ModerationStatus.Deferred,
  ModerationStatus.Deleted,
  ModerationStatus.NewEntry
];

statusActions = ModerationStatusActions;



  command: ModerateCompensationRateCommand = {
    rateId: 0,
    rateType:this.isPlantRate?RateType._0:RateType._1,
    newRate: undefined,
    newDiscretionInfo: '',
    moderator: '',
    moderationNotes: '',
    status: undefined,
    isDeferred: false,
    deferredReason: '',
    noRate: undefined
  };

  saving=false;

  /**
   *
   */
  constructor(private api:APIService, private auth:AuthService) {
    // this.command = this.createEmptyCommand();
    
    
  }
  ngOnInit() {
    this.command = {
      rateId: this.rate?.id,
      rateType:this.isPlantRate?RateType._0:RateType._1,
      newRate: undefined,
      newDiscretionInfo: '',
      moderator: this.moderator,
      moderationNotes: '',
      status: undefined,
      isDeferred: false,
      deferredReason: ''
    };
  }
  selectStatus(status: ModerationStatus): void {
    this.command.status = status as any;

    switch (status) {
      case ModerationStatus.Approved:
      case ModerationStatus.NewEntry:
        this.command.newRate = this.rate.rate;
        this.command.newDiscretionInfo = '';
        break;
      case ModerationStatus.Revised:
        this.command.newDiscretionInfo = '';
        break;
      case ModerationStatus.Deferred:
        this.command.deferredReason = '';
        break;
      case ModerationStatus.Deleted:
        this.command.newRate = undefined;
        this.command.newDiscretionInfo = '';
        this.command.moderationNotes = '';
        break;
    }
  }
  onToggleNoRate(): void {
    if (this.command.noRate) {
      this.command.newRate = undefined;
    } else {
      this.command.newDiscretionInfo = '';
    }
  }
  
  canSelectStatus(status: ModerationStatus): boolean {
   /* if (status === ModerationStatus.Deleted && this.moderatorRole !== 'Admin') {
      return false;
    }*/
    return true;
  }

  getTooltip(status: ModerationStatus): string {
    switch (status) {
      case ModerationStatus.Approved:
        return 'Approve the current rate without making changes.';
  
      case ModerationStatus.Revised:
        return 'Enter a new rate to revise the currently submitted one.';
  
      case ModerationStatus.Deferred:
        return 'Postpone this rate to a future review session. A reason is required.';
  
      case ModerationStatus.Deleted:
        return 'Mark this rate for deletion. Moderation notes are required.';
  
      case ModerationStatus.NewEntry:
        return 'Confirm this as a newly submitted rate. Must match the current rate.';
  
      default:
        return '';
    }
  }
  

  getStatusColor(status: ModerationStatus): string {
    switch (status) {
      case ModerationStatus.Approved: return 'bg-green-600 text-white';
     // case ModerationStatus.Rejected: return 'bg-red-600 text-white';
      case ModerationStatus.Deferred: return 'bg-yellow-500 text-white';
      case ModerationStatus.Revised: return 'bg-blue-600 text-white';
      case ModerationStatus.Deleted: return 'bg-gray-600 text-white';
      case ModerationStatus.NewEntry: return 'bg-purple-600 text-white';
      default: return 'bg-gray-100 text-gray-700';
    }
  }

  validate(): boolean {
    const s = this.command.status as any as ModerationStatus;
    if (s === ModerationStatus.Revised && !this.command.newRate) {
      alert('Please provide a revised rate.');
      return false;
    }
    if (s === ModerationStatus.Deferred && !this.command.deferredReason) {
      alert('Deferred reason is required.');
      return false;
    }
    if (s === ModerationStatus.Deleted && !this.command.moderationNotes) {
      alert('Moderation notes required for deletion.');
      return false;
    }
    return true;
  }
  handleSave(next = false) {
   // this.command.isDeferred = this.command.status === 'Deferred';
   
   const confirmMsg = next
   ? 'Are you sure you want to save and move to the next rate?'
   : (this.editMode?'Are you sure you want to save changes':'Are you sure you want to save and close this moderation?');

 if (!window.confirm(confirmMsg)) return;

// this.command.isDeferred = this.command.status === ModerationStatus.Deferred;
this.saving = true;
this.api.moderate(this.rate.id,this.command).subscribe({
  next: () => {
    this.saving=false;
    // ✅ Emit result to parent after success
    next
      ? this.onSubmitAndNext.emit(this.command)
      : this.onSubmit.emit(this.command);
//this.command.status=undefined //this.createEmptyCommand();
  },
  error: (err) => {
    this.saving=false;
    alert('Failed to save moderation. Please try again.');
    console.error(err);
  }
});
  }
createEmptyCommand(): ModerateCompensationRateCommand {
  return {
    rateId: 0,
    rateType:this.isPlantRate?RateType._0:RateType._1,
    newRate: undefined,
    noRate: false,
    newDiscretionInfo: '',
    moderator: '',
    moderationNotes: '',
    status: undefined!,
    isDeferred: false,
    deferredReason: ''
  };
}
  close() {
    this.onClose.emit();
  }
  getGroupStageName(id:number){
    return this.growth_list?.find(x => x.id === id)?.name;
  }
  getStageName(id: number) {
    return this.growth_list.find(x => x.id === id)?.name ?? '';
  //  return this.plants.find(x => x.id === plantId)?.growthStages?.find(x => x.growthStageId === id)?.name ?? '';
    
  }
  findEnumKeyByValue<T extends string | number | symbol>(descriptionMap: { [key in T]: string }, label: string): T | undefined {
    const entry = Object.entries(descriptionMap).find(([_, value]) => value === label);
    return entry ? (Number(entry[0]) as T) : undefined;
  }
  GetAttributeNames(attribute: any): string {
    return attribute.map((opt:any) => opt.name).join(', ')
   }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['rate'] && this.rate) {
      this.command = {
        rateId: this.rate.id,
        rateType:this.isPlantRate?RateType._0:RateType._1,
        newRate: this.rate.rate,
        newDiscretionInfo: this.rate.discretionInfo || '',
        moderator: this.moderator,
        moderationNotes: this.rate.moderationNotes || '',
        status: this.findEnumKeyByValue(ModerationStatusDescriptions, this.rate.status) as any,
        noRate: !!this.rate.noRate,
        isDeferred: false,
        deferredReason: ''
      };
     
    }
  }
  get canManage(){
    return this.auth.userValue?.roles?.includes('admin') || this.auth.userValue?.roles?.includes('superadmin');
  }
}
