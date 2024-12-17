using System.ComponentModel.DataAnnotations;

namespace Anet.API;

public class AppConfiguration
{
  [Required]
  public string ConnectionString { get; set; }
}