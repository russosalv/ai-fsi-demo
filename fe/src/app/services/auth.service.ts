import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { BankingApiService } from './banking-api.service';
import { BankAccount } from '../models/bank-account.model';
import { LoginRequest } from '../models/login.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<any>(null);
  private bankAccountsSubject = new BehaviorSubject<BankAccount[]>([]);
  
  public currentUser$ = this.currentUserSubject.asObservable();
  public bankAccounts$ = this.bankAccountsSubject.asObservable();

  constructor(private bankingApiService: BankingApiService) {
    // Check for existing session
    const savedUser = localStorage.getItem('currentUser');
    const savedAccounts = localStorage.getItem('bankAccounts');
    
    if (savedUser && savedAccounts) {
      this.currentUserSubject.next(JSON.parse(savedUser));
      this.bankAccountsSubject.next(JSON.parse(savedAccounts));
    }
  }

  login(loginRequest: LoginRequest): Observable<{ success: boolean; message?: string }> {
    // Note: We ignore the password as per requirements (any password is accepted)
    return this.bankingApiService.getBankAccountsByTaxCode(loginRequest.taxCode).pipe(
      map(bankAccounts => {
        if (bankAccounts && bankAccounts.length > 0) {
          // Login successful
          const user = {
            taxCode: loginRequest.taxCode,
            customerName: bankAccounts[0].customerName,
            customerId: bankAccounts[0].customerId
          };
          
          // Save to localStorage and update subjects
          localStorage.setItem('currentUser', JSON.stringify(user));
          localStorage.setItem('bankAccounts', JSON.stringify(bankAccounts));
          
          this.currentUserSubject.next(user);
          this.bankAccountsSubject.next(bankAccounts);
          
          return { success: true };
        } else {
          return { success: false, message: 'Utente inesistente' };
        }
      }),
      catchError(error => {
        console.error('Login error:', error);
        if (error.status === 404) {
          return of({ success: false, message: 'Utente inesistente' });
        }
        return of({ success: false, message: 'Errore durante il login' });
      })
    );
  }

  logout(): void {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('bankAccounts');
    this.currentUserSubject.next(null);
    this.bankAccountsSubject.next([]);
  }

  isAuthenticated(): boolean {
    return this.currentUserSubject.value !== null;
  }

  getCurrentUser(): any {
    return this.currentUserSubject.value;
  }

  getBankAccounts(): BankAccount[] {
    return this.bankAccountsSubject.value;
  }
} 