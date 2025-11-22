import { Pipe, PipeTransform } from '@angular/core';
import { CharacterClass } from '@core/models/character.model';

@Pipe({
  name: 'characterClass',
  standalone: true
})
export class CharacterClassPipe implements PipeTransform {
  private readonly classColors: Record<CharacterClass, string> = {
    [CharacterClass.Warrior]: '#C79C6E',
    [CharacterClass.Paladin]: '#F58CBA',
    [CharacterClass.Hunter]: '#ABD473',
    [CharacterClass.Rogue]: '#FFF569',
    [CharacterClass.Priest]: '#FFFFFF',
    [CharacterClass.Shaman]: '#0070DE',
    [CharacterClass.Mage]: '#69CCF0',
    [CharacterClass.Warlock]: '#9482C9',
    [CharacterClass.Monk]: '#00FF96',
    [CharacterClass.Druid]: '#FF7D0A',
    [CharacterClass.DemonHunter]: '#A330C9',
    [CharacterClass.DeathKnight]: '#C41F3B',
    [CharacterClass.Evoker]: '#33937F'
  };

  transform(characterClass: CharacterClass | undefined | null, type: 'color' | 'name' = 'name'): string {
    if (!characterClass) {
      return type === 'color' ? '#FFFFFF' : 'Unknown';
    }
    
    if (type === 'color') {
      return this.classColors[characterClass] || '#FFFFFF';
    }
    return this.formatClassName(characterClass);
  }

  private formatClassName(characterClass: CharacterClass): string {
    if (!characterClass) return 'Unknown';
    // Convert PascalCase to space-separated words
    const str = String(characterClass);
    return str.replace(/([A-Z])/g, ' $1').trim();
  }
}
