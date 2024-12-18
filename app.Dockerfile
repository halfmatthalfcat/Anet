# syntax=docker/dockerfile:latest

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./Anet.sln ./Anet.sln
COPY ./Anet.API ./Anet.API
COPY ./Anet.Core ./Anet.Core
COPY ./Anet.Db ./Anet.Db

RUN dotnet restore
RUN dotnet build "Anet.API/Anet.API.csproj" -c Release -o /src/build
RUN dotnet publish "Anet.API/Anet.API.csproj" -c Release -o /src/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=publish /src/publish .
ENTRYPOINT [ "dotnet", "Anet.API.dll" ]