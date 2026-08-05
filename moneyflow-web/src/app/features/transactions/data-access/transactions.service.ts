import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

export interface Transaction {
  id: number;
  amount: number;
  type: 'income' | 'expense';
  categoryName: string;
  date: string;
}

@Injectable({ providedIn: 'root' })
export class TransactionsService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/v1/transactions`;

  getAll(): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(this.apiUrl);
  }
}
