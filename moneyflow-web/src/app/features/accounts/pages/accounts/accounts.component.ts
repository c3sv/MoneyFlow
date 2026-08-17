import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable } from 'rxjs';

import { AccountsService } from '../../data-access/accounts.service';
import {
  Account,
  AccountType,
  CreateAccountRequest,
  UpdateAccountRequest,
} from '../../models/account.models';

interface AccountTypeOption {
  value: AccountType;
  label: string;
}

const ACCOUNT_TYPE_OPTIONS: readonly AccountTypeOption[] = [
  {
    value: 'Checking',
    label: 'Cuenta corriente',
  },
  {
    value: 'Savings',
    label: 'Cuenta de ahorros',
  },
  {
    value: 'Credit',
    label: 'Tarjeta de crédito',
  },
  {
    value: 'Investment',
    label: 'Inversión',
  },
  {
    value: 'Wallet',
    label: 'Billetera digital',
  },
];

@Component({
  selector: 'app-accounts',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './accounts.component.html',
  styleUrl: './accounts.component.scss',
})
export class AccountsComponent implements OnInit {
  private readonly accountsService = inject(AccountsService);

  private readonly formBuilder = inject(FormBuilder);

  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  readonly accountTypeOptions = ACCOUNT_TYPE_OPTIONS;

  readonly accountForm = this.formBuilder.nonNullable.group({
    bank: ['', [Validators.required, Validators.maxLength(100)]],
    nickname: ['', [Validators.required, Validators.maxLength(100)]],
    type: ['Checking' as AccountType, Validators.required],
    last4: ['', [Validators.required, Validators.pattern(/^\d{4}$/)]],
    initialBalance: [0, Validators.required],
  });

  accounts: Account[] = [];

  editingAccount: Account | null = null;
  accountPendingDelete: Account | null = null;

  loading = true;
  isSaving = false;
  isDeleting = false;
  isFormOpen = false;

  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadAccounts();
  }

  get netWorth(): number {
    return this.accounts.reduce((total, account) => total + account.balance, 0);
  }

  get totalAssets(): number {
    return this.accounts
      .filter((account) => account.balance > 0)
      .reduce((total, account) => total + account.balance, 0);
  }

  get totalDebt(): number {
    return this.accounts
      .filter((account) => account.balance < 0)
      .reduce((total, account) => total + Math.abs(account.balance), 0);
  }

  get isEditing(): boolean {
    return this.editingAccount !== null;
  }

  loadAccounts(): void {
    this.loading = true;
    this.errorMessage = '';

    this.accountsService.getAll().subscribe({
      next: (accounts) => {
        this.accounts = accounts;
        this.loading = false;

        this.changeDetectorRef.markForCheck();
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage = this.resolveError(
          error,
          'No pudimos cargar tus cuentas. Inténtalo nuevamente.',
        );

        this.loading = false;

        this.changeDetectorRef.markForCheck();
      },
    });
  }

  openCreateForm(): void {
    this.editingAccount = null;
    this.errorMessage = '';
    this.successMessage = '';

    this.accountForm.reset({
      bank: '',
      nickname: '',
      type: 'Checking',
      last4: '',
      initialBalance: 0,
    });

    this.isFormOpen = true;
  }

  openEditForm(account: Account): void {
    this.editingAccount = account;
    this.errorMessage = '';
    this.successMessage = '';

    this.accountForm.reset({
      bank: account.bank,
      nickname: account.nickname,
      type: account.type,
      last4: account.last4,
      initialBalance: account.balance,
    });

    this.isFormOpen = true;
  }

  closeForm(): void {
    if (this.isSaving) {
      return;
    }

    this.isFormOpen = false;
    this.editingAccount = null;
  }

  submitAccount(): void {
    if (this.accountForm.invalid || this.isSaving) {
      this.accountForm.markAllAsTouched();
      return;
    }

    const value = this.accountForm.getRawValue();

    const bank = value.bank.trim();
    const nickname = value.nickname.trim();
    const last4 = value.last4.trim();

    if (!bank) {
      this.accountForm.controls.bank.setErrors({
        required: true,
      });

      return;
    }

    if (!nickname) {
      this.accountForm.controls.nickname.setErrors({
        required: true,
      });

      return;
    }

    const wasEditing = this.editingAccount !== null;

    let request$: Observable<unknown>;

    if (this.editingAccount) {
      const request: UpdateAccountRequest = {
        bank,
        nickname,
        type: value.type,
        last4,
      };

      request$ = this.accountsService.update(this.editingAccount.id, request);
    } else {
      const request: CreateAccountRequest = {
        bank,
        nickname,
        type: value.type,
        last4,
        initialBalance: Number(value.initialBalance),
        currency: 'PEN',
      };

      request$ = this.accountsService.create(request);
    }

    this.isSaving = true;
    this.errorMessage = '';

    request$.subscribe({
      next: () => {
        this.isSaving = false;

        this.successMessage = wasEditing
          ? 'Cuenta actualizada correctamente.'
          : 'Cuenta creada correctamente.';

        this.closeForm();
        this.loadAccounts();

        this.changeDetectorRef.markForCheck();
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage = this.resolveError(
          error,
          'No pudimos guardar la cuenta. Revisa los datos e inténtalo nuevamente.',
        );

        this.isSaving = false;

        this.changeDetectorRef.markForCheck();
      },
    });
  }

  requestDelete(account: Account): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.accountPendingDelete = account;
  }

  cancelDelete(): void {
    if (!this.isDeleting) {
      this.accountPendingDelete = null;
    }
  }

  confirmDelete(): void {
    if (!this.accountPendingDelete || this.isDeleting) {
      return;
    }

    const accountId = this.accountPendingDelete.id;

    this.isDeleting = true;
    this.errorMessage = '';

    this.accountsService.delete(accountId).subscribe({
      next: () => {
        this.accounts = this.accounts.filter((account) => account.id !== accountId);

        this.accountPendingDelete = null;
        this.isDeleting = false;

        this.successMessage = 'Cuenta eliminada correctamente.';

        this.changeDetectorRef.markForCheck();
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage =
          error.status === 409
            ? 'Esta cuenta no se puede eliminar porque tiene transacciones vinculadas.'
            : this.resolveError(error, 'No pudimos eliminar la cuenta. Inténtalo nuevamente.');

        this.accountPendingDelete = null;
        this.isDeleting = false;

        this.changeDetectorRef.markForCheck();
      },
    });
  }

  typeLabel(type: AccountType): string {
    return this.accountTypeOptions.find((option) => option.value === type)?.label ?? type;
  }

  formatCurrency(value: number): string {
    return value.toLocaleString('es-PE', {
      style: 'currency',
      currency: 'PEN',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });
  }

  formatUpdatedAt(updatedAt: string): string {
    const date = new Date(updatedAt);

    if (Number.isNaN(date.getTime())) {
      return 'Fecha no disponible';
    }

    return new Intl.DateTimeFormat('es-PE', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }).format(date);
  }

  trackByAccountId(_index: number, account: Account): number {
    return account.id;
  }

  private resolveError(error: HttpErrorResponse, fallback: string): string {
    const detail = error.error?.detail;

    return typeof detail === 'string' && detail.trim() ? detail : fallback;
  }
}
