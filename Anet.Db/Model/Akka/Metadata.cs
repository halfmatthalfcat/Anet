using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Anet.Db.Model.Akka;

[Table("metadata", Schema = "akka")]
[PrimaryKey(nameof(PersistenceId), nameof(SequenceNr))]
public sealed class Metadata
{
  [Column("persistence_id")]
  [Required]
  public required string PersistenceId { get; set; }

  [Column("sequence_nr")]
  [Required]
  public required long SequenceNr { get; set; }
}