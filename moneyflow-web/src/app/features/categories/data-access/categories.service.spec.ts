import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { environment } from '../../../../environments/environment';
import {
  Category,
  CreateCategoryRequest,
  UpdateCategoryRequest,
} from '../models/category.models';
import { CategoriesService } from './categories.service';

describe('CategoriesService', () => {
  const categoriesUrl = `${environment.apiUrl}/api/v1/categories`;

  let service: CategoriesService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        CategoriesService,
      ],
    });

    service = TestBed.inject(CategoriesService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('gets all categories', () => {
    const categories: Category[] = [
      { id: 1, name: 'Alimentación', type: 'Expense', icon: 'utensils' },
      { id: 2, name: 'Sueldo', type: 'Income', icon: 'wallet-cards' },
    ];

    service.getAll().subscribe((response) => {
      expect(response).toEqual(categories);
    });

    const request = httpTesting.expectOne(categoriesUrl);
    expect(request.request.method).toBe('GET');
    request.flush(categories);
  });

  it('creates a category', () => {
    const payload: CreateCategoryRequest = {
      name: 'Mascotas',
      type: 'Expense',
      icon: 'heart-pulse',
    };

    service.create(payload).subscribe((response) => {
      expect(response).toEqual({ id: 3 });
    });

    const request = httpTesting.expectOne(categoriesUrl);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(payload);
    request.flush({ id: 3 }, { status: 201, statusText: 'Created' });
  });

  it('updates a category without sending its type', () => {
    const payload: UpdateCategoryRequest = {
      name: 'Comida',
      icon: 'utensils',
    };

    service.update(1, payload).subscribe();

    const request = httpTesting.expectOne(`${categoriesUrl}/1`);
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(payload);
    expect(request.request.body).not.toHaveProperty('type');
    request.flush(null, { status: 204, statusText: 'No Content' });
  });

  it('deletes a category by id', () => {
    service.delete(2).subscribe();

    const request = httpTesting.expectOne(`${categoriesUrl}/2`);
    expect(request.request.method).toBe('DELETE');
    request.flush(null, { status: 204, statusText: 'No Content' });
  });
});