import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { environment } from '../../../../environments/environment';
import {
  SaveTransactionRequest,
  Transaction,
} from '../models/transaction.models';
import { TransactionsService } from './transactions.service';

describe('TransactionsService', () => {
  const transactionsUrl =
    `${environment.apiUrl}/api/v1/transactions`;

  let service: TransactionsService;
  let httpTesting: HttpTestingController;

  const transaction: Transaction = {
    id: 1,
    accountId: 10,
    categoryId: 20,
    amount: 150,
    description: 'Compra semanal',
    date: '2026-08-12',
    type: 'Expense',
  };

  const payload: SaveTransactionRequest = {
    accountId: 10,
    categoryId: 20,
    amount: 150,
    description: 'Compra semanal',
    date: '2026-08-12',
    type: 'Expense',
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        TransactionsService,
      ],
    });

    service = TestBed.inject(TransactionsService);
    httpTesting = TestBed.inject(
      HttpTestingController,
    );
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('gets all transactions', () => {
    service.getAll().subscribe((response) => {
      expect(response).toEqual([transaction]);
    });

    const request =
      httpTesting.expectOne(transactionsUrl);

    expect(request.request.method).toBe('GET');
    request.flush([transaction]);
  });

  it('gets a transaction by id', () => {
    service.getById(1).subscribe((response) => {
      expect(response).toEqual(transaction);
    });

    const request = httpTesting.expectOne(
      `${transactionsUrl}/1`,
    );

    expect(request.request.method).toBe('GET');
    request.flush(transaction);
  });

  it('creates a transaction with a positive amount', () => {
    service.create(payload).subscribe((response) => {
      expect(response).toEqual({ id: 1 });
    });

    const request =
      httpTesting.expectOne(transactionsUrl);

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(payload);
    expect(
      request.request.body.amount,
    ).toBeGreaterThan(0);

    request.flush(
      { id: 1 },
      {
        status: 201,
        statusText: 'Created',
      },
    );
  });

  it('updates a transaction', () => {
    service.update(1, payload).subscribe();

    const request = httpTesting.expectOne(
      `${transactionsUrl}/1`,
    );

    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(payload);

    request.flush(null, {
      status: 204,
      statusText: 'No Content',
    });
  });

  it('deletes a transaction', () => {
    service.delete(1).subscribe();

    const request = httpTesting.expectOne(
      `${transactionsUrl}/1`,
    );

    expect(request.request.method).toBe('DELETE');

    request.flush(null, {
      status: 204,
      statusText: 'No Content',
    });
  });
});