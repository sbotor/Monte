import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from '@auth/auth.service';
import { SpinnerComponent } from '@components/spinner';
import { map } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    MatSidenavModule,
    RouterOutlet,
    SpinnerComponent,
    MatButtonModule,
    RouterLink,
    MatToolbarModule,
    MatIconModule,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'monte';

  public readonly loggedIn$ = this.auth.loginSuccessful$;
  public readonly user$ = this.auth.user$;
  public readonly isAdmin$ = this.auth.user$.pipe(map((x) => x.admin));

  constructor(private readonly auth: AuthService) {
    this.auth.configure();
  }

  public logout() {
    this.auth.logout();
  }
}
