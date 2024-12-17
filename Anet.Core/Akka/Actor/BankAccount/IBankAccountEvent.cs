namespace Anet.Core.Akka.Actor.BankAccount;

public interface IBankAccountEvent { }

// Public Events

public sealed class OpenAccount : IBankAccountEvent
{
  public static OpenAccount Instance { get; } = new();
  private OpenAccount() { }
}

public sealed record Deposit(decimal Amount) : IBankAccountEvent;
public sealed record Withdraw(decimal Amount) : IBankAccountEvent;

public sealed class CloseAccount : IBankAccountEvent
{
  public static CloseAccount Instance { get; } = new();
  private CloseAccount() { }
}

public sealed class StatusRequest : IBankAccountEvent
{
  public static StatusRequest Instance { get; } = new();
  private StatusRequest() { }
}

public sealed record AccountDetail(decimal Balance, string Message);
public sealed record StatusResponse(Either<string, AccountDetail> Response) : IBankAccountEvent
{
  public static StatusResponse Success(AccountDetail balance) => new(Either<string, AccountDetail>.Right(balance));
  public static StatusResponse Failure(string message) => new(Either<string, AccountDetail>.Left(message));
}

// Internal Events

internal sealed class LockAccount : IBankAccountEvent
{
  public static LockAccount Instance { get; } = new();
  private LockAccount() { }
}

internal sealed class UnlockAccount : IBankAccountEvent
{
  public static UnlockAccount Instance { get; } = new();
  private UnlockAccount() { }
}