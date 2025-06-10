import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';
import { ToastComponent } from './components/toast/toast.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, ToastComponent],
  template: `
    <router-outlet></router-outlet>
    <app-toast></app-toast>
  `,
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'banking-poc';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Check if user is already authenticated and redirect appropriately
    if (this.authService.isAuthenticated()) {
      if (this.router.url === '/' || this.router.url === '/login') {
        this.router.navigate(['/home']);
      }
    }
  }
} 