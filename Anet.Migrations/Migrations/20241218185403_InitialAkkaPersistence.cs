using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Anet.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialAkkaPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "akka");

            migrationBuilder.CreateTable(
                name: "event_journal",
                schema: "akka",
                columns: table => new
                {
                    ordering = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    persistence_id = table.Column<string>(type: "text", nullable: false),
                    sequence_nr = table.Column<long>(type: "bigint", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    manifest = table.Column<string>(type: "text", nullable: false),
                    payload = table.Column<byte[]>(type: "bytea", nullable: false),
                    tags = table.Column<string>(type: "text", nullable: true),
                    serializer_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_journal", x => x.ordering);
                });

            migrationBuilder.CreateTable(
                name: "metadata",
                schema: "akka",
                columns: table => new
                {
                    persistence_id = table.Column<string>(type: "text", nullable: false),
                    sequence_nr = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadata", x => new { x.persistence_id, x.sequence_nr });
                });

            migrationBuilder.CreateTable(
                name: "snapshot_store",
                schema: "akka",
                columns: table => new
                {
                    persistence_id = table.Column<string>(type: "text", nullable: false),
                    sequence_nr = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    manifest = table.Column<string>(type: "text", nullable: false),
                    payload = table.Column<byte[]>(type: "bytea", nullable: false),
                    serializer_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_snapshot_store", x => new { x.persistence_id, x.sequence_nr });
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_journal_persistence_id_sequence_nr",
                schema: "akka",
                table: "event_journal",
                columns: new[] { "persistence_id", "sequence_nr" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_journal",
                schema: "akka");

            migrationBuilder.DropTable(
                name: "metadata",
                schema: "akka");

            migrationBuilder.DropTable(
                name: "snapshot_store",
                schema: "akka");
        }
    }
}
