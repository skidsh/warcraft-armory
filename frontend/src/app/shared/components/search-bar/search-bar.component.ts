import { Component, input, output } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './search-bar.component.html',
  styleUrl: './search-bar.component.scss'
})
export class SearchBarComponent {
  placeholder = input<string>('Search...');
  debounce = input<number>(300);
  
  search = output<string>();
  searchControl = new FormControl('');

  constructor() {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(this.debounce()),
        distinctUntilChanged()
      )
      .subscribe((value) => {
        if (value && value.trim().length > 0) {
          this.search.emit(value.trim());
        }
      });
  }

  onSearch(): void {
    const value = this.searchControl.value;
    if (value && value.trim().length > 0) {
      this.search.emit(value.trim());
    }
  }

  onClear(): void {
    this.searchControl.setValue('');
    this.search.emit('');
  }
}
