import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable } from 'rxjs';

import { CategoriesService } from '../../data-access/categories.service';
import { Category, CategoryType } from '../../models/category.models';

type CategoryFilter = 'All' | CategoryType;

interface IconOption {
  value: string;
  label: string;
  glyph: string;
}

const ICON_OPTIONS: readonly IconOption[] = [
  { value: 'utensils', label: 'Alimentación', glyph: '🍴' },
  { value: 'bus', label: 'Transporte', glyph: '↔' },
  { value: 'house', label: 'Vivienda', glyph: '⌂' },
  { value: 'lightbulb', label: 'Servicios', glyph: '⚡' },
  { value: 'heart-pulse', label: 'Salud', glyph: '♡' },
  { value: 'graduation-cap', label: 'Educación', glyph: '◇' },
  { value: 'gamepad-2', label: 'Entretenimiento', glyph: '◈' },
  { value: 'shopping-bag', label: 'Compras', glyph: '▢' },
  { value: 'wallet-cards', label: 'Sueldo', glyph: '$' },
  { value: 'laptop', label: 'Freelance', glyph: '⌘' },
  { value: 'gift', label: 'Bonos', glyph: '✦' },
  { value: 'chart-no-axes-combined', label: 'Inversiones', glyph: '↗' },
  { value: 'circle-plus', label: 'Otro ingreso', glyph: '+' },
  { value: 'ellipsis', label: 'Otros', glyph: '•••' },
];

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss',
})
export class CategoriesComponent implements OnInit {
  private readonly categoriesService = inject(CategoriesService);
  private readonly formBuilder = inject(FormBuilder);

  readonly iconOptions = ICON_OPTIONS;
  readonly categoryForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    type: ['Expense' as CategoryType, Validators.required],
    icon: ['ellipsis', Validators.maxLength(100)],
  });

  categories: Category[] = [];
  selectedFilter: CategoryFilter = 'All';
  editingCategory: Category | null = null;
  categoryPendingDelete: Category | null = null;
  isLoading = true;
  isSaving = false;
  isDeleting = false;
  isFormOpen = false;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadCategories();
  }

  get filteredCategories(): Category[] {
    if (this.selectedFilter === 'All') {
      return this.categories;
    }

    return this.categories.filter(
      (category) => category.type === this.selectedFilter,
    );
  }

  get expenseCount(): number {
    return this.categories.filter((category) => category.type === 'Expense')
      .length;
  }

  get incomeCount(): number {
    return this.categories.filter((category) => category.type === 'Income')
      .length;
  }

  loadCategories(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.categoriesService.getAll().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.isLoading = false;
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage = this.resolveError(
          error,
          'No pudimos cargar tus categorías. Inténtalo nuevamente.',
        );
        this.isLoading = false;
      },
    });
  }

  selectFilter(filter: CategoryFilter): void {
    this.selectedFilter = filter;
  }

  openCreateForm(): void {
    this.editingCategory = null;
    this.errorMessage = '';
    this.categoryForm.controls.type.enable();
    this.categoryForm.reset({
      name: '',
      type: 'Expense',
      icon: 'ellipsis',
    });
    this.isFormOpen = true;
  }

  openEditForm(category: Category): void {
    this.editingCategory = category;
    this.errorMessage = '';
    this.categoryForm.reset({
      name: category.name,
      type: category.type,
      icon: category.icon ?? 'ellipsis',
    });
    this.categoryForm.controls.type.disable();
    this.isFormOpen = true;
  }

  closeForm(): void {
    if (this.isSaving) {
      return;
    }

    this.isFormOpen = false;
    this.editingCategory = null;
    this.categoryForm.controls.type.enable();
  }

  submitCategory(): void {
    if (this.categoryForm.invalid || this.isSaving) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    const value = this.categoryForm.getRawValue();
    const name = value.name.trim();
    const icon = value.icon.trim() || null;

    if (!name) {
      this.categoryForm.controls.name.setErrors({ required: true });
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    const request$: Observable<unknown> = this.editingCategory
      ? this.categoriesService.update(this.editingCategory.id, { name, icon })
      : this.categoriesService.create({ name, type: value.type, icon });

    request$.subscribe({
      next: () => {
        this.isSaving = false;
        this.successMessage = this.editingCategory
          ? 'Categoría actualizada correctamente.'
          : 'Categoría creada correctamente.';
        this.closeForm();
        this.loadCategories();
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage = this.resolveError(
          error,
          'No pudimos guardar la categoría. Revisa los datos e inténtalo nuevamente.',
        );
        this.isSaving = false;
      },
    });
  }

  requestDelete(category: Category): void {
    this.errorMessage = '';
    this.categoryPendingDelete = category;
  }

  cancelDelete(): void {
    if (!this.isDeleting) {
      this.categoryPendingDelete = null;
    }
  }

  confirmDelete(): void {
    if (!this.categoryPendingDelete || this.isDeleting) {
      return;
    }

    const categoryId = this.categoryPendingDelete.id;
    this.isDeleting = true;
    this.errorMessage = '';

    this.categoriesService.delete(categoryId).subscribe({
      next: () => {
        this.categories = this.categories.filter(
          (category) => category.id !== categoryId,
        );
        this.categoryPendingDelete = null;
        this.isDeleting = false;
        this.successMessage = 'Categoría eliminada correctamente.';
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage =
          error.status === 409
            ? 'Esta categoría no se puede eliminar porque está vinculada a transacciones o a un plan mensual.'
            : this.resolveError(
                error,
                'No pudimos eliminar la categoría. Inténtalo nuevamente.',
              );
        this.categoryPendingDelete = null;
        this.isDeleting = false;
      },
    });
  }

  iconGlyph(icon: string | null): string {
    return (
      this.iconOptions.find((option) => option.value === icon)?.glyph ?? '•'
    );
  }

  typeLabel(type: CategoryType): string {
    return type === 'Expense' ? 'Gasto' : 'Ingreso';
  }

  trackByCategoryId(_index: number, category: Category): number {
    return category.id;
  }

  private resolveError(error: HttpErrorResponse, fallback: string): string {
    const detail = error.error?.detail;
    return typeof detail === 'string' && detail.trim() ? detail : fallback;
  }
}