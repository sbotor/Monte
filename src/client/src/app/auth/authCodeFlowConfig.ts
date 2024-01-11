import { AuthConfig } from "angular-oauth2-oidc"
import { environment } from "environments/environment.development"

export const authCodeFlowConfig: AuthConfig = {
  issuer: environment.authRootUrl,
  redirectUri: window.location.origin + '/',
  clientId: 'monte_client',
  responseType: 'code',
  scope: 'openid roles profile monte_main_api',
  showDebugInformation: true,
}
