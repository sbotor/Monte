import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav'
import { RouterOutlet } from '@angular/router';
import { LoaderComponent } from '../loader/loader.component';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, MatSidenavModule, RouterOutlet, LoaderComponent],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  public readonly loggedIn = this.auth.loggedIn;

  constructor(private readonly auth: AuthService) {
  }

  public getToken() {
    return this.auth.getToken();
  }
}
