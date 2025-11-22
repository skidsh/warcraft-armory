import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '@core/services/api.service';
import { Character } from '@core/models/character.model';

@Injectable({
  providedIn: 'root'
})
export class CharacterService {
  private readonly apiService = inject(ApiService);

  getCharacter(region: string, realm: string, name: string): Observable<Character> {
    return this.apiService.getCharacter(region, realm, name);
  }
}
