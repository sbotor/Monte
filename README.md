# Monte
Monte is a simple resource usage monitoring system.

# Deployment
The server and client parts of Monte can be deployed independently of the agents. Each agent should be run as a service on the monitored machine.

## Server and client
The simplest way to deploy and launch the server and client components is with [Docker](https://www.docker.com/) and [Docker Compose](https://docs.docker.com/compose/). After installing the tools and starting the Docker daemon run `docker compose up` in the `src` directory where `compose.yaml` is located. This will build the required containers and run them in a default development configuration. Go to http://localhost:42000 to access the Angular application. To log in as the default admin user use the `admin` username with password `admin@DM1N`. The auth server can be found on http://localhost:7481 and the main API on http://localhost:7480. The PostgreSQL database server should be accessible on port `6543` with both username and password `postgres`.

### Configuration
There are four steps that should be configured: the auth server, the main API, the Angular client and the Docker Compose file.
The first two ASP.NET Core apps can be configured by using the `appsettings.json` (base config) and `appsettings.Docker.json` (enironment-specific config) files in their respective directories in the `server` folder. More information can be found [here](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0). The client and Compose configuration is straightforward and requires modifying a single file for each component.

#### Monte.AuthServer
The base configuration file for the auth server contains basic OpenID Connect (OIDC) settings:
- `TokenSettings.Lifetimes.AuthCode` - authorization code lifetime for the authorization code flow
- `TokenSettings.Lifetimes.AccessToken` - bearer access token lifetime
- `OidcAppSettings.Agent.ClientId` and `OidcAppSettings.Client.ClientId` - OIDC client IDs for the agent and client applications

Environment specific configuration contains:
- `ConnectionStrings.Default` - PostgreSQL connection string
- `AllowedOrigins` - array containing the allowed origins for CORS (has to be adjusted if the rest of the components' hosts change)
- `AllowHttp` - flag enabling non-HTTPS connections
- `AuthSettings.RedirectUri` - URI for the authorization code flow redirect (usually the same as the client's host)
- `AuthSettings.IsDevelopment` - flag enabling additional development tools such as Postman and SwaggerUI to access the API
- `TokenSettings.SigningKey` - secret key used in the JWT signing process
- `TokenSettings.Issuer` - URI of the JWT issuer (should be the same as the auth server's host)
- `OidcAppSettings.Agent.ClientSecret` - secret used in the client credentials flow in the agent authentication process

#### Monte.WebApi
In the case of the main API only the `appsettings.Docker.json` is of significance:
- `AllowedOrigins` - CORS allowed origins array (adjust if the client's host changes)
- `ConnectionStrings.Default` - PostgreSQL connection string
- `TokenSettings.Issuer` - issuer of the JWT (should be the same as the auth server's host)
- `TokenSettings.Key` - secret key used in the JWT signing process (must be the same key as the one used in auth server)

#### Client
The client config can be found in `client/environments/environment.docker.ts`:
- `apiUrl` - URI pointing to the main Monte API (including the `/api/` suffix)
- `authRootUrl` - URI pointing to the auth token issuer (host only, without any paths)
- `requireAuthHttp` - flag disabling connections to non-HTTPS auth servers (if omitted requires HTTPS for non-*localhost* hosts)
- `isProduction` - flag disabling development-related console information

#### Compose
All of the above components are connected via the Docker Compose configuration. Docker port mapping can be configured here, as well as environment variables can be modified or added to the ASP.NET Core-based projects. PostgreSQL can also be configured here. More information can be found [here](https://docs.docker.com/compose/compose-file/).