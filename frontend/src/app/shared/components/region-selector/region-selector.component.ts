import { Component, input, output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { Region } from '@core/models/character.model';
import { RegionService } from '@core/services/region.service';

@Component({
  selector: 'app-region-selector',
  standalone: true,
  imports: [CommonModule, MatSelectModule, MatFormFieldModule],
  templateUrl: './region-selector.component.html',
  styleUrl: './region-selector.component.scss'
})
export class RegionSelectorComponent {
  private readonly regionService = inject(RegionService);
  
  selectedRegion = input<Region>(this.regionService.selectedRegion());
  availableRegions = input<Region[]>(this.regionService.availableRegions);
  
  regionChange = output<Region>();

  onRegionChange(region: Region): void {
    this.regionService.setRegion(region);
    this.regionChange.emit(region);
  }

  getRegionLabel(region: Region): string {
    return this.regionService.getRegionLabel(region);
  }
}
