import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectorRef,
  Component,
  OnInit,
  inject,
} from '@angular/core';
import {
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { forkJoin, Observable } from 'rxjs';

import {
  Account,
  AccountsService,
} from '../../../accounts/data-access/accounts.service';
import { CategoriesService } from '../../../categories/data-access/categories.service';
import { Category } from '../../../categories/models/category.models';
import { TransactionsService } from '../../data-access/transactions.service';
import {
  SaveTransactionRequest,
  Transaction,
  TransactionType,
} from '../../models/transaction.models';

type TransactionFilter = 'all' | 'income' | 'expense';

interface AccountOption {
  id: number;
  label: string;
}

interface TransactionGroup {
  date: string;
  label: string;
  transactions: Transaction[];
}

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  templateUrl: './transactions.component.html',
  styleUrl: './transactions.component.scss',
})
export class TransactionsComponent implements OnInit {
  private readonly transactionsService = inject(
    TransactionsService,
  );

  private readonly accountsService = inject(AccountsService);
  private readonly categoriesService = inject(
    CategoriesService,
  );

  private readonly formBuilder = inject(FormBuilder);
  private readonly changeDetectorRef = inject(
    ChangeDetectorRef,
  );

  readonly transactionForm =
    this.formBuilder.nonNullable.group({
      type: [
        'Expense' as TransactionType,
        Validators.required,
      ],
      accountId: [
        0,
        [Validators.required, Validators.min(1)],
      ],
      categoryId: [
        0,
        [Validators.required, Validators.min(1)],
      ],
      amount: [
        0,
        [Validators.required, Validators.min(0.01)],
      ],
      date: [
        this.today(),
        Validators.required,
      ],
      description: [
        '',
        Validators.maxLength(500),
      ],
    });

  transactions: Transaction[] = [];
  accounts: AccountOption[] = [];
  categories: Category[] = [];

  searchTerm = '';
  selectedCategory = 'all';
  selectedType: TransactionFilter = 'all';

  editingTransaction: Transaction | null = null;
  transactionPendingDelete: Transaction | null = null;

  isLoading = true;
  isSaving = false;
  isDeleting = false;
  isFormOpen = false;

  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadData();
  }

  get filteredTransactions(): Transaction[] {
    const term = this.searchTerm
      .trim()
      .toLowerCase();

    return this.transactions.filter((transaction) => {
      const category =
        this.categoryById(transaction.categoryId);

      const account =
        this.accountById(transaction.accountId);

      const description =
        transaction.description ?? '';

      const matchesTerm =
        !term ||
        description.toLowerCase().includes(term) ||
        category?.name.toLowerCase().includes(term) ||
        account?.label.toLowerCase().includes(term);

      const matchesCategory =
        this.selectedCategory === 'all' ||
        transaction.categoryId ===
          Number(this.selectedCategory);

      const matchesType =
        this.selectedType === 'all' ||
        (this.selectedType === 'income' &&
          transaction.type === 'Income') ||
        (this.selectedType === 'expense' &&
          transaction.type === 'Expense');

      return (
        matchesTerm &&
        matchesCategory &&
        matchesType
      );
    });
  }

  get groupedTransactions(): TransactionGroup[] {
    const groups = new Map<string, Transaction[]>();

    for (const transaction of this.filteredTransactions) {
      const group = groups.get(transaction.date);

      if (group) {
        group.push(transaction);
      } else {
        groups.set(transaction.date, [transaction]);
      }
    }

    return Array.from(groups.entries()).map(
      ([date, transactions]) => ({
        date,
        label: this.formatDateLabel(date),
        transactions,
      }),
    );
  }

  get totalIncome(): number {
    return this.filteredTransactions
      .filter(
        (transaction) =>
          transaction.type === 'Income',
      )
      .reduce(
        (total, transaction) =>
          total + transaction.amount,
        0,
      );
  }

  get totalExpense(): number {
    return this.filteredTransactions
      .filter(
        (transaction) =>
          transaction.type === 'Expense',
      )
      .reduce(
        (total, transaction) =>
          total + transaction.amount,
        0,
      );
  }

  get formCategories(): Category[] {
    const type =
      this.transactionForm.controls.type.value;

    return this.categories.filter(
      (category) => category.type === type,
    );
  }

  get canCreateTransaction(): boolean {
    return (
      !this.isLoading &&
      this.accounts.length > 0 &&
      this.categories.length > 0
    );
  }

  loadData(): void {
    this.isLoading = true;
    this.errorMessage = '';

    forkJoin({
      transactions:
        this.transactionsService.getAll(),
      accounts: this.accountsService.getAll(),
      categories: this.categoriesService.getAll(),
    }).subscribe({
      next: ({ transactions, accounts, categories }) => {
        this.transactions = [...transactions].sort(
          (first, second) =>
            second.date.localeCompare(first.date) ||
            second.id - first.id,
        );

        this.accounts = accounts.map(
          (account: Account) => ({
            id: Number(account.id),
            label: `${account.bank} · ${account.nickname}`,
          }),
        );

        this.categories = categories;
        this.isLoading = false;

        this.changeDetectorRef.markForCheck();
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage = this.resolveError(
          error,
          'No pudimos cargar tus transacciones. Inténtalo nuevamente.',
        );

        this.isLoading = false;
        this.changeDetectorRef.markForCheck();
      },
    });
  }

  setType(type: TransactionFilter): void {
    this.selectedType = type;
  }

  selectFormType(type: TransactionType): void {
    this.transactionForm.controls.type.setValue(type);

    const selectedCategoryId =
      this.transactionForm.controls.categoryId.value;

    const selectedCategory =
      this.categoryById(selectedCategoryId);

    if (
      selectedCategory &&
      selectedCategory.type !== type
    ) {
      this.transactionForm.controls.categoryId.setValue(
        0,
      );
    }
  }

  openCreateForm(): void {
    this.editingTransaction = null;
    this.errorMessage = '';

    const defaultAccountId =
      this.accounts[0]?.id ?? 0;

    const defaultType: TransactionType = 'Expense';

    const defaultCategoryId =
      this.categories.find(
        (category) =>
          category.type === defaultType,
      )?.id ?? 0;

    this.transactionForm.reset({
      type: defaultType,
      accountId: defaultAccountId,
      categoryId: defaultCategoryId,
      amount: 0,
      date: this.today(),
      description: '',
    });

    this.isFormOpen = true;
  }

  openEditForm(transaction: Transaction): void {
    this.editingTransaction = transaction;
    this.errorMessage = '';

    this.transactionForm.reset({
      type: transaction.type,
      accountId: transaction.accountId,
      categoryId: transaction.categoryId,
      amount: transaction.amount,
      date: transaction.date,
      description:
        transaction.description ?? '',
    });

    this.isFormOpen = true;
  }

  closeForm(): void {
    if (this.isSaving) {
      return;
    }

    this.isFormOpen = false;
    this.editingTransaction = null;
  }

  submitTransaction(): void {
    if (
      this.transactionForm.invalid ||
      this.isSaving
    ) {
      this.transactionForm.markAllAsTouched();
      return;
    }

    const value =
      this.transactionForm.getRawValue();

    const category = this.categoryById(
      value.categoryId,
    );

    if (!category || category.type !== value.type) {
      this.errorMessage =
        'La categoría seleccionada no corresponde al tipo de transacción.';

      return;
    }

    const request: SaveTransactionRequest = {
      accountId: value.accountId,
      categoryId: value.categoryId,
      amount: Number(value.amount),
      description:
        value.description.trim() || null,
      date: value.date,
      type: value.type,
    };

    this.isSaving = true;
    this.errorMessage = '';

    const request$: Observable<unknown> =
      this.editingTransaction
        ? this.transactionsService.update(
            this.editingTransaction.id,
            request,
          )
        : this.transactionsService.create(request);

    request$.subscribe({
      next: () => {
        this.isSaving = false;

        this.successMessage =
          this.editingTransaction
            ? 'Transacción actualizada correctamente.'
            : 'Transacción creada correctamente.';

        this.closeForm();
        this.loadData();

        this.changeDetectorRef.markForCheck();
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage = this.resolveError(
          error,
          'No pudimos guardar la transacción. Revisa los datos e inténtalo nuevamente.',
        );

        this.isSaving = false;
        this.changeDetectorRef.markForCheck();
      },
    });
  }

  requestDelete(transaction: Transaction): void {
    this.errorMessage = '';
    this.transactionPendingDelete = transaction;
  }

  cancelDelete(): void {
    if (!this.isDeleting) {
      this.transactionPendingDelete = null;
    }
  }

  confirmDelete(): void {
    if (
      !this.transactionPendingDelete ||
      this.isDeleting
    ) {
      return;
    }

    const transactionId =
      this.transactionPendingDelete.id;

    this.isDeleting = true;
    this.errorMessage = '';

    this.transactionsService
      .delete(transactionId)
      .subscribe({
        next: () => {
          this.transactions =
            this.transactions.filter(
              (transaction) =>
                transaction.id !== transactionId,
            );

          this.transactionPendingDelete = null;
          this.isDeleting = false;

          this.successMessage =
            'Transacción eliminada correctamente.';

          this.changeDetectorRef.markForCheck();
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessage = this.resolveError(
            error,
            'No pudimos eliminar la transacción. Inténtalo nuevamente.',
          );

          this.transactionPendingDelete = null;
          this.isDeleting = false;

          this.changeDetectorRef.markForCheck();
        },
      });
  }

  categoryById(
    categoryId: number,
  ): Category | undefined {
    return this.categories.find(
      (category) => category.id === categoryId,
    );
  }

  accountById(
    accountId: number,
  ): AccountOption | undefined {
    return this.accounts.find(
      (account) => account.id === accountId,
    );
  }

  transactionTitle(
    transaction: Transaction,
  ): string {
    return (
      transaction.description?.trim() ||
      (transaction.type === 'Income'
        ? 'Ingreso sin descripción'
        : 'Gasto sin descripción')
    );
  }

  categoryName(transaction: Transaction): string {
    return (
      this.categoryById(transaction.categoryId)
        ?.name ?? 'Sin categoría'
    );
  }

  accountName(transaction: Transaction): string {
    return (
      this.accountById(transaction.accountId)
        ?.label ?? 'Cuenta no disponible'
    );
  }

  categoryIcon(transaction: Transaction): string {
    const icon =
      this.categoryById(transaction.categoryId)
        ?.icon;

    const icons: Record<string, string> = {
      utensils: '🍴',
      bus: '↔',
      house: '⌂',
      lightbulb: '⚡',
      'heart-pulse': '♡',
      'graduation-cap': '◇',
      'gamepad-2': '◈',
      'shopping-bag': '▢',
      'wallet-cards': '$',
      laptop: '⌘',
      gift: '✦',
      'chart-no-axes-combined': '↗',
      'circle-plus': '+',
      ellipsis: '•••',
    };

    return icon ? (icons[icon] ?? '•') : '•';
  }

  signedAmount(transaction: Transaction): number {
    return transaction.type === 'Income'
      ? transaction.amount
      : -transaction.amount;
  }

  formatCurrency(value: number): string {
    return value.toLocaleString('es-PE', {
      style: 'currency',
      currency: 'PEN',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });
  }

  trackByTransactionId(
    _index: number,
    transaction: Transaction,
  ): number {
    return transaction.id;
  }

  private formatDateLabel(date: string): string {
    const today = this.today();

    const yesterdayDate = new Date();
    yesterdayDate.setDate(
      yesterdayDate.getDate() - 1,
    );

    const yesterday =
      this.toDateInputValue(yesterdayDate);

    if (date === today) {
      return 'Hoy';
    }

    if (date === yesterday) {
      return 'Ayer';
    }

    return new Intl.DateTimeFormat('es-PE', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
    }).format(
      new Date(`${date}T00:00:00`),
    );
  }

  private today(): string {
    return this.toDateInputValue(new Date());
  }

  private toDateInputValue(date: Date): string {
    const year = date.getFullYear();

    const month = String(
      date.getMonth() + 1,
    ).padStart(2, '0');

    const day = String(
      date.getDate(),
    ).padStart(2, '0');

    return `${year}-${month}-${day}`;
  }

  private resolveError(
    error: HttpErrorResponse,
    fallback: string,
  ): string {
    const detail = error.error?.detail;

    return typeof detail === 'string' &&
      detail.trim()
      ? detail
      : fallback;
  }
}