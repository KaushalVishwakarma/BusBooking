import { Routes } from '@angular/router';
import { authGuard, adminGuard, publicGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    canActivate: [publicGuard],
    loadComponent: () => import('./pages/auth/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    canActivate: [publicGuard],
    loadComponent: () => import('./pages/auth/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'user',
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', loadComponent: () => import('./pages/user/dashboard/user-dashboard.component').then(m => m.UserDashboardComponent) },
      { path: 'search', loadComponent: () => import('./pages/user/search/search.component').then(m => m.SearchComponent) },
      { path: 'bookings', loadComponent: () => import('./pages/user/bookings/bookings.component').then(m => m.BookingsComponent) },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  {
    path: 'admin',
    canActivate: [adminGuard],
    children: [
      { path: 'dashboard', loadComponent: () => import('./pages/admin/dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent) },
      { path: 'buses', loadComponent: () => import('./pages/admin/buses/buses.component').then(m => m.BusesComponent) },
      { path: 'routes', loadComponent: () => import('./pages/admin/routes/routes.component').then(m => m.RoutesComponent) },
      { path: 'schedules', loadComponent: () => import('./pages/admin/schedules/schedules.component').then(m => m.SchedulesComponent) },
      { path: 'bookings', loadComponent: () => import('./pages/admin/bookings/admin-bookings.component').then(m => m.AdminBookingsComponent) },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
