import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

export interface SavingsGoal {
  id: number;
  name: string;
  targetAmount: number;
  currentAmount: number;
}

@Injectable({ providedIn: 'root' })
export class SavingsGoalsService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/v1/savings-goals`;

  getAll(): Observable<SavingsGoal[]> {
    return this.http.get<SavingsGoal[]>(this.apiUrl);
  }
}
