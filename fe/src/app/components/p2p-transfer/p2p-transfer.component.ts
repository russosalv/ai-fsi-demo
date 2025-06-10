import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';

import { BankingApiService } from '../../services/banking-api.service';
import { ContactService } from '../../services/contact.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import { Contact, RecentTransfer } from '../../models/contact.model';
import { P2PTransferRequest, P2PTransferResponse, ValidationResult } from '../../models/p2p.model';
import { Customer } from '../../models/customer.model';

@Component({
  selector: 'app-p2p-transfer',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  template: `
    <div class="p2p-transfer-container">
      <!-- Header -->
      <div class="header">
        <button class="back-btn" (click)="goBack()">
          <span class="icon">←</span>
        </button>
        <h1>Trasferimento P2P</h1>
      </div>

      <!-- Transfer Form -->
      <div class="transfer-form" *ngIf="!showSuccess">
        <form [formGroup]="transferForm" (ngSubmit)="onSubmit()">
          
          <!-- Recipient Selection -->
          <div class="form-section">
            <h2>Destinatario</h2>
            
            <!-- Search Bar -->
            <div class="search-container">
              <input 
                type="text" 
                class="search-input"
                placeholder="Cerca per nome o codice fiscale..."
                [(ngModel)]="searchQuery"
                [ngModelOptions]="{standalone: true}"
                (input)="onSearchChange($event)">
              <span class="search-icon">🔍</span>
            </div>

            <!-- Quick Options -->
            <div class="quick-options">
              <button 
                type="button" 
                class="option-btn"
                [class.active]="activeTab === 'favorites'"
                (click)="setActiveTab('favorites')">
                Preferiti
              </button>
              <button 
                type="button" 
                class="option-btn"
                [class.active]="activeTab === 'recent'"
                (click)="setActiveTab('recent')">
                Recenti
              </button>
              <button 
                type="button" 
                class="option-btn"
                [class.active]="activeTab === 'all'"
                (click)="setActiveTab('all')">
                Tutti i contatti
              </button>
              <button 
                type="button" 
                class="option-btn add-new"
                (click)="showAddContact = true">
                + Nuovo
              </button>
            </div>

            <!-- Selected Recipient -->
            <div class="selected-recipient" *ngIf="selectedContact">
              <div class="recipient-card">
                <div class="recipient-info">
                  <div class="name">{{selectedContact.firstName}} {{selectedContact.lastName}}</div>
                  <div class="tax-code">{{selectedContact.taxCode}}</div>
                  <div class="iban" *ngIf="selectedContact.iban">{{selectedContact.iban}}</div>
                </div>
                <button type="button" class="remove-btn" (click)="clearRecipient()">✕</button>
              </div>
            </div>

            <!-- Contact List -->
            <div class="contact-list" *ngIf="!selectedContact">
              
              <!-- Loading -->
              <div class="loading" *ngIf="loading">
                Caricamento contatti...
              </div>

              <!-- Favorites -->
              <div *ngIf="activeTab === 'favorites' && !loading">
                <div class="contact-item" 
                     *ngFor="let contact of favoriteContacts" 
                     (click)="selectContact(contact)">
                  <div class="contact-avatar">
                    <span class="favorite-star">⭐</span>
                    {{getInitials(contact)}}
                  </div>
                  <div class="contact-info">
                    <div class="name">{{contact.firstName}} {{contact.lastName}}</div>
                    <div class="tax-code">{{contact.taxCode}}</div>
                  </div>
                  <span class="select-icon">›</span>
                </div>
                <div class="empty-state" *ngIf="favoriteContacts.length === 0">
                  Nessun contatto preferito
                </div>
              </div>

              <!-- Recent -->
              <div *ngIf="activeTab === 'recent' && !loading">
                <div class="recent-item" 
                     *ngFor="let transfer of recentTransfers" 
                     (click)="selectContact(transfer.contact)">
                  <div class="contact-avatar">
                    {{getInitials(transfer.contact)}}
                  </div>
                  <div class="contact-info">
                    <div class="name">{{transfer.contact.firstName}} {{transfer.contact.lastName}}</div>
                    <div class="last-transfer">
                      Ultimo: {{formatCurrency(transfer.amount)}} - {{formatDate(transfer.date)}}
                    </div>
                  </div>
                  <span class="select-icon">›</span>
                </div>
                <div class="empty-state" *ngIf="recentTransfers.length === 0">
                  Nessun trasferimento recente
                </div>
              </div>

              <!-- All Contacts -->
              <div *ngIf="activeTab === 'all' && !loading">
                <div class="contact-group" *ngFor="let group of contactGroups">
                  <div class="group-letter">{{group.letter}}</div>
                  <div class="contact-item" 
                       *ngFor="let contact of group.contacts" 
                       (click)="selectContact(contact)">
                    <div class="contact-avatar">
                      {{getInitials(contact)}}
                      <span class="favorite-star" *ngIf="contact.isFavorite">⭐</span>
                    </div>
                    <div class="contact-info">
                      <div class="name">{{contact.firstName}} {{contact.lastName}}</div>
                      <div class="tax-code">{{contact.taxCode}}</div>
                    </div>
                    <span class="select-icon">›</span>
                  </div>
                </div>
                <div class="empty-state" *ngIf="contactGroups.length === 0">
                  Nessun contatto trovato
                </div>
              </div>
            </div>
          </div>

          <!-- Transfer Details -->
          <div class="form-section" *ngIf="selectedContact">
            <h2>Dettagli Trasferimento</h2>
            
            <div class="form-field">
              <label for="amount">Importo (EUR)</label>
              <input 
                type="number" 
                id="amount"
                formControlName="amount"
                placeholder="0.00"
                step="0.01"
                min="0.01"
                max="50000">
              <div class="field-error" *ngIf="transferForm.get('amount')?.touched && transferForm.get('amount')?.errors">
                <span *ngIf="transferForm.get('amount')?.errors?.['required']">L'importo è obbligatorio</span>
                <span *ngIf="transferForm.get('amount')?.errors?.['min']">L'importo minimo è €0.01</span>
                <span *ngIf="transferForm.get('amount')?.errors?.['max']">L'importo massimo è €50,000</span>
              </div>
            </div>

            <div class="form-field">
              <label for="description">Descrizione (opzionale)</label>
              <textarea 
                id="description"
                formControlName="description"
                placeholder="Causale del pagamento..."
                maxlength="200"
                rows="3"></textarea>
              <div class="char-count">
                {{transferForm.get('description')?.value?.length || 0}}/200 caratteri
              </div>
            </div>

            <div class="form-field">
              <label for="referenceId">ID Riferimento (opzionale)</label>
              <input 
                type="text" 
                id="referenceId"
                formControlName="referenceId"
                placeholder="REF-2025-..."
                maxlength="50">
            </div>          </div>

          <!-- Action Buttons -->
          <div class="action-buttons" *ngIf="selectedContact">
            <button 
              type="button" 
              class="btn btn-secondary"
              (click)="validateTransfer()"
              [disabled]="transferForm.invalid || validating">
              {{validating ? 'Validazione...' : 'Valida'}}
            </button>
            
            <button 
              type="submit" 
              class="btn btn-primary"
              [disabled]="transferForm.invalid || processing || validationErrors.length > 0">
              {{processing ? 'Trasferimento...' : 'Trasferisci'}}
            </button>
          </div>
        </form>
      </div>

      <!-- Success Screen -->
      <div class="success-screen" *ngIf="showSuccess && transferResult">
        <div class="success-icon">✅</div>
        <h2>Trasferimento Completato!</h2>
        
        <div class="success-details">
          <div class="detail-row">
            <span class="label">ID Transazione:</span>
            <span class="value">{{transferResult.transaction_id}}</span>
          </div>
          <div class="detail-row">
            <span class="label">Importo:</span>
            <span class="value">{{formatCurrency(transferResult.amount)}}</span>
          </div>
          <div class="detail-row">
            <span class="label">Destinatario:</span>
            <span class="value">{{selectedContact?.firstName}} {{selectedContact?.lastName}}</span>
          </div>
          <div class="detail-row">
            <span class="label">Data:</span>
            <span class="value">{{formatDateTime(transferResult.timestamp)}}</span>
          </div>
          <div class="detail-row" *ngIf="transferResult.fees.amount > 0">
            <span class="label">Commissioni:</span>
            <span class="value">{{formatCurrency(transferResult.fees.amount)}}</span>
          </div>
        </div>

        <div class="success-actions">
          <button class="btn btn-secondary" (click)="resetForm()">
            Nuovo Trasferimento
          </button>
          <button class="btn btn-primary" (click)="goToHome()">
            Torna alla Home
          </button>
        </div>
      </div>

      <!-- Add Contact Modal -->
      <div class="modal-overlay" *ngIf="showAddContact" (click)="closeAddContact()">
        <div class="modal-content" (click)="$event.stopPropagation()">
          <div class="modal-header">
            <h3>Aggiungi Nuovo Contatto</h3>
            <button class="close-btn" (click)="closeAddContact()">✕</button>
          </div>
            <form [formGroup]="contactForm" (ngSubmit)="addNewContact()">
            <div class="form-field">
              <label for="firstName">Nome *</label>
              <input type="text" id="firstName" formControlName="firstName" 
                     [class.error]="contactForm.get('firstName')?.invalid && contactForm.get('firstName')?.touched"
                     maxlength="50" required>
              <div class="field-error" *ngIf="contactForm.get('firstName')?.touched && contactForm.get('firstName')?.errors">
                <span *ngIf="contactForm.get('firstName')?.errors?.['required']">Il nome è obbligatorio</span>
                <span *ngIf="contactForm.get('firstName')?.errors?.['maxlength']">Il nome non può superare i 50 caratteri</span>
              </div>
            </div>
            
            <div class="form-field">
              <label for="lastName">Cognome *</label>
              <input type="text" id="lastName" formControlName="lastName" 
                     [class.error]="contactForm.get('lastName')?.invalid && contactForm.get('lastName')?.touched"
                     maxlength="50" required>
              <div class="field-error" *ngIf="contactForm.get('lastName')?.touched && contactForm.get('lastName')?.errors">
                <span *ngIf="contactForm.get('lastName')?.errors?.['required']">Il cognome è obbligatorio</span>
                <span *ngIf="contactForm.get('lastName')?.errors?.['maxlength']">Il cognome non può superare i 50 caratteri</span>
              </div>
            </div>
              <div class="form-field">
              <label for="taxCodeNew">Codice Fiscale *</label>
              <input type="text" id="taxCodeNew" formControlName="taxCode" 
                     placeholder="RSSMRA85M01H501U" 
                     [class.error]="contactForm.get('taxCode')?.invalid && contactForm.get('taxCode')?.touched"
                     style="text-transform: uppercase" 
                     maxlength="16" required>
              <div class="field-error" *ngIf="contactForm.get('taxCode')?.touched && contactForm.get('taxCode')?.errors">
                <span *ngIf="contactForm.get('taxCode')?.errors?.['required']">Il codice fiscale è obbligatorio</span>
                <span *ngIf="contactForm.get('taxCode')?.errors?.['pattern']">
                  Il codice fiscale deve essere nel formato italiano: 6 lettere + 2 numeri + 1 lettera + 2 numeri + 1 lettera + 3 numeri + 1 lettera (es: RSSMRA85M01H501U)
                </span>
              </div>
              <div class="field-success" *ngIf="contactForm.get('taxCode')?.valid && contactForm.get('taxCode')?.touched">
                Codice fiscale valido
              </div>
              <div class="field-help">
                Formato: RSSMRA85M01H501U (16 caratteri - formato italiano)
              </div>
            </div>
            
            <div class="form-field">
              <label for="email">Email</label>
              <input type="email" id="email" formControlName="email"
                     [class.error]="contactForm.get('email')?.invalid && contactForm.get('email')?.touched"
                     placeholder="nome@esempio.com">
              <div class="field-error" *ngIf="contactForm.get('email')?.touched && contactForm.get('email')?.errors">
                <span *ngIf="contactForm.get('email')?.errors?.['email']">Inserisci un indirizzo email valido</span>
              </div>
            </div>
            
            <div class="form-field">
              <label for="phone">Telefono</label>
              <input type="tel" id="phone" formControlName="phone"
                     placeholder="+39 123 456 7890">
            </div>
              <div class="form-field">
              <label for="iban">IBAN</label>
              <input type="text" id="iban" formControlName="iban" 
                     [class.error]="contactForm.get('iban')?.invalid && contactForm.get('iban')?.touched"
                     placeholder="IT60X0542811101000000123456"
                     style="text-transform: uppercase"
                     maxlength="27">
              <div class="field-error" *ngIf="contactForm.get('iban')?.touched && contactForm.get('iban')?.errors">
                <span *ngIf="contactForm.get('iban')?.errors?.['pattern']">
                  L'IBAN deve essere nel formato italiano: IT + 2 numeri + 1 lettera + 22 numeri (es: IT60X0542811101000000123456)
                </span>
              </div>
              <div class="field-success" *ngIf="contactForm.get('iban')?.valid && contactForm.get('iban')?.touched && contactForm.get('iban')?.value">
                IBAN valido
              </div>
              <div class="field-help">
                Formato: IT22X1234567890123456789012 (27 caratteri - solo IBAN italiani)
              </div>
            </div>
            
            <div class="modal-actions">
              <button type="button" class="btn btn-secondary" (click)="closeAddContact()">
                Annulla
              </button>
              <button type="submit" class="btn btn-primary" [disabled]="contactForm.invalid">
                Aggiungi
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./p2p-transfer.component.scss']
})
export class P2PTransferComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  transferForm: FormGroup;
  contactForm: FormGroup;
  
  // UI State
  activeTab: 'favorites' | 'recent' | 'all' = 'favorites';
  searchQuery = '';
  showAddContact = false;
  showSuccess = false;
  loading = false;
  processing = false;
  validating = false;
  
  // Data
  selectedContact: Contact | null = null;
  favoriteContacts: Contact[] = [];
  recentTransfers: RecentTransfer[] = [];
  contactGroups: any[] = [];
  allContacts: Contact[] = [];
  filteredContacts: Contact[] = [];
  
  // Transfer
  validationErrors: string[] = [];
  transferResult: P2PTransferResponse | null = null;
  currentUser: Customer | null = null;
  constructor(
    private fb: FormBuilder,
    private bankingService: BankingApiService,
    private contactService: ContactService,
    private authService: AuthService,
    private toastService: ToastService,
    private router: Router
  ) {
    this.transferForm = this.createTransferForm();
    this.contactForm = this.createContactForm();
  }

  // Getter per accesso facile ai campi del form
  get contactFormControls() {
    return this.contactForm.controls;
  }

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.loadData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private createTransferForm(): FormGroup {
    return this.fb.group({
      amount: ['', [Validators.required, Validators.min(0.01), Validators.max(50000)]],
      description: ['', [Validators.maxLength(200)]],
      referenceId: ['', [Validators.maxLength(50), Validators.pattern(/^[a-zA-Z0-9_-]*$/)]]
    });
  }
  private createContactForm(): FormGroup {
    const form = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      taxCode: ['', [Validators.required, Validators.pattern(/^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$/)]],
      email: ['', [Validators.email]],
      phone: [''],
      iban: ['', [Validators.pattern(/^IT[0-9]{2}[A-Z][0-9]{22}$/)]]
    });

    // Aggiungi listeners per validazione real-time
    form.get('taxCode')?.valueChanges.pipe(
      takeUntil(this.destroy$)
    ).subscribe(value => {
      if (value) {
        form.get('taxCode')?.setValue(value.toUpperCase(), { emitEvent: false });
      }
    });

    form.get('iban')?.valueChanges.pipe(
      takeUntil(this.destroy$)
    ).subscribe(value => {
      if (value) {
        form.get('iban')?.setValue(value.toUpperCase(), { emitEvent: false });
      }
    });

    return form;
  }

  private loadData(): void {
    this.loading = true;
    
    // Load favorites
    this.contactService.getFavoriteContacts()
      .pipe(takeUntil(this.destroy$))
      .subscribe(favorites => {
        this.favoriteContacts = favorites;
      });

    // Load recent transfers
    this.contactService.getRecentTransfers()
      .pipe(takeUntil(this.destroy$))
      .subscribe(recent => {
        this.recentTransfers = recent;
      });

    // Load all contacts grouped
    this.contactService.getContactsGrouped()
      .pipe(takeUntil(this.destroy$))
      .subscribe(groups => {
        this.contactGroups = groups;
        this.loading = false;
      });

    // Load all contacts for search
    this.contactService.getContacts()
      .pipe(takeUntil(this.destroy$))
      .subscribe(contacts => {
        this.allContacts = contacts;
        this.filteredContacts = contacts;
      });
  }

  setActiveTab(tab: 'favorites' | 'recent' | 'all'): void {
    this.activeTab = tab;
    this.searchQuery = '';
  }

  onSearchChange(event: any): void {
    const query = event.target.value;
    this.searchQuery = query;
    
    if (query.trim()) {
      this.filteredContacts = this.contactService.searchContacts(query);
      this.contactGroups = this.groupContactsByLetter(this.filteredContacts);
    } else {
      this.loadData();
    }
  }

  selectContact(contact: Contact): void {
    this.selectedContact = contact;
    this.validationErrors = [];
  }

  clearRecipient(): void {
    this.selectedContact = null;
    this.validationErrors = [];
  }
  async validateTransfer(): Promise<void> {
    if (!this.selectedContact || !this.currentUser) return;
    
    this.validating = true;
    this.validationErrors = [];
    
    const request: P2PTransferRequest = {
      sender_tax_id: this.currentUser.taxCode,
      recipient_tax_id: this.selectedContact.taxCode,
      amount: this.transferForm.get('amount')?.value,
      currency: 'EUR',
      description: this.transferForm.get('description')?.value || undefined,
      reference_id: this.transferForm.get('referenceId')?.value || undefined
    };

    try {
      const result = await this.bankingService.validateP2PTransfer(request).toPromise();
      
      if (result && !result.isValid) {
        this.validationErrors = result.errors;
        this.toastService.showValidationErrors(result.errors);
      } else {
        this.validationErrors = [];
        this.toastService.showSuccess(
          'Validazione Completata', 
          'Tutti i dati sono corretti. Puoi procedere con il trasferimento.'
        );
      }
    } catch (error: any) {
      console.error('Validation error:', error);
      if (error.error?.errors) {
        this.validationErrors = error.error.errors;
        this.toastService.showValidationErrors(error.error.errors);
      } else {
        this.validationErrors = ['Errore durante la validazione. Riprova.'];
        this.toastService.showError(
          'Errore di Validazione',
          error.error?.errorMessage || 'Errore durante la validazione. Riprova.'
        );
      }
    } finally {
      this.validating = false;
    }
  }
  async onSubmit(): Promise<void> {
    if (!this.selectedContact || !this.currentUser || this.transferForm.invalid) return;
    
    this.processing = true;
    
    const request: P2PTransferRequest = {
      sender_tax_id: this.currentUser.taxCode,
      recipient_tax_id: this.selectedContact.taxCode,
      amount: this.transferForm.get('amount')?.value,
      currency: 'EUR',
      description: this.transferForm.get('description')?.value || undefined,
      reference_id: this.transferForm.get('referenceId')?.value || undefined
    };

    try {
      const result = await this.bankingService.executeP2PTransfer(request).toPromise();
      
      if (result) {
        this.transferResult = result;
        
        // Add to recent transfers
        this.contactService.addRecentTransfer({
          contactId: this.selectedContact.id,
          contact: this.selectedContact,
          amount: request.amount,
          description: request.description || '',
          transactionId: result.transaction_id
        });
        
        this.toastService.showSuccess(
          'Trasferimento Completato!',
          `€${request.amount.toFixed(2)} inviati a ${this.selectedContact.firstName} ${this.selectedContact.lastName}`
        );
        
        this.showSuccess = true;
      }
    } catch (error: any) {
      console.error('Transfer error:', error);
      
      const errorResponse = error.error;
      let errorTitle = 'Errore Trasferimento';
      let errorMessage = 'Si è verificato un errore durante il trasferimento.';
      
      if (errorResponse?.errorCode) {
        switch (errorResponse.errorCode) {
          case 'INSUFFICIENT_FUNDS':
            errorTitle = 'Fondi Insufficienti';
            errorMessage = 'Il saldo del tuo conto non è sufficiente per completare il trasferimento.';
            break;
          case 'INVALID_TAX_ID':
            errorTitle = 'Codice Fiscale Non Valido';
            errorMessage = 'Il codice fiscale del destinatario non è valido o non è registrato.';
            break;
          case 'ACCOUNT_BLOCKED':
            errorTitle = 'Account Bloccato';
            errorMessage = 'L\'account del destinatario è temporaneamente bloccato.';
            break;
          case 'DAILY_LIMIT_EXCEEDED':
            errorTitle = 'Limite Giornaliero Superato';
            errorMessage = 'Hai raggiunto il limite massimo di trasferimenti giornalieri.';
            break;
          case 'DUPLICATE_REFERENCE':
            errorTitle = 'Riferimento Duplicato';
            errorMessage = 'L\'ID di riferimento è già stato utilizzato per un altro trasferimento.';
            break;
          case 'SYSTEM_ERROR':
            errorTitle = 'Errore di Sistema';
            errorMessage = 'Il sistema di Banca Alfa è temporaneamente non disponibile. Riprova più tardi.';
            break;
          default:
            if (errorResponse.errorMessage) {
              errorMessage = errorResponse.errorMessage;
            }
        }
      }
      
      this.toastService.showError(errorTitle, errorMessage, 8000); // 8 secondi per errori
    } finally {
      this.processing = false;
    }
  }  addNewContact(): void {
    // Marca tutti i campi come touched per mostrare gli errori
    this.contactForm.markAllAsTouched();
    
    if (this.contactForm.invalid) {
      this.toastService.showWarning(
        'Modulo Non Valido',
        'Verifica i campi evidenziati in rosso e correggi gli errori.'
      );
      return;
    }
    
    const formValue = this.contactForm.value;
    
    // Verifica se il codice fiscale esiste già
    const existingContact = this.contactService.getContactByTaxCode(formValue.taxCode.toUpperCase());
    if (existingContact) {
      this.toastService.showWarning(
        'Contatto Già Esistente',
        `Un contatto con questo codice fiscale è già presente nella rubrica: ${existingContact.firstName} ${existingContact.lastName}`
      );
      return;
    }
    
    const newContact = this.contactService.addContact({
      firstName: formValue.firstName,
      lastName: formValue.lastName,
      taxCode: formValue.taxCode.toUpperCase(),
      email: formValue.email || undefined,
      phone: formValue.phone || undefined,
      iban: formValue.iban || undefined,
      isFavorite: false
    });
    
    this.toastService.showSuccess(
      'Contatto Aggiunto',
      `${newContact.firstName} ${newContact.lastName} è stato aggiunto alla rubrica.`
    );
    
    this.selectContact(newContact);
    this.closeAddContact();
  }

  closeAddContact(): void {
    this.showAddContact = false;
    this.contactForm.reset();
  }

  resetForm(): void {
    this.showSuccess = false;
    this.transferResult = null;
    this.selectedContact = null;
    this.transferForm.reset();
    this.validationErrors = [];
    this.activeTab = 'favorites';
  }

  goBack(): void {
    this.router.navigate(['/home']);
  }

  goToHome(): void {
    this.router.navigate(['/home']);
  }

  getInitials(contact: Contact): string {
    return (contact.firstName.charAt(0) + contact.lastName.charAt(0)).toUpperCase();
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('it-IT', {
      style: 'currency',
      currency: 'EUR'
    }).format(amount);
  }

  formatDate(date: Date): string {
    return new Intl.DateTimeFormat('it-IT', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).format(new Date(date));
  }

  formatDateTime(date: Date): string {
    return new Intl.DateTimeFormat('it-IT', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(new Date(date));
  }

  private groupContactsByLetter(contacts: Contact[]): any[] {
    const groups: { [key: string]: Contact[] } = {};
    
    contacts.forEach(contact => {
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
}
