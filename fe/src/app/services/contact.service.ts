import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Contact, ContactGroup, RecentTransfer } from '../models/contact.model';

@Injectable({
  providedIn: 'root'
})
export class ContactService {
  private contacts: Contact[] = [];
  private recentTransfers: RecentTransfer[] = [];
  
  private contactsSubject = new BehaviorSubject<Contact[]>([]);
  private recentTransfersSubject = new BehaviorSubject<RecentTransfer[]>([]);

  public contacts$ = this.contactsSubject.asObservable();
  public recentTransfers$ = this.recentTransfersSubject.asObservable();

  constructor() {
    this.loadContacts();
    this.loadRecentTransfers();
  }

  // Load contacts from localStorage
  private loadContacts(): void {
    const stored = localStorage.getItem('banking-contacts');
    if (stored) {
      this.contacts = JSON.parse(stored).map((c: any) => ({
        ...c,
        addedDate: new Date(c.addedDate),
        lastUsed: c.lastUsed ? new Date(c.lastUsed) : undefined
      }));
    } else {
      // Initialize with some sample contacts
      this.contacts = this.getInitialContacts();
      this.saveContacts();
    }
    this.contactsSubject.next(this.contacts);
  }

  // Load recent transfers from localStorage
  private loadRecentTransfers(): void {
    const stored = localStorage.getItem('banking-recent-transfers');
    if (stored) {
      this.recentTransfers = JSON.parse(stored).map((t: any) => ({
        ...t,
        date: new Date(t.date),
        contact: {
          ...t.contact,
          addedDate: new Date(t.contact.addedDate),
          lastUsed: t.contact.lastUsed ? new Date(t.contact.lastUsed) : undefined
        }
      }));
    }
    this.recentTransfersSubject.next(this.recentTransfers);
  }

  // Save contacts to localStorage
  private saveContacts(): void {
    localStorage.setItem('banking-contacts', JSON.stringify(this.contacts));
    this.contactsSubject.next(this.contacts);
  }

  // Save recent transfers to localStorage
  private saveRecentTransfers(): void {
    localStorage.setItem('banking-recent-transfers', JSON.stringify(this.recentTransfers));
    this.recentTransfersSubject.next(this.recentTransfers);
  }

  // Get all contacts
  getContacts(): Observable<Contact[]> {
    return this.contacts$;
  }

  // Get contacts grouped by first letter
  getContactsGrouped(): Observable<ContactGroup[]> {
    return new Observable(observer => {
      this.contacts$.subscribe(contacts => {
        const grouped = this.groupContactsByLetter(contacts);
        observer.next(grouped);
      });
    });
  }

  // Get favorite contacts
  getFavoriteContacts(): Observable<Contact[]> {
    return new Observable(observer => {
      this.contacts$.subscribe(contacts => {
        const favorites = contacts.filter(c => c.isFavorite);
        observer.next(favorites);
      });
    });
  }

  // Get recent transfers
  getRecentTransfers(): Observable<RecentTransfer[]> {
    return this.recentTransfers$;
  }

  // Add new contact
  addContact(contact: Omit<Contact, 'id' | 'addedDate'>): Contact {
    const newContact: Contact = {
      ...contact,
      id: this.generateId(),
      addedDate: new Date()
    };
    
    this.contacts.push(newContact);
    this.saveContacts();
    return newContact;
  }

  // Update contact
  updateContact(contactId: string, updates: Partial<Contact>): void {
    const index = this.contacts.findIndex(c => c.id === contactId);
    if (index !== -1) {
      this.contacts[index] = { ...this.contacts[index], ...updates };
      this.saveContacts();
    }
  }

  // Delete contact
  deleteContact(contactId: string): void {
    this.contacts = this.contacts.filter(c => c.id !== contactId);
    this.saveContacts();
  }

  // Toggle favorite
  toggleFavorite(contactId: string): void {
    const contact = this.contacts.find(c => c.id === contactId);
    if (contact) {
      contact.isFavorite = !contact.isFavorite;
      this.saveContacts();
    }
  }

  // Search contacts
  searchContacts(query: string): Contact[] {
    if (!query.trim()) {
      return this.contacts;
    }

    const searchTerm = query.toLowerCase();
    return this.contacts.filter(contact => 
      contact.firstName.toLowerCase().includes(searchTerm) ||
      contact.lastName.toLowerCase().includes(searchTerm) ||
      contact.taxCode.toLowerCase().includes(searchTerm) ||
      (contact.email && contact.email.toLowerCase().includes(searchTerm))
    );
  }

  // Get contact by tax code
  getContactByTaxCode(taxCode: string): Contact | undefined {
    return this.contacts.find(c => c.taxCode === taxCode);
  }

  // Add recent transfer
  addRecentTransfer(transfer: Omit<RecentTransfer, 'id' | 'date'>): void {
    const newTransfer: RecentTransfer = {
      ...transfer,
      id: this.generateId(),
      date: new Date()
    };

    // Update contact last used date
    this.updateContact(transfer.contactId, { lastUsed: new Date() });

    // Add to recent transfers (keep only last 10)
    this.recentTransfers.unshift(newTransfer);
    this.recentTransfers = this.recentTransfers.slice(0, 10);
    this.saveRecentTransfers();
  }

  // Group contacts by first letter
  private groupContactsByLetter(contacts: Contact[]): ContactGroup[] {
    const sorted = contacts.sort((a, b) => 
      `${a.firstName} ${a.lastName}`.localeCompare(`${b.firstName} ${b.lastName}`)
    );

    const groups: { [key: string]: Contact[] } = {};
    
    sorted.forEach(contact => {
      const letter = contact.firstName.charAt(0).toUpperCase();
      if (!groups[letter]) {
        groups[letter] = [];
      }
      groups[letter].push(contact);
    });

    return Object.keys(groups)
      .sort()
      .map(letter => ({ letter, contacts: groups[letter] }));
  }

  // Generate unique ID
  private generateId(): string {
    return 'contact_' + Date.now().toString(36) + Math.random().toString(36).substr(2);
  }

  // Get initial sample contacts
  private getInitialContacts(): Contact[] {
    return [
      {
        id: 'contact_1',
        firstName: 'Mario',
        lastName: 'Rossi',
        taxCode: 'RSSMRA85M01H501U',
        email: 'mario.rossi@email.com',
        phone: '+39 335 1234567',
        iban: 'IT60X0542811101000000123456',
        isFavorite: true,
        addedDate: new Date('2025-01-15')
      },
      {
        id: 'contact_2',
        firstName: 'Giuseppe',
        lastName: 'Bianchi',
        taxCode: 'BNCGPP90A01F205X',
        email: 'giuseppe.bianchi@email.com',
        phone: '+39 340 9876543',
        iban: 'IT28A0300203280123456789012',
        isFavorite: false,
        addedDate: new Date('2025-02-10')
      },
      {
        id: 'contact_3',
        firstName: 'Anna',
        lastName: 'Verdi',
        taxCode: 'VRDNNA88H41F205Y',
        email: 'anna.verdi@email.com',
        phone: '+39 347 5551234',
        iban: 'IT90Y0500811101000000654321',
        isFavorite: true,
        addedDate: new Date('2025-03-05')
      },
      {
        id: 'contact_4',
        firstName: 'Luigi',
        lastName: 'Neri',
        taxCode: 'NRELGU92B15H501Z',
        email: 'luigi.neri@email.com',
        phone: '+39 338 7777888',
        iban: 'IT75S0100812101000000998877',
        isFavorite: false,
        addedDate: new Date('2025-04-20')
      }
    ];
  }
}
