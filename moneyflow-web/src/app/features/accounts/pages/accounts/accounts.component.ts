import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountsService, Account } from '../../data-access/accounts.service';

@Component({
  selector: 'app-accounts',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './accounts.component.html',
  styleUrl: './accounts.component.scss',
})
export class AccountsComponent implements OnInit {
  private accountsService = inject(AccountsService);

  accounts: Account[] = [];
  loading = true;

  ngOnInit(): void {
    this.accountsService.getAll().subscribe({
      next: (data) => {
        this.accounts = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading accounts', err);
        this.loading = false;
      },
    });
  }

  get netWorth(): number {
    return this.accounts.reduce((sum, acc) => sum + acc.balance, 0);
  }

  get totalAssets(): number {
    return this.accounts.filter((a) => a.balance > 0).reduce((sum, acc) => sum + acc.balance, 0);
  }

  get totalDebt(): number {
    return this.accounts
      .filter((a) => a.balance < 0)
      .reduce((sum, acc) => sum + Math.abs(acc.balance), 0);
  }

  typeLabel(type: Account['type']): string {
    const map = {
      checking: 'Cuenta corriente',
      savings: 'Ahorros',
      credit: 'Tarjeta de crédito',
      investment: 'Inversión',
      wallet: 'Billetera digital',
    };
    return map[type];
  }

  formatCurrency(value: number, currency: 'PEN' | 'USD' = 'PEN'): string {
    return value.toLocaleString('es-PE', {
      style: 'currency',
      currency,
      minimumFractionDigits: 2,
    });
  }
}
