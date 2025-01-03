using Akka.Actor;
using Akka.Hosting;

using Microsoft.AspNetCore.Mvc;

using Anet.Core.Akka.Actor.BankAccount;
using Anet.Core.Akka.Actor.Bank;
using Anet.Core.Akka.Util;

namespace Anet.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BankController(
  IRequiredActor<BankAccountShardActor> bankAccountShard
) : ControllerBase
{
  [HttpGet]
  [Route("{id:int}/status")]
  [Tags("public")]
  public Task<ObjectResult> Status(int id) =>
    bankAccountShard.ActorRef.Ask<StatusResponse>(
      BankAccountShardActor.Envelope(id, StatusRequest.Instance)
    ).Map(response => response.Response switch {
      IResult<AccountDetail>.Success success => Ok(GetSuccessMessage(success.Value)),
      IResult<AccountDetail>.Failure failure => BadRequest(failure.Message),
      _ => Problem()
    });

  [HttpGet]
  [Route("{id:int}/open")]
  [Tags("public")]
  public Task<ObjectResult> Open(int id) =>
    bankAccountShard.ActorRef.Ask<StatusResponse>(
        BankAccountShardActor.Envelope(id, OpenAccount.Instance)
    ).Map(response => response.Response switch {
      IResult<AccountDetail>.Success success => Ok(GetSuccessMessage(success.Value)),
      IResult<AccountDetail>.Failure failure => BadRequest(failure.Message),
      _ => Problem()
    });

  [HttpGet]
  [Route("{id:int}/deposit")]
  [Tags("public")]
  public Task<ObjectResult> Deposit(int id, [FromQuery] decimal amount) =>
    bankAccountShard.ActorRef.Ask<StatusResponse>(
      BankAccountShardActor.Envelope(id, new Deposit(amount))
    ).Map(response => response.Response switch {
      IResult<AccountDetail>.Success success => Ok(GetSuccessMessage(success.Value)),
      IResult<AccountDetail>.Failure failure => BadRequest(failure.Message),
      _ => Problem()
    });

  [HttpGet]
  [Route("{id:int}/withdraw")]
  [Tags("public")]
  public Task<ObjectResult> Withdraw(int id, [FromQuery] decimal amount) =>
    bankAccountShard.ActorRef.Ask<StatusResponse>(
        BankAccountShardActor.Envelope(id, new Withdraw(amount))
    ).Map(response => response.Response switch {
      IResult<AccountDetail>.Success success => Ok(GetSuccessMessage(success.Value)),
      IResult<AccountDetail>.Failure failure => BadRequest(failure.Message),
      _ => Problem()
    });

  [HttpGet]
  [Route("{id:int}/close")]
  [Tags("public")]
  public Task<ObjectResult> Close(int id) =>
    bankAccountShard.ActorRef.Ask<StatusResponse>(
        BankAccountShardActor.Envelope(id, CloseAccount.Instance)
    ).Map(response => response.Response switch {
      IResult<AccountDetail>.Success success => Ok(GetSuccessMessage(success.Value)),
      IResult<AccountDetail>.Failure failure => BadRequest(failure.Message),
      _ => Problem()
    });

  private static string GetSuccessMessage(AccountDetail detail) =>
    $"Balance: {detail.Balance}, Message: {detail.Message}";
}