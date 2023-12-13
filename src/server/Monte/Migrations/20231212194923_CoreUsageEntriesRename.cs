using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monte.Migrations
{
    /// <inheritdoc />
    public partial class CoreUsageEntriesRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoreUsageEntry_MetricsEntries_EntryId",
                table: "CoreUsageEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoreUsageEntry",
                table: "CoreUsageEntry");

            migrationBuilder.RenameTable(
                name: "CoreUsageEntry",
                newName: "CoreUsageEntries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoreUsageEntries",
                table: "CoreUsageEntries",
                columns: new[] { "EntryId", "Ordinal" });

            migrationBuilder.AddForeignKey(
                name: "FK_CoreUsageEntries_MetricsEntries_EntryId",
                table: "CoreUsageEntries",
                column: "EntryId",
                principalTable: "MetricsEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoreUsageEntries_MetricsEntries_EntryId",
                table: "CoreUsageEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoreUsageEntries",
                table: "CoreUsageEntries");

            migrationBuilder.RenameTable(
                name: "CoreUsageEntries",
                newName: "CoreUsageEntry");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoreUsageEntry",
                table: "CoreUsageEntry",
                columns: new[] { "EntryId", "Ordinal" });

            migrationBuilder.AddForeignKey(
                name: "FK_CoreUsageEntry_MetricsEntries_EntryId",
                table: "CoreUsageEntry",
                column: "EntryId",
                principalTable: "MetricsEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
