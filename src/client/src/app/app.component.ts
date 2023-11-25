import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterOutlet } from '@angular/router';
import { SpinnerComponent } from './core/spinner/spinner.component';
import { AuthService } from './auth/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule,
    MatSidenavModule,
    RouterOutlet,
    SpinnerComponent,
    MatButtonModule,
    RouterLink,
    MatToolbarModule,],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'monte';

  public readonly loggedIn = this.auth.loggedIn;
  public readonly user = this.auth.user;

  constructor(private readonly auth: AuthService) {
    this.auth.configure();
  }

  public logout() {
    this.auth.logout();
  }
}
