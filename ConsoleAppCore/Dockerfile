FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY ConsoleAppCore/ConsoleAppCore.csproj ConsoleAppCore/
RUN dotnet restore ConsoleAppCore/ConsoleAppCore.csproj
COPY . .
WORKDIR /src/ConsoleAppCore
RUN dotnet build ConsoleAppCore.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ConsoleAppCore.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ConsoleAppCore.dll"]
