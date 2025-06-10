import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.formBuilder.group({
      taxCode: ['', [Validators.required, Validators.minLength(16), Validators.maxLength(16)]],
      password: ['', [Validators.required, Validators.minLength(1)]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';

      const loginRequest = {
        taxCode: this.loginForm.value.taxCode.toUpperCase(),
        password: this.loginForm.value.password
      };

      this.authService.login(loginRequest).subscribe({
        next: (result) => {
          this.isLoading = false;
          if (result.success) {
            this.router.navigate(['/home']);
          } else {
            this.errorMessage = result.message || 'Errore durante il login';
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = 'Errore di connessione. Riprova più tardi.';
          console.error('Login error:', error);
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.loginForm.controls).forEach(key => {
      const control = this.loginForm.get(key);
      control?.markAsTouched();
    });
  }

  get taxCode() { return this.loginForm.get('taxCode'); }
  get password() { return this.loginForm.get('password'); }
} 