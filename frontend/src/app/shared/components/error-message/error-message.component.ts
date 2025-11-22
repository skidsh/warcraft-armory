import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ApiError } from '@core/models/api-response.model';

@Component({
  selector: 'app-error-message',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule],
  templateUrl: './error-message.component.html',
  styleUrl: './error-message.component.scss'
})
export class ErrorMessageComponent {
  error = input<ApiError | null>(null);
  showRetry = input<boolean>(true);
  
  retry = output<void>();
  dismiss = output<void>();

  getErrorMessages(error: ApiError): string[] {
    if (!error.errors) return [];
    return Object.values(error.errors).flat();
  }
}
