using Akka.Actor;
using Akka.Hosting;

using Microsoft.AspNetCore.Mvc;

using Anet.Core.Akka.Actor.Locking;

namespace Anet.API.Controllers;

[ApiController]
[Route("api/v1/test/[controller]")]
public class LockController(
  IRequiredActor<LockingActor> lockingActor
) : ControllerBase
{
  [HttpGet]
  [Route("lock")]
  [Tags("public")]
  public Task<LockingActor.LockResponse> Lock() =>
    lockingActor
      .ActorRef
      .Ask<LockingActor.LockResponse>(LockingActor.LockRequest.Instance);

  [HttpGet]
  [Route("lock-status")]
  [Tags("public")]
  public Task<LockingActor.StatusResponse> LockStatus() =>
    lockingActor
      .ActorRef
      .Ask<LockingActor.StatusResponse>(LockingActor.StatusRequest.Instance);
}