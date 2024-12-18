using Akka.Actor;
using Akka.Hosting;

using Microsoft.AspNetCore.Mvc;

using Anet.Core.Akka.Actor;

namespace Anet.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PingController(
  IRequiredActor<PingActor> pingActor
) : ControllerBase
{

  [HttpGet]
  [Route("")]
  [Tags("public")]
  public ActionResult Ping([FromQuery] string message)
  {
    // Tell is "fire-and-forget"
    pingActor.ActorRef.Tell(new PingActor.PingMessage(message));
    return Ok();
  }

  [HttpGet]
  [Route("pong")]
  [Tags("public")]
  public Task<string> Pong([FromQuery] string message) =>
    pingActor.ActorRef.Ask<
      PingActor.PingPongResponse
    >(new PingActor.PingPongRequest(message)).Map(response => response.Message);
}