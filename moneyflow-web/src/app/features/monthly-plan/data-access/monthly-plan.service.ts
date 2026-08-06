import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

export interface MonthlyPlan {
  id: number;
  totalBudget: number;
  spent: number;
}

@Injectable({ providedIn: 'root' })
export class MonthlyPlanService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/v1/monthly-plans`;

  get(): Observable<MonthlyPlan> {
    return this.http.get<MonthlyPlan>(this.apiUrl);
  }
}
