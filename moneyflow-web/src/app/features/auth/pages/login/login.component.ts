import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);

  private readonly authService = inject(AuthService);

  private readonly router = inject(Router);

  private readonly activatedRoute = inject(ActivatedRoute);

  readonly isLoading = signal(false);
  readonly submitted = signal(false);
  readonly passwordVisible = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly sessionExpired = signal(
    this.activatedRoute.snapshot.queryParamMap.get('sessionExpired') === '1',
  );

  readonly loginForm = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
    rememberMe: [true],
  });

  submit(): void {
    this.submitted.set(true);
    this.errorMessage.set(null);

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const { email, password, rememberMe } = this.loginForm.getRawValue();

    this.isLoading.set(true);

    this.authService
      .login(
        {
          email: email.trim(),
          password,
        },
        rememberMe,
      )
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: () => {
          void this.router.navigateByUrl(this.getReturnUrl());
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessage.set(this.getLoginErrorMessage(error));
        },
      });
  }

  togglePasswordVisibility(): void {
    this.passwordVisible.update((visible) => !visible);
  }

  showFieldError(field: 'email' | 'password'): boolean {
    const control = this.loginForm.controls[field];

    return control.invalid && (control.touched || this.submitted());
  }

  fieldHasError(field: 'email' | 'password', errorCode: string): boolean {
    return this.showFieldError(field) && this.loginForm.controls[field].hasError(errorCode);
  }

  private getReturnUrl(): string {
    const returnUrl = this.activatedRoute.snapshot.queryParamMap.get('returnUrl');

    if (returnUrl && returnUrl.startsWith('/') && !returnUrl.startsWith('//')) {
      return returnUrl;
    }
    return '/dashboard';
  }


  private getLoginErrorMessage(error: HttpErrorResponse): string {
    if (error.status === 0) {
      return 'No fue posible conectarse con MoneyFlow. Verifica que el backend esté ejecutándose.';
    }

    if (error.status === 400 || error.status === 401) {
      return 'El correo o la contraseña son incorrectos.';
    }

    if (error.status >= 500) {
      return 'MoneyFlow no está disponible en este momento. Inténtalo nuevamente.';
    }

    return 'No fue posible iniciar sesión. Revisa tus datos e inténtalo nuevamente.';
  }
}
