import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { 
    path: 'login', 
    loadComponent: () => import('./components/login/login.component').then(c => c.LoginComponent) 
  },
  { 
    path: 'home', 
    loadComponent: () => import('./components/home/home.component').then(c => c.HomeComponent),
    canActivate: [AuthGuard]
  },
  { 
    path: 'dashboard', 
    loadComponent: () => import('./app.component').then(c => c.AppComponent) 
  },
  { path: '**', redirectTo: '/login' }
]; 