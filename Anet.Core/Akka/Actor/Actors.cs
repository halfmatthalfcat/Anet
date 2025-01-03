using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Cluster.Sharding;
using Akka.Hosting;
using Anet.Core.Akka.Actor.Bank;
using Anet.Core.Akka.Actor.BankAccount;
using Anet.Core.Akka.Actor.Locking;

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

            var bankAccountActor = system.ActorOf(BankAccountActor.Props("BankAccount"), "BankAccountActor");
            registry.Register<BankAccountActor>(bankAccountActor);
        }).WithShardRegion<BankAccountShardActor>("BankAccount",
            s => Props.Create(() => new BankAccountShardActor($"BankAccount-{s}")), 
            new BankAccountMessageExtractor(),
            new ShardOptions() {
                RememberEntities = true,
                StateStoreMode = StateStoreMode.DData,
                PassivateIdleEntityAfter = TimeSpan.FromHours(1),
            }
        );
    }
}