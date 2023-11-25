import { AuthConfig } from "angular-oauth2-oidc"

export const authCodeFlowConfig: AuthConfig = {
  issuer: 'https://localhost:7049/',
  redirectUri: window.location.origin,
  clientId: 'monte_client',
  responseType: 'code',
  scope: 'openid roles profile monte_main_api',
  showDebugInformation: true,
}
