FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GithubRepositoryAnalyzer.EventMessaging.Worker/GithubRepositoryAnalyzer.EventMessaging.Worker.csproj", "GithubRepositoryAnalyzer.EventMessaging.Worker/"]
COPY ["GithubRepositoryAnalyzer.EventMessaging.Contracts/GithubRepositoryAnalyzer.EventMessaging.Contracts.csproj", "GithubRepositoryAnalyzer.EventMessaging.Contracts/"]
COPY ["GithubRepositoryAnalyzer.Kernel/GithubRepositoryAnalyzer.Kernel.csproj", "GithubRepositoryAnalyzer.Kernel/"]
RUN dotnet restore "GithubRepositoryAnalyzer.EventMessaging.Worker/GithubRepositoryAnalyzer.EventMessaging.Worker.csproj"
COPY . .
WORKDIR "/src/GithubRepositoryAnalyzer.EventMessaging.Worker"
RUN dotnet build "GithubRepositoryAnalyzer.EventMessaging.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GithubRepositoryAnalyzer.EventMessaging.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GithubRepositoryAnalyzer.EventMessaging.Worker.dll"]
