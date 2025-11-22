import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RegionSelectorComponent } from '@shared/components/region-selector/region-selector.component';
import { RegionService } from '@core/services/region.service';
import { Region } from '@core/models/character.model';

@Component({
  selector: 'app-character-search',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    RegionSelectorComponent
  ],
  templateUrl: './character-search.component.html',
  styleUrl: './character-search.component.scss'
})
export class CharacterSearchComponent {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly regionService = inject(RegionService);

  selectedRegion = this.regionService.selectedRegion;

  searchForm: FormGroup = this.fb.group({
    realm: ['', [Validators.required, Validators.minLength(2)]],
    name: ['', [Validators.required, Validators.minLength(2)]]
  });

  onRegionChange(region: Region): void {
    this.regionService.setRegion(region);
  }

  onSubmit(): void {
    if (this.searchForm.valid) {
      const { realm, name } = this.searchForm.value;
      const region = this.selectedRegion();
      this.router.navigate(['/characters', region, realm.toLowerCase(), name.toLowerCase()]);
    }
  }
}
