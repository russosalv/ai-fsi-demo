export interface Balance {
  iban: string;
  accountName: string;
  accountType: string;
  balance: number;
  lastUpdated: Date;
  isActive: boolean;
  currency: string;
} 