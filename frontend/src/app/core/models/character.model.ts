export interface Character {
  id: number;
  name: string;
  realm: string;
  region: string;
  level: number;
  class: string; // Backend returns string
  race: string; // Backend returns string
  faction: string; // Backend returns string
  gender: string; // Backend returns string
  achievementPoints: number;
  averageItemLevel: number;
  equippedItemLevel: number;
  thumbnailUrl?: string;
  guildId?: number;
  lastModified?: Date;
}

export enum CharacterClass {
  Warrior = 'Warrior',
  Paladin = 'Paladin',
  Hunter = 'Hunter',
  Rogue = 'Rogue',
  Priest = 'Priest',
  Shaman = 'Shaman',
  Mage = 'Mage',
  Warlock = 'Warlock',
  Monk = 'Monk',
  Druid = 'Druid',
  DemonHunter = 'DemonHunter',
  DeathKnight = 'DeathKnight',
  Evoker = 'Evoker'
}

export enum CharacterRace {
  Human = 'Human',
  Orc = 'Orc',
  Dwarf = 'Dwarf',
  NightElf = 'NightElf',
  Undead = 'Undead',
  Tauren = 'Tauren',
  Gnome = 'Gnome',
  Troll = 'Troll',
  BloodElf = 'BloodElf',
  Draenei = 'Draenei',
  Goblin = 'Goblin',
  Worgen = 'Worgen',
  Pandaren = 'Pandaren',
  VoidElf = 'VoidElf',
  LightforgedDraenei = 'LightforgedDraenei',
  HighmountainTauren = 'HighmountainTauren',
  Nightborne = 'Nightborne',
  MagharOrc = 'MagharOrc',
  DarkIronDwarf = 'DarkIronDwarf',
  KulTiran = 'KulTiran',
  ZandalariTroll = 'ZandalariTroll',
  Vulpera = 'Vulpera',
  Mechagnome = 'Mechagnome',
  Dracthyr = 'Dracthyr',
  EarthenDwarf = 'EarthenDwarf'
}

export enum Faction {
  Alliance = 'Alliance',
  Horde = 'Horde',
  Neutral = 'Neutral'
}

export enum Gender {
  Male = 'Male',
  Female = 'Female'
}

export enum Region {
  US = 'us',
  EU = 'eu',
  KR = 'kr',
  TW = 'tw',
  CN = 'cn'
}
