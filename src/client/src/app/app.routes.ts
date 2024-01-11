import { Routes } from '@angular/router';
import { AgentListComponent } from './pages';
import { loggedInGuard } from '@auth/logged-in.guard';
import { NotFoundComponent } from './pages/not-found/not-found.component';
import { ResourceChartComponent } from './pages/charts/resource-chart/resource-chart.component';
import { UserListComponent } from './pages/users/user-list/user-list.component';
import { isAdminGuard } from '@auth/is-admin.guard';
import { UserManagementComponent } from './pages/users/user-management/user-management.component';

export const routes: Routes = [
  { path: '', redirectTo: 'agents', pathMatch: 'full' },
  {
    path: 'agents',

    children: [
      {
        path: '',
        component: AgentListComponent,
        canActivate: [loggedInGuard],
      },
      {
        path: ':id/chart',
        component: ResourceChartComponent,
        canActivate: [loggedInGuard],
      },
    ],
  },
  {
    path: 'users',
    children: [
      {
        path: '',
        component: UserListComponent,
        canActivate: [loggedInGuard, isAdminGuard],
      },
      {
        path: 'manage',
        children: [
          {
            path: '',
            component: UserManagementComponent,
            canActivate: [loggedInGuard],
          },
          {
            path: ':id',
            component: UserManagementComponent,
            canActivate: [loggedInGuard, isAdminGuard],
          },
        ],
      },
    ],
  },
  { path: 'notFound', component: NotFoundComponent },
  { path: '**', redirectTo: 'notFound' },
];
