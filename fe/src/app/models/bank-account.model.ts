export interface BankAccount {
  id: number;
  iban: string;
  accountName: string;
  accountType: string;
  balance: number;
  openDate: Date;
  lastUpdated: Date;
  isActive: boolean;
  customerId: number;
  customerName?: string;
} 