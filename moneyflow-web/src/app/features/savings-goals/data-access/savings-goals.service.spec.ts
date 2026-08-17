import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { environment } from '../../../../environments/environment';
import {
  AddSavingsGoalProgressRequest,
  CreateSavingsGoalRequest,
  SavingsGoal,
  UpdateSavingsGoalRequest,
} from '../models/savings-goal.models';
import { SavingsGoalsService } from './savings-goals.service';

describe('SavingsGoalsService', () => {
  const apiUrl = `${environment.apiUrl}/api/v1/savings-goals`;

  const savingsGoal: SavingsGoal = {
    id: 1,
    title: 'Fondo de emergencia',
    targetAmount: 10000,
    currentAmount: 2500,
    deadline: '2027-03-01',
  };

  let service: SavingsGoalsService;

  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SavingsGoalsService, provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(SavingsGoalsService);

    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('gets all savings goals', () => {
    service.getAll().subscribe((goals) => {
      expect(goals).toEqual([savingsGoal]);
    });

    const request = httpTestingController.expectOne(apiUrl);

    expect(request.request.method).toBe('GET');

    request.flush([savingsGoal]);
  });

  it('gets a savings goal by id', () => {
    service.getById(1).subscribe((goal) => {
      expect(goal).toEqual(savingsGoal);
    });

    const request = httpTestingController.expectOne(`${apiUrl}/1`);

    expect(request.request.method).toBe('GET');

    request.flush(savingsGoal);
  });

  it('creates a savings goal', () => {
    const createRequest: CreateSavingsGoalRequest = {
      title: 'Viaje a Cusco',
      targetAmount: 5000,
      deadline: '2027-07-01',
    };

    service.create(createRequest).subscribe((response) => {
      expect(response).toEqual({ id: 2 });
    });

    const request = httpTestingController.expectOne(apiUrl);

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(createRequest);

    request.flush({ id: 2 });
  });

  it('updates a savings goal', () => {
    const updateRequest: UpdateSavingsGoalRequest = {
      title: 'Fondo familiar',
      targetAmount: 12000,
      deadline: '2027-05-01',
    };

    service.update(1, updateRequest).subscribe();

    const request = httpTestingController.expectOne(`${apiUrl}/1`);

    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(updateRequest);

    request.flush(null);
  });

  it('adds progress to a savings goal', () => {
    const progressRequest: AddSavingsGoalProgressRequest = {
      amount: 500,
    };

    service.addProgress(1, progressRequest).subscribe();

    const request = httpTestingController.expectOne(`${apiUrl}/1/progress`);

    expect(request.request.method).toBe('PATCH');
    expect(request.request.body).toEqual(progressRequest);

    request.flush(null);
  });

  it('deletes a savings goal', () => {
    service.delete(1).subscribe();

    const request = httpTestingController.expectOne(`${apiUrl}/1`);

    expect(request.request.method).toBe('DELETE');

    request.flush(null);
  });
});
