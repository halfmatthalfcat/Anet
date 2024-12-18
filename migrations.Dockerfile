# syntax=docker/dockerfile:latest

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG RUNTIME
WORKDIR /src

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

COPY ./Anet.sln ./Anet.sln
COPY ./Anet.Db ./Anet.Db
COPY ./Anet.Migrations ./Anet.Migrations

WORKDIR /src/Anet.Migrations

RUN dotnet restore
RUN dotnet ef migrations bundle --self-contained -r ${RUNTIME}

FROM ubuntu
WORKDIR /app
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
COPY --from=build /src/Anet.Migrations/efbundle ./efbundle
# Run with --connection <CONNECTION_STRING>
ENTRYPOINT [ "./efbundle" ]