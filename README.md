# Akka.NET Expirementaiton

Uses:

- .NET 8
- [Akka.NET](https://getakka.net/articles/intro/what-is-akka.html) (Core, Hosting, DI)
- [LanguageExt](https://github.com/louthy/language-ext) (Functional C# Utilities)
- [ASP.NET](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-8.0&tabs=visual-studio) (Minimal API)

## Running

```sh
dotnet run
```

API available at http://localhost:5289

## Projects

- /Anet.API: The ASP.NET API Server
- /Anet.Core: Akka Implementation Details (Actors, Utilites, etc)

## Actors

### PingPong

Run a simple ping and/or ping-pong through an actor.

#### Endpoints

##### GET /t1/ping

Return a simple 200. PingActor will log out to console.

##### POST /t1/pingpong

POST text to the PingActor and get that text back.

### LockingActor

Mimick a "locking" resource with a static timeout. Implements Akka.Net's `FSM` + Timers.

#### Endpoints

##### GET /t2/lock

Initiate a lock or retrieve the lock status.

##### GET /t2/status

Get the current status of the LockingActor.
