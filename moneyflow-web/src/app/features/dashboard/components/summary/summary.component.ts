import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

interface Kpi {
  label: string;
  value: number;
  trend: number;
  positive: boolean;
}

interface CashflowPoint {
  month: string;
  income: number;
  expense: number;
}

interface CategorySlice {
  name: string;
  amount: number;
  percent: number;
  color: string;
  dashArray: string;
  dashOffset: number;
}

interface Transaction {
  id: string;
  merchant: string;
  category: string;
  date: string;
  amount: number; // negativo = gasto, positivo = ingreso
  icon: 'cart' | 'stream' | 'salary' | 'fuel' | 'health';
}

interface Goal {
  name: string;
  current: number;
  target: number;
  dueLabel: string;
}

@Component({
  selector: 'app-summary',
  imports: [CommonModule, RouterLink],
  templateUrl: './summary.component.html',
  styleUrl: './summary.component.scss',
})
export class SummaryComponent {
  readonly userFirstName = 'Carlos';

  readonly kpis: Kpi[] = [
    { label: 'Balance total', value: 24850.3, trend: 4.2, positive: true },
    { label: 'Ingresos del mes', value: 8200.0, trend: 2.1, positive: true },
    { label: 'Gastos del mes', value: 5340.75, trend: -6.5, positive: true },
    { label: 'Tasa de ahorro', value: 34.9, trend: 1.8, positive: true },
  ];

  readonly cashflow: CashflowPoint[] = [
    { month: 'Feb', income: 6900, expense: 5100 },
    { month: 'Mar', income: 7200, expense: 5600 },
    { month: 'Abr', income: 7050, expense: 4950 },
    { month: 'May', income: 7800, expense: 6100 },
    { month: 'Jun', income: 7600, expense: 5800 },
    { month: 'Jul', income: 8200, expense: 5340.75 },
  ];

  readonly categories: CategorySlice[] = this.buildCategorySlices([
    { name: 'Vivienda', amount: 1710, color: 'var(--chart-1)' },
    { name: 'Alimentación', amount: 1281, color: 'var(--chart-2)' },
    { name: 'Transporte', amount: 748, color: 'var(--chart-3)' },
    { name: 'Entretenimiento', amount: 641, color: 'var(--chart-4)' },
    { name: 'Salud', amount: 534, color: 'var(--chart-5)' },
    { name: 'Otros', amount: 427, color: 'var(--chart-6)' },
  ]);

  readonly transactions: Transaction[] = [
    {
      id: 't1',
      merchant: 'Supermercado Wong',
      category: 'Alimentación',
      date: '20 jul',
      amount: -145.3,
      icon: 'cart',
    },
    {
      id: 't2',
      merchant: 'Netflix',
      category: 'Entretenimiento',
      date: '19 jul',
      amount: -45.9,
      icon: 'stream',
    },
    {
      id: 't3',
      merchant: 'Depósito de nómina',
      category: 'Ingreso',
      date: '18 jul',
      amount: 4200.0,
      icon: 'salary',
    },
    {
      id: 't4',
      merchant: 'Grifo Primax',
      category: 'Transporte',
      date: '17 jul',
      amount: -120.0,
      icon: 'fuel',
    },
    {
      id: 't5',
      merchant: 'Farmacia Inkafarma',
      category: 'Salud',
      date: '16 jul',
      amount: -68.5,
      icon: 'health',
    },
  ];

  readonly goals: Goal[] = [
    { name: 'Fondo de emergencia', current: 6800, target: 10000, dueLabel: 'dic 2026' },
    { name: 'Viaje a Cusco', current: 2000, target: 5000, dueLabel: 'mar 2027' },
  ];

  get maxCashflowValue(): number {
    return Math.max(...this.cashflow.map((p) => Math.max(p.income, p.expense)));
  }

  barHeight(value: number): number {
    return Math.round((value / this.maxCashflowValue) * 100);
  }

  goalPercent(goal: Goal): number {
    return Math.min(100, Math.round((goal.current / goal.target) * 100));
  }

  formatCurrency(value: number): string {
    return value.toLocaleString('es-PE', {
      style: 'currency',
      currency: 'PEN',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });
  }

  private buildCategorySlices(
    input: { name: string; amount: number; color: string }[],
  ): CategorySlice[] {
    const total = input.reduce((sum, c) => sum + c.amount, 0);
    const circumference = 2 * Math.PI * 42; // radio 42 usado en el SVG
    let cumulativeBefore = 0;

    return input.map((c) => {
      const percent = (c.amount / total) * 100;
      const arcLength = (percent / 100) * circumference;
      const dashOffset = circumference - (cumulativeBefore / total) * circumference;

      cumulativeBefore += c.amount;

      return {
        ...c,
        percent: Math.round(percent),
        dashArray: `${arcLength} ${circumference}`,
        dashOffset,
      };
    });
  }
}
