FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GithubRepositoryAnalyzer/GithubRepositoryAnalyzer.csproj", "GithubRepositoryAnalyzer/"]
COPY ["GithubRepositoryAnalyzer.Domain/GithubRepositoryAnalyzer.Domain.csproj", "GithubRepositoryAnalyzer.Domain/"]
COPY ["GithubRepositoryAnalyzer.Config/GithubRepositoryAnalyzer.Config.csproj", "GithubRepositoryAnalyzer.Config/"]
COPY ["GithubRepositoryAnalyzer.EventMessaging.Contracts/GithubRepositoryAnalyzer.EventMessaging.Contracts.csproj", "GithubRepositoryAnalyzer.EventMessaging.Contracts/"]
COPY ["GithubRepositoryAnalyzer.Kernel/GithubRepositoryAnalyzer.Kernel.csproj", "GithubRepositoryAnalyzer.Kernel/"]
RUN dotnet restore "GithubRepositoryAnalyzer/GithubRepositoryAnalyzer.csproj"
COPY . .
WORKDIR "/src/GithubRepositoryAnalyzer"
RUN dotnet build "GithubRepositoryAnalyzer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GithubRepositoryAnalyzer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GithubRepositoryAnalyzer.dll"]
