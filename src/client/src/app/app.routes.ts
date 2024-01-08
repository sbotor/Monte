import { Routes } from '@angular/router';
import { AgentListComponent } from './pages';
import { loggedInGuard } from '@auth/logged-in.guard';
import { NotFoundComponent } from './pages/not-found/not-found.component';
import { ResourceChartComponent } from './pages/charts/resource-chart/resource-chart.component';
import { UserListComponent } from './pages/users/user-list/user-list.component';
import { isAdminGuard } from '@auth/is-admin.guard';
import { CreateUserFormComponent } from '@features/users/create-user-form/create-user-form.component';

export const routes: Routes = [
  { path: '', redirectTo: 'agents', pathMatch: 'full' },
  { path: 'agents', component: AgentListComponent, canActivate: [loggedInGuard] },
  { path: 'agents/:id/chart', component: ResourceChartComponent, canActivate: [loggedInGuard] },
  { path: 'users', component: UserListComponent, canActivate: [loggedInGuard, isAdminGuard] },
  { path: 'users/new', component: CreateUserFormComponent, canActivate: [loggedInGuard, isAdminGuard] },
  { path: 'notFound', component: NotFoundComponent },
  { path: '**', redirectTo: 'notFound' }
];
