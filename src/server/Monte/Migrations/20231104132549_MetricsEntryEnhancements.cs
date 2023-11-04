using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monte.Migrations
{
    /// <inheritdoc />
    public partial class MetricsEntryEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Cpu_AveragePercentUsed",
                table: "MetricsEntries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Cpu_Load",
                table: "MetricsEntries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Memory_Available",
                table: "MetricsEntries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Memory_PercentUsed",
                table: "MetricsEntries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Memory_SwapAvailable",
                table: "MetricsEntries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Memory_SwapPercentUsed",
                table: "MetricsEntries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "CoreUsageEntry",
                columns: table => new
                {
                    Ordinal = table.Column<int>(type: "integer", nullable: false),
                    EntryId = table.Column<long>(type: "bigint", nullable: false),
                    PercentUsed = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoreUsageEntry", x => new { x.EntryId, x.Ordinal });
                    table.ForeignKey(
                        name: "FK_CoreUsageEntry_MetricsEntries_EntryId",
                        column: x => x.EntryId,
                        principalTable: "MetricsEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoreUsageEntry");

            migrationBuilder.DropColumn(
                name: "Cpu_AveragePercentUsed",
                table: "MetricsEntries");

            migrationBuilder.DropColumn(
                name: "Cpu_Load",
                table: "MetricsEntries");

            migrationBuilder.DropColumn(
                name: "Memory_Available",
                table: "MetricsEntries");

            migrationBuilder.DropColumn(
                name: "Memory_PercentUsed",
                table: "MetricsEntries");

            migrationBuilder.DropColumn(
                name: "Memory_SwapAvailable",
                table: "MetricsEntries");

            migrationBuilder.DropColumn(
                name: "Memory_SwapPercentUsed",
                table: "MetricsEntries");
        }
    }
}
