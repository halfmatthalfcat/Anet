using Anet.Db.Context;
using Microsoft.EntityFrameworkCore;

namespace Anet.Migrations;

public class AkkaMigrationContext : AkkaContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("");
}