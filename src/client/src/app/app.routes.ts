import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { loggedInGuard } from './auth/logged-in.guard';
import { MachineListComponent } from './pages/machine-list/machine-list.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'machines', component: MachineListComponent, canActivate: [loggedInGuard] },
];
