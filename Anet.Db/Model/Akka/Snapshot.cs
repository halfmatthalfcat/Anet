using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Anet.Db.Model.Akka;

[Table("snapshot_store", Schema = "akka")]
[PrimaryKey(nameof(PersistenceId), nameof(SequenceNr))]
public sealed class Snapshot
{
  [Column("persistence_id")]
  [Required]
  public required string PersistenceId { get; set; }

  [Column("sequence_nr")]
  [Required]
  public required long SequenceNr { get; set; }

  [Column("created_at")]
  [Required]
  public required long CreatedAt { get; set; }

  [Column("manifest")]
  [Required]
  public required string Manifest { get; set; }

  [Column("payload")]
  [Required]
  public required byte[] Payload { get; set; }

  [Column("serializer_id")]
  public int? SerializerId { get; set; }
}