import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { ApiService } from '@core/api.service';
import { AuthService } from './auth.service';
import { of, switchMap } from 'rxjs';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const api = inject(ApiService);

  const url = req.url.toLowerCase();
  const apiUrl = api.baseUrl.toLowerCase();

  if (!url.startsWith(apiUrl)) {
    return next(req);
  }

  const auth = inject(AuthService);

  return auth.loginSuccessful$.pipe(
    switchMap((x) => (x ? auth.token$ : of(null))),
    switchMap((x) => {
      if (x) {
        req = req.clone({
          headers: req.headers.append('Authorization', `Bearer ${x}`),
        });
      }

      return next(req);
    })
  );
};
