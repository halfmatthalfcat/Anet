using Akka.Hosting;

namespace Anet.Core.Akka.Actor;

public static class Actors
{
    public static AkkaConfigurationBuilder WithAnetActors(this AkkaConfigurationBuilder builder)
    {
        return builder.WithActors((system, registry, resolver) =>
        {
            var pingProps = resolver.Props<PingActor>();
            var pingActor = system.ActorOf(pingProps, "PingActor");
            registry.Register<PingActor>(pingActor);

            var lockProps = resolver.Props<LockingActor>();
            var lockActor = system.ActorOf(lockProps, "LockingActor");
            registry.Register<LockingActor>(lockActor);

            var bankAccountProps = resolver.Props<BankAccountActor>();
            var bankAccountActor = system.ActorOf(bankAccountProps, "BankAccountActor");
            registry.Register<BankAccountActor>(bankAccountActor);
        })
    }
}