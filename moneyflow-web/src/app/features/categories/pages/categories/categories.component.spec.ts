import { HttpErrorResponse } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';

import { CategoriesService } from '../../data-access/categories.service';
import { Category } from '../../models/category.models';
import { CategoriesComponent } from './categories.component';

describe('CategoriesComponent', () => {
  const categories: Category[] = [
    { id: 1, name: 'Alimentación', type: 'Expense', icon: 'utensils' },
    { id: 2, name: 'Sueldo', type: 'Income', icon: 'wallet-cards' },
  ];

  let fixture: ComponentFixture<CategoriesComponent>;
  let component: CategoriesComponent;
  let categoriesService: {
    getAll: ReturnType<typeof vi.fn>;
    create: ReturnType<typeof vi.fn>;
    update: ReturnType<typeof vi.fn>;
    delete: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    categoriesService = {
      getAll: vi.fn().mockReturnValue(of(categories)),
      create: vi.fn(),
      update: vi.fn(),
      delete: vi.fn(),
    };

    await TestBed.configureTestingModule({
      imports: [CategoriesComponent],
      providers: [{ provide: CategoriesService, useValue: categoriesService }],
    }).compileComponents();

    fixture = TestBed.createComponent(CategoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('loads categories and calculates the expense and income totals', () => {
    expect(categoriesService.getAll).toHaveBeenCalledOnce();
    expect(component.categories).toEqual(categories);
    expect(component.expenseCount).toBe(1);
    expect(component.incomeCount).toBe(1);
    expect(component.isLoading).toBe(false);
  });

  it('filters categories by transaction type', () => {
    component.selectFilter('Expense');

    expect(component.filteredCategories).toEqual([categories[0]]);

    component.selectFilter('Income');

    expect(component.filteredCategories).toEqual([categories[1]]);
  });

  it('creates a category and reloads the list', () => {
    categoriesService.create.mockReturnValue(of({ id: 3 }));
    component.openCreateForm();
    component.categoryForm.setValue({
      name: '  Mascotas  ',
      type: 'Expense',
      icon: 'heart-pulse',
    });

    component.submitCategory();

    expect(categoriesService.create).toHaveBeenCalledWith({
      name: 'Mascotas',
      type: 'Expense',
      icon: 'heart-pulse',
    });
    expect(categoriesService.getAll).toHaveBeenCalledTimes(2);
    expect(component.successMessage).toBe('Categoría creada correctamente.');
    expect(component.isFormOpen).toBe(false);
  });

  it('updates only the editable category details', () => {
    categoriesService.update.mockReturnValue(of(undefined));
    component.openEditForm(categories[0]);
    component.categoryForm.controls.name.setValue('Comida');

    expect(component.categoryForm.controls.type.disabled).toBe(true);

    component.submitCategory();

    expect(categoriesService.update).toHaveBeenCalledWith(1, {
      name: 'Comida',
      icon: 'utensils',
    });
    expect(component.successMessage).toBe(
      'Categoría actualizada correctamente.',
    );
  });

  it('explains why an assigned category cannot be deleted', () => {
    categoriesService.delete.mockReturnValue(
      throwError(
        () =>
          new HttpErrorResponse({
            status: 409,
            statusText: 'Conflict',
          }),
      ),
    );
    component.requestDelete(categories[0]);

    component.confirmDelete();

    expect(categoriesService.delete).toHaveBeenCalledWith(1);
    expect(component.errorMessage).toContain('vinculada a transacciones');
    expect(component.categoryPendingDelete).toBeNull();
    expect(component.isDeleting).toBe(false);
  });
});