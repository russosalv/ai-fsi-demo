// P2P Transfer Models
export interface P2PTransferRequest {
  sender_tax_id: string;
  recipient_tax_id: string;
  amount: number;
  currency: string;
  description?: string;
  reference_id?: string;
}

export interface P2PTransferResponse {
  status: string;
  transaction_id: string;
  timestamp: Date;
  amount: number;
  currency: string;
  sender: ParticipantInfo;
  recipient: ParticipantInfo;
  fees: FeeInfo;
  execution_date: Date;
  reference_id?: string;
}

export interface ParticipantInfo {
  tax_id: string;
  account_iban: string;
}

export interface FeeInfo {
  amount: number;
  currency: string;
}

// Validation Models
export interface ValidationResult {
  isValid: boolean;
  errors: string[];
  timestamp: Date;
  referenceId?: string;
}

// Error Response Models
export interface P2PErrorResponse {
  errorCode: string;
  errorMessage: string;
  timestamp: Date;
  referenceId?: string;
  details?: { [key: string]: any };
}

// Health Check Model
export interface HealthCheckResponse {
  status: string;
  service: string;
  timestamp: Date;
  version: string;
}
