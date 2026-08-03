import { Component } from '@angular/core';
import * as L from 'leaflet';
import { APIService, PublishedRateSummaryDto } from '../api.service';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-crd-map',
  standalone: false,
  templateUrl: './crd-map.component.html',
  styleUrl: './crd-map.component.css'
})
export class CrdMapComponent {
  private map!: L.Map;
  private geoLayer!: L.GeoJSON;
  districtStatusMap: { [key: string]: PublishedRateSummaryDto } = {};
  filter: string = 'all';
  geojson: any;  
  expiringSoonDistricts = new Set<string>();
  metrics: PublishedRateMetrics = {
    total: 0,
    valid: 0,
    expired: 0,
    notPublished: 0,
    inWorkflow: 0,
    expiringSoon: 0
  };

  /**
   *
   */
  constructor(private api:APIService,private http: HttpClient,private router:Router) {
    // Initialize the map and load districts
    
    
  }
  ngOnInit(): void {
    this.loadMap();
    this.loadDistricts();
    this.addLegend();
  }

  loadMap(): void {
    this.map = L.map('map').setView([1.3, 32.5], 7); // Adjust center as needed

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 18
    }).addTo(this.map);
  }

  loadDistricts(): void {
    this.http.get('/assets/districts.json').subscribe(geo => {
      this.geojson = geo;
      this.refreshLayer();
    });

    //this.loading = true;
    this.api.getPublishedRates(this.filter).subscribe({
      next: rates => {
        this.districtStatusMap = {};
        const now = new Date();
const cutoff = new Date(now.getFullYear() + 1, now.getMonth(), now.getDate());
        for (const rate of rates) {
          // Normalize district name to uppercase and trim whitespace
          // to ensure consistent matching with geojson properties
          if (!rate.districtName) continue;
          this.districtStatusMap[rate.districtName.trim().toUpperCase()] = rate;
          const validTo = rate.year ? rate.year +1: null;
          if (validTo && (validTo+1) == cutoff.getFullYear()) {
            this.expiringSoonDistricts.add(rate?.districtName?.trim().toUpperCase());
          }
        }
        this.computeMetrics(rates)
        this.refreshLayer();
      },
      error: () =>{} //this.loading = false
    });
  
  }

  refreshLayer(): void {
    if (!this.geojson || !this.districtStatusMap) return;

    if (this.geoLayer) this.geoLayer.remove();

    this.geoLayer = L.geoJSON(this.geojson, {
      style: (feature: any) => {
        const name = feature.properties.District_3?.trim().toUpperCase();
        const status = this.districtStatusMap[name]?.status?.toLowerCase();
        const strokeColor = this.expiringSoonDistricts.has(name) ? 'blue' : '#333';

        return {
          weight: 1,
          color: strokeColor,
          fillColor: this.getColorByStatus(status),
          fillOpacity: 0.6
        };
      },
      onEachFeature: (feature, layer) => {
        const name = feature.properties.District_3?.trim().toUpperCase();
        const status = this.districtStatusMap[name];
        const nextYear = status?.year ? Number(status.year) + 1 : null;


        layer.bindPopup(`
          <strong>${name}</strong><br>
          Status: ${status?.status ?? 'Unknown'}<br>
          Year: ${ (status?.year ? status.year+'/'+nextYear : 'N/A') }<br>`
         + (nextYear&&((nextYear+1)==(new Date().getFullYear()+1))?'<span class="text-blue-500">Expiring Soon</span>':''))
        // + status?.inWorkflowProcess?(`<span class="text-blue-500">${status?.currentWorkflowStatusName}</span>`:'');
        layer.on('click', () => {
          /* if (status?.districtId) {
            this.router.navigate(['/portal/district', status.districtId, 'published']);
          } */
        });
        
      },
      
      
    });

    this.geoLayer.addTo(this.map);
  }

  getColorByStatus(status?: string): string {
    switch (status) {
      case 'published': return '#38a169'; // green
      case 'expired': return '#d69e2e'; // yellow
      case 'not published': return '#e53e3e'; // red
      case 'under review': return '#2877df'
      default: return '#cbd5e0'; // gray
    }
  }

  applyFilter(f: string): void {
    this.filter = f;
    this.loadDistricts();
  }
  addLegend(): void {
    const legend = new L.Control({ position: 'bottomright' });
  
    legend.onAdd = () => {
      const div = L.DomUtil.create('div', 'info legend bg-white p-3 rounded shadow text-sm leading-5');
      div.innerHTML = `
        <strong>Status Legend</strong><br/>
        <span class="inline-block w-3 h-3 bg-[#38a169] mr-2"></span> Published<br/>
        <span class="inline-block w-3 h-3 bg-[#2877df] mr-2"></span> Under Review<br/>
        <span class="inline-block w-3 h-3 bg-[#d69e2e] mr-2"></span> Expired<br/>
        <span class="inline-block w-3 h-3 bg-[#e53e3e] mr-2"></span> Not Published<br/>
        <span class="inline-block w-3 h-3 bg-white border border-blue-600 mr-2"></span> Expiring Soon<br/>
        <span class="inline-block w-3 h-3 bg-[#cbd5e0] mr-2"></span> Unknown
      `;
      return div;
    };
  
    legend.addTo(this.map);
  }
  computeMetrics(rates:any[]): void {
    const now = new Date();
    const cutoff = new Date(now.getFullYear() + 1, now.getMonth(), now.getDate());
  console.log(rates.filter(r => r.inWorkflowProcess || (r.status !== 'Published' && r.status !== 'Expired')))
    this.metrics.total = rates.length;
    this.metrics.valid = rates.filter(r => r.year && !r.isExpired).length;
    this.metrics.expired = rates.filter(r => r.isExpired).length;
    this.metrics.notPublished = rates.filter(r => !r.year).length;
    this.metrics.inWorkflow = rates.filter(r => r.inWorkflowProcess && r.status !== 'Published' && r.status !== 'Expired').length;
    this.metrics.expiringSoon = rates.filter(r =>
      r.year && (r.year+2) == (now.getFullYear()+1)).length;
  }
  
}
interface PublishedRateMetrics {
  total: number;
  valid: number;
  expired: number;
  notPublished: number;
  inWorkflow: number;
  expiringSoon: number;
}

