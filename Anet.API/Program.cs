using Akka.Hosting;
using Akka.Cluster.Hosting;
using Akka.HealthCheck.Hosting;
using Akka.HealthCheck.Hosting.Web;
using Akka.Remote.Hosting;
using Akka.Persistence.PostgreSql.Hosting;

using Anet.API;
using Anet.Core.Akka.Actor;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var appConfigration = new AppConfiguration();

builder.Services.AddOptions<AppConfiguration>();

builder.Services.AddControllers();
builder.Services.WithAkkaHealthCheck(HealthCheckType.All);

// Start up the Actor System in an IHostedService
builder.Services.AddAkka("AnetSystem", (b, sp) => 
  b
    .WithWebHealthCheck(sp)
    .WithAnetActors()
    .WithPostgreSqlPersistence(
      appConfigration.ConnectionString,
      schemaName: "akka"
    )
    .WithRemoting(
      appConfigration.RemoteHost,
      port: appConfigration.RemotePort
    )
    .WithClustering(
      new ClusterOptions(){
        SeedNodes = [..appConfigration.Peers],
      }
    )
);

var app = builder.Build();

app.MapControllers();
app.MapAkkaHealthCheckRoutes();
app.Run();
