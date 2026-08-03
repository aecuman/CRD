import { Component, OnInit } from '@angular/core';
import {
  APIService,
  RateTemplateViewModel,
  CreateRateTemplateCommand,
  PlantTemplateItem,
  StructureTemplateItem,
  PlantListViewModel,
  GroupedPlantListViewModel,
  StructureViewDto,
  OptionViewModel,
  TemplateCategoryInfoSelection
} from '../api.service';
import { AuthService } from '../auth.service';

interface PlantConfig {
  key: string;
  plantId?: number;
  plantType?: string;
  groupedPlantId?: number;
  groupName?: string;
  groupedPlantIds?: number[];
  displayName: string;
  availableStages: OptionViewModel[];        // for individual plants (GrowthStageViewModel)
  availableStageIds: number[];               // for grouped plants (number[])
  selectedGrowthStageIds: number[];
  selectedUnits: number[];
  selectedQualities: string[];
  categoryInfos: TemplateCategoryInfoSelection[];
  infoCategories?: any[];  // Plant's category info structure for configuration display
  isGroup: boolean;
}

@Component({
  selector: 'app-rate-template',
  standalone: false,
  templateUrl: './rate-template.component.html',
  styleUrl: './rate-template.component.css'
})
export class RateTemplateComponent implements OnInit {

  templates: RateTemplateViewModel[] = [];
  plants: PlantListViewModel[] = [];
  groupedPlants: GroupedPlantListViewModel[] = [];
  structures: StructureViewDto[] = [];
  growthStageOptions: OptionViewModel[] = [];
  cachedGroupedStructures: { categoryName: string; items: StructureViewDto[] }[] = [];

  // builder state
  showBuilder = false;
  templateName = '';
  templateDescription = '';
  activeMainTab: 'crops' | 'structures' = 'crops';
  activeCropsSubTab: 'single' | 'groups' = 'single';
  selectedKey: string | null = null;

  plantConfigs: Map<string, PlantConfig> = new Map();
  structureConfigs: Map<number, StructureTemplateItem> = new Map();

  saving = false;
  errorMsg = '';
  successMsg = '';

  expandedPreviewRows: { [idx: number]: boolean } = {};
  previewItemOrder: string[] = [];  // Track order of items in preview
  previewActiveTab: 'plants' | 'structures' = 'plants';  // Tab control in preview modal
  showPreviewModal = false;

  readonly unitLabels: Record<number, string> = {
    0: 'Per Square Metre', 1: 'Per Tree', 2: 'Per Plant', 3: 'Per Acre', 4: 'Per Clump'
  };
  readonly structureUnitLabels: Record<number, string> = {
    0: 'Per Square Metre', 1: 'Per Metre', 2: 'Per Unit', 3: 'Per Foot', 4: 'Per Cubic Metre'
  };
  readonly plantUnitKeys = [0, 1, 2, 3, 4];
  readonly structureUnitKeys = [0, 1, 2, 3, 4];
  readonly qualityOptions = ['Good', 'Medium', 'Poor'];

  constructor(private api: APIService, public auth: AuthService) {}

  ngOnInit(): void {
    this.loadTemplates();
    this.api.plantsAll().subscribe(p => this.plants = p ?? []);
    this.api.getGroupedPlants().subscribe(g => this.groupedPlants = g ?? []);
    this.api.structuresAll().subscribe(s => {
      this.structures = s ?? [];
      this.updateCachedGroupedStructures();
    });
    this.api.optionsGET().subscribe(o => this.growthStageOptions = o.growthStages ?? []);
  }

  loadTemplates() {
    this.api.getTemplates().subscribe(t => this.templates = t ?? []);
  }

  // ── Plant selection ─────────────────────────────────────────────────────────

  isPlantSelected(key: string): boolean { return this.plantConfigs.has(key); }

  addPlant(plant: PlantListViewModel): void {
    const key = 'p_' + plant.id;
    if (!this.plantConfigs.has(key)) {
      this.togglePlant(plant);
      this.selectedKey = key;
      this.expandedPreviewRows[this.previewItemOrder.indexOf(key)] = true;
    }
  }

  togglePlant(plant: PlantListViewModel): void {
    const key = 'p_' + plant.id;
    if (this.plantConfigs.has(key)) {
      this.plantConfigs.delete(key);
      this.previewItemOrder = this.previewItemOrder.filter(k => k !== key);
      if (this.selectedKey === key) this.selectedKey = null;
    } else {
      this.plantConfigs.set(key, {
        key,
        plantId: plant.id,
        plantType: plant.plantType ?? '',
        displayName: plant.commonName ?? plant.botanicalName ?? '',
        availableStages: plant.growthStages?.map(s => ({ id: s.growthStageId, name: s.name })) ?? [],
        availableStageIds: [],
        selectedGrowthStageIds: plant.growthStages?.map(s => s.growthStageId!) ?? [],
        selectedUnits: [0],
        selectedQualities: ['Good'],
        categoryInfos: [],
        infoCategories: plant.infoCategories?.map(ic => ({
          categoryId: ic.categoryId,
          id: ic.id,
          name: ic.name,
          info: ic.info ?? []
        })) ?? [],
        isGroup: false
      });
      this.previewItemOrder.push(key);
    }
  }

  addGroupedPlant(group: GroupedPlantListViewModel): void {
    const key = 'g_' + group.id;
    if (!this.plantConfigs.has(key)) {
      this.toggleGroupedPlant(group);
      this.selectedKey = key;
      this.expandedPreviewRows[this.previewItemOrder.indexOf(key)] = true;
    }
  }

  toggleGroupedPlant(group: GroupedPlantListViewModel): void {
    const key = 'g_' + group.id;
    if (this.plantConfigs.has(key)) {
      this.plantConfigs.delete(key);
      this.previewItemOrder = this.previewItemOrder.filter(k => k !== key);
      if (this.selectedKey === key) this.selectedKey = null;
    } else {
      // resolve stage names from global growth stage list
      const availableStages = (group.growthStages ?? [])
        .map(id => this.growthStageOptions.find(s => s.id === id))
        .filter(Boolean) as OptionViewModel[];

      this.plantConfigs.set(key, {
        key,
        groupedPlantId: group.id,
        groupName: group.name ?? '',
        groupedPlantIds: group.plants?.map(p => p.id!) ?? [],
        displayName: group.name ?? '',
        availableStages,
        availableStageIds: group.growthStages ?? [],
        selectedGrowthStageIds: group.growthStages ?? [],
        selectedUnits: [1],
        selectedQualities: ['Good'],
        categoryInfos: [],
        infoCategories: [],
        isGroup: true
      });
      this.previewItemOrder.push(key);
    }
  }

  selectConfig(key: string): void { this.selectedKey = key; }
  get currentConfig(): PlantConfig | undefined { return this.selectedKey ? this.plantConfigs.get(this.selectedKey) : undefined; }

  toggleStage(config: PlantConfig, stageId: number): void {
    const idx = config.selectedGrowthStageIds.indexOf(stageId);
    idx === -1 ? config.selectedGrowthStageIds.push(stageId) : config.selectedGrowthStageIds.splice(idx, 1);
  }

  toggleUnit(config: PlantConfig, unit: number): void {
    const idx = config.selectedUnits.indexOf(unit);
    idx === -1 ? config.selectedUnits.push(unit) : config.selectedUnits.splice(idx, 1);
  }

  toggleQuality(config: PlantConfig, q: string): void {
    if (q === 'Good') return; // always included
    const idx = config.selectedQualities.indexOf(q);
    idx === -1 ? config.selectedQualities.push(q) : config.selectedQualities.splice(idx, 1);
  }

  getConfigRowCount(config: PlantConfig): number {
    const stageCount = config.selectedGrowthStageIds.length || 1;
    const unitCount = config.selectedUnits.length || 1;
    const qualityCount = config.selectedQualities.length || 1;
    const categoryCount = config.categoryInfos.length || 1;
    return stageCount * unitCount * qualityCount * categoryCount;
  }

  stageSelected(config: PlantConfig, id: number): boolean { return config.selectedGrowthStageIds.includes(id); }
  unitSelected(config: PlantConfig, u: number): boolean { return config.selectedUnits.includes(u); }
  qualitySelected(config: PlantConfig, q: string): boolean { return config.selectedQualities.includes(q); }

  // ── Category info selection ──────────────────────────────────────────────────

  getPlantData(config: PlantConfig): PlantListViewModel | undefined {
    if (!config.plantId) return undefined;
    return this.plants.find(p => p.id === config.plantId);
  }

  getCategoryOptionName(config: PlantConfig, categoryInfoId: number, categoryInfoOption: number): string {
    const plant = this.getPlantData(config);
    if (!plant) return `Opt ${categoryInfoOption}`;
    const catInfo = plant.infoCategories?.find(ci => ci.id === categoryInfoId);
    if (!catInfo || !catInfo.info) return `Opt ${categoryInfoOption}`;
    return catInfo.info[categoryInfoOption] ?? `Opt ${categoryInfoOption}`;
  }

  isCategoryInfoSelected(config: PlantConfig, categoryId: number, categoryInfoId: number, categoryInfoOption: number): boolean {
    return config.categoryInfos.some(item =>
      item.categoryId === categoryId &&
      item.categoryInfoId === categoryInfoId &&
      item.categoryInfoOption === categoryInfoOption
    );
  }

  toggleCategoryInfo(config: PlantConfig, categoryId: number, categoryInfoId: number, categoryInfoOption: number): void {
    const idx = config.categoryInfos.findIndex(
      item => item.categoryId === categoryId && item.categoryInfoId === categoryInfoId && item.categoryInfoOption === categoryInfoOption
    );
    if (idx !== -1) {
      config.categoryInfos.splice(idx, 1);
    } else {
      config.categoryInfos.push({ categoryId, categoryInfoId, categoryInfoOption });
    }
  }

  // ── Structure selection ─────────────────────────────────────────────────────

  isStructureSelected(id: number): boolean { return this.structureConfigs.has(id); }

  addStructure(s: StructureViewDto): void {
    if (!this.structureConfigs.has(s.id!)) {
      this.toggleStructure(s);
    }
  }

  toggleStructure(s: StructureViewDto): void {
    if (this.structureConfigs.has(s.id!)) {
      this.structureConfigs.delete(s.id!);
    } else {
      this.structureConfigs.set(s.id!, { structureId: s.id!, unit: 0 });
    }
  }

  removeItem(idx: number): void {
    const items = this.getPreviewItems();
    if (idx >= 0 && idx < items.length) {
      const key = items[idx].key;
      this.plantConfigs.delete(key);
      this.previewItemOrder = this.previewItemOrder.filter(k => k !== key);
      delete this.expandedPreviewRows[idx];
    }
  }

  removeStructure(structureId: number): void {
    this.structureConfigs.delete(structureId);
  }

  moveStructure(structureId: number, direction: 'up' | 'down'): void {
    // Get list of all structure IDs in order
    const allStructures = Array.from(this.structureConfigs.keys());
    const currentIdx = allStructures.indexOf(structureId);
    
    if (currentIdx === -1) return;
    
    const newIdx = direction === 'up' ? currentIdx - 1 : currentIdx + 1;
    if (newIdx < 0 || newIdx >= allStructures.length) return;
    
    // Swap in the Map (recreate with new order)
    const newConfigs = new Map<number, StructureTemplateItem>();
    allStructures.forEach((id, idx) => {
      if (idx === currentIdx) {
        newConfigs.set(allStructures[newIdx], this.structureConfigs.get(allStructures[newIdx])!);
      } else if (idx === newIdx) {
        newConfigs.set(allStructures[currentIdx], this.structureConfigs.get(allStructures[currentIdx])!);
      } else {
        newConfigs.set(id, this.structureConfigs.get(id)!);
      }
    });
    this.structureConfigs = newConfigs;
  }

  closeBuilder(): void {
    this.showBuilder = false;
    this.plantConfigs.clear();
    this.structureConfigs.clear();
    this.previewItemOrder = [];
    this.expandedPreviewRows = {};
    this.templateName = '';
    this.templateDescription = '';
    this.selectedKey = null;
  }

  setStructureUnit(id: number, unit: number): void {
    const cfg = this.structureConfigs.get(id);
    if (cfg) cfg.unit = unit;
  }

  private updateCachedGroupedStructures(): void {
    const map = new Map<string, StructureViewDto[]>();
    for (const s of this.structures) {
      const cat = s.category?.name ?? 'Uncategorised';
      if (!map.has(cat)) map.set(cat, []);
      map.get(cat)!.push(s);
    }
    this.cachedGroupedStructures = Array.from(map.entries()).map(([categoryName, items]) => ({ categoryName, items }));
  }

  get groupedStructures(): { categoryName: string; items: StructureViewDto[] }[] {
    return this.cachedGroupedStructures;
  }

  getSummaryRowCount(): number {
    const plantTotal = Array.from(this.plantConfigs.values()).reduce((sum, config) => {
      const stageCount = config.selectedGrowthStageIds.length || 1;
      const unitCount = config.selectedUnits.length || 1;
      const qualityCount = config.selectedQualities.length || 1;
      const categoryCount = config.categoryInfos.length || 1;
      return sum + (stageCount * unitCount * qualityCount * categoryCount);
    }, 0);
    const structureTotal = this.structureConfigs.size;
    return plantTotal + structureTotal;
  }

  getPreviewItems(): any[] {
    // Build preview items from plant and structure configs in order
    const items: any[] = [];
    
    // Initialize item order if empty
    if (this.previewItemOrder.length === 0) {
      for (const [key] of this.plantConfigs) {
        this.previewItemOrder.push(key);
      }
    }

    // Add plants in configured order
    for (const key of this.previewItemOrder) {
      const config = this.plantConfigs.get(key);
      if (config) {
        const stageCount = config.selectedGrowthStageIds.length || 1;
        const unitCount = config.selectedUnits.length || 1;
        const qualityCount = config.selectedQualities.length || 1;
        const categoryCount = config.categoryInfos.length || 1;
        const rowCount = stageCount * unitCount * qualityCount * categoryCount;

        items.push({
          key,
          type: config.isGroup ? 'group' : 'plant',
          displayName: config.displayName,
          stageCount,
          unitCount,
          qualityCount,
          categoryCount: categoryCount > 1 ? categoryCount : 0,
          rowCount,
          availableStages: config.availableStages,
          selectedGrowthStageIds: config.selectedGrowthStageIds,
          selectedUnits: config.selectedUnits,
          selectedQualities: config.selectedQualities,
          infoCategories: config.infoCategories,
          categoryInfos: config.categoryInfos,
          // For display
          growthStages: config.selectedGrowthStageIds.map(id => {
            const stage = config.availableStages.find(s => s.id === id);
            return stage?.name ?? id.toString();
          }),
          units: config.selectedUnits.map(u => this.unitLabels[u]),
          qualities: config.selectedQualities,
          categoryLabels: config.categoryInfos.map(ci => this.getCategoryOptionName(config, ci.categoryInfoId!, ci.categoryInfoOption))
        });
      }
    }

    return items;
  }

  getStructurePreviewItems(): any[] {
    const items: any[] = [];
    for (const config of this.structureConfigs.values()) {
      items.push({
        displayName: this.structureName(config.structureId),
        unit: this.structureUnitLabels[config.unit]
      });
    }
    return items;
  }

  getStructuresByCategory(): { [key: string]: any[] } {
    const grouped: { [key: string]: any[] } = {};
    
    for (const config of this.structureConfigs.values()) {
      const struct = this.structures.find(s => s.id === config.structureId);
      if (!struct) continue;
      
      const categoryName = struct.category?.name ?? 'Uncategorised';
      if (!grouped[categoryName]) {
        grouped[categoryName] = [];
      }
      
      // Generate description from attributeSelections
      const descriptionParts: string[] = [];
      if (struct.attributeSelections && Array.isArray(struct.attributeSelections)) {
        for (const attr of struct.attributeSelections) {
          if (attr.selectedOptions && attr.selectedOptions.length > 0) {
            const optionNames = attr.selectedOptions.map(o => o.name).join(', ');
            descriptionParts.push(`${attr.attributeName}: ${optionNames}`);
          }
        }
      }
      const description = descriptionParts.join('\n');
      
      grouped[categoryName].push({
        displayName: struct.name ?? struct.id?.toString(),
        unit: config.unit,
        structureId: config.structureId,
        description: description
      });
    }
    
    return grouped;
  }

  getSimulatedRateRows(): any[] {
    const rows: any[] = [];

    // Generate all possible combinations for each plant config
    for (const config of this.plantConfigs.values()) {
      const stages = config.selectedGrowthStageIds.length > 0 
        ? config.selectedGrowthStageIds.map(id => {
            const stage = config.availableStages.find(s => s.id === id);
            return stage?.name ?? id.toString();
          })
        : ['All'];

      const units = config.selectedUnits.length > 0 
        ? config.selectedUnits.map(u => this.unitLabels[u])
        : ['All'];

      const qualities = config.selectedQualities.length > 0 
        ? config.selectedQualities 
        : ['Good'];

      const categories = config.categoryInfos.length > 0 
        ? config.categoryInfos.map(ci => this.getCategoryOptionName(config, ci.categoryInfoId!, ci.categoryInfoOption))
        : [''];

      // Create row for each combination
      for (const unit of units) {
        for (const stage of stages) {
          for (const quality of qualities) {
            if (quality === 'Good') {
              // Good quality doesn't show in display
              for (const cat of categories) {
                // Combine plant name with category for grouping in preview
                const displayName = cat && cat !== '—' 
                  ? `${cat} - ${config.displayName}` 
                  : config.displayName;
                rows.push({
                  plantName: displayName,
                  unit: unit,
                  stage: stage,
                  category: cat || '—'
                });
              }
            } else {
              // Show quality for non-Good qualities
              for (const cat of categories) {
                // Combine plant name with category for grouping in preview
                const displayName = cat && cat !== '—' 
                  ? `${cat} - ${config.displayName}` 
                  : config.displayName;
                rows.push({
                  plantName: displayName,
                  unit: unit,
                  stage: `${stage} (${quality})`,
                  category: cat || '—'
                });
              }
            }
          }
        }
      }
    }

    // Add structure rows
    for (const config of this.structureConfigs.values()) {
      rows.push({
        plantName: this.structureName(config.structureId),
        unit: this.structureUnitLabels[config.unit],
        stage: '—',
        category: '—'
      });
    }

    return rows;
  }

  // Group simulated rows by Plant Name (with category) and Unit
  getGroupedPreviewRows(): any[] {
    const rows = this.getSimulatedRateRows();
    const grouped: { [key: string]: any } = {};

    for (const row of rows) {
      // Combine plant name with category for display
      const displayName = row.category && row.category !== '—' 
        ? `${row.category} - ${row.plantName}` 
        : row.plantName;
      const key = `${displayName}__${row.unit}`;

      if (!grouped[key]) {
        grouped[key] = {
          plantName: displayName,
          unit: row.unit,
          stages: []
        };
      }
      grouped[key].stages.push(row.stage);
    }

    return Object.values(grouped);
  }

  /**
   * Prepare preview rows with rowspan information for proper table cell merging
   * Structure: Plant Name (merged for all its units) > Unit (merged for all its stages) > Growth Stage
   */
  getPreviewRowsWithRowspan(): any[] {
    const rows = this.getSimulatedRateRows();
    const result: any[] = [];
    
    // Group by plant name
    const plantGroups: { [plantName: string]: any[] } = {};
    for (const row of rows) {
      if (!plantGroups[row.plantName]) {
        plantGroups[row.plantName] = [];
      }
      plantGroups[row.plantName].push(row);
    }

    // Process each plant group
    for (const plantName of Object.keys(plantGroups)) {
      const plantRows = plantGroups[plantName];
      
      // Group by unit within each plant
      const unitGroups: { [unit: string]: any[] } = {};
      for (const row of plantRows) {
        if (!unitGroups[row.unit]) {
          unitGroups[row.unit] = [];
        }
        unitGroups[row.unit].push(row);
      }

      let isFirstPlantRow = true;
      const plantRowCount = plantRows.length;

      // Process each unit group
      for (const unit of Object.keys(unitGroups)) {
        const unitRows = unitGroups[unit];
        let isFirstUnitRow = true;

        // Add stage rows
        for (const row of unitRows) {
          result.push({
            ...row,
            showPlant: isFirstPlantRow,
            plantRowspan: plantRowCount,
            showUnit: isFirstUnitRow,
            unitRowspan: unitRows.length
          });
          isFirstPlantRow = false;
          isFirstUnitRow = false;
        }
      }
    }

    return result;
  }

  togglePreviewRow(idx: number): void {
    this.expandedPreviewRows[idx] = !this.expandedPreviewRows[idx];
  }

  movePreviewItem(idx: number, direction: 'up' | 'down'): void {
    const items = this.getPreviewItems();
    if (direction === 'up' && idx > 0) {
      const temp = this.previewItemOrder[idx - 1];
      this.previewItemOrder[idx - 1] = this.previewItemOrder[idx];
      this.previewItemOrder[idx] = temp;
    } else if (direction === 'down' && idx < items.length - 1) {
      const temp = this.previewItemOrder[idx + 1];
      this.previewItemOrder[idx + 1] = this.previewItemOrder[idx];
      this.previewItemOrder[idx] = temp;
    }
  }

  // ── Save template ───────────────────────────────────────────────────────────

  get canSave(): boolean {
    return this.templateName.trim().length > 0 &&
           (this.plantConfigs.size > 0 || this.structureConfigs.size > 0);
  }

  saveTemplate(): void {
    if (!this.canSave) return;
    this.saving = true;
    this.errorMsg = '';

    const plantItems: PlantTemplateItem[] = Array.from(this.plantConfigs.values()).map(c => ({
      plantId: c.plantId,
      plantType: c.plantType,
      groupedPlantId: c.groupedPlantId,
      groupName: c.groupName,
      groupedPlantIds: c.groupedPlantIds,
      growthStageIds: c.selectedGrowthStageIds,
      units: c.selectedUnits,
      qualities: c.selectedQualities,
      categoryInfos: c.categoryInfos.length ? c.categoryInfos : undefined
    }));

    const structureItems: StructureTemplateItem[] = Array.from(this.structureConfigs.values());

    const cmd: CreateRateTemplateCommand = {
      name: this.templateName.trim(),
      description: this.templateDescription.trim() || undefined,
      config: { plantItems, structureItems }
    };

    this.api.createTemplate(cmd).subscribe({
      next: () => {
        this.saving = false;
        this.successMsg = 'Template saved.';
        this.loadTemplates();
        setTimeout(() => {
          this.showBuilder = false;
          this.resetBuilder();
          this.successMsg = '';
        }, 1200);
      },
      error: (e: any) => {
        this.saving = false;
        this.errorMsg = e?.error?.message ?? 'Failed to save template.';
      }
    });
  }

  deleteTemplate(id: number): void {
    if (!confirm('Delete this template?')) return;
    this.api.deleteTemplate(id).subscribe(() => this.loadTemplates());
  }

  resetBuilder(): void {
    this.templateName = '';
    this.templateDescription = '';
    this.plantConfigs.clear();
    this.structureConfigs.clear();
    this.selectedKey = null;
    this.activeMainTab = 'crops';
    this.activeCropsSubTab = 'single';
    this.errorMsg = '';
    this.successMsg = '';
  }

  openBuilder(): void { this.resetBuilder(); this.showBuilder = true; }

  get plantConfigList(): PlantConfig[] { return Array.from(this.plantConfigs.values()); }
  get structureConfigList(): StructureTemplateItem[] { return Array.from(this.structureConfigs.values()); }

  structureName(id: number): string {
    return this.structures.find(s => s.id === id)?.name ?? 'Unknown';
  }
}
