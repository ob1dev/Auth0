FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 43163
EXPOSE 44399

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY OneGit.Web/OneGit.Web.csproj OneGit.Web/
RUN dotnet restore OneGit.Web/OneGit.Web.csproj
COPY . .
WORKDIR /src/OneGit.Web
RUN dotnet build OneGit.Web.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish OneGit.Web.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OneGit.Web.dll"]
