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

## Agent
Agent that communicates with the server can be found in `agent` directory. It is written in Python and monitors resources of the machine. To achieve consistent resources monitoring, it is advised that the agent be run as a service, which will be discussed in the next section. Before starting the service, you probably want to take a look at `config.yaml` in the `agent` directory. To avoid confusion, the **reporting period** field sets the reporting time in *seconds*. To run the script itself, it is enough to do `python ./main.py`. However you can choose your desired configuration by using the config switch, so you can run `python ./main.py --config development`, or whatever you defined in config.yaml. By default, the agent run as a service will use the production configuration, however, that can be changed in the scripts.
### Installation
The agent can be run as a service on systemd-based GNU/Linux and Windows operating systems, but there are important changes as to how to start them.
#### Systemd
You just run raas_systemd.sh and it works. Nah, not really but this isn't too far from the truth.

1. Create Python venv and install requirements.
2. Run `raas_systemd.sh` from `scripts` directory(it needs administrator access but the service itself is not run as a root)
3. The script copies relevant directories and files to `/usr/local/share/Monte`. To verify that it really does work, you can check out the `/home/user/monte/logs`. There should be a log file that describes what the service is currently doing.

To change `config.yaml`, find it and edit it in the `/usr/local/share/Monte` directory. When you are done, use `systemctl restart monte` to restart the service.

To uninstall the service you can do `systemctl stop monte` followed by `systemctl disable monte`. Finally you can do `systemctl daemon-reload`, `systemctl reset-failed` and delete `monte.service` and the `/usr/local/share/Monte` directory. 

#### Windows 
1. Get the release from GitHub. It is conveniently named monte_windows. Unzip it.
2. Open your PowerShell session as an administrator.
3. Give your session the ability to run scripts from the outside. Run `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass`
4. Run script from whereever you unzipped the scripts. It uses nssm.exe to run the agent as a service.
5. Assuming that there weren't any errors, you should be able to see `Agent for Monte` in the Windows services browser. 
6. All files relevant to the service are stored in `C:\ProgramData\Monte`. There should be a `logs` directory, if logs are present, service is (or was) running.

To uninstall the service you can open cmd as an administrator and run `sc stop monteagent` and `sc delete monteagent`. Windows will delete the service sometime. Maybe.

# Usage
None of the individual parts of the whole system have any interaction in the console. Behaviour can only be controlled by the respective config files. 

After the whole system is deployed and some machines have successfully started the MonteAgent service you can monitor the system in the client server, which is an angular application hosted on http://localhost:42000 (if deployed on docker). 

Upon entering you will see a login page. By default there is only one user with username `admin` and password `admin@DM1N`. After logging in you will see a list of agents ergo machines that have the MonteAgent service. Clicking on a agent on the list shows a detailed information of the machine's specification. The information has a button in the up right corner directing to charts. That page allows you to see the change in time of parameters of the choosen machine with different modes of data aggregation. The parameters that can be watched are: CPU usage (as a whole and as a single logical core), CPU load, memory usage and availability. The memory can be seen as regular memory or swap memory.

If you are logged in as an admin you have also the possibility to manage users. To do so, go to the `Users` tab in the navbar. There you can see a list of all users including yourself. On that page you can add a new user, delete a user or after clicking on the `Manage` button on a user you can change their username and password. 
A regular user can also change their username and password by clicking on their username on the right side of the navbar. 
One thing, that should be noted, is that only one admin can exist in the system and the admin cannot be removed.
