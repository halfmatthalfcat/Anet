using Akka.Cluster.Sharding;
using Anet.Core.Akka.Actor.BankAccount;

namespace Anet.Core.Akka.Actor.Bank;

public sealed record BankAccountShardEnvelope(int EntityId, IBankAccountEvent Message);

public sealed class BankAccountMessageExtractor : HashCodeMessageExtractor
{
  public BankAccountMessageExtractor() : base(maxNumberOfShards: 30) { }

  public override string? EntityId(object message) =>
      message switch
      {
        BankAccountShardEnvelope envelope => envelope.EntityId.ToString(),
        _ => null,
      };
}