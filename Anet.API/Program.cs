using System.ComponentModel.DataAnnotations;

using Akka.Hosting;
using Akka.Persistence.PostgreSql.Hosting;

using Anet.API;
using Anet.Core.Akka.Actor;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var appConfigration = new AppConfiguration();

builder.Services.AddOptions<AppConfiguration>();

builder.Services.AddControllers();

// Start up the Actor System in an IHostedService
builder.Services.AddAkka("AnetSystem", (b, sp) => 
  b
    .WithAnetActors()
    .WithPostgreSqlPersistence(
      appConfigration.ConnectionString,
      schemaName: "akka"
    )
);

var app = builder.Build();

app.MapControllers();
app.Run();
