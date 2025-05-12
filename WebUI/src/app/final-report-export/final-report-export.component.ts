import { Component } from '@angular/core';
import { ExportService } from '../export.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-final-report-export',
  standalone: false,
  templateUrl: './final-report-export.component.html',
  styleUrl: './final-report-export.component.css'
})
export class FinalReportExportComponent {
  exportType: 'pdf' | 'excel' | 'print' = 'pdf';
  previewOnly = false;
  loading = true;

  constructor(
    public exportService: ExportService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.exportType = (params['type'] as any) ?? 'pdf';
      this.previewOnly = params['preview'] === 'true';

      setTimeout(() => {
        this.loading = false;
        this.exportService.triggerExport(this.exportType, this.previewOnly);
        if (!this.previewOnly) {
          setTimeout(() => this.close(), 3000);
        }
      }, 1200);
    });
  }

  close(): void {
    this.router.navigate([{ outlets: { print: null } }]);
  }
}
