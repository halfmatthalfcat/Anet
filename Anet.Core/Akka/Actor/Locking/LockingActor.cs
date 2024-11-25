using Akka.Actor;

namespace Anet.Core.Akka.Actor.Locking;

public enum State
{
  Locked,
  Unlocked
}

public interface IData { }
public sealed record LockedData(DateTime LockedAt) : IData;
public sealed record UnlockedData(Option<DateTime> LastLockedAt) : IData;

public class LockingActor : FSM<State, IData>
{
  private const string LockTimerName = "LockTimer";

  public sealed record LockRequest();
  public sealed record LockResponse(bool DidLock, bool IsLocked, DateTime LockedAt);
  public sealed record StatusRequest();
  public sealed record StatusResponse(bool IsLocked, Option<DateTime> LastLockedAt);
  private sealed record Unlock();

  public LockingActor()
  {
    StartWith(State.Unlocked, new UnlockedData(None));

    When(State.Unlocked, @event =>
    {
      switch ((@event.FsmEvent, @event.StateData))
      {
        case (LockRequest, _):
          var now = DateTime.UtcNow;

          SetTimer(LockTimerName, new Unlock(), TimeSpan.FromMinutes(1), false);
          return GoTo(State.Locked)
            .Using(new LockedData(now))
            .Replying(new LockResponse(true, true, now));

        case (StatusRequest, UnlockedData(Option<DateTime> lastLockedAt)):
          return Stay().Replying(new StatusResponse(false, lastLockedAt));

        default:
          return null;
      }
    });

    When(State.Locked, @event =>
    {
      switch ((@event.FsmEvent, @event.StateData))
      {
        case (Unlock, LockedData(DateTime LockedAt)):
          return GoTo(State.Unlocked).Using(new UnlockedData(Some(LockedAt)));

        case (StatusRequest, LockedData(DateTime LockedAt)):
          return Stay().Replying(new StatusResponse(true, LockedAt));

        case (LockRequest, LockedData(DateTime LockedAt)):
          return Stay().Replying(new LockResponse(false, true, LockedAt));

        default:
          return null;
      }
    });

    Initialize();
  }
}