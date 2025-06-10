import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BankAccount } from '../models/bank-account.model';
import { Balance } from '../models/balance.model';
import { Customer } from '../models/customer.model';
import { appConfig } from '../config/app.config';

@Injectable({
  providedIn: 'root'
})
export class BankingApiService {
  private get apiUrl(): string {
    return appConfig.apiUrl;
  }

  constructor(private http: HttpClient) { }

  // Get bank accounts by tax code (for login)
  getBankAccountsByTaxCode(taxCode: string): Observable<BankAccount[]> {
    return this.http.get<BankAccount[]>(`${this.apiUrl}/customers/taxcode/${taxCode}/bank-accounts`);
  }

  // Get balance by IBAN
  getBalanceByIban(iban: string): Observable<Balance> {
    return this.http.get<Balance>(`${this.apiUrl}/bankaccounts/iban/${iban}/balance`);
  }

  // Get balances for multiple IBANs
  getBalancesByIbans(ibans: string[]): Observable<Balance[]> {
    const ibansList = ibans.join(',');
    return this.http.get<Balance[]>(`${this.apiUrl}/bankaccounts/balances?ibans=${ibansList}`);
  }

  // Get all customers
  getCustomers(): Observable<Customer[]> {
    return this.http.get<Customer[]>(`${this.apiUrl}/customers`);
  }

  // Get customer by ID
  getCustomerById(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.apiUrl}/customers/${id}`);
  }
} 