import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from './auth.service';
import { inject } from '@angular/core';
import { ApiService } from '../core/api.service';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const api = inject(ApiService);

  const url = req.url.toLowerCase();
  const apiUrl = api.baseUrl.toLocaleLowerCase();

  if (url.startsWith(apiUrl)) {
    const auth = inject(AuthService);

    if (!auth.loggedIn()) {
      return next(req);
    }

    const token = auth.getToken();
    req = req.clone({
      headers: req.headers.append('Authorization', `Bearer ${token}`)
    });
  }

  return next(req);
};
