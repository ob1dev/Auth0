FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 55836
EXPOSE 44359

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY OneGit.Api/OneGit.Api.csproj OneGit.Api/
RUN dotnet restore OneGit.Api/OneGit.Api.csproj
COPY . .
WORKDIR /src/OneGit.Api
RUN dotnet build OneGit.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish OneGit.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OneGit.Api.dll"]
