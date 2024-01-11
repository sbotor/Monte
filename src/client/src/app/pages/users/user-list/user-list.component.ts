import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserDetails, UsersService } from '@features/users/users.service';
import { MatIconModule } from '@angular/material/icon';
import { SpinnerComponent } from '@components/spinner';
import { AuthService, AuthUserInfo } from '@auth/auth.service';
import {
  BehaviorSubject,
  Subject,
  combineLatest,
  filter,
  map,
  switchMap,
  takeUntil,
} from 'rxjs';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { userRoles } from '@auth/roles';
import { UserDialogsService } from '@features/users/user-dialogs.service';
import { MatCardModule } from '@angular/material/card';
import { RouterLink } from '@angular/router';

export interface UserListDetails extends UserDetails {
  isCurrent: boolean;
  isAdmin: boolean;
}

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    CommonModule,
    MatListModule,
    SpinnerComponent,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    RouterLink
  ],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss',
})
export class UserListComponent implements OnInit, OnDestroy {
  private readonly destroyed$ = new Subject<void>();

  private readonly userList$ = new BehaviorSubject<UserDetails[]>([]);
  private readonly isLoading$ = new BehaviorSubject<boolean>(true);

  public readonly users$ = combineLatest([
    this.userList$,
    this.auth.user$,
    this.isLoading$,
  ]).pipe(map((x) => this.mapUserData(x[0], x[1], x[2])));

  constructor(
    private readonly api: UsersService,
    private readonly auth: AuthService,
    private readonly dialog: UserDialogsService
  ) {}

  ngOnInit(): void {
    this.fetchUsers();
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  public onDelete(user: UserDetails) {
    this.dialog
      .openDeleteConfirmationDialog(user.name)
      .pipe(
        takeUntil(this.destroyed$),
        filter((x) => x === true),
        switchMap((_) => this.api.deleteUser(user.id))
      )
      .subscribe(() => this.fetchUsers());
  }

  public onNewUserClicked() {
    this.dialog
      .openNewUserDialog()
      .pipe(
        takeUntil(this.destroyed$),
        filter((x) => x !== undefined),
        switchMap((x) => this.api.createUser(x!))
      )
      .subscribe(() => this.fetchUsers());
  }

  private fetchUsers() {
    this.isLoading$.next(true);

    this.api
      .getUsers()
      .pipe(takeUntil(this.destroyed$))
      .subscribe((x) => {
        this.userList$.next(x);
        this.isLoading$.next(false);
      });
  }

  private mapUserData(
    users: UserDetails[],
    current: AuthUserInfo,
    isLoading: boolean
  ): UserListDetails[] | null {
    if (isLoading) {
      return null;
    }

    const mapped = users.map<UserListDetails>((y) => {
      return {
        ...y,
        isCurrent: y.id === current.id,
        isAdmin: y.role === userRoles.admin,
      };
    });

    return mapped;
  }
}
