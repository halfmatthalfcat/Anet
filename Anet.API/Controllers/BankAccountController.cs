using Akka.Actor;
using Akka.Hosting;

using Microsoft.AspNetCore.Mvc;

using Anet.Core.Akka.Actor.BankAccount;

namespace Anet.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BankAccountController(
  IRequiredActor<BankAccountActor> bankAccountActor
) : ControllerBase
{
  [HttpGet]
  [Route("status")]
  [Tags("public")]
  public Task<ActionResult> Status() =>
    bankAccountActor.ActorRef.Ask<StatusResponse>(
      StatusRequest.Instance
    ).Map(response => response.Response.Match<ActionResult>(
      success => Ok(success.Balance),
      failure => BadRequest(failure)
    ));

  [HttpGet]
  [Route("open")]
  [Tags("public")]
  public Task<ActionResult> Open() =>
    bankAccountActor.ActorRef.Ask<StatusResponse>(
      OpenAccount.Instance
    ).Map(response => response.Response.Match<ActionResult>(
      success => Ok(success.Balance),
      failure => BadRequest(failure)
    ));

  [HttpGet]
  [Route("deposit")]
  [Tags("public")]
  public Task<ActionResult> Deposit([FromQuery] decimal amount) =>
    bankAccountActor.ActorRef.Ask<StatusResponse>(
      new Deposit(amount)
    ).Map(response => response.Response.Match<ActionResult>(
      success => Ok(success.Balance),
      failure => BadRequest(failure)
    ));

  [HttpGet]
  [Route("withdraw")]
  [Tags("public")]
  public Task<ActionResult> Withdraw([FromQuery] decimal amount) =>
    bankAccountActor.ActorRef.Ask<StatusResponse>(
      new Withdraw(amount)
    ).Map(response => response.Response.Match<ActionResult>(
      success => Ok(success.Balance),
      failure => BadRequest(failure)
    ));

  [HttpGet]
  [Route("close")]
  [Tags("public")]
  public Task<ActionResult> Close() =>
    bankAccountActor.ActorRef.Ask<StatusResponse>(
      CloseAccount.Instance
    ).Map(response => response.Response.Match<ActionResult>(
      success => Ok(),
      failure => BadRequest(failure)
    ));
}