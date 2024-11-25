using Akka.Actor;

namespace Anet.Core.Akka.Actor;

public class PingActor : ReceiveActor
{
  public sealed record PingMessage(string Message);

  public sealed record PingPongRequest(string Message);
  public sealed record PingPongResponse(string Message);

  public PingActor()
  {
    Receive<PingMessage>(msg =>
    {
      Console.WriteLine($"PingActor Pinged with {msg.Message}");
    });

    Receive<PingPongRequest>(msg =>
    {
      Console.WriteLine($"PingActor PingPonged with {msg.Message}");
      Sender.Tell(new PingPongResponse(msg.Message), Self);
    });
  }
}