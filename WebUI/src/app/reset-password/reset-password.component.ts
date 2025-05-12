import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { APIService } from '../api.service';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-reset-password',
  standalone: false,
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent {
  resetForm: FormGroup;
  token: string;
  email: string;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private api: APIService,
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.resetForm = this.fb.group({
      password: ['', [
        Validators.required,
        Validators.minLength(6),
        Validators.pattern(/^(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$/) // Must contain at least 1 digit & 1 special char
      ]],
      confirmPassword: ['', Validators.required],
    }, { validator: this.passwordMatchValidator });

    this.token = this.route.snapshot.queryParams['token'];
    this.email = this.route.snapshot.queryParams['email'];
    this.authService.clearSession();
  }

  passwordMatchValidator(form: FormGroup) {
    return form.get('password')?.value === form.get('confirmPassword')?.value
      ? null : { mismatch: true };
  }

  submit() {
    if (this.resetForm.invalid) return;

    const request = {
      email: this.email,
      token: this.token,
      newPassword: this.resetForm.value.password,
    };

    this.api.resetPassword(request).subscribe({
      next:() => {
        this.successMessage = 'Password reset successful! Redirecting...';
        setTimeout(() => this.router.navigate(['/login']), 3000);
      },
      error:(error) => this.errorMessage = error.error.message || 'Failed to reset password'
    }
    );
  }
}
