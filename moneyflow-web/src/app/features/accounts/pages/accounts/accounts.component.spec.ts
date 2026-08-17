import { HttpErrorResponse } from '@angular/common/http';
import { provideZonelessChangeDetection } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, Subject, throwError } from 'rxjs';

import { AccountsService } from '../../data-access/accounts.service';
import { Account } from '../../models/account.models';
import { AccountsComponent } from './accounts.component';

describe('AccountsComponent', () => {
  const accounts: Account[] = [
    {
      id: 1,
      bank: 'BCP',
      nickname: 'Cuenta principal',
      type: 'Checking',
      last4: '1234',
      balance: 2500,
      currency: 'PEN',
      updatedAt: '2026-08-16T20:00:00Z',
    },
    {
      id: 2,
      bank: 'Interbank',
      nickname: 'Tarjeta de crédito',
      type: 'Credit',
      last4: '5678',
      balance: -500,
      currency: 'PEN',
      updatedAt: '2026-08-16T20:00:00Z',
    },
  ];

  let fixture: ComponentFixture<AccountsComponent>;

  let component: AccountsComponent;

  let accountsService: {
    getAll: ReturnType<typeof vi.fn>;
    getById: ReturnType<typeof vi.fn>;
    create: ReturnType<typeof vi.fn>;
    update: ReturnType<typeof vi.fn>;
    delete: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    accountsService = {
      getAll: vi.fn().mockReturnValue(of(accounts)),
      getById: vi.fn(),
      create: vi.fn(),
      update: vi.fn(),
      delete: vi.fn(),
    };

    await TestBed.configureTestingModule({
      imports: [AccountsComponent],
      providers: [
        provideZonelessChangeDetection(),
        {
          provide: AccountsService,
          useValue: accountsService,
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AccountsComponent);

    component = fixture.componentInstance;
  });

  it('renders accounts after asynchronous loading in zoneless mode', async () => {
    const accountsResult = new Subject<Account[]>();

    accountsService.getAll.mockReturnValue(accountsResult.asObservable());

    fixture.detectChanges();

    expect(component.loading).toBe(true);

    expect(fixture.nativeElement.textContent).toContain('Cargando cuentas...');

    accountsResult.next(accounts);
    accountsResult.complete();

    await fixture.whenStable();

    expect(component.loading).toBe(false);
    expect(component.accounts).toEqual(accounts);

    expect(fixture.nativeElement.querySelectorAll('article.account-card')).toHaveLength(2);

    expect(fixture.nativeElement.textContent).toContain('Cuenta principal');
  });

  it('calculates net worth, assets and debt', () => {
    fixture.detectChanges();

    expect(component.netWorth).toBe(2000);
    expect(component.totalAssets).toBe(2500);
    expect(component.totalDebt).toBe(500);
  });

  it('creates an account and reloads the list', () => {
    fixture.detectChanges();

    accountsService.create.mockReturnValue(of({ id: 3 }));

    component.openCreateForm();

    component.accountForm.setValue({
      bank: '  BBVA  ',
      nickname: '  Cuenta sueldo  ',
      type: 'Savings',
      last4: '9012',
      initialBalance: 1500,
    });

    component.submitAccount();

    expect(accountsService.create).toHaveBeenCalledWith({
      bank: 'BBVA',
      nickname: 'Cuenta sueldo',
      type: 'Savings',
      last4: '9012',
      initialBalance: 1500,
      currency: 'PEN',
    });

    expect(accountsService.getAll).toHaveBeenCalledTimes(2);

    expect(component.isFormOpen).toBe(false);

    expect(component.successMessage).toBe('Cuenta creada correctamente.');
  });

  it('updates an existing account', () => {
    fixture.detectChanges();

    accountsService.update.mockReturnValue(of(undefined));

    component.openEditForm(accounts[0]);

    component.accountForm.controls.nickname.setValue('Cuenta de gastos');

    component.submitAccount();

    expect(accountsService.update).toHaveBeenCalledWith(1, {
      bank: 'BCP',
      nickname: 'Cuenta de gastos',
      type: 'Checking',
      last4: '1234',
    });

    expect(component.isFormOpen).toBe(false);

    expect(component.successMessage).toBe('Cuenta actualizada correctamente.');
  });

  it('removes a deleted account from the visible list', () => {
    fixture.detectChanges();

    accountsService.delete.mockReturnValue(of(undefined));

    component.requestDelete(accounts[0]);
    component.confirmDelete();

    expect(accountsService.delete).toHaveBeenCalledWith(1);

    expect(component.accounts).toEqual([accounts[1]]);

    expect(component.successMessage).toBe('Cuenta eliminada correctamente.');

    expect(component.accountPendingDelete).toBeNull();

    expect(component.isDeleting).toBe(false);
  });

  it('explains why an account with transactions cannot be deleted', () => {
    fixture.detectChanges();

    accountsService.delete.mockReturnValue(
      throwError(
        () =>
          new HttpErrorResponse({
            status: 409,
            statusText: 'Conflict',
          }),
      ),
    );

    component.requestDelete(accounts[0]);
    component.confirmDelete();

    expect(component.errorMessage).toContain('transacciones vinculadas');

    expect(component.accountPendingDelete).toBeNull();

    expect(component.isDeleting).toBe(false);
  });

  it('shows a loading error and stops the spinner', () => {
    accountsService.getAll.mockReturnValue(
      throwError(
        () =>
          new HttpErrorResponse({
            status: 500,
            statusText: 'Server Error',
          }),
      ),
    );

    fixture.detectChanges();

    expect(component.loading).toBe(false);

    expect(component.errorMessage).toBe('No pudimos cargar tus cuentas. Inténtalo nuevamente.');
  });
});
