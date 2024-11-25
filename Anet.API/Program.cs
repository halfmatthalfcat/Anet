using Akka.Hosting;
using Akka.Actor;
using Anet.Core.Akka.Actor;
using System.Text;
using Anet.Core.Akka.Actor.Locking;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAkka("Anet", (builder, sp) =>
{
  builder.WithActors((system, registry, resolver) =>
  {
    var pingProps = resolver.Props<PingActor>();
    var pingActor = system.ActorOf(pingProps, "PingActor");
    registry.Register<PingActor>(pingActor);

    var lockProps = resolver.Props<LockingActor>();
    var lockActor = system.ActorOf(lockProps, "LockingActor");
    registry.Register<LockingActor>(lockActor);
  });
});

var app = builder.Build();

app.MapGet("/t1/ping", (IRequiredActor<PingActor> pingActor) =>
{
  pingActor.ActorRef.Tell(new PingActor.PingMessage("ping"));
  return Results.Ok();
});

app.MapPost("/t1/pingpong", async (HttpRequest Request, IRequiredActor<PingActor> PingActor) =>
{
  string rawContent = string.Empty;
  using (var reader = new StreamReader(Request.Body,
                  encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
  {
    rawContent = await reader.ReadToEndAsync();
  }

  var response = await PingActor.ActorRef.Ask<PingActor.PingPongResponse>(
    new PingActor.PingPongRequest(rawContent)
  );

  return Results.Ok(response.Message);
});

app.MapGet("/t2/lock", async (IRequiredActor<LockingActor> lockActor) =>
{
  var response = await lockActor.ActorRef.Ask<LockingActor.LockResponse>(new LockingActor.LockRequest());
  return Results.Ok(response);
});

app.MapGet("/t2/status", async (IRequiredActor<LockingActor> lockActor) =>
{
  var response = await lockActor.ActorRef.Ask<LockingActor.StatusResponse>(new LockingActor.StatusRequest());
  return Results.Ok(response);
});

app.Run();
