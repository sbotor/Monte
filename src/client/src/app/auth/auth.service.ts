import {
  Injectable,
  Signal,
  WritableSignal,
  computed,
  signal,
} from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { authCodeFlowConfig } from './authCodeFlowConfig';
import { filter } from 'rxjs';

export interface UserInfo {
  id: string;
  name: string;
  role: string;
  loggedIn: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  public readonly loggedIn: Signal<boolean>;

  private readonly _user: WritableSignal<UserInfo>;
  public readonly user: Signal<UserInfo>;

  constructor(private readonly oauth: OAuthService) {
    this._user = signal<UserInfo>(this.emptyUser());

    this.user = this._user.asReadonly();
    this.loggedIn = computed(() => this.user().loggedIn);
  }

  public configure() {
    this.oauth.configure(authCodeFlowConfig);

    this.oauth.loadDiscoveryDocumentAndLogin().then((_) => {
      this.update();
    });

    this.oauth.events
      .pipe(filter((e) => e.type === 'token_received'))
      .subscribe((_) => {
        this.update();
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

  private update() {
    const claims = this.oauth.getIdentityClaims();

    const data: UserInfo = !claims
      ? this.emptyUser()
      : {
          id: claims['sub'],
          name: claims['name'],
          role: claims['role'],
          loggedIn: true,
        };

    this._user.set(data);
  }

  private emptyUser(): UserInfo {
    return {
      id: '',
      name: '',
      role: '',
      loggedIn: false,
    };
  }
}
