FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
EXPOSE 80

COPY Monte/*.csproj ./Monte/
COPY Monte.WebApi/*.csproj ./Monte.WebApi/
RUN dotnet restore ./Monte.WebApi/Monte.WebApi.csproj

COPY Monte/ ./Monte/
COPY Monte.WebApi/ ./Monte.WebApi/
WORKDIR /src/Monte.WebApi
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Monte.WebApi.dll"]
