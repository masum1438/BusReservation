import { Routes } from '@angular/router';
import { authGuard, adminGuard } from './core/interceptors/jwt.interceptor';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./features/home/home').then(m => m.Home) },
  { path: 'login',    loadComponent: () => import('./features/auth/login/login').then(m => m.Login) },
  { path: 'register', loadComponent: () => import('./features/auth/register/register').then(m => m.Register) },
  { path: 'search',   loadComponent: () => import('./features/search/search').then(m => m.Search) },
  {
    path: 'booking/:scheduleId',
    loadComponent: () => import('./features/booking/booking').then(m => m.Booking),
    canActivate: [authGuard]
  },
  {
    path: 'my-bookings',
    loadComponent: () => import('./features/my-bookings/my-bookings').then(m => m.MyBookings),
    canActivate: [authGuard]
  },
  {
    path: 'admin',
    canActivate: [adminGuard],
    loadComponent: () => import('./features/admin/layout/admin-layout/admin-layout').then(m => m.AdminLayout),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard',  loadComponent: () => import('./features/admin/dashboard/admin-dashboard/admin-dashboard').then(m => m.AdminDashboard) },
      { path: 'buses',      loadComponent: () => import('./features/admin/buses/admin-buses/admin-buses').then(m => m.AdminBuses) },
      { path: 'routes',     loadComponent: () => import('./features/admin/routes/admin-routes/admin-routes').then(m => m.AdminRoutes) },
      { path: 'schedules',  loadComponent: () => import('./features/admin/schedules/admin-shcedules/admin-shcedules').then(m => m.AdminSchedules) },
      { path: 'bookings',   loadComponent: () => import('./features/admin/bookings/admin-bookings/admin-bookings').then(m => m.AdminBookings) },
      { path: 'passengers', loadComponent: () => import('./features/admin/passengers/admin-passengers/admin-passengers').then(m => m.AdminPassengers) },
    ]
  },
  { path: '**', redirectTo: '' }
];
