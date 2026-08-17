import { HttpErrorResponse } from '@angular/common/http';
import {
  provideZonelessChangeDetection,
} from '@angular/core';
import {
  ComponentFixture,
  TestBed,
} from '@angular/core/testing';
import {
  of,
  Subject,
  throwError,
} from 'rxjs';

import { SavingsGoalsService } from '../../data-access/savings-goals.service';
import { SavingsGoal } from '../../models/savings-goal.models';
import { SavingsGoalsComponent } from './savings-goals.component';

describe('SavingsGoalsComponent', () => {
  const savingsGoals: SavingsGoal[] = [
    {
      id: 1,
      title: 'Fondo de emergencia',
      targetAmount: 10000,
      currentAmount: 2500,
      deadline: '2027-03-01',
    },
    {
      id: 2,
      title: 'Laptop nueva',
      targetAmount: 5000,
      currentAmount: 5000,
      deadline: null,
    },
  ];

  let fixture:
    ComponentFixture<SavingsGoalsComponent>;

  let component: SavingsGoalsComponent;

  let savingsGoalsService: {
    getAll: ReturnType<typeof vi.fn>;
    getById: ReturnType<typeof vi.fn>;
    create: ReturnType<typeof vi.fn>;
    update: ReturnType<typeof vi.fn>;
    addProgress: ReturnType<typeof vi.fn>;
    delete: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    savingsGoalsService = {
      getAll:
        vi.fn().mockReturnValue(
          of(savingsGoals),
        ),
      getById: vi.fn(),
      create: vi.fn(),
      update: vi.fn(),
      addProgress: vi.fn(),
      delete: vi.fn(),
    };

    await TestBed.configureTestingModule({
      imports: [SavingsGoalsComponent],
      providers: [
        provideZonelessChangeDetection(),
        {
          provide: SavingsGoalsService,
          useValue: savingsGoalsService,
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(
      SavingsGoalsComponent,
    );

    component = fixture.componentInstance;
  });

  it(
    'renders goals after asynchronous loading in zoneless mode',
    async () => {
      const goalsResult =
        new Subject<SavingsGoal[]>();

      savingsGoalsService.getAll
        .mockReturnValue(
          goalsResult.asObservable(),
        );

      fixture.detectChanges();

      expect(component.loading).toBe(true);

      expect(
        fixture.nativeElement.textContent,
      ).toContain(
        'Cargando metas de ahorro...',
      );

      goalsResult.next(savingsGoals);
      goalsResult.complete();

      await fixture.whenStable();

      expect(component.loading).toBe(false);

      expect(component.savingsGoals).toEqual(
        savingsGoals,
      );

      expect(
        fixture.nativeElement.querySelectorAll(
          'article.goal-card',
        ),
      ).toHaveLength(2);

      expect(
        fixture.nativeElement.textContent,
      ).toContain('Fondo de emergencia');
    },
  );

  it(
    'calculates total target, saved amount and completed goals',
    () => {
      fixture.detectChanges();

      expect(
        component.totalTargetAmount,
      ).toBe(15000);

      expect(
        component.totalCurrentAmount,
      ).toBe(7500);

      expect(
        component.completedGoalsCount,
      ).toBe(1);

      expect(
        component.progressPercent(
          savingsGoals[0],
        ),
      ).toBe(25);

      expect(
        component.remainingAmount(
          savingsGoals[0],
        ),
      ).toBe(7500);
    },
  );

  it(
    'creates a goal and reloads the list',
    () => {
      fixture.detectChanges();

      savingsGoalsService.create
        .mockReturnValue(
          of({ id: 3 }),
        );

      component.openCreateForm();

      component.goalForm.setValue({
        title: '  Viaje a Cusco  ',
        targetAmount: 4500,
        deadline: '2027-07-01',
      });

      component.submitGoal();

      expect(
        savingsGoalsService.create,
      ).toHaveBeenCalledWith({
        title: 'Viaje a Cusco',
        targetAmount: 4500,
        deadline: '2027-07-01',
      });

      expect(
        savingsGoalsService.getAll,
      ).toHaveBeenCalledTimes(2);

      expect(
        component.isGoalFormOpen,
      ).toBe(false);

      expect(component.successMessage).toBe(
        'Meta creada correctamente.',
      );
    },
  );

  it('updates an existing goal', () => {
    fixture.detectChanges();

    savingsGoalsService.update
      .mockReturnValue(of(undefined));

    component.openEditForm(
      savingsGoals[0],
    );

    component.goalForm.controls.title
      .setValue(
        'Fondo de emergencia familiar',
      );

    component.goalForm.controls.targetAmount
      .setValue(12000);

    component.submitGoal();

    expect(
      savingsGoalsService.update,
    ).toHaveBeenCalledWith(1, {
      title:
        'Fondo de emergencia familiar',
      targetAmount: 12000,
      deadline: '2027-03-01',
    });

    expect(
      component.isGoalFormOpen,
    ).toBe(false);

    expect(component.successMessage).toBe(
      'Meta actualizada correctamente.',
    );
  });

  it(
    'does not update a target below its current progress',
    () => {
      fixture.detectChanges();

      component.openEditForm(
        savingsGoals[0],
      );

      component.goalForm.controls.targetAmount
        .setValue(1000);

      component.submitGoal();

      expect(
        savingsGoalsService.update,
      ).not.toHaveBeenCalled();

      expect(component.errorMessage).toContain(
        'progreso actual',
      );
    },
  );

  it(
    'adds progress and reloads the list',
    () => {
      fixture.detectChanges();

      savingsGoalsService.addProgress
        .mockReturnValue(of(undefined));

      component.openProgressForm(
        savingsGoals[0],
      );

      component.progressForm.controls.amount
        .setValue(500);

      component.submitProgress();

      expect(
        savingsGoalsService.addProgress,
      ).toHaveBeenCalledWith(1, {
        amount: 500,
      });

      expect(
        savingsGoalsService.getAll,
      ).toHaveBeenCalledTimes(2);

      expect(
        component.isProgressFormOpen,
      ).toBe(false);

      expect(component.successMessage).toBe(
        'Progreso agregado correctamente.',
      );
    },
  );

  it(
    'does not add progress above the remaining amount',
    () => {
      fixture.detectChanges();

      component.openProgressForm(
        savingsGoals[0],
      );

      component.progressForm.controls.amount
        .setValue(8000);

      component.submitProgress();

      expect(
        savingsGoalsService.addProgress,
      ).not.toHaveBeenCalled();

      expect(
        component.progressForm.controls.amount
          .invalid,
      ).toBe(true);
    },
  );

  it(
    'removes a deleted goal from the visible list',
    () => {
      fixture.detectChanges();

      savingsGoalsService.delete
        .mockReturnValue(of(undefined));

      component.requestDelete(
        savingsGoals[0],
      );

      component.confirmDelete();

      expect(
        savingsGoalsService.delete,
      ).toHaveBeenCalledWith(1);

      expect(
        component.savingsGoals,
      ).toEqual([savingsGoals[1]]);

      expect(
        component.goalPendingDelete,
      ).toBeNull();

      expect(component.isDeleting).toBe(false);

      expect(component.successMessage).toBe(
        'Meta eliminada correctamente.',
      );
    },
  );

  it(
    'shows a loading error and stops the spinner',
    () => {
      savingsGoalsService.getAll
        .mockReturnValue(
          throwError(
            () =>
              new HttpErrorResponse({
                status: 500,
                statusText:
                  'Server Error',
              }),
          ),
        );

      fixture.detectChanges();

      expect(component.loading).toBe(false);

      expect(component.errorMessage).toBe(
        'No pudimos cargar tus metas de ahorro. Inténtalo nuevamente.',
      );
    },
  );
});
