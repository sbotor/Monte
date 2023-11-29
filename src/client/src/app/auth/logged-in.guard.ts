import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from './auth.service';
import { tap } from 'rxjs';

export const loggedInGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  return auth.loginSuccessful$.pipe(tap(x => x || auth.login(state.url)));
};
