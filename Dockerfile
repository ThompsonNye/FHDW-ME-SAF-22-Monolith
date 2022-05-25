FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM base AS health_prep
RUN apk --no-cache add curl

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS publish
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
COPY . .
WORKDIR /src/WebApi
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish

FROM health_prep AS final
WORKDIR /app
COPY --from=publish /app/publish .
HEALTHCHECK --interval=5s --timeout=3s CMD curl --fail http://localhost/health || exit 1
ENTRYPOINT ["dotnet", "WebApi.dll"]