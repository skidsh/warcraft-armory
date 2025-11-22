export interface Item {
  id: number;
  name: string;
  description?: string;
  quality: ItemQuality;
  itemLevel: number;
  requiredLevel: number;
  itemClass: string;
  itemSubclass: string;
  inventoryType: string;
  maxDurability?: number;
  sellPrice?: number;
  previewItemId?: number;
}

export enum ItemQuality {
  Poor = 'Poor',
  Common = 'Common',
  Uncommon = 'Uncommon',
  Rare = 'Rare',
  Epic = 'Epic',
  Legendary = 'Legendary',
  Artifact = 'Artifact',
  Heirloom = 'Heirloom'
}
