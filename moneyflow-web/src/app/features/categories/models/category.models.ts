export type CategoryType = 'Income' | 'Expense';

export interface Category {
  id: number;
  name: string;
  type: CategoryType;
  icon: string | null;
}

export interface CreateCategoryRequest {
  name: string;
  type: CategoryType;
  icon: string | null;
}

export interface CreateCategoryResponse {
  id: number;
}

export interface UpdateCategoryRequest {
  name: string;
  icon: string | null;
}