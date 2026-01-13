import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { BusteContantiComponent } from './components/buste-contanti/buste-contanti.component';
import { CassaInteligenteComponent } from './components/cassa-intelligente/cassa-intelligente.component';
import { FondoSpeseComponent } from './components/fondo-spese/fondo-spese.component';
import { FondoCassaComponent } from './components/fondo-cassa/fondo-cassa.component';
import { SovvMonetariaComponent } from './components/sovv-monetaria/sovv-monetaria.component';
import { UserManagementComponent } from './components/user-management/user-management.component';
import { authGuard } from './guards/auth.guard';
import { guestGuard } from './guards/guest.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { 
    path: 'login', 
    component: LoginComponent,
    canActivate: [guestGuard]
  },
  { 
    path: 'register', 
    component: RegisterComponent,
    canActivate: [guestGuard]
  },
  { 
    path: 'dashboard', 
    component: DashboardComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'buste-contanti', 
    component: BusteContantiComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'cassa-intelligente', 
    component: CassaInteligenteComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'fondo-spese', 
    component: FondoSpeseComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'fondo-cassa', 
    component: FondoCassaComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'sovv-monetaria', 
    component: SovvMonetariaComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'user-management', 
    component: UserManagementComponent,
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '/dashboard' }
];
