using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Anet.Db.Model.Akka;

[Table("event_journal", Schema = "akka")]
[Index(nameof(PersistenceId), nameof(SequenceNr), IsUnique = true)]
public sealed class Journal
{
  [Key, Column("ordering")]
  public long Ordering { get; set; }

  [Column("persistence_id")]
  [Required]
  public required string PersistenceId { get; set; }

  [Column("sequence_nr")]
  [Required]
  public required long SequenceNr { get; set; }

  [Column("is_deleted")]
  [Required]
  public required bool IsDeleted { get; set; }

  [Column("created_at")]
  [Required]
  public required long CreatedAt { get; set; }

  [Column("manifest")]
  [Required]
  public required string Manifest { get; set; }  

  [Column("payload")]
  [Required]
  public required byte[] Payload { get; set; }

  [Column("tags")]
  public string? Tags { get; set; }

  [Column("serializer_id")]
  public int? SerializerId { get; set; }
}