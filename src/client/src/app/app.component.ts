import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutComponent } from './core/layout/layout.component';
import { AuthService } from './auth/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, LayoutComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'monte';

  constructor(private readonly auth: AuthService) {
    this.auth.configure();
  }
}
