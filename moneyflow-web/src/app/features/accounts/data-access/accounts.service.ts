import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import {
  Account,
  CreateAccountRequest,
  CreateAccountResponse,
  UpdateAccountRequest,
} from '../models/account.models';

export type {
  Account,
  AccountCurrency,
  AccountType,
  CreateAccountRequest,
  CreateAccountResponse,
  UpdateAccountRequest,
} from '../models/account.models';

@Injectable({ providedIn: 'root' })
export class AccountsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/accounts`;

  getAll(): Observable<Account[]> {
    return this.http.get<Account[]>(this.apiUrl);
  }

  getById(accountId: number): Observable<Account> {
    return this.http.get<Account>(`${this.apiUrl}/${accountId}`);
  }

  create(request: CreateAccountRequest): Observable<CreateAccountResponse> {
    return this.http.post<CreateAccountResponse>(this.apiUrl, request);
  }

  update(accountId: number, request: UpdateAccountRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${accountId}`, request);
  }

  delete(accountId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${accountId}`);
  }
}
