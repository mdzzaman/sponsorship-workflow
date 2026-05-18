import { Routes } from '@angular/router';
import { authGuard, roleGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },

  // Requestor routes
  {
    path: 'requestor',
    canActivate: [authGuard, roleGuard(['Requestor'])],
    children: [
      { path: '', loadComponent: () => import('./features/requestor/dashboard/requestor-dashboard.component').then(m => m.RequestorDashboardComponent) },
      { path: 'new', loadComponent: () => import('./features/requestor/request-form/request-form.component').then(m => m.RequestFormComponent) },
      { path: 'edit/:id', loadComponent: () => import('./features/requestor/request-form/request-form.component').then(m => m.RequestFormComponent) },
      { path: 'detail/:id', loadComponent: () => import('./features/requestor/request-detail/request-detail.component').then(m => m.RequestDetailComponent) },
    ]
  },

  // Manager routes
  {
    path: 'manager',
    canActivate: [authGuard, roleGuard(['Manager'])],
    children: [
      { path: '', loadComponent: () => import('./features/manager/dashboard/manager-dashboard.component').then(m => m.ManagerDashboardComponent) },
      {
        path: 'detail/:id',
        loadComponent: () => import('./shared/components/request-detail-view.component').then(m => m.RequestDetailViewComponent),
        data: { backRoute: '/manager', navItems: [{ label: 'Pending Approvals', route: '/manager', icon: 'approval' }] }
      },
    ]
  },

  // Finance routes
  {
    path: 'finance',
    canActivate: [authGuard, roleGuard(['FinanceAdmin'])],
    children: [
      { path: '', loadComponent: () => import('./features/finance/dashboard/finance-dashboard.component').then(m => m.FinanceDashboardComponent) },
      {
        path: 'detail/:id',
        loadComponent: () => import('./shared/components/request-detail-view.component').then(m => m.RequestDetailViewComponent),
        data: { backRoute: '/finance', navItems: [{ label: 'Finance Review', route: '/finance', icon: 'account_balance' }] }
      },
    ]
  },

  // Admin routes
  {
    path: 'admin',
    canActivate: [authGuard, roleGuard(['SystemAdmin'])],
    children: [
      { path: '', loadComponent: () => import('./features/admin/dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent) },
      {
        path: 'detail/:id',
        loadComponent: () => import('./shared/components/request-detail-view.component').then(m => m.RequestDetailViewComponent),
        data: { backRoute: '/admin', navItems: [{ label: 'Admin Dashboard', route: '/admin', icon: 'admin_panel_settings' }] }
      },
    ]
  },

  { path: '**', redirectTo: 'login' }
];
