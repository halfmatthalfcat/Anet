using Akka.Persistence.Fsm;

namespace Anet.Core.Akka.Actor.BankAccount;

public interface IBankAccountState : PersistentFSM.IFsmState { }

public sealed class NewState : IBankAccountState
{
  public static NewState Instance { get; } = new();
  private NewState() {}
  public string Identifier => "New";
}

public sealed class OpenedState : IBankAccountState
{
  public static OpenedState Instance { get; } = new();
  private OpenedState() {}
  public string Identifier => "Opened";
}

public sealed class ClosedState : IBankAccountState
{
  public static ClosedState Instance { get; } = new();
  private ClosedState() {}
  public string Identifier => "Closed";
}

public class LockedState : IBankAccountState
{
  public static LockedState Instance { get; } = new();
  private LockedState() {}
  public string Identifier => "Locked";
}