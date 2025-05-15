import { Component } from '@angular/core';
import { APIService, ModeratedPlantRateViewModel, ModerationReportViewModel, OptionsListViewModel, OptionViewModel } from '../api.service';
import { ActivatedRoute, Router } from '@angular/router';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import * as XLSX from 'xlsx';
import { ExportService } from '../export.service';
import { AuthService } from '../auth.service';


@Component({
  selector: 'app-moderation-report',
  standalone: false,
  templateUrl: './moderation-report.component.html',
  styleUrl: './moderation-report.component.css'
})
export class ModerationReportComponent {

  activeTab: 'plants' | 'structures' = 'plants';

  report?: ModerationReportViewModel;
  PlantData: ModeratedPlantRateViewModel[]=[];

  plantRateSearch:string ='';
  structureRateSearch:string ='';
  growth_list: OptionViewModel[]=[];
  structureRatesFlat: { categoryName: string; items: any[]; }[]=[];
  /**
   *
   */
  constructor(private api:APIService, private route:ActivatedRoute, public exportService:ExportService, private router:Router,private auth:AuthService) {
    this.getAllOptions();
    this.route.params.subscribe(params => {

      if (params['id']) {        
        
          this.getReport(params['id']);
        
      }
    });
  }
    PrintReport(mode: 'pdf' | 'excel' | 'print'){
      this.exportService.SetData(this.report,this.PlantData,this.structureRatesFlat,this.growth_list,'Draft');

      this.router.navigate([{ outlets: { print: ['final-report-download'] } }], {
        queryParams: { type: mode, preview: 'true' }
      });
      //this.exportService.triggerExport(mode);
    }
  getReport(id:number){
    this.api.getModerationReport(id).subscribe(data => {
      this.report=data;
      this.groupRows();
      this.groupRatesByStructure();
    });
  }
      getAllOptions() {
        this.api.optionsGET().subscribe({
          next: (value: OptionsListViewModel) => {
           
            this.growth_list =  value.growthStages || [];
          }
        });
      }
  groupRows() {
    // First group by a unified key: either plantName or groupName
    const groupedByKey = this.groupBy(this.report?.plantRates??[], item =>
      item.groupedPlantId ? item.groupName ?? '' : item.plantName ?? ''
    );
  
    this.PlantData = [];
  
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
  
          this.PlantData.push(item);
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
  hasNonGoodQuality(name: string): boolean {
    return this.PlantData.some(rate =>
      (rate.groupName === name || rate.plantName === name) &&
      rate.quality !== 'Good'
    );
  }
  getGroupStageName(id:number){
    return this.growth_list?.find(x => x.id === id)?.name;
  }
  getStageName(id: number) {
    return this.growth_list.find(x => x.id === id)?.name ?? '';
  //  return this.plants.find(x => x.id === plantId)?.growthStages?.find(x => x.growthStageId === id)?.name ?? '';
    
  }
  groupRatesByStructure() {
    const grouped: { [key: string]: any[] } = {};
  
    for (let rate of this.report?.structureRates??[]) {
      const categoryName = rate.structure?.category?.name || 'Uncategorized';
      if (!grouped[categoryName]) {
        grouped[categoryName] = [];
      }
      grouped[categoryName].push(rate);
    }
  
    this.structureRatesFlat = Object.entries(grouped).map(([categoryName, items]) => ({
      categoryName,
      items
    }));
  }
  GetAttributeNames(attribute: any): string {
    return attribute.map((opt:any) => opt.name).join(', ')
   }
  
  exportModerationReportToPdf(): void {
    const doc = new jsPDF('landscape', 'pt', 'a4');
    const logo = new Image();
    logo.src = 'assets/logo.png'; // Ensure the logo path is correct
  
    logo.onload = () => {
      const summary = this.getModerationSummary();
  
      // 🔹 COVER PAGE
      doc.addImage(logo, 'PNG', (doc.internal.pageSize.getWidth() - 140) / 2, 60, 140, 140);
      doc.setFontSize(20);
      doc.text(
        `Moderation Report of District Compensation Rates for ${this.report?.districtName} ${this.report?.year}/${(this.report?.year ?? 0) + 1}`,
        doc.internal.pageSize.getWidth() / 2,
        230,
        { align: 'center' }
      );
      doc.setFontSize(12);
      doc.text(`District: ${this.report?.districtName}`, doc.internal.pageSize.getWidth() / 2, 260, { align: 'center' });
      doc.text(`Year: ${this.report?.year}`, doc.internal.pageSize.getWidth() / 2, 280, { align: 'center' });
      doc.text(`Generated on: ${new Date().toLocaleDateString()}`, doc.internal.pageSize.getWidth() / 2, 300, { align: 'center' });
  
      // 🔹 SUMMARY STATISTICS
      doc.addPage();
      doc.setFontSize(14);
      doc.text('1. Summary Statistics', 40, 60);
      autoTable(doc, {
        startY: 80,
        head: [['Type', 'Total', 'Approved', 'Revised', 'Deferred', 'Unmoderated']],
        body: [
          ['Crops/Trees Rates', summary.plant.total, summary.plant.approved, summary.plant.revised, summary.plant.deferred, summary.plant.unmoderated],
          ['Structure Rates', summary.structure.total, summary.structure.approved, summary.structure.revised, summary.structure.deferred, summary.structure.unmoderated]
        ],
        styles: {
          fontSize: 10,
          lineWidth: 0.2,
          lineColor: 80
        },
        headStyles: { fillColor: [60, 141, 188], textColor: 255 },
        columnStyles: {
          2: { textColor: [0, 128, 0] },
          3: { textColor: [0, 0, 255] },
          4: { textColor: [255, 165, 0] },
          5: { textColor: [150, 150, 150] }
        },
        theme: 'grid',
        margin: { left: 40, right: 40 },
        didDrawPage: (data) => {
          const pageSize = data.doc.internal.pageSize;
          const pageHeight = pageSize.height;
          const pageWidth = pageSize.width;
          const pageNumber = data.doc.internal.getNumberOfPages();
      
          data.doc.setFontSize(9);
          data.doc.setTextColor(120);
          data.doc.text(
            `© Ministry of Lands, Housing & Urban Development - ${new Date().getFullYear()}`,
            40,
            pageHeight - 20
          );
      
          data.doc.text(
            `Page ${pageNumber}`,
            pageWidth - 80,
            pageHeight - 20
          );
        }
    
      });
  
      // 🔹 CROP/TREE COMPENSATION RATES
      doc.addPage();
      doc.setFillColor(0, 122, 204);
      doc.rect(0, 20, doc.internal.pageSize.getWidth(), 10, 'F');
      doc.setTextColor(0, 0, 0);
      doc.setFontSize(14);
      doc.text('2. Crop/Tree Compensation Rates', 40, 50);
  
      autoTable(doc, {
        startY: 70,
        head: [['Crop/Tree', 'Unit', 'Stage', 'Original', 'New', 'Status']],
        body: this.PlantData.map((row: any) => [
          row.showPlant ? (row.groupedPlantId ? row.groupName : row.plantName) : '',
          row.showUnit ? row.unit : '',
          row.groupedPlantId ? this.getGroupStageName(row.growthStage) : this.getStageName(row.growthStage),
          row.originalRate ? 'UGX ' + row.originalRate.toLocaleString() : '—',
          row.latestModeration?.newRate ? 'UGX ' + row.latestModeration.newRate.toLocaleString() : '—',
          row.latestModeration?.status ?? ''
        ]),
        styles: {
          fontSize: 9,
          cellPadding: 3,
          lineWidth: 0.2,
          lineColor: 80
        },
        theme: 'grid',
        margin: { left: 40, right: 40 },
        didDrawPage: (data) => {
          const pageSize = data.doc.internal.pageSize;
          const pageHeight = pageSize.height;
          const pageWidth = pageSize.width;
          const pageNumber = data.doc.internal.getNumberOfPages();
      
          data.doc.setFontSize(9);
          data.doc.setTextColor(120);
          data.doc.text(
            `© Ministry of Lands, Housing & Urban Development - ${new Date().getFullYear()}`,
            40,
            pageHeight - 20
          );
      
          data.doc.text(
            `Page ${pageNumber}`,
            pageWidth - 80,
            pageHeight - 20
          );
        },      
        didParseCell: (data) => {
          const rowIndex = data.row.index;
          const colIndex = data.column.index;
          const currentRow:any = this.PlantData[rowIndex];
      
          // ✅ Gray background for new group rows
          if (currentRow?.showPlant && data.section === 'body') {
            data.cell.styles.fillColor = [220, 220, 220]; // light gray
          }
      
          // ✅ Color the status text
          if (colIndex === 5 && data.section === 'body') {
            const status = currentRow?.latestModeration?.status ?? '';
            switch (status) {
              case 'Approved':
                data.cell.styles.textColor = [0, 128, 0]; // green
                break;
              case 'Revised':
                data.cell.styles.textColor = [0, 0, 255]; // blue
                break;
              case 'Deferred':
                data.cell.styles.textColor = [255, 165, 0]; // orange
                break;
              default:
                data.cell.styles.textColor = [128, 128, 128]; // gray
            }
          }
        }
      });
  
      // 🔹 STRUCTURE COMPENSATION RATES
      doc.addPage();
      doc.setFillColor(0, 122, 204);
      doc.rect(0, 20, doc.internal.pageSize.getWidth(), 10, 'F');
      doc.setTextColor(0, 0, 0);
      doc.setFontSize(14);
      doc.text('3. Structure Compensation Rates', 40, 50);
  
      for (const group of this.structureRatesFlat ?? []) {
        // Add group header row manually
        autoTable(doc, {
          startY: (doc as any).lastAutoTable?.finalY ? (doc as any).lastAutoTable.finalY + 0 : 70,
          head: [[group.categoryName]],
          theme: 'grid',
          headStyles: {
            fillColor: [240, 240, 240],    // Light gray background
            textColor: 50,
            fontSize: 12,
            fontStyle: 'bold',
            halign: 'left',
            valign: 'top',
            cellPadding: { top: 6, bottom: 6 }  // Force enough height to avoid centering
          },
          styles: {
            overflow: 'linebreak',
            minCellHeight: 24                   // ✅ Force height to keep top alignment
          },
          columnStyles: {
            0: { cellWidth: 'wrap' }            // Prevents stretching
          },
          margin: { left: 40, right: 40 }
        });
      
        // Map body rows under this group
        const body = group.items.map((rate: any) => [
          rate.structureName,
          rate.structure?.structureType?.name ?? '—',
          rate.structure?.attributeSelections?.map((attr: any) => `${attr.attributeName}: ${attr.selectedOptions?.map((o: any) => o.name).join(', ')}`
        ).join('\n')  ?? '—',
          rate.unit,
          rate.originalRate ? 'UGX ' + rate.originalRate.toLocaleString() : '—',
          rate.latestModeration?.newRate ? 'UGX ' + rate.latestModeration.newRate.toLocaleString() : '—',
          rate.latestModeration?.status ?? 'Unmoderated'
        ]);
      
        autoTable(doc, {
          head: [['Structure', 'Type', 'Description', 'Unit', 'Original', 'New', 'Status']],
          body,
          startY: (doc as any).lastAutoTable?.finalY ? (doc as any).lastAutoTable.finalY + 5 : 70,
          styles: {
            fontSize: 9,
            cellPadding: 3,
            lineWidth: 0.2,
            lineColor: 80,
            valign: 'top'
          },
          margin: { left: 40, right: 40 },
        didDrawPage: (data) => {
          const pageSize = data.doc.internal.pageSize;
          const pageHeight = pageSize.height;
          const pageWidth = pageSize.width;
          const pageNumber = data.doc.internal.getNumberOfPages();
      
          data.doc.setFontSize(9);
          data.doc.setTextColor(120);
          data.doc.text(
            `© Ministry of Lands, Housing & Urban Development - ${new Date().getFullYear()}`,
            40,
            pageHeight - 20
          );
      
          data.doc.text(
            `Page ${pageNumber}`,
            pageWidth - 80,
            pageHeight - 20
          );
        },      
        didParseCell: (data) => {
          const rowIndex = data.row.index;
          const colIndex = data.column.index;
          const currentRow:any = this.PlantData[rowIndex];
      
          // ✅ Gray background for new group rows
          if (currentRow?.showPlant && data.section === 'body') {
            data.cell.styles.fillColor = [220, 220, 220]; // light gray
          }
      
          // ✅ Color the status text
          if (colIndex === 6 && data.section === 'body') {
            const status = currentRow?.latestModeration?.status ?? '';
            switch (status) {
              case 'Approved':
                data.cell.styles.textColor = [0, 128, 0]; // green
                break;
              case 'Revised':
                data.cell.styles.textColor = [0, 0, 255]; // blue
                break;
              case 'Deferred':
                data.cell.styles.textColor = [255, 165, 0]; // orange
                break;
              default:
                data.cell.styles.textColor = [128, 128, 128]; // gray
            }
          }
        }
      
      });
    }
  
      // 🔹 SIGNATURES
      doc.addPage();
      doc.setFontSize(14);
      doc.text('4. Signatures', 40, 80);
      doc.setFontSize(12);
      doc.text('Approved by:', 40, 120);
      doc.text('_________________________', 40, 150);
      doc.text('Name & Title', 40, 170);
  
      doc.text('Reviewed by:', 320, 120);
      doc.text('_________________________', 320, 150);
      doc.text('Name & Title', 320, 170);
  
      // ✅ SAVE PDF
      doc.save(`Moderation_Report_${this.report?.districtName}_${this.report?.year}.pdf`);
    };
  }
  
  

  exportModerationReportToExcel(): void {
    const workbook = XLSX.utils.book_new();

    // 📊 Summary Sheet FIRST
    const summary = this.getModerationSummary();
    const summaryData = [
      ['Category', 'Total', 'Approved', 'Revised', 'Deferred', 'Unmoderated'],
      ['Plant Rates', summary.plant.total, summary.plant.approved, summary.plant.revised, summary.plant.deferred, summary.plant.unmoderated],
      ['Structure Rates', summary.structure.total, summary.structure.approved, summary.structure.revised, summary.structure.deferred, summary.structure.unmoderated]
    ];
  
    const summarySheet = XLSX.utils.aoa_to_sheet(summaryData);
    // Mark header row height visually
    summarySheet['!rows'] = [{ hpt: 20 }, {}, {}];
    XLSX.utils.book_append_sheet(workbook, summarySheet, 'Summary Stats');
  
    // 🌿 Grouped Plant Rates
    const plantData = this.PlantData.map((row: any) => ({
      'Crop/Tree': row.showPlant ? (row.groupedPlantId ? row.groupName : row.plantName) : '',
      'Unit': row.showUnit ? row.unit : '',
      'Stage': row.groupedPlantId ? this.getGroupStageName(row.growthStage) : this.getStageName(row.growthStage),
      'Original Rate': row.originalRate ?? '—',
      'New Rate': row.latestModeration?.newRate ?? '—',
      'Status': row.latestModeration?.status ?? ''
    }));
    const plantSheet = XLSX.utils.json_to_sheet(plantData);
    XLSX.utils.book_append_sheet(workbook, plantSheet, 'Crop-Tree Rates');
  
    // 🧱 Structure Rates
    const structureData = (this.report?.structureRates ?? []).map((r: any) => ({
      'Structure': r.structureName,
      'Unit': r.unit,
      'Current Rate': r.rate ?? '—',
      'Previous Rate': r.originalRate ?? '—',
      'Moderated': r.isModerated ? 'Yes' : 'No',
      'Status': r.isModerated ? r.latestModeration?.status ?? '' : 'Unmoderated'
    }));
    const structureSheet = XLSX.utils.json_to_sheet(structureData);
    XLSX.utils.book_append_sheet(workbook, structureSheet, 'Structure Rates');
  
    // ✅ Borders & formatting (limited in open-source)
    // NOTE: Real border styling requires XLSX-style JSON, SheetJS Pro, or post-export editing.
  
    XLSX.writeFile(workbook, `Moderation_Report_${this.report?.districtName}_${this.report?.year}.xlsx`);
  }  
  
getModerationSummary() {
  const summarize = (items: any[]) => {
    const stats = {
      total: items.length,
      approved: 0,
      revised: 0,
      deferred: 0,
      unmoderated: 0,
    };

    for (const item of items) {
      const status = item?.latestModeration?.status ?? '';
      if (!item.isModerated) stats.unmoderated++;
      else if (status === 'Approved') stats.approved++;
      else if (status === 'Revised') stats.revised++;
      else if (status === 'Deferred') stats.deferred++;
    }

    return stats;
  };

  return {
    plant: summarize(this.PlantData),
    structure: summarize(this.structureRatesFlat),
  };
}
get canManage(){
  return this.auth.userValue?.roles?.includes('admin') || this.auth.userValue?.roles?.includes('superadmin');
}
}
    
