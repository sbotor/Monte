import { Routes } from '@angular/router';
import { AgentListComponent } from './pages';
import { loggedInGuard } from '@auth/logged-in.guard';
import { NotFoundComponent } from './pages/not-found/not-found.component';
import { ResourceChartComponent } from './pages/charts/resource-chart/resource-chart.component';

export const routes: Routes = [
  { path: '', redirectTo: 'agents', pathMatch: 'full' },
  { path: 'agents', component: AgentListComponent, canActivate: [loggedInGuard] },
  { path: 'agents/:id/chart', component: ResourceChartComponent, canActivate: [loggedInGuard] },
  { path: 'notFound', component: NotFoundComponent },
  { path: '**', redirectTo: 'notFound' }
];
