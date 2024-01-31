import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { authCodeFlowConfig } from './authCodeFlowConfig';
import {
  BehaviorSubject,
  ReplaySubject,
  distinctUntilChanged,
  filter,
  switchMap,
} from 'rxjs';
import { Router } from '@angular/router';
import { userRoles } from './roles';

export interface AuthUserInfo {
  id: string;
  name: string;
  admin: boolean;
  external: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly _isLoaded = new BehaviorSubject(false);
  public readonly isLoaded$ = this._isLoaded
    .asObservable()
    .pipe(distinctUntilChanged());

  private readonly _isLoggedIn = new BehaviorSubject(false);
  public readonly isLoggedIn$ = this._isLoggedIn
    .asObservable()
    .pipe(distinctUntilChanged());

  public readonly loginSuccessful$ = this.isLoaded$.pipe(
    filter((x) => x),
    switchMap((_) => this.isLoggedIn$)
  );

  private readonly _token = new ReplaySubject<string>(1);
  public readonly token$ = this._token
    .asObservable()
    .pipe(distinctUntilChanged());

  private readonly _user = new ReplaySubject<AuthUserInfo>(1);
  public readonly user$ = this._user.asObservable();

  constructor(
    private readonly oauth: OAuthService,
    private readonly router: Router
  ) {}

  public configure() {
    this.oauth.configure(authCodeFlowConfig);

    this.oauth.events.subscribe((_) => this.update(false));

    this.oauth.loadDiscoveryDocumentAndTryLogin().then((_) => {
      this._isLoaded.next(true);

      this.update(true);

      let state = this.oauth.state;
      if (state && state.length > 0) {
        if (state.startsWith('%2F')) {
          state = state.substring(3);
        }

        this.router.navigateByUrl(state);
      }
    });
  }

  public logout() {
    this.oauth.logOut();
  }

  public login(returnUrl?: string) {
    this.oauth.initCodeFlow(returnUrl || this.router.url);
  }

  private update(loadUserProfile: boolean) {
    const hasValidToken = this.oauth.hasValidAccessToken();
    this._isLoggedIn.next(hasValidToken);

    if (hasValidToken) {
      if (loadUserProfile) {
        this.oauth.loadUserProfile();
      }
      this._token.next(this.oauth.getAccessToken());
    }

    const claims = this.oauth.getIdentityClaims();

    if (!claims) {
      return;
    }

    const data: AuthUserInfo = {
      id: claims['sub'],
      name: claims['name'],
      admin: claims['role'] === userRoles.admin,
      external: (claims['is_external'] as String)?.toLowerCase() === 'true'
    };

    this._user.next(data);
  }
}
