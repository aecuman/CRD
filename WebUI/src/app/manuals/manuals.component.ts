import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { marked } from 'marked';

interface Manual {
  label: string;
  file: string;
  icon: string;
}

@Component({
  selector: 'app-manuals',
  standalone: false,
  templateUrl: './manuals.component.html',
  styleUrl: './manuals.component.css'
})
export class ManualsComponent implements OnInit {
  manuals: Manual[] = [
    { label: 'User Manual', file: 'assets/docs/user-manual.md', icon: '📖' },
    { label: 'System Admin Manual', file: 'assets/docs/system-admin-manual.md', icon: '🛠️' },
    { label: 'Mobile App Manual', file: 'assets/docs/mobile-app-manual.md', icon: '📱' },
  ];

  activeIndex = 0;
  renderedContent: SafeHtml = '';
  loading = false;
  error = false;

  constructor(private http: HttpClient, private sanitizer: DomSanitizer) {}

  ngOnInit(): void {
    this.loadManual(this.activeIndex);
  }

  selectTab(index: number): void {
    if (this.activeIndex === index) return;
    this.activeIndex = index;
    this.loadManual(index);
  }

  private loadManual(index: number): void {
    this.loading = true;
    this.error = false;
    this.renderedContent = '';

    this.http.get(this.manuals[index].file, { responseType: 'text' }).subscribe({
      next: (markdown) => {
        const html = marked.parse(markdown) as string;
        this.renderedContent = this.sanitizer.bypassSecurityTrustHtml(html);
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }
}
