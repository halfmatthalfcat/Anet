using _Actor = Akka.Actor;
using Akka.Persistence.Fsm;
using Anet.Core.Akka.Actor.BankAccount;

namespace Anet.Core.Akka.Actor.Bank;

public sealed class BankAccountShardActor : PersistentFSM<
  IBankAccountState, IBankAccountData, IBankAccountEvent>
{
  public override string PersistenceId { get; }
  private const string LockTimer = "LockTimer";

  public static _Actor.Props Props(string persistenceId) =>
   _Actor.Props.Create(() => new BankAccountActor(persistenceId));

  public static BankAccountShardEnvelope Envelope(int entityId, IBankAccountEvent message) =>
      new(entityId, message);

  public BankAccountShardActor(string persistenceId)
  {
    PersistenceId = persistenceId;

    StartWith(NewState.Instance, EmptyData.Instance);

    When(NewState.Instance, (evt, _) =>
    {
      return evt.FsmEvent switch
      {
        BankAccountShardEnvelope { Message: OpenAccount } _ => GoTo(OpenedState.Instance)
        .Applying(OpenAccount.Instance)
        .AndThen(account =>
        {
          if (account is BankAccountData bankAccountData)
          {
            Sender.Tell(StatusResponse.Success(new AccountDetail(
                bankAccountData.Balance, "Account opened"
            )), Self);
          }
        }),
        _ => Stay().Replying(StatusResponse.Failure("Account is not open")),
      };
    });

    When(OpenedState.Instance, (evt, _) =>
    {
      return (evt.FsmEvent, evt.StateData) switch
      {
        (_, BankAccountData { IsLocked: true }) => Stay().Replying(StatusResponse.Failure("Account is locked")),
        (_, BankAccountData bankAccountData) when bankAccountData.CheckLock() => GoTo(LockedState.Instance)
          .Applying(LockAccount.Instance)
          .AndThen(account =>
          {
            if (account is BankAccountData bankAccountData)
            {
              Sender.Tell(StatusResponse.Failure("Account is locked"), Self);
              SetTimer(LockTimer, UnlockAccount.Instance, TimeSpan.FromSeconds(5), false);
            }
          }),
        (BankAccountShardEnvelope { Message: UnlockAccount }, _) => GoTo(OpenedState.Instance)
          .Applying(UnlockAccount.Instance),
        (BankAccountShardEnvelope { Message: Deposit deposit }, _) => Stay()
          .Applying(deposit)
          .AndThen(account =>
          {
            if (account is BankAccountData bankAccountData)
            {
              Sender.Tell(StatusResponse.Success(new AccountDetail(
                  bankAccountData.Balance, "Deposit successful"
              )), Self);
            }
          }),
        (BankAccountShardEnvelope { Message: Withdraw withdraw }, _) => Stay()
          .Applying(withdraw)
          .AndThen(account =>
          {
            if (account is BankAccountData bankAccountData)
            {
              Sender.Tell(StatusResponse.Success(new AccountDetail(
                  bankAccountData.Balance, "Withdrawal successful"
              )), Self);
            }
          }),
        (BankAccountShardEnvelope { Message: CloseAccount } _, BankAccountData { Balance: >= 0 }) => GoTo(ClosedState.Instance)
          .Applying(CloseAccount.Instance)
          .AndThen(account =>
          {
            if (account is BankAccountData bankAccountData)
            {
              Sender.Tell(StatusResponse.Success(new AccountDetail(
                  bankAccountData.Balance, "Account closed"
              )), Self);
            }
          }),
        (BankAccountShardEnvelope { Message: OpenAccount }, _) => Stay().Replying(StatusResponse.Failure("Account is already open")),
        (BankAccountShardEnvelope { Message: CloseAccount }, _) => Stay().Replying(StatusResponse.Failure("Account balance is negative")),
        (BankAccountShardEnvelope { Message: StatusRequest }, BankAccountData bankAccountData) => Stay().Replying(StatusResponse.Success(
          new AccountDetail(bankAccountData.Balance, "Account status requested")
        )),
        _ => Stay().Replying(StatusResponse.Failure("Unknown command")),
      };
    });

    When(LockedState.Instance, (evt, _) => Stay().Replying(StatusResponse.Failure("Account is locked")));

    When(ClosedState.Instance, (evt, _) => Stay().Replying(StatusResponse.Failure("Account is closed")));
  }

  protected override IBankAccountData ApplyEvent(IBankAccountEvent evt, IBankAccountData data)
  {
    var newState = evt switch
    {
      OpenAccount => BankAccountData.Initial,
      CloseAccount => BankAccountData.Initial,
      Deposit deposit => data switch
      {
        BankAccountData bankAccountData => bankAccountData with
        {
          Balance = bankAccountData.Balance + deposit.Amount,
          LastTransactionAt = DateTime.UtcNow,
          TransactionWindow = bankAccountData.TransactionWindow.Add(DateTime.UtcNow)
        },
        _ => data
      },
      Withdraw withdraw => data switch
      {
        BankAccountData bankAccountData => bankAccountData with
        {
          Balance = bankAccountData.Balance - withdraw.Amount,
          LastTransactionAt = DateTime.UtcNow,
          TransactionWindow = bankAccountData.TransactionWindow.Add(DateTime.UtcNow)
        },
        _ => data
      },
      LockAccount _ => data switch
      {
        BankAccountData bankAccountData => bankAccountData with
        {
          IsLocked = true,
        },
        _ => data
      },
      UnlockAccount _ => data switch
      {
        BankAccountData bankAccountData => bankAccountData with
        {
          IsLocked = false,
          TransactionWindow = Seq<DateTime>()
        },
        _ => data
      },
      _ => data
    };

    return newState;
  }
}