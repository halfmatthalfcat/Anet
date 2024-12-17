using Akka.Persistence.Fsm;

namespace Anet.Core.Akka.Actor.BankAccount;

public sealed class BankAccountActor : PersistentFSM<
  IBankAccountState, IBankAccountData, IBankAccountEvent>
{
  public override string PersistenceId => "BankAccount";
  private const string LockTimer = "LockTimer";

  public BankAccountActor()
  {
    StartWith(NewState.Instance, EmptyData.Instance);

    When(NewState.Instance, (evt, _) => evt.FsmEvent switch
    {
      OpenAccount _ => GoTo(OpenedState.Instance)
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
    });

    When(OpenedState.Instance, (evt, state) => 
    {
      if (state.StateData is BankAccountData data && data.CheckLock())
      {
        return GoTo(LockedState.Instance)
          .Applying(LockAccount.Instance)
          .AndThen(account =>
          {
            if (account is BankAccountData bankAccountData)
            {
              Sender.Tell(StatusResponse.Failure("Account is locked"), Self);
              SetTimer(LockTimer, UnlockAccount.Instance, TimeSpan.FromSeconds(5), false);
            }
          });
      }

      return (evt.FsmEvent, state.StateData) switch
      {
        (UnlockAccount, _) => GoTo(OpenedState.Instance)
          .Applying(UnlockAccount.Instance),
        (Deposit deposit, _) => Stay()
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
        (Withdraw withdraw, _) => Stay()
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
        (CloseAccount _, BankAccountData { Balance: >= 0 }) => GoTo(ClosedState.Instance)
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
        (CloseAccount, _) => Stay().Replying(StatusResponse.Failure("Account balance is negative")),
        (StatusRequest, BankAccountData bankAccountData) => Stay().Replying(StatusResponse.Success(
          new AccountDetail(bankAccountData.Balance, "Account status requested")
        )),
        _ => Stay(),
      };
    });

    When(ClosedState.Instance, (evt, _) => Stay().Replying(StatusResponse.Failure("Account is closed")));
  }

  protected override IBankAccountData ApplyEvent(IBankAccountEvent evt, IBankAccountData data)
  {
    return evt switch {
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
        },
        _ => data
      },
      _ => data
    };
  }
}