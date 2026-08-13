export type TransactionType = 'Income' | 'Expense';

export interface Transaction {
  id: number;
  accountId: number;
  categoryId: number;
  amount: number;
  description: string | null;
  date: string;
  type: TransactionType;
}

export interface SaveTransactionRequest {
  accountId: number;
  categoryId: number;
  amount: number;
  description: string | null;
  date: string;
  type: TransactionType;
}

export interface CreateTransactionResponse {
  id: number;
}