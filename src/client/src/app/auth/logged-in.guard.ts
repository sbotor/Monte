import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from './auth.service';

export const loggedInGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);

  if (auth.loggedIn()) {
    return true;
  }

  auth.login();

  return false;
};
