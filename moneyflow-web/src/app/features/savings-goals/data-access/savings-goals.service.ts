import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import {
  AddSavingsGoalProgressRequest,
  CreateSavingsGoalRequest,
  CreateSavingsGoalResponse,
  SavingsGoal,
  UpdateSavingsGoalRequest,
} from '../models/savings-goal.models';

export type {
  AddSavingsGoalProgressRequest,
  CreateSavingsGoalRequest,
  CreateSavingsGoalResponse,
  SavingsGoal,
  UpdateSavingsGoalRequest,
} from '../models/savings-goal.models';

@Injectable({ providedIn: 'root' })
export class SavingsGoalsService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = `${environment.apiUrl}/api/v1/savings-goals`;

  getAll(): Observable<SavingsGoal[]> {
    return this.http.get<SavingsGoal[]>(this.apiUrl);
  }

  getById(savingsGoalId: number): Observable<SavingsGoal> {
    return this.http.get<SavingsGoal>(`${this.apiUrl}/${savingsGoalId}`);
  }

  create(request: CreateSavingsGoalRequest): Observable<CreateSavingsGoalResponse> {
    return this.http.post<CreateSavingsGoalResponse>(this.apiUrl, request);
  }

  update(savingsGoalId: number, request: UpdateSavingsGoalRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${savingsGoalId}`, request);
  }

  addProgress(savingsGoalId: number, request: AddSavingsGoalProgressRequest): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${savingsGoalId}/progress`, request);
  }

  delete(savingsGoalId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${savingsGoalId}`);
  }
}
