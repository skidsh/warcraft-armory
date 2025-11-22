import { Injectable, signal } from '@angular/core';
import { Region } from '../models';

@Injectable({
  providedIn: 'root'
})
export class RegionService {
  private readonly REGION_STORAGE_KEY = 'selectedRegion';
  
  readonly selectedRegion = signal<Region>(this.getStoredRegion());
  readonly availableRegions: Region[] = [
    Region.US,
    Region.EU,
    Region.KR,
    Region.TW,
    Region.CN
  ];

  private getStoredRegion(): Region {
    const stored = localStorage.getItem(this.REGION_STORAGE_KEY);
    return (stored as Region) || Region.US;
  }

  setRegion(region: Region): void {
    this.selectedRegion.set(region);
    localStorage.setItem(this.REGION_STORAGE_KEY, region);
  }

  getRegionLabel(region: Region): string {
    const labels: Record<Region, string> = {
      [Region.US]: 'Americas',
      [Region.EU]: 'Europe',
      [Region.KR]: 'Korea',
      [Region.TW]: 'Taiwan',
      [Region.CN]: 'China'
    };
    return labels[region];
  }
}
