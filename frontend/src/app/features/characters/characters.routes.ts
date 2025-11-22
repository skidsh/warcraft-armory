import { Routes } from '@angular/router';

export const characterRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/character-search/character-search.component').then(
        (m) => m.CharacterSearchComponent
      )
  },
  {
    path: ':region/:realm/:name',
    loadComponent: () =>
      import('./components/character-detail/character-detail.component').then(
        (m) => m.CharacterDetailComponent
      )
  }
];
