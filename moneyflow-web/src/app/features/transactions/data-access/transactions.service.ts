import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import {
  CreateTransactionResponse,
  SaveTransactionRequest,
  Transaction,
} from '../models/transaction.models';

@Injectable({
  providedIn: 'root',
})
export class TransactionsService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/api/v1/transactions`;

  getAll(): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(this.apiUrl);
  }

  getById(transactionId: number): Observable<Transaction> {
    return this.http.get<Transaction>(
      `${this.apiUrl}/${transactionId}`,
    );
  }

  create(
    request: SaveTransactionRequest,
  ): Observable<CreateTransactionResponse> {
    return this.http.post<CreateTransactionResponse>(
      this.apiUrl,
      request,
    );
  }

  update(
    transactionId: number,
    request: SaveTransactionRequest,
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${transactionId}`,
      request,
    );
  }

  delete(transactionId: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${transactionId}`,
    );
  }
}