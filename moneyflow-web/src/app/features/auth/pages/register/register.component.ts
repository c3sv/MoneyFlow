import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../auth.service';

export const passwordMatchValidator: ValidatorFn = (
  control: AbstractControl,
): ValidationErrors | null => {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');

  return password && confirmPassword && password.value === confirmPassword.value
    ? null
    : { passwordMismatch: true };
};

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: '../login/login.component.scss',
})
export class RegisterComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly isLoading = signal(false);
  readonly submitted = signal(false);

  readonly passwordVisible = signal(false);
  readonly confirmPasswordVisible = signal(false);

  readonly errorMessage = signal<string | null>(null);

  readonly registerForm = this.formBuilder.group(
    {
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
    },
    { validators: passwordMatchValidator },
  );

  submit(): void {
    this.submitted.set(true);
    this.errorMessage.set(null);

    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const { firstName, lastName, email, password } = this.registerForm.getRawValue();

    this.isLoading.set(true);

    this.authService
      .register({
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        email: email.trim(),
        password,
      })
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: () => {
          void this.router.navigateByUrl('/dashboard');
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessage.set(this.getRegisterErrorMessage(error));
        },
      });
  }

  togglePasswordVisibility(): void {
    this.passwordVisible.update((visible) => !visible);
  }

  toggleConfirmPasswordVisibility(): void {
    this.confirmPasswordVisible.update((visible) => !visible);
  }

  showFieldError(
    field: 'firstName' | 'lastName' | 'email' | 'password' | 'confirmPassword',
  ): boolean {
    const control = this.registerForm.controls[field];
    return control.invalid && (control.touched || this.submitted());
  }

  fieldHasError(
    field: 'firstName' | 'lastName' | 'email' | 'password' | 'confirmPassword',
    errorCode: string,
  ): boolean {
    return this.showFieldError(field) && this.registerForm.controls[field].hasError(errorCode);
  }

  formHasError(errorCode: string): boolean {
    return this.registerForm.hasError(errorCode) && (this.registerForm.touched || this.submitted());
  }

  private getRegisterErrorMessage(error: HttpErrorResponse): string {
    if (error.status === 0) {
      return 'No fue posible conectarse con MoneyFlow. Verifica que el backend esté ejecutándose.';
    }

    if (error.status === 409 || error.status === 400) {
      return 'El correo electrónico ya está registrado. Intenta iniciar sesión.';
    }

    if (error.status >= 500) {
      return 'MoneyFlow no está disponible en este momento. Inténtalo nuevamente.';
    }

    return 'No fue posible crear la cuenta. Revisa tus datos e inténtalo nuevamente.';
  }
}
