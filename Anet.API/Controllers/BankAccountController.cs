using Akka.Actor;
using Akka.Hosting;

using Microsoft.AspNetCore.Mvc;

using Anet.Core.Akka.Actor.BankAccount;
using Anet.Core.Akka.Util;

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
  public Task<ObjectResult> Status() =>
    bankAccountActor.ActorRef.Ask<StatusResponse>(
      StatusRequest.Instance
    ).Map(response => response.Response switch {
        IResult<AccountDetail>.Success success => Ok(GetSuccessMessage(success.Value)),
        IResult<AccountDetail>.Failure failure => BadRequest(failure.Message),
        _ => Problem()
    });

  [HttpGet]
  [Route("open")]
  [Tags("public")]
  public Task<ObjectResult> Open() =>
    bankAccountActor.ActorRef.Ask<StatusResponse>(
      OpenAccount.Instance
    ).Map(response => response.Response switch {
        IResult<AccountDetail>.Success success => Ok(GetSuccessMessage(success.Value)),
        IResult<AccountDetail>.Failure failure => BadRequest(failure.Message),
        _ => Problem()
    });

  [HttpGet]
  [Route("deposit")]
  [Tags("public")]
  public Task<ObjectResult> Deposit([FromQuery] decimal amount) =>
    bankAccountActor.ActorRef.Ask<StatusResponse>(
      new Deposit(amount)
    ).Map(response => response.Response switch {
        IResult<AccountDetail>.Success success => Ok(GetSuccessMessage(success.Value)),
        IResult<AccountDetail>.Failure failure => BadRequest(failure.Message),
        _ => Problem()
    });

  [HttpGet]
  [Route("withdraw")]
  [Tags("public")]
  public Task<ObjectResult> Withdraw([FromQuery] decimal amount) =>
    bankAccountActor.ActorRef.Ask<StatusResponse>(
      new Withdraw(amount)
    ).Map(response => response.Response switch {
        IResult<AccountDetail>.Success success => Ok(GetSuccessMessage(success.Value)),
        IResult<AccountDetail>.Failure failure => BadRequest(failure.Message),
        _ => Problem()
    });

  [HttpGet]
  [Route("close")]
  [Tags("public")]
  public Task<ObjectResult> Close() =>
    bankAccountActor.ActorRef.Ask<StatusResponse>(
      CloseAccount.Instance
    ).Map(response => response.Response switch {
        IResult<AccountDetail>.Success success => Ok(GetSuccessMessage(success.Value)),
        IResult<AccountDetail>.Failure failure => BadRequest(failure.Message),
        _ => Problem()
    });

  private static string GetSuccessMessage(AccountDetail detail) =>
    $"Balance: {detail.Balance}, Message: {detail.Message}";
}