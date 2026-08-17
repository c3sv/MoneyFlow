import { HttpErrorResponse } from '@angular/common/http';
import {
  provideZonelessChangeDetection,
} from '@angular/core';
import {
  ComponentFixture,
  TestBed,
} from '@angular/core/testing';
import {
  of,
  Subject,
  throwError,
} from 'rxjs';

import {
  Account,
  AccountsService,
} from '../../../accounts/data-access/accounts.service';
import {
  CategoriesService,
} from '../../../categories/data-access/categories.service';
import {
  Category,
} from '../../../categories/models/category.models';
import {
  TransactionsService,
} from '../../data-access/transactions.service';
import {
  Transaction,
} from '../../models/transaction.models';
import {
  TransactionsComponent,
} from './transactions.component';

describe('TransactionsComponent', () => {
  const transactions: Transaction[] = [
    {
      id: 1,
      accountId: 10,
      categoryId: 20,
      amount: 150,
      description: 'Compra semanal',
      date: '2026-08-12',
      type: 'Expense',
    },
    {
      id: 2,
      accountId: 10,
      categoryId: 21,
      amount: 2500,
      description: 'Sueldo agosto',
      date: '2026-08-11',
      type: 'Income',
    },
  ];

  const accounts: Account[] = [
    {
      id: 10,
      bank: 'BCP',
      nickname: 'Cuenta principal',
      type: 'Checking',
      last4: '1234',
      balance: 5000,
      currency: 'PEN',
      updatedAt: '2026-08-16T20:00:00Z',
    },
  ];

  const categories: Category[] = [
    {
      id: 20,
      name: 'Alimentación',
      type: 'Expense',
      icon: 'utensils',
    },
    {
      id: 21,
      name: 'Sueldo',
      type: 'Income',
      icon: 'wallet-cards',
    },
  ];

  let fixture:
    ComponentFixture<TransactionsComponent>;

  let component: TransactionsComponent;

  let transactionsService: {
    getAll: ReturnType<typeof vi.fn>;
    create: ReturnType<typeof vi.fn>;
    update: ReturnType<typeof vi.fn>;
    delete: ReturnType<typeof vi.fn>;
  };

  let accountsService: {
    getAll: ReturnType<typeof vi.fn>;
  };

  let categoriesService: {
    getAll: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    transactionsService = {
      getAll:
        vi.fn().mockReturnValue(of(transactions)),
      create: vi.fn(),
      update: vi.fn(),
      delete: vi.fn(),
    };

    accountsService = {
      getAll:
        vi.fn().mockReturnValue(of(accounts)),
    };

    categoriesService = {
      getAll:
        vi.fn().mockReturnValue(of(categories)),
    };

    await TestBed.configureTestingModule({
      imports: [TransactionsComponent],
      providers: [
        provideZonelessChangeDetection(),
        {
          provide: TransactionsService,
          useValue: transactionsService,
        },
        {
          provide: AccountsService,
          useValue: accountsService,
        },
        {
          provide: CategoriesService,
          useValue: categoriesService,
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(
      TransactionsComponent,
    );

    component = fixture.componentInstance;
  });

  it(
    'renders transactions after asynchronous data loading in zoneless mode',
    async () => {
      const transactionsResult =
        new Subject<Transaction[]>();

      const accountsResult =
        new Subject<Account[]>();

      const categoriesResult =
        new Subject<Category[]>();

      transactionsService.getAll.mockReturnValue(
        transactionsResult.asObservable(),
      );

      accountsService.getAll.mockReturnValue(
        accountsResult.asObservable(),
      );

      categoriesService.getAll.mockReturnValue(
        categoriesResult.asObservable(),
      );

      fixture.detectChanges();

      expect(
        fixture.nativeElement.querySelectorAll(
          '.transaction-skeleton',
        ),
      ).toHaveLength(5);

      transactionsResult.next(transactions);
      transactionsResult.complete();

      accountsResult.next(accounts);
      accountsResult.complete();

      categoriesResult.next(categories);
      categoriesResult.complete();

      await fixture.whenStable();

      expect(component.isLoading).toBe(false);
      expect(
        component.transactions,
      ).toHaveLength(2);

      expect(component.accounts).toEqual([
        {
          id: 10,
          label: 'BCP · Cuenta principal',
        },
      ]);

      expect(
        fixture.nativeElement.querySelectorAll(
          '.transaction-row',
        ),
      ).toHaveLength(2);

      expect(
        fixture.nativeElement.querySelectorAll(
          '.transaction-skeleton',
        ),
      ).toHaveLength(0);
    },
  );

  it(
    'filters transactions by type, category and search term',
    () => {
      fixture.detectChanges();

      component.setType('expense');

      expect(
        component.filteredTransactions,
      ).toEqual([transactions[0]]);

      component.setType('all');
      component.selectedCategory = '21';

      expect(
        component.filteredTransactions,
      ).toEqual([transactions[1]]);

      component.selectedCategory = 'all';
      component.searchTerm = 'alimentación';

      expect(
        component.filteredTransactions,
      ).toEqual([transactions[0]]);
    },
  );

  it(
    'creates an expense with a positive amount and reloads data',
    () => {
      fixture.detectChanges();

      transactionsService.create.mockReturnValue(
        of({ id: 3 }),
      );

      component.openCreateForm();

      component.transactionForm.setValue({
        type: 'Expense',
        accountId: 10,
        categoryId: 20,
        amount: 75.5,
        date: '2026-08-12',
        description: '  Cena  ',
      });

      component.submitTransaction();

      expect(
        transactionsService.create,
      ).toHaveBeenCalledWith({
        type: 'Expense',
        accountId: 10,
        categoryId: 20,
        amount: 75.5,
        date: '2026-08-12',
        description: 'Cena',
      });

      expect(
        transactionsService.getAll,
      ).toHaveBeenCalledTimes(2);

      expect(component.successMessage).toBe(
        'Transacción creada correctamente.',
      );
    },
  );

  it('updates an existing transaction', () => {
    fixture.detectChanges();

    transactionsService.update.mockReturnValue(
      of(undefined),
    );

    component.openEditForm(transactions[0]);

    component.transactionForm.controls.amount
      .setValue(200);

    component.submitTransaction();

    expect(
      transactionsService.update,
    ).toHaveBeenCalledWith(1, {
      type: 'Expense',
      accountId: 10,
      categoryId: 20,
      amount: 200,
      date: '2026-08-12',
      description: 'Compra semanal',
    });

    expect(component.successMessage).toBe(
      'Transacción actualizada correctamente.',
    );
  });

  it(
    'removes a deleted transaction from the visible list',
    () => {
      fixture.detectChanges();

      transactionsService.delete.mockReturnValue(
        of(undefined),
      );

      component.requestDelete(transactions[0]);
      component.confirmDelete();

      expect(
        transactionsService.delete,
      ).toHaveBeenCalledWith(1);

      expect(component.transactions).toEqual([
        transactions[1],
      ]);

      expect(component.successMessage).toBe(
        'Transacción eliminada correctamente.',
      );
    },
  );

  it(
    'shows the backend detail when a transaction cannot be saved',
    () => {
      fixture.detectChanges();

      transactionsService.create.mockReturnValue(
        throwError(
          () =>
            new HttpErrorResponse({
              status: 400,
              statusText: 'Bad Request',
              error: {
                detail:
                  'The account does not have enough balance.',
              },
            }),
        ),
      );

      component.openCreateForm();

      component.transactionForm.setValue({
        type: 'Expense',
        accountId: 10,
        categoryId: 20,
        amount: 999999,
        date: '2026-08-12',
        description: 'Compra imposible',
      });

      component.submitTransaction();

      expect(component.errorMessage).toBe(
        'The account does not have enough balance.',
      );

      expect(component.isSaving).toBe(false);
    },
  );
});
