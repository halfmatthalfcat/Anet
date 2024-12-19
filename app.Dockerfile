# syntax=docker/dockerfile:latest

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./Anet.sln ./Anet.sln
COPY ./Anet.API ./Anet.API
COPY ./Anet.Core ./Anet.Core
COPY ./Anet.Db ./Anet.Db

WORKDIR /src/Anet.API

RUN dotnet restore
RUN dotnet build "Anet.API.csproj" -c Release -o /src/build
RUN dotnet publish "Anet.API.csproj" -c Release -o /src/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

RUN apt-get update
RUN apt-get install curl -y

COPY --from=build /src/publish .
ENTRYPOINT [ "dotnet", "Anet.API.dll" ]