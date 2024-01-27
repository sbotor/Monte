FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
EXPOSE 80

COPY Monte.AuthServer/*.csproj ./Monte.AuthServer/
RUN dotnet restore ./Monte.AuthServer/Monte.AuthServer.csproj

COPY Monte.AuthServer/ ./Monte.AuthServer/
WORKDIR /src/Monte.AuthServer
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Monte.AuthServer.dll"]
