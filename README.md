# Akka.NET Experimentation

Uses:

- .NET 8
- [Akka.NET](https://getakka.net/articles/intro/what-is-akka.html) (Core, Hosting, DI)
- [LanguageExt](https://github.com/louthy/language-ext) (Functional C# Utilities)
- [ASP.NET](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-8.0&tabs=visual-studio)

## Running

Running via the supplied `docker-compose` is the easiest and most straightforward way to run the entire project. It will
automatically build and start the API server, bootstrap Akka, start Postgres and apply the necessary EF migrations.

> Note, if you're running on OSX, you may need to prepend DOCKER_DEFAULT_PLATFORM=linux/arm64

```sh
docker-compose up
```

API available at http://localhost:5000

## Projects

- /Anet.API: The ASP.NET API Server
- /Anet.Core: Akka Implementation Details (Actors, Utilites, etc)
- /Anet.Db: The Entity Framework schema/model definitions
- /Anet.Migrations: The Entity Framework Postgres migrations

## Interaction

A `bruno` collection (located at `.bruno`) is included in the project that enumerates the various API endpoints, which interact with the various Actors implemented in the project.