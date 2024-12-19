# Akka.NET Experimentation

Uses:

- .NET 8
- [Akka.NET](https://getakka.net/articles/intro/what-is-akka.html) (Core, Hosting, DI)
- [LanguageExt](https://github.com/louthy/language-ext) (Functional C# Utilities)
- [ASP.NET](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-8.0&tabs=visual-studio)

## Running

### via Docker-Compose

Running via the supplied `docker-compose` is the easiest and most straightforward way to run the entire project. It will
automatically:
* Start Postgres
* Apply EF migrations from `Anet.Migrations`
* Start the `primary` Anet Actor System
* Start the peer Anet Actor Systems to form a cluster

> Note, if you're running on OSX, you may need to prepend DOCKER_DEFAULT_PLATFORM=linux/arm64

```sh
docker-compose up
```

API available at http://localhost:8000

### via `donet` CLI

You can run a single instance of Anet via the `dotnet` CLI (via `dotnet run`), however you must have Postgres running with the appropriate schema so Akka Persistence may still work. You can do this by simply upping Postgres and the EF migrations from the docker-compose by running `docker-compose up pg migrations`.

## Projects

- /Anet.API: The ASP.NET API Server
- /Anet.Core: Akka Implementation Details (Actors, Utilites, etc)
- /Anet.Db: The Entity Framework schema/model definitions
- /Anet.Migrations: The Entity Framework Postgres migrations

## Interaction

A [`bruno`](https://www.usebruno.com/) collection (located at `.bruno`) is included in the project that enumerates the various API endpoints, which interact with the various Actors implemented in the project.