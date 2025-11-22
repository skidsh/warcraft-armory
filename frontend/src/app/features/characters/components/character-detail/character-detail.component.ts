import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { ErrorMessageComponent } from '@shared/components/error-message/error-message.component';
import { CharacterClassPipe } from '@shared/pipes/character-class.pipe';
import { CharacterService } from '../../services/character.service';
import { Character } from '@core/models/character.model';
import { ErrorHandlerService } from '@core/services/error-handler.service';

@Component({
  selector: 'app-character-detail',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDividerModule,
    ErrorMessageComponent,
    CharacterClassPipe
  ],
  templateUrl: './character-detail.component.html',
  styleUrl: './character-detail.component.scss'
})
export class CharacterDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly characterService = inject(CharacterService);
  readonly errorHandler = inject(ErrorHandlerService);

  character = signal<Character | null>(null);
  error = this.errorHandler.currentError;

  private readonly classColors: Record<string, string> = {
    'Warrior': '#C79C6E',
    'Paladin': '#F58CBA',
    'Hunter': '#ABD473',
    'Rogue': '#FFF569',
    'Priest': '#FFFFFF',
    'Shaman': '#0070DE',
    'Mage': '#69CCF0',
    'Warlock': '#9482C9',
    'Monk': '#00FF96',
    'Druid': '#FF7D0A',
    'DemonHunter': '#A330C9',
    'Demon Hunter': '#A330C9',
    'DeathKnight': '#C41F3B',
    'Death Knight': '#C41F3B',
    'Evoker': '#33937F'
  };

  ngOnInit(): void {
    this.loadCharacter();
  }

  loadCharacter(): void {
    const region = this.route.snapshot.paramMap.get('region')!;
    const realm = this.route.snapshot.paramMap.get('realm')!;
    const name = this.route.snapshot.paramMap.get('name')!;

    this.characterService.getCharacter(region, realm, name).subscribe({
      next: (character) => {
        this.character.set(character);
        this.errorHandler.clearError();
      },
      error: () => {
        // Error is handled by interceptor
        this.character.set(null);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/characters']);
  }

  getClassColor(className: string): string {
    return this.classColors[className] || '#FFFFFF';
  }
}
