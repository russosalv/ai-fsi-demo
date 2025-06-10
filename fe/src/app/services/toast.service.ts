import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Toast {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message: string;
  duration?: number;
  dismissible?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private toasts: Toast[] = [];
  private toastsSubject = new BehaviorSubject<Toast[]>([]);
  public toasts$ = this.toastsSubject.asObservable();

  private defaultDuration = 5000; // 5 secondi

  constructor() {}

  // Mostra toast di successo
  showSuccess(title: string, message: string, duration?: number): void {
    this.addToast({
      type: 'success',
      title,
      message,
      duration: duration || this.defaultDuration,
      dismissible: true
    });
  }

  // Mostra toast di errore
  showError(title: string, message: string, duration?: number): void {
    this.addToast({
      type: 'error',
      title,
      message,
      duration: duration || this.defaultDuration * 2, // Errori durano di più
      dismissible: true
    });
  }

  // Mostra toast di warning
  showWarning(title: string, message: string, duration?: number): void {
    this.addToast({
      type: 'warning',
      title,
      message,
      duration: duration || this.defaultDuration,
      dismissible: true
    });
  }

  // Mostra toast informativo
  showInfo(title: string, message: string, duration?: number): void {
    this.addToast({
      type: 'info',
      title,
      message,
      duration: duration || this.defaultDuration,
      dismissible: true
    });
  }

  // Mostra toast di validazione (multipli errori)
  showValidationErrors(errors: string[]): void {
    if (errors.length === 1) {
      this.showError('Errore di Validazione', errors[0]);
    } else if (errors.length > 1) {
      const message = errors.map((error, index) => `${index + 1}. ${error}`).join('\n');
      this.showError(
        `Errori di Validazione (${errors.length})`,
        message,
        this.defaultDuration * 2
      );
    }
  }

  // Aggiunge un toast alla lista
  private addToast(toast: Omit<Toast, 'id'>): void {
    const id = this.generateId();
    const newToast: Toast = { ...toast, id };
    
    this.toasts.unshift(newToast); // Aggiunge in cima
    
    // Limita il numero di toast visibili
    if (this.toasts.length > 5) {
      this.toasts = this.toasts.slice(0, 5);
    }
    
    this.toastsSubject.next([...this.toasts]);

    // Auto-dismiss se ha durata
    if (newToast.duration && newToast.duration > 0) {
      setTimeout(() => {
        this.removeToast(id);
      }, newToast.duration);
    }
  }

  // Rimuove un toast specifico
  removeToast(id: string): void {
    this.toasts = this.toasts.filter(toast => toast.id !== id);
    this.toastsSubject.next([...this.toasts]);
  }

  // Rimuove tutti i toast
  clearAll(): void {
    this.toasts = [];
    this.toastsSubject.next([]);
  }

  // Genera ID univoco
  private generateId(): string {
    return 'toast_' + Date.now().toString(36) + Math.random().toString(36).substr(2);
  }
}
