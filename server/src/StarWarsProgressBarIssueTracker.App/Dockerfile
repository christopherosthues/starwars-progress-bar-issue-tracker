FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StarWarsProgressBarIssueTracker.App/StarWarsProgressBarIssueTracker.App.csproj", "StarWarsProgressBarIssueTracker.App/"]
COPY ["StarWarsProgressBarIssueTracker.Domain/StarWarsProgressBarIssueTracker.Domain.csproj", "StarWarsProgressBarIssueTracker.Domain/"]
COPY ["StarWarsProgressBarIssueTracker.Infrastructure/StarWarsProgressBarIssueTracker.Infrastructure.csproj", "StarWarsProgressBarIssueTracker.Infrastructure/"]
COPY ["StarWarsProgressBarIssueTracker.Infrastructure.GitHub/StarWarsProgressBarIssueTracker.Infrastructure.GitHub.csproj", "StarWarsProgressBarIssueTracker.Infrastructure.GitHub/"]
COPY ["StarWarsProgressBarIssueTracker.Infrastructure.Gitlab/StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.csproj", "StarWarsProgressBarIssueTracker.Infrastructure.Gitlab/"]
RUN dotnet restore "StarWarsProgressBarIssueTracker.App/StarWarsProgressBarIssueTracker.App.csproj"
COPY . .
WORKDIR "/src/StarWarsProgressBarIssueTracker.App"
RUN dotnet build "StarWarsProgressBarIssueTracker.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StarWarsProgressBarIssueTracker.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StarWarsProgressBarIssueTracker.App.dll"]
CMD []
