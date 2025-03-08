FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
RUN apt update && apt install -y postgresql-client && apt clean
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG SERVER_CONNECTION_STRING
ARG GITLAB_PROJECT_PATH
ARG GITLAB_TOKEN
ARG GITHUB_PROJECT_PATH
ARG GITHUB_TOKEN
ARG HUSKY
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Build.targets", "."]
COPY ["Directory.Packages.props", "."]
COPY ["StarWarsProgressBarIssueTracker.App/StarWarsProgressBarIssueTracker.App.csproj", "StarWarsProgressBarIssueTracker.App/"]
COPY ["StarWarsProgressBarIssueTracker.Domain/StarWarsProgressBarIssueTracker.Domain.csproj", "StarWarsProgressBarIssueTracker.Domain/"]
COPY ["StarWarsProgressBarIssueTracker.Infrastructure/StarWarsProgressBarIssueTracker.Infrastructure.csproj", "StarWarsProgressBarIssueTracker.Infrastructure/"]
COPY ["StarWarsProgressBarIssueTracker.Infrastructure.GitHub/StarWarsProgressBarIssueTracker.Infrastructure.GitHub.csproj", "StarWarsProgressBarIssueTracker.Infrastructure.GitHub/"]
COPY ["StarWarsProgressBarIssueTracker.Infrastructure.Gitlab/StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.csproj", "StarWarsProgressBarIssueTracker.Infrastructure.Gitlab/"]
ENV ConnectionStrings__IssueTrackerContext=${SERVER_CONNECTION_STRING}
ENV Gitlab__ProjectPath=${GITLAB_PROJECT_PATH}
ENV Gitlab__Token=${GITLAB_TOKEN}
ENV GitHub__ProjectPath=${GITHUB_PROJECT_PATH}
ENV GitHub__Token=${GITHUB_TOKEN}
ENV Keycloak__Audience=${KEYCLOAK_AUDIENCE}
ENV Keycloak__MetadataAddress=${KEYCLOAK_METADATA_ADDRESS}
ENV Keycloak__ValidIssuer=${KEYCLOAK_VALID_ISSUER}
ENV Keycloak__ClientId=${KEYCLOAK_CLIENT_ID}
ENV Keycloak__ClientSecret=${KEYCLOAK_CLIENT_SECRET}
ENV Keycloak__Authority=${KEYCLOAK_AUTHORITY}
ENV Keycloak__RegistrationUrl=${KEYCLOAK_REGISTRATION_URL}
ENV Keycloak__TokenUrl=${KEYCLOAK_TOKEN_URL}
ENV HUSKY=${HUSKY}
RUN dotnet restore "StarWarsProgressBarIssueTracker.App/StarWarsProgressBarIssueTracker.App.csproj"
COPY . .
WORKDIR "/src/StarWarsProgressBarIssueTracker.App"
RUN dotnet build "StarWarsProgressBarIssueTracker.App.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StarWarsProgressBarIssueTracker.App.csproj" \
    -c "$BUILD_CONFIGURATION" \
    -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StarWarsProgressBarIssueTracker.App.dll"]
CMD []
