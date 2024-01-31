import { Component, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { ActivatedRoute, Router } from '@angular/router';
import { UserDetails, UsersService } from '@features/users/users.service';
import {
  BehaviorSubject,
  Observable,
  ReplaySubject,
  Subject,
  catchError,
  combineLatest,
  map,
  of,
  takeUntil,
  throwError,
} from 'rxjs';
import { AuthService } from '@auth/auth.service';
import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangePasswordFormComponent,
  ChangePasswordFormValues,
} from '@features/users/change-password-form/change-password-form.component';
import { MatIconModule } from '@angular/material/icon';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { InputValidationService } from '@core/input-validation.service';
import {
  ChangeUsernameFormComponent,
  ChangeUsernameFormValues,
} from '@features/users/change-username-form/change-username-form.component';

interface OperationStatus {
  msg?: string;
  success?: boolean;
  cssClass?: string;
}

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    ChangePasswordFormComponent,
    MatIconModule,
    ChangeUsernameFormComponent,
  ],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.scss',
})
export class UserManagementComponent implements OnDestroy {
  private readonly destroyed$ = new Subject<void>();

  private readonly _isLoading = new BehaviorSubject(true);

  private readonly _passwordStatus = signal<OperationStatus>({ success: true });
  public readonly passwordStatus = this._passwordStatus.asReadonly();

  private readonly _usernameStatus = signal<OperationStatus>({ success: true });
  public readonly usernameStatus = this._usernameStatus.asReadonly();

  private readonly _user = new ReplaySubject<UserDetails>(1);
  public readonly user$ = combineLatest([this._user, this._isLoading]).pipe(
    map((x) => {
      const isLoading = x[1];
      if (isLoading) {
        return null;
      }

      const user = x[0];
      return { name: user.name, isAdmin: user.role === 'monte_admin' };
    })
  );

  private userId = '';

  constructor(
    private readonly router: Router,
    private readonly api: UsersService,
    auth: AuthService,
    route: ActivatedRoute
  ) {
    combineLatest([route.paramMap, auth.user$])
      .pipe(takeUntil(this.destroyed$))
      .subscribe((x) => {
        const params = x[0];
        const user = x[1];
        this.userId = params.get('id') ?? user.id;

        this.fetchData();
      });
  }

  public onPasswordChange(values?: ChangePasswordFormValues) {
    if (!values) {
      return;
    }

    this._passwordStatus.set({});

    this.api
      .changePassword(this.userId, {
        oldPassword: values.currentPassword,
        newPassword: values.newPassword,
      })
      .pipe(takeUntil(this.destroyed$), catchError(this.catchBadException))
      .subscribe((x) => {
        if (x === false) {
          this._passwordStatus.set({
            msg: 'Invalid data.',
            success: false,
            cssClass: 'text-error',
          });
        } else {
          this._passwordStatus.set({
            msg: 'Password has been changed.',
            success: true,
          });

          values.reset();
        }
      });
  }

  public onUsernameChange(values: ChangeUsernameFormValues) {
    if (!values) {
      return;
    }

    this.api
      .changeUsername(this.userId, { newUsername: values.username })
      .pipe(takeUntil(this.destroyed$), catchError(this.catchBadException))
      .subscribe((x) => {
        if (x === false) {
          this._usernameStatus.set({
            msg: 'Invalid or duplicate username.',
            success: false,
            cssClass: 'text-error',
          });
        } else {
          this._usernameStatus.set({
            msg: 'Username has been changed',
            success: true,
          });

          values.reset();
          this.fetchData();
        }
      });
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  private fetchData() {
    this._isLoading.next(true);

    this.api
      .getUser(this.userId)
      .pipe(
        takeUntil(this.destroyed$),
        catchError((x) => this.catchAuthError(x))
      )
      .subscribe((x) => {
        if (!x) {
          return;
        }

        this._user.next(x);
        this._isLoading.next(false);
      });
  }

  private catchAuthError(err: HttpErrorResponse): Observable<null> {
    if (err.status === 401 || err.status === 403 || err.status === 400) {
      this.router.navigate(['notFound']);
      return of(null);
    }

    return throwError(() => err);
  }

  private catchBadException(err: HttpErrorResponse) {
    if (err.status === 400) {
      return of(false);
    }

    return throwError(() => err);
  }
}
