import { Component, Input, OnInit } from '@angular/core';
const html2pdf = require('html2pdf.js');
import * as XLSX from 'xlsx';
import { Router } from '@angular/router';
import { ExportService } from '../export.service';

@Component({
  selector: 'app-final-report',
  standalone: false,
  templateUrl: './final-report.component.html',
  styleUrl: './final-report.component.css'
})
export class FinalReportComponent {
  @Input() report: any; // Assume injected from service
  @Input() groupedPlantRates: any[] = [];
  @Input() groupedStructureRates: any[] = [];
  @Input() title: string = 'Final Adjusted';
 // @Input() previewOnly = false;
 // @Input() exportType: 'pdf' | 'excel' | 'print' = 'pdf';
  today = new Date();
  growth_list: any[]=[];

  constructor(private router: Router, private exportService:ExportService) {
    this.growth_list = this.exportService.growth_list
  }

  ngOnInit(): void {
 /*    setTimeout(() => {
      this.report = this.exportService.report;

      if (!this.report) {
        console.error('Report data not found!');
        return;
      }
      this.exportService.triggerExport(this.exportType, this.previewOnly);
      
  
      if (!this.previewOnly) {
        setTimeout(() => {
          this.router.navigate([{ outlets: { print: null } }]);
        }, 3000);
      }
    }, 1500); */
  }
  getGroupStageName(id:number){
    return this.growth_list?.find(x => x.id === id)?.name;
  }
  getStageName(id: number) {
    return this.growth_list.find(x => x.id === id)?.name ?? '';
  //  return this.plants.find(x => x.id === plantId)?.growthStages?.find(x => x.growthStageId === id)?.name ?? '';
    
  }
  hasNonGoodQuality(name: string): boolean {
    return this.groupedPlantRates.some(rate =>
      (rate.groupName === name || rate.plantName === name) &&
      rate.quality !== 'Good'
    );
  }
}
