import { Routes } from '@angular/router';
import { MachineListComponent } from './pages';
import { loggedInGuard } from '@auth/logged-in.guard';
import { NotFoundComponent } from './pages/not-found/not-found.component';
import { AvgCpuUsageComponent } from './pages/charts/avg-cpu-usage/avg-cpu-usage.component';

export const routes: Routes = [
  { path: '', redirectTo: 'machines', pathMatch: 'full' },
  { path: 'machines', component: MachineListComponent, canActivate: [loggedInGuard] },
  { path: 'machines/:id/cpu', component: AvgCpuUsageComponent, canActivate: [loggedInGuard] },
  { path: 'notFound', component: NotFoundComponent },
  { path: '**', redirectTo: 'notFound' }
];
