import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

export type AccountType = 'checking' | 'savings' | 'credit' | 'investment' | 'wallet';

export interface Account {
  id: string;
  bank: string;
  nickname: string;
  type: AccountType;
  last4: string;
  balance: number;
  currency: 'PEN' | 'USD';
  lastSynced: string;
}

@Injectable({ providedIn: 'root' })
export class AccountsService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/v1/accounts`;

  getAll(): Observable<Account[]> {
    return this.http.get<Account[]>(this.apiUrl);
  }
}
