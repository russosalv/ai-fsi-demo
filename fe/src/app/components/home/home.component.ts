import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { BankingApiService } from '../../services/banking-api.service';
import { BankAccount } from '../../models/bank-account.model';
import { Balance } from '../../models/balance.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  currentUser: any;
  bankAccounts: BankAccount[] = [];
  selectedIban: string = '';
  currentBalance: Balance | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private bankingApiService: BankingApiService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.bankAccounts = this.authService.getBankAccounts();

    if (this.bankAccounts.length > 0) {
      // Auto-select first IBAN
      this.selectedIban = this.bankAccounts[0].iban;
      this.loadBalance(this.selectedIban);
    }
  }

  onIbanSelect(iban: string): void {
    this.selectedIban = iban;
    this.loadBalance(iban);
  }

  loadBalance(iban: string): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.bankingApiService.getBalanceByIban(iban).subscribe({
      next: (balance) => {
        this.currentBalance = balance;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading balance:', error);
        this.errorMessage = 'Errore nel caricamento del saldo';
        this.isLoading = false;
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('it-IT', {
      style: 'currency',
      currency: 'EUR'
    }).format(amount);
  }

  formatDate(date: Date | string): string {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return new Intl.DateTimeFormat('it-IT', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(dateObj);
  }

  getAccountTypeLabel(type: string): string {
    switch (type) {
      case 'CHECKING':
        return 'Conto Corrente';
      case 'SAVINGS':
        return 'Conto Risparmio';
      default:
        return type;
    }
  }

  getSelectedAccount(): BankAccount | undefined {
    return this.bankAccounts.find(account => account.iban === this.selectedIban);
  }
  navigateToService(service: string): void {
    if (service === 'p2p-transfer') {
      this.router.navigate(['/p2p-transfer']);
    } else {
      // Placeholder per futuro routing ai servizi
      console.log(`Navigating to service: ${service}`);
      alert(`Servizio "${service}" non ancora implementato`);
    }
  }

  navigateToP2P(): void {
    this.router.navigate(['/p2p-transfer']);
  }
} 