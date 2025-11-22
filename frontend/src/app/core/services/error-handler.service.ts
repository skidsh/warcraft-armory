import { Injectable, signal } from '@angular/core';
import { ApiError } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {
  readonly currentError = signal<ApiError | null>(null);

  handleError(error: any): void {
    console.error('Error occurred:', error);
    
    if (error.error) {
      // API error from backend
      this.currentError.set(error.error as ApiError);
    } else if (error.status === 0) {
      // Network error
      this.currentError.set({
        type: 'NetworkError',
        title: 'Network Error',
        status: 0,
        detail: 'Unable to connect to the server. Please check your internet connection.'
      });
    } else {
      // Generic error
      this.currentError.set({
        type: 'Error',
        title: 'An Error Occurred',
        status: error.status || 500,
        detail: error.message || 'An unexpected error occurred. Please try again.'
      });
    }
  }

  clearError(): void {
    this.currentError.set(null);
  }

  formatErrorMessage(error: ApiError): string {
    if (error.errors) {
      const errorMessages = Object.values(error.errors).flat();
      return errorMessages.join(', ');
    }
    return error.detail || error.title;
  }
}
