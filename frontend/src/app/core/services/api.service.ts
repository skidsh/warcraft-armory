import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';
import { Character } from '../models/character.model';
import { Item } from '../models/item.model';
import { Guild } from '../models/guild.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  // Character endpoints
  getCharacter(region: string, realm: string, name: string): Observable<Character> {
    return this.http.get<Character>(`${this.apiUrl}/characters/${region}/${realm}/${name}`);
  }

  // Item endpoints
  getItem(region: string, itemId: number): Observable<Item> {
    return this.http.get<Item>(`${this.apiUrl}/items/${region}/${itemId}`);
  }

  searchItems(region: string, query: string): Observable<Item[]> {
    const params = new HttpParams().set('q', query);
    return this.http.get<Item[]>(`${this.apiUrl}/items/${region}/search`, { params });
  }

  // Guild endpoints
  getGuild(region: string, realm: string, name: string): Observable<Guild> {
    return this.http.get<Guild>(`${this.apiUrl}/guilds/${region}/${realm}/${name}`);
  }

  // Health check
  checkHealth(): Observable<any> {
    return this.http.get(`${this.apiUrl.replace('/api', '')}/health`);
  }
}
