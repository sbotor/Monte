import { Injectable, signal } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { authCodeFlowConfig } from './authCodeFlowConfig';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly _loggedIn = signal<boolean>(false);
  public readonly loggedIn = this._loggedIn.asReadonly();

  constructor(private readonly oauth: OAuthService) { }

  public configure() {
    this.oauth.configure(authCodeFlowConfig);
    this.oauth.loadDiscoveryDocumentAndTryLogin().then(x => {
      if (!this.oauth.hasValidAccessToken()) {
        this.oauth.initCodeFlow();
      }

      this._loggedIn.set(true);
    });

    this.oauth.events.subscribe(x => console.log(x));
  }

  public login() {
    this.oauth.initCodeFlow();
  }

  public logout() {
    this.oauth.logOut();
  }

  public getToken() {
    return this.oauth.getAccessToken();
  }

  public isLoggedIn() {
    return this.oauth.hasValidAccessToken();
  }
}
