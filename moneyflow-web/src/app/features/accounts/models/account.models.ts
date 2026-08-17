export type AccountType = 'Checking' | 'Savings' | 'Credit' | 'Investment' | 'Wallet';

export type AccountCurrency = 'PEN';

export interface Account {
  id: number;
  bank: string;
  nickname: string;
  type: AccountType;
  last4: string;
  balance: number;
  currency: AccountCurrency;
  updatedAt: string;
}

export interface CreateAccountRequest {
  bank: string;
  nickname: string;
  type: AccountType;
  last4: string;
  initialBalance: number;
  currency: AccountCurrency;
}

export interface CreateAccountResponse {
  id: number;
}

export interface UpdateAccountRequest {
  bank: string;
  nickname: string;
  type: AccountType;
  last4: string;
}
