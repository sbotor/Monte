import { Injectable, Signal, WritableSignal, computed, signal } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { authCodeFlowConfig } from './authCodeFlowConfig';

export interface UserInfo {
  id: string;
  name: string;
  role: string;
  loggedIn: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  public readonly loggedIn: Signal<boolean>;

  private readonly _user: WritableSignal<UserInfo>;
  public readonly user: Signal<UserInfo>;

  constructor(private readonly oauth: OAuthService) {
    this._user = signal<UserInfo>({
      id: '',
      name: '',
      role: '',
      loggedIn: false
    });

    this.user = this._user.asReadonly();
    this.loggedIn = computed(() => this.user().loggedIn);
  }

  public configure() {
    this.oauth.configure(authCodeFlowConfig);
    this.oauth.loadDiscoveryDocumentAndTryLogin().then(_ => {
      if (!this.oauth.hasValidAccessToken()) {
        this.login();
      }

      this._user.set(this.extractUser());
    });
  }

  public logout() {
    this.oauth.logOut();
  }

  public login() {
    this.oauth.initCodeFlow();
  }

  public getToken() {
    return this.oauth.getAccessToken();
  }

  private extractUser() {
    const claims = this.oauth.getIdentityClaims();

    const data: UserInfo = {
      id: claims['sub'],
      name: claims['name'],
      role: claims['role'],
      loggedIn: true,
    };

    return data;
  }
}
