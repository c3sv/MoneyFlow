import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

type TxIcon = 'cart' | 'stream' | 'salary' | 'fuel' | 'health' | 'home' | 'other';
type TxType = 'income' | 'expense';

interface Transaction {
  id: string;
  merchant: string;
  account: string;
  category: string;
  date: string; // etiqueta ya agrupable, ej. "Hoy", "Ayer", "18 jul"
  amount: number; // negativo = gasto, positivo = ingreso
  icon: TxIcon;
}

interface TransactionGroup {
  date: string;
  transactions: Transaction[];
}

const CATEGORIES = [
  'Todas',
  'Alimentación',
  'Transporte',
  'Entretenimiento',
  'Salud',
  'Vivienda',
  'Ingresos',
  'Otros',
];

@Component({
  selector: 'app-transactions',
  imports: [CommonModule, FormsModule],
  templateUrl: './transactions.component.html',
  styleUrl: './transactions.component.scss',
})
export class TransactionsComponent {
  readonly categories = CATEGORIES;

  searchTerm = '';
  selectedCategory = 'Todas';
  selectedType: 'all' | TxType = 'all';

  readonly transactions: Transaction[] = [
    {
      id: 't1',
      merchant: 'Supermercado Wong',
      account: 'BCP · Cuenta sueldo',
      category: 'Alimentación',
      date: 'Hoy',
      amount: -145.3,
      icon: 'cart',
    },
    {
      id: 't2',
      merchant: 'Netflix',
      account: 'BBVA · Visa Oro',
      category: 'Entretenimiento',
      date: 'Hoy',
      amount: -45.9,
      icon: 'stream',
    },
    {
      id: 't3',
      merchant: 'Grifo Primax',
      account: 'BCP · Cuenta sueldo',
      category: 'Transporte',
      date: 'Ayer',
      amount: -120.0,
      icon: 'fuel',
    },
    {
      id: 't4',
      merchant: 'Farmacia Inkafarma',
      account: 'Yape · Billetera diaria',
      category: 'Salud',
      date: 'Ayer',
      amount: -68.5,
      icon: 'health',
    },
    {
      id: 't5',
      merchant: 'Depósito de nómina',
      account: 'BCP · Cuenta sueldo',
      category: 'Ingresos',
      date: '19 jul',
      amount: 4200.0,
      icon: 'salary',
    },
    {
      id: 't6',
      merchant: 'Alquiler departamento',
      account: 'BCP · Cuenta sueldo',
      category: 'Vivienda',
      date: '19 jul',
      amount: -1710.0,
      icon: 'home',
    },
    {
      id: 't7',
      merchant: 'Restobar La 73',
      account: 'BBVA · Visa Oro',
      category: 'Entretenimiento',
      date: '18 jul',
      amount: -89.0,
      icon: 'stream',
    },
    {
      id: 't8',
      merchant: 'Uber',
      account: 'Yape · Billetera diaria',
      category: 'Transporte',
      date: '18 jul',
      amount: -34.5,
      icon: 'fuel',
    },
    {
      id: 't9',
      merchant: 'Botica Arcángel',
      account: 'BCP · Cuenta sueldo',
      category: 'Salud',
      date: '17 jul',
      amount: -42.0,
      icon: 'health',
    },
    {
      id: 't10',
      merchant: 'Plaza Vea',
      account: 'BCP · Cuenta sueldo',
      category: 'Alimentación',
      date: '16 jul',
      amount: -212.4,
      icon: 'cart',
    },
    {
      id: 't11',
      merchant: 'Transferencia de Ana',
      account: 'Interbank · Ahorros metas',
      category: 'Ingresos',
      date: '15 jul',
      amount: 300.0,
      icon: 'salary',
    },
    {
      id: 't12',
      merchant: 'Suscripción gimnasio',
      account: 'BBVA · Visa Oro',
      category: 'Otros',
      date: '15 jul',
      amount: -99.0,
      icon: 'other',
    },
  ];

  get filteredTransactions(): Transaction[] {
    const term = this.searchTerm.trim().toLowerCase();

    return this.transactions.filter((tx) => {
      const matchesTerm = !term || tx.merchant.toLowerCase().includes(term);
      const matchesCategory =
        this.selectedCategory === 'Todas' || tx.category === this.selectedCategory;
      const matchesType =
        this.selectedType === 'all' ||
        (this.selectedType === 'income' && tx.amount > 0) ||
        (this.selectedType === 'expense' && tx.amount < 0);

      return matchesTerm && matchesCategory && matchesType;
    });
  }

  get groupedTransactions(): TransactionGroup[] {
    const groups: TransactionGroup[] = [];

    for (const tx of this.filteredTransactions) {
      const existing = groups.find((g) => g.date === tx.date);
      if (existing) {
        existing.transactions.push(tx);
      } else {
        groups.push({ date: tx.date, transactions: [tx] });
      }
    }

    return groups;
  }

  get totalIncome(): number {
    return this.filteredTransactions.filter((t) => t.amount > 0).reduce((s, t) => s + t.amount, 0);
  }

  get totalExpense(): number {
    return this.filteredTransactions
      .filter((t) => t.amount < 0)
      .reduce((s, t) => s + Math.abs(t.amount), 0);
  }

  setType(type: 'all' | TxType): void {
    this.selectedType = type;
  }

  formatCurrency(value: number): string {
    return value.toLocaleString('es-PE', {
      style: 'currency',
      currency: 'PEN',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });
  }
}
