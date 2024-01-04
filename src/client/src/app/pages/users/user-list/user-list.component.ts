import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsersService } from '@features/users/users.service';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { SpinnerComponent } from '@components/spinner';
import { AuthService } from '@auth/auth.service';
import { combineLatest, map, of } from 'rxjs';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, MatListModule, MatIconModule, SpinnerComponent, MatCardModule],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss',
})
export class UserListComponent {
  public readonly users$ = combineLatest([
    this.api.getUsers(),
    this.auth.user$,
  ]).pipe(
    map((x) => {
      const mapped = x[0].map((y) => {
        return { ...y, isCurrent: y.id === x[1].id };
      });

      return mapped;
    })
  );

  constructor(
    private readonly api: UsersService,
    private readonly auth: AuthService
  ) {}
}
