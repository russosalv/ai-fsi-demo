export interface Contact {
  id: string;
  firstName: string;
  lastName: string;
  taxCode: string;
  email?: string;
  phone?: string;
  iban?: string;
  isFavorite: boolean;
  addedDate: Date;
  lastUsed?: Date;
}

export interface ContactGroup {
  letter: string;
  contacts: Contact[];
}

export interface RecentTransfer {
  id: string;
  contactId: string;
  contact: Contact;
  amount: number;
  description: string;
  date: Date;
  transactionId?: string;
}
