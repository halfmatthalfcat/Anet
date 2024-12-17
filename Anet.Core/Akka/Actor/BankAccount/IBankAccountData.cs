namespace Anet.Core.Akka.Actor.BankAccount;

public interface IBankAccountData { }

public sealed class EmptyData : IBankAccountData
{
  public static EmptyData Instance { get; } = new();
  private EmptyData() { }
}

public sealed record BankAccountData(
  decimal Balance,
  DateTime LastTransactionAt,
  bool IsLocked,
  Seq<DateTime> TransactionWindow
) : IBankAccountData
{
  public static BankAccountData Initial => new(
    0, DateTime.UtcNow, false, Seq<DateTime>());

  public bool CheckLock()
  {
    var now = DateTime.UtcNow;

    return TransactionWindow
      .Add(DateTime.UtcNow)
      .Filter(tx => tx > now.AddSeconds(-5))
      .Count() > 5;
  }
}