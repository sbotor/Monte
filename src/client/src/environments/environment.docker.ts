import { Environment } from "./environment.interface";

export const environment: Environment = {
  apiUrl: 'http://localhost:7480/api/',
  authRootUrl: 'http://localhost:7481/',
  requireAuthHttp: false,
  isProduction: false,
};
