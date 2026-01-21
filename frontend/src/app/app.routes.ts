import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { authGuard } from './guards/auth.guard';
import { guestGuard } from './guards/guest.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { 
    path: 'login', 
    component: LoginComponent,
    canActivate: [guestGuard],
    data: { animation: 'LoginPage' }
  },
  { 
    path: 'register', 
    component: RegisterComponent,
    canActivate: [guestGuard],
    data: { animation: 'RegisterPage' }
  },
  { 
    path: 'dashboard', 
    component: DashboardComponent,
    canActivate: [authGuard],
    data: { animation: 'DashboardPage' }
  },
  { path: '**', redirectTo: '/dashboard' }
];
