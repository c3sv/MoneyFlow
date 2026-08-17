import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { environment } from '../../../../environments/environment';
import { Account, CreateAccountRequest, UpdateAccountRequest } from '../models/account.models';
import { AccountsService } from './accounts.service';

describe('AccountsService', () => {
  const apiUrl = `${environment.apiUrl}/api/v1/accounts`;

  const account: Account = {
    id: 1,
    bank: 'BCP',
    nickname: 'Cuenta principal',
    type: 'Checking',
    last4: '1234',
    balance: 2500,
    currency: 'PEN',
    updatedAt: '2026-08-16T20:00:00Z',
  };

  let service: AccountsService;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AccountsService, provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(AccountsService);

    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('gets all accounts', () => {
    service.getAll().subscribe((accounts) => {
      expect(accounts).toEqual([account]);
    });

    const request = httpTestingController.expectOne(apiUrl);

    expect(request.request.method).toBe('GET');

    request.flush([account]);
  });

  it('gets an account by id', () => {
    service.getById(1).subscribe((result) => {
      expect(result).toEqual(account);
    });

    const request = httpTestingController.expectOne(`${apiUrl}/1`);

    expect(request.request.method).toBe('GET');

    request.flush(account);
  });

  it('creates an account', () => {
    const createRequest: CreateAccountRequest = {
      bank: 'BCP',
      nickname: 'Cuenta principal',
      type: 'Checking',
      last4: '1234',
      initialBalance: 2500,
      currency: 'PEN',
    };

    service.create(createRequest).subscribe((response) => {
      expect(response).toEqual({ id: 1 });
    });

    const request = httpTestingController.expectOne(apiUrl);

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(createRequest);

    request.flush({ id: 1 });
  });

  it('updates an account', () => {
    const updateRequest: UpdateAccountRequest = {
      bank: 'Interbank',
      nickname: 'Cuenta de ahorros',
      type: 'Savings',
      last4: '5678',
    };

    service.update(1, updateRequest).subscribe();

    const request = httpTestingController.expectOne(`${apiUrl}/1`);

    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(updateRequest);

    request.flush(null);
  });

  it('deletes an account', () => {
    service.delete(1).subscribe();

    const request = httpTestingController.expectOne(`${apiUrl}/1`);

    expect(request.request.method).toBe('DELETE');

    request.flush(null);
  });
});
