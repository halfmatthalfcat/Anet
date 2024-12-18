using System.ComponentModel.DataAnnotations;
using Akka.Configuration;

namespace Anet.API;

public class AppConfiguration
{
  [ConfigurationKeyName("CONNECTION_STRING")]
  [Required]
  public string ConnectionString { get; set; } = null!;

  public AppConfiguration()
  {
    new ConfigurationBuilder()
      .AddEnvironmentVariables()
      .Build()
      .Bind(this);

    Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
  }
}