using System.ComponentModel.DataAnnotations;
using Akka.Configuration;

namespace Anet.API;

public class AppConfiguration
{
  [ConfigurationKeyName("CONNECTION_STRING")]
  [Required]
  public string ConnectionString { get; set; } = null!;

  [ConfigurationKeyName("REMOTE_PORT")]
  [Required]
  public int RemotePort { get; set; }

  [ConfigurationKeyName("REMOTE_HOST")]
  [Required]
  public string RemoteHost { get; set; } = null!;

  [ConfigurationKeyName("PEERS")]
  public List<string> Peers { get; set; } = [];

  public AppConfiguration()
  {
    new ConfigurationBuilder()
      .AddEnvironmentVariables()
      .Build()
      .Bind(this);

    Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
  }
}