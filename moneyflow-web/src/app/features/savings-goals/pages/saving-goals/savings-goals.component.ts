import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectorRef,
  Component,
  OnInit,
  inject,
} from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Observable } from 'rxjs';

import { SavingsGoalsService } from '../../data-access/savings-goals.service';
import {
  CreateSavingsGoalRequest,
  SavingsGoal,
  UpdateSavingsGoalRequest,
} from '../../models/savings-goal.models';

@Component({
  selector: 'app-savings-goals',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
  ],
  templateUrl: './savings-goals.component.html',
  styleUrl: './savings-goals.component.scss',
})
export class SavingsGoalsComponent
  implements OnInit {
  private readonly savingsGoalsService =
    inject(SavingsGoalsService);

  private readonly formBuilder =
    inject(FormBuilder);

  private readonly changeDetectorRef =
    inject(ChangeDetectorRef);

  readonly minimumDeadline = this.today();

  readonly goalForm =
    this.formBuilder.nonNullable.group({
      title: [
        '',
        [
          Validators.required,
          Validators.maxLength(200),
        ],
      ],
      targetAmount: [
        0,
        [
          Validators.required,
          Validators.min(0.01),
        ],
      ],
      deadline: [''],
    });

  readonly progressForm =
    this.formBuilder.nonNullable.group({
      amount: [
        0,
        [
          Validators.required,
          Validators.min(0.01),
        ],
      ],
    });

  savingsGoals: SavingsGoal[] = [];

  editingGoal: SavingsGoal | null = null;
  progressGoal: SavingsGoal | null = null;
  goalPendingDelete: SavingsGoal | null = null;

  loading = true;
  isSaving = false;
  isAddingProgress = false;
  isDeleting = false;

  isGoalFormOpen = false;
  isProgressFormOpen = false;

  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadSavingsGoals();
  }

  get totalTargetAmount(): number {
    return this.savingsGoals.reduce(
      (total, goal) =>
        total + goal.targetAmount,
      0,
    );
  }

  get totalCurrentAmount(): number {
    return this.savingsGoals.reduce(
      (total, goal) =>
        total + goal.currentAmount,
      0,
    );
  }

  get completedGoalsCount(): number {
    return this.savingsGoals.filter(
      (goal) => this.isCompleted(goal),
    ).length;
  }

  get isEditing(): boolean {
    return this.editingGoal !== null;
  }

  loadSavingsGoals(): void {
    this.loading = true;
    this.errorMessage = '';

    this.savingsGoalsService
      .getAll()
      .subscribe({
        next: (savingsGoals) => {
          this.savingsGoals = savingsGoals;
          this.loading = false;

          this.changeDetectorRef.markForCheck();
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessage = this.resolveError(
            error,
            'No pudimos cargar tus metas de ahorro. Inténtalo nuevamente.',
          );

          this.loading = false;

          this.changeDetectorRef.markForCheck();
        },
      });
  }

  openCreateForm(): void {
    this.editingGoal = null;
    this.errorMessage = '';
    this.successMessage = '';

    this.goalForm.reset({
      title: '',
      targetAmount: 0,
      deadline: '',
    });

    this.isGoalFormOpen = true;
  }

  openEditForm(goal: SavingsGoal): void {
    this.editingGoal = goal;
    this.errorMessage = '';
    this.successMessage = '';

    this.goalForm.reset({
      title: goal.title,
      targetAmount: goal.targetAmount,
      deadline: goal.deadline ?? '',
    });

    this.isGoalFormOpen = true;
  }

  closeGoalForm(): void {
    if (this.isSaving) {
      return;
    }

    this.isGoalFormOpen = false;
    this.editingGoal = null;
  }

  submitGoal(): void {
    if (
      this.goalForm.invalid ||
      this.isSaving
    ) {
      this.goalForm.markAllAsTouched();
      return;
    }

    const value =
      this.goalForm.getRawValue();

    const title = value.title.trim();
    const targetAmount = Number(
      value.targetAmount,
    );

    if (!title) {
      this.goalForm.controls.title.setErrors({
        required: true,
      });

      return;
    }

    if (
      this.editingGoal &&
      targetAmount <
      this.editingGoal.currentAmount
    ) {
      this.errorMessage =
        'El monto objetivo no puede ser menor que el progreso actual.';

      return;
    }

    const deadline =
      value.deadline || null;

    const wasEditing =
      this.editingGoal !== null;

    let request$: Observable<unknown>;

    if (this.editingGoal) {
      const request:
        UpdateSavingsGoalRequest = {
        title,
        targetAmount,
        deadline,
      };

      request$ =
        this.savingsGoalsService.update(
          this.editingGoal.id,
          request,
        );
    } else {
      const request:
        CreateSavingsGoalRequest = {
        title,
        targetAmount,
        deadline,
      };

      request$ =
        this.savingsGoalsService.create(
          request,
        );
    }

    this.isSaving = true;
    this.errorMessage = '';

    request$.subscribe({
      next: () => {
        this.isSaving = false;

        this.successMessage = wasEditing
          ? 'Meta actualizada correctamente.'
          : 'Meta creada correctamente.';

        this.closeGoalForm();
        this.loadSavingsGoals();

        this.changeDetectorRef.markForCheck();
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage = this.resolveError(
          error,
          'No pudimos guardar la meta. Revisa los datos e inténtalo nuevamente.',
        );

        this.isSaving = false;

        this.changeDetectorRef.markForCheck();
      },
    });
  }

  openProgressForm(goal: SavingsGoal): void {
    if (this.isCompleted(goal)) {
      return;
    }

    const remaining =
      this.remainingAmount(goal);

    this.progressGoal = goal;
    this.errorMessage = '';
    this.successMessage = '';

    this.progressForm.controls.amount
      .setValidators([
        Validators.required,
        Validators.min(0.01),
        Validators.max(remaining),
      ]);

    this.progressForm.reset({
      amount: 0,
    });

    this.progressForm.controls.amount
      .updateValueAndValidity();

    this.isProgressFormOpen = true;
  }

  closeProgressForm(): void {
    if (this.isAddingProgress) {
      return;
    }

    this.isProgressFormOpen = false;
    this.progressGoal = null;
  }

  submitProgress(): void {
    if (
      !this.progressGoal ||
      this.progressForm.invalid ||
      this.isAddingProgress
    ) {
      this.progressForm.markAllAsTouched();
      return;
    }

    const amount = Number(
      this.progressForm.controls.amount.value,
    );

    const remaining =
      this.remainingAmount(
        this.progressGoal,
      );

    if (
      amount <= 0 ||
      amount > remaining
    ) {
      this.progressForm.controls.amount
        .setErrors({
          amountOutOfRange: true,
        });

      return;
    }

    this.isAddingProgress = true;
    this.errorMessage = '';

    this.savingsGoalsService
      .addProgress(
        this.progressGoal.id,
        { amount },
      )
      .subscribe({
        next: () => {
          this.isAddingProgress = false;

          this.successMessage =
            'Progreso agregado correctamente.';

          this.closeProgressForm();
          this.loadSavingsGoals();

          this.changeDetectorRef.markForCheck();
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessage =
            this.resolveError(
              error,
              'No pudimos agregar el progreso. Inténtalo nuevamente.',
            );

          this.isAddingProgress = false;

          this.changeDetectorRef.markForCheck();
        },
      });
  }

  requestDelete(goal: SavingsGoal): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.goalPendingDelete = goal;
  }

  cancelDelete(): void {
    if (!this.isDeleting) {
      this.goalPendingDelete = null;
    }
  }

  confirmDelete(): void {
    if (
      !this.goalPendingDelete ||
      this.isDeleting
    ) {
      return;
    }

    const savingsGoalId =
      this.goalPendingDelete.id;

    this.isDeleting = true;
    this.errorMessage = '';

    this.savingsGoalsService
      .delete(savingsGoalId)
      .subscribe({
        next: () => {
          this.savingsGoals =
            this.savingsGoals.filter(
              (goal) =>
                goal.id !== savingsGoalId,
            );

          this.goalPendingDelete = null;
          this.isDeleting = false;

          this.successMessage =
            'Meta eliminada correctamente.';

          this.changeDetectorRef.markForCheck();
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessage =
            this.resolveError(
              error,
              'No pudimos eliminar la meta. Inténtalo nuevamente.',
            );

          this.goalPendingDelete = null;
          this.isDeleting = false;

          this.changeDetectorRef.markForCheck();
        },
      });
  }

  progressPercent(goal: SavingsGoal): number {
    if (goal.targetAmount <= 0) {
      return 0;
    }

    return Math.min(
      100,
      Math.round(
        (
          goal.currentAmount /
          goal.targetAmount
        ) * 100,
      ),
    );
  }

  remainingAmount(goal: SavingsGoal): number {
    return Math.max(
      0,
      goal.targetAmount -
      goal.currentAmount,
    );
  }

  isCompleted(goal: SavingsGoal): boolean {
    return (
      goal.currentAmount >=
      goal.targetAmount
    );
  }

  formatCurrency(value: number): string {
    return value.toLocaleString('es-PE', {
      style: 'currency',
      currency: 'PEN',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });
  }

  formatDeadline(
    deadline: string | null,
  ): string {
    if (!deadline) {
      return 'Sin fecha límite';
    }

    const date = new Date(
      `${deadline}T00:00:00`,
    );

    if (Number.isNaN(date.getTime())) {
      return 'Fecha no disponible';
    }

    return new Intl.DateTimeFormat(
      'es-PE',
      {
        day: '2-digit',
        month: 'short',
        year: 'numeric',
      },
    ).format(date);
  }

  trackBySavingsGoalId(
    _index: number,
    goal: SavingsGoal,
  ): number {
    return goal.id;
  }

  private today(): string {
    const date = new Date();

    const year = date.getFullYear();

    const month = String(
      date.getMonth() + 1,
    ).padStart(2, '0');

    const day = String(
      date.getDate(),
    ).padStart(2, '0');

    return `${year}-${month}-${day}`;
  }

  private resolveError(
    error: HttpErrorResponse,
    fallback: string,
  ): string {
    const detail = error.error?.detail;

    return typeof detail === 'string' &&
    detail.trim()
      ? detail
      : fallback;
  }
}
