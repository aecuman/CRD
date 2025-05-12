import { Injectable } from '@angular/core';
const html2pdf = require('html2pdf.js');
import * as XLSX from 'xlsx';
import jsPDF from 'jspdf';
import autoTable, { HookData } from 'jspdf-autotable';

@Injectable({ providedIn: 'root' })
export class ExportService {
  report:any;
  groupedPlantRates: any[] = [];
  groupedStructureRates: { categoryName: string; items: any[]; }[] = [];
  growth_list: any[]=[];
  title: string = 'Draft';
  constructor() {}

  /**
   * Main export trigger
   * @param mode 'pdf' | 'excel' | 'print'
   * @param previewOnly disables auto-download/print
   */
  SetData(report:any,plantRates:any,structureRates:any, growth_list:any[],title:string){
this.report = report;
this.groupedPlantRates = plantRates;
this.groupedStructureRates = structureRates;
this.growth_list=growth_list
this.title = title;
  }
  triggerExport(mode: 'pdf' | 'excel' | 'print', previewOnly = false): void {
    const element = document.getElementById('moderationReport');
    if (!element) return;

    if (previewOnly) return;

    switch (mode) {
      case 'pdf': this.exportAsPdf(element); break;
      case 'excel': this.exportAsExcel2(element); break;
      case 'print': this.printElement(element); break;
    }
  }

 addFooter(data:HookData): void {

    const pageCount = data.doc.getNumberOfPages();
    const currentPage =data.doc.internal.getCurrentPageInfo().pageNumber;
    const year = new Date().getFullYear();
  
    data.doc.setFontSize(8);
    data.doc.setTextColor(120);
    data.doc.text(`Page ${currentPage} of ${pageCount}`, 510, 570);
    data.doc.text(`© ${year} Ministry of Lands Housing & Urban Development`, 40, 570);
  }
  private exportAsPdf(element: HTMLElement): void {
    const doc = new jsPDF('landscape', 'pt', 'a4');
    const logo = new Image();
    logo.src = 'assets/logo.png'; // Ensure the logo path is correct
    doc.addImage(logo, 'PNG', (doc.internal.pageSize.getWidth() - 140) / 2, 60, 140, 140);
    doc.setFontSize(20);
    doc.text(
      `${this.title} District Compensation Rates for ${this.report?.districtName} ${this.report?.year}/${(this.report?.year ?? 0) + 1}`,
      doc.internal.pageSize.getWidth() / 2,
      230,
      { align: 'center' }
    );
    doc.setFontSize(12);
   /* doc.text(`District: ${this.report?.districtName}`, doc.internal.pageSize.getWidth() / 2, 260, { align: 'center' });
    doc.text(`Year: ${this.report?.year}`, doc.internal.pageSize.getWidth() / 2, 280, { align: 'center' });
  // 1. COVER PAGE
  doc.addImage(logo, 'PNG', 250, 40, 100, 100);
  doc.setFontSize(20);
  doc.text(`Approved District Compensation Rates`, 40, 180);
  doc.text(`${this.report.districtName} (${this.report.year}/${this.report.year + 1})`, 40, 210);

  doc.setFontSize(12);
  doc.text(`District: ${this.report.districtName}`, 40, 250);
  doc.text(`Year: ${this.report.year}`, 40, 270);
  doc.text(`Generated: ${new Date().toLocaleDateString()}`, 40, 290);*/

 /* // 2. SUMMARY STATS
  doc.addPage();
  doc.setFontSize(14);
  doc.text('1. Summary Statistics', 40, 40);

  autoTable(doc, {
    startY: 60,
    head: [['Type', 'Total', 'Approved', 'Revised', 'Deferred', 'Unmoderated']],
    body: [
      ['Plant Rates', summary.plant.total, summary.plant.approved, summary.plant.revised, summary.plant.deferred, summary.plant.unmoderated],
      ['Structure Rates', summary.structure.total, summary.structure.approved, summary.structure.revised, summary.structure.deferred, summary.structure.unmoderated]
    ],
    headStyles: { fillColor: [60, 141, 188], textColor: 255 },
    styles: { fontSize: 10, cellPadding: 6 },
    columnStyles: {
      2: { textColor: [0, 128, 0] },     // Approved
      3: { textColor: [0, 0, 255] },     // Revised
      4: { textColor: [255, 165, 0] },   // Deferred
      5: { textColor: [150, 150, 150] }  // Unmoderated
    },
    margin: { left: 40, right: 40 }
  });
*/
  // 3. PLANT RATES
  doc.addPage();
  doc.setFontSize(14);
  doc.text('2. Crop/Tree Compensation Rates', 40, 40);

  const plantBody = this.groupedPlantRates.map((row: any) => [
    row.showPlant ? (row.groupedPlantId ? row.groupName : row.plantName) : '',
    row.showUnit ? row.unit : '',
    row.groupedPlantId ? this.getGroupStageName(row.growthStage) : this.getStageName(row.growthStage),
    //row.originalRate ? 'UGX ' + row.originalRate.toLocaleString() : '—',
    row.latestModeration?.newRate ? 'UGX ' + row.latestModeration.newRate.toLocaleString() : '—',
   // row.latestModeration?.status ?? ''
  ]) /* this.groupedPlantRates.map((rate:any) => [
    rate.groupedPlantId ? rate.groupName : rate.plantName,
    rate.unit,
    rate.growthStage,
    rate.latestModeration?.newRate ? `UGX ${rate.latestModeration.newRate.toLocaleString()}` : '—'
  ]); */

  autoTable(doc, {
    startY: 60,
    head: [['Crop/Tree', 'Unit', 'Growth Stage', 'Rate (UGX)']],
    body: plantBody,
   // styles: { fontSize: 9 },
    margin: { left: 40, right: 40 },    
        styles: {
          fontSize: 10,
          cellPadding: 3,
          lineWidth: 0.2,
          lineColor: 80
        },
    theme: 'grid',
    headStyles: { fillColor: [60, 141, 188], textColor: 255 },
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
      const currentRow:any = this.groupedPlantRates[rowIndex];
  
      // ✅ Gray background for new group rows
      if (currentRow?.showPlant && data.section === 'body') {
        data.cell.styles.fillColor = [220, 220, 220]; // light gray
      }
  
    }
  });

  // 4. STRUCTURE RATES
  doc.addPage();
  doc.setFontSize(14);
  doc.text('3. Structure Compensation Rates', 40, 40);
 for (const group of this.groupedStructureRates ?? []) {
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
         // rate.originalRate ? 'UGX ' + rate.originalRate.toLocaleString() : '—',
          rate.latestModeration?.newRate ? 'UGX ' + rate.latestModeration.newRate.toLocaleString() : '—',
       //   rate.latestModeration?.status ?? 'Unmoderated'
        ]);
      
        autoTable(doc, {
          head: [['Structure','Type', 'Description', 'Unit', 'Rate']],
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
            const currentRow:any = group.items[rowIndex];
      
            // ✅ Gray background for new group rows
            if (currentRow?.showPlant && data.section === 'body') {
              data.cell.styles.fillColor = [220, 220, 220]; // light gray
            }
      
          }
      
      });
    }
  /*const structureBody = this.groupedStructureRates.map((rate:any) => [
    rate.structureName,
    rate.unit,
    rate.rate ? `UGX ${rate.rate.toLocaleString()}` : '—',
    rate.originalRate ? `UGX ${rate.originalRate.toLocaleString()}` : '—',
    rate.isModerated ? 'Yes' : 'No',
    rate.isModerated ? rate.latestModeration?.status : 'Unmoderated'
  ]);

  autoTable(doc, {
    startY: 60,
    head: [['Structure', 'Unit', 'Current Rate', 'Previous Rate', 'Moderated', 'Status']],
    body: structureBody,
    styles: { fontSize: 9 },
    margin: { left: 40, right: 40 },
    didDrawPage: (data) => this.addFooter(data)
  });
*/
  // 5. SIGNATURES PAGE
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

  //this.addFooter(doc);

  doc.save(`${this.title}_${this.report.districtName}_${this.report.year}.pdf`);
   /* const opt = {
      margin: 0.5,
      filename: `Final_Moderation_Report_${new Date().getFullYear()}.pdf`,
      image: { type: 'jpeg', quality: 0.98 },
      html2canvas: { scale: 2 },
      jsPDF: { unit: 'in', format: 'a4', orientation: 'landscape' }
    };
    html2pdf().from(element).set(opt).save();*/
  }

  private exportAsExcel(element: HTMLElement): void {
    const table = element.querySelector('table');
    if (!table) return;
    const worksheet = XLSX.utils.table_to_sheet(table, { raw: false });
  // Apply borders, vertical centering, and bold headers
  const range = XLSX.utils.decode_range(worksheet['!ref'] ?? '');
  for (let R = range.s.r; R <= range.e.r; ++R) {
    for (let C = range.s.c; C <= range.e.c; ++C) {
      const cellAddress = { r: R, c: C };
      const cellRef = XLSX.utils.encode_cell(cellAddress);
      const cell = worksheet[cellRef];

      if (!cell) continue;

      // Define standard style
      const style:any = {
        alignment: { vertical: 'center', wrapText: true },
        border: {
          top: { style: 'thin', color: { rgb: '000000' } },
          bottom: { style: 'thin', color: { rgb: '000000' } },
          left: { style: 'thin', color: { rgb: '000000' } },
          right: { style: 'thin', color: { rgb: '000000' } }
        }
      };

      // Make headers bold
      if (R === 0) {
        style.font = { bold: true };
      }

      cell.s = style;
    }
  }

    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, this.title);
    XLSX.writeFile(workbook, `${this.title}_${this.report.districtName}_${this.report.year}_${new Date().getFullYear()}.xlsx`);
  }
exportAsExcel2(element: HTMLElement): void{
    const tables = element.querySelectorAll('table');
    if (!tables.length) {
      console.warn('No tables found for Excel export');
      return;
    }
  
    const workbook = XLSX.utils.book_new();
  
    tables.forEach((table, index) => {
      const ws = XLSX.utils.table_to_sheet(table, { raw: true });
  
      // Optionally mark cells as read-only (Excel protection is limited via SheetJS)
      Object.keys(ws).forEach(key => {
        if (!key.startsWith('!')) {
          ws[key].s = { protection: { locked: true } };
        }
      });
  
      const sheetName = table.getAttribute('data-sheet-name') ?? `Sheet ${index + 1}`;
      XLSX.utils.book_append_sheet(workbook, ws, sheetName);
    });
  
    XLSX.writeFile(workbook, `${this.title}_${this.report.districtName}_${this.report.year}_${new Date().getFullYear()}.xlsx`);
  }
  

  private printElement(element: HTMLElement): void {
    const win = window.open('', '_blank');
    if (!win) return;

    win.document.write(`
      <html><head><title>Print</title>
        <style>
          body { font-family: Arial; padding: 40px; }
          table { border-collapse: collapse; width: 100%; }
          th, td { border: 1px solid #ccc; padding: 8px; }
          .page-break { page-break-before: always; }
          @media print {
            .page-break { page-break-before: always; }
          }
        </style>
      </head><body onload="window.print();window.close();">
        ${element.innerHTML}
      </body></html>
    `);
    win.document.close();
  }
  hasNonGoodQuality(name: string): boolean {
    return this.groupedPlantRates.some(rate =>
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
}
