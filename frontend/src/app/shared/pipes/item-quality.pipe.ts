import { Pipe, PipeTransform } from '@angular/core';
import { ItemQuality } from '@core/models/item.model';

@Pipe({
  name: 'itemQuality',
  standalone: true
})
export class ItemQualityPipe implements PipeTransform {
  private readonly qualityColors: Record<ItemQuality, string> = {
    [ItemQuality.Poor]: '#9d9d9d',
    [ItemQuality.Common]: '#ffffff',
    [ItemQuality.Uncommon]: '#1eff00',
    [ItemQuality.Rare]: '#0070dd',
    [ItemQuality.Epic]: '#a335ee',
    [ItemQuality.Legendary]: '#ff8000',
    [ItemQuality.Artifact]: '#e6cc80',
    [ItemQuality.Heirloom]: '#00ccff'
  };

  transform(quality: ItemQuality, type: 'color' | 'name' = 'name'): string {
    if (type === 'color') {
      return this.qualityColors[quality];
    }
    return quality;
  }
}
