import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideOAuthClient } from 'angular-oauth2-oidc';
import { tokenInterceptor } from './auth/token.interceptor';
import { provideAnimations } from '@angular/platform-browser/animations';
import { DATE_PIPE_DEFAULT_OPTIONS } from '@angular/common';

const opts = Intl.DateTimeFormat().resolvedOptions();
const timeZone = opts.timeZone;
const locale = opts.locale;

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([tokenInterceptor])),
    provideOAuthClient(),
    provideAnimations(),
    {
      provide: DATE_PIPE_DEFAULT_OPTIONS,
      useValue: {
        timezone: timeZone,
        locale: locale,
      },
    }
  ],
};
