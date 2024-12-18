using Anet.Db.Model.Akka;
using Microsoft.EntityFrameworkCore;

namespace Anet.Db.Context;

public partial class AkkaContext : DbContext
{
  public required DbSet<Journal> Journals { get; set; }
  public required DbSet<Snapshot> Snapshots { get; set; }
  public required DbSet<Metadata> Metadata { get; set; }
}