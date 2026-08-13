import { HttpErrorResponse } from '@angular/common/http';
import { provideZonelessChangeDetection } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, Subject, throwError } from 'rxjs';

import { CategoriesService } from '../../data-access/categories.service';
import { Category } from '../../models/category.models';
import { CategoriesComponent } from './categories.component';

describe('CategoriesComponent', () => {
  const categories: Category[] = [
    {
      id: 1,
      name: 'Alimentación',
      type: 'Expense',
      icon: 'utensils',
    },
    {
      id: 2,
      name: 'Sueldo',
      type: 'Income',
      icon: 'wallet-cards',
    },
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
      providers: [
        provideZonelessChangeDetection(),
        {
          provide: CategoriesService,
          useValue: categoriesService,
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CategoriesComponent);
    component = fixture.componentInstance;
  });

  it('renders categories after an asynchronous load in zoneless mode', async () => {
    const categoriesResult = new Subject<Category[]>();

    categoriesService.getAll.mockReturnValue(
      categoriesResult.asObservable(),
    );

    fixture.detectChanges();

    expect(
      fixture.nativeElement.querySelectorAll('.category-skeleton'),
    ).toHaveLength(6);

    categoriesResult.next(categories);
    categoriesResult.complete();

    await fixture.whenStable();

    expect(categoriesService.getAll).toHaveBeenCalledOnce();
    expect(component.categories).toEqual(categories);
    expect(component.expenseCount).toBe(1);
    expect(component.incomeCount).toBe(1);
    expect(component.isLoading).toBe(false);

    expect(
      fixture.nativeElement.querySelectorAll('.category-card'),
    ).toHaveLength(2);

    expect(
      fixture.nativeElement.querySelectorAll('.category-skeleton'),
    ).toHaveLength(0);
  });

  it('filters categories by transaction type', () => {
    fixture.detectChanges();

    component.selectFilter('Expense');

    expect(component.filteredCategories).toEqual([
      categories[0],
    ]);

    component.selectFilter('Income');

    expect(component.filteredCategories).toEqual([
      categories[1],
    ]);
  });

  it('creates a category and reloads the list', () => {
    fixture.detectChanges();

    categoriesService.create.mockReturnValue(
      of({
        id: 3,
      }),
    );

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
    expect(component.successMessage).toBe(
      'Categoría creada correctamente.',
    );
    expect(component.isFormOpen).toBe(false);
  });

  it('updates only the editable category details', () => {
    fixture.detectChanges();

    categoriesService.update.mockReturnValue(of(undefined));

    component.openEditForm(categories[0]);
    component.categoryForm.controls.name.setValue('Comida');

    expect(
      component.categoryForm.controls.type.disabled,
    ).toBe(true);

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
    fixture.detectChanges();

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
    expect(component.errorMessage).toContain(
      'vinculada a transacciones',
    );
    expect(component.categoryPendingDelete).toBeNull();
    expect(component.isDeleting).toBe(false);
  });
});