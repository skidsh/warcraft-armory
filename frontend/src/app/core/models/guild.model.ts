import { Faction } from './character.model';

export interface Guild {
  id: number;
  name: string;
  realm: string;
  region: string;
  faction: Faction;
  memberCount: number;
  achievementPoints: number;
  createdAt?: Date;
  description?: string;
}
