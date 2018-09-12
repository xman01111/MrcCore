FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY MrcCore/MrcCore.csproj MrcCore/
RUN dotnet restore MrcCore/MrcCore.csproj
COPY . .
WORKDIR /src/MrcCore
RUN dotnet build MrcCore.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish MrcCore.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MrcCore.dll"]
