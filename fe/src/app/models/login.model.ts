export interface LoginRequest {
  taxCode: string;
  password: string;
}

export interface LoginResponse {
  success: boolean;
  customer?: any;
  bankAccounts?: any[];
  message?: string;
} 