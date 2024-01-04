using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monte.Migrations
{
    /// <inheritdoc />
    public partial class AgentRenameWithMetricsKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetricsEntries_Machines_MachineId",
                table: "MetricsEntries");

            migrationBuilder.RenameColumn(
                name: "MachineId",
                table: "MetricsEntries",
                newName: "AgentId");

            migrationBuilder.RenameIndex(
                name: "IX_MetricsEntries_MachineId",
                table: "MetricsEntries",
                newName: "IX_MetricsEntries_AgentId");

            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrdinalNumber = table.Column<int>(type: "integer", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HeartbeatDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MetricsKey = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Cpu_LogicalCount = table.Column<int>(type: "integer", nullable: false),
                    Cpu_PhysicalCount = table.Column<int>(type: "integer", nullable: false),
                    Cpu_MinFreq = table.Column<double>(type: "double precision", nullable: false),
                    Cpu_MaxFreq = table.Column<double>(type: "double precision", nullable: false),
                    Memory_Total = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Memory_Swap = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.Id);
                });

            migrationBuilder.Sql("""
                 INSERT INTO "Agents" (
                     "Id",
                     "Name",
                     "OrdinalNumber",
                     "CreatedDateTime",
                     "HeartbeatDateTime",
                     "MetricsKey",
                     "Cpu_LogicalCount",
                     "Cpu_PhysicalCount",
                     "Cpu_MinFreq",
                     "Cpu_MaxFreq",
                     "Memory_Total",
                     "Memory_Swap")
                 (SELECT
                     "Id",
                     "Name",
                     "OrdinalNumber",
                     "CreatedDateTime",
                     "HeartbeatDateTime",
                     '',
                     "Cpu_LogicalCount",
                     "Cpu_PhysicalCount",
                     "Cpu_MinFreq",
                     "Cpu_MaxFreq",
                     "Memory_Total",
                     "Memory_Swap"
                 FROM "Machines");
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_MetricsEntries_Agents_AgentId",
                table: "MetricsEntries",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            
            migrationBuilder.DropTable(
                name: "Machines");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetricsEntries_Agents_AgentId",
                table: "MetricsEntries");

            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.RenameColumn(
                name: "AgentId",
                table: "MetricsEntries",
                newName: "MachineId");

            migrationBuilder.RenameIndex(
                name: "IX_MetricsEntries_AgentId",
                table: "MetricsEntries",
                newName: "IX_MetricsEntries_MachineId");

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HeartbeatDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrdinalNumber = table.Column<int>(type: "integer", nullable: false),
                    Cpu_LogicalCount = table.Column<int>(type: "integer", nullable: false),
                    Cpu_MaxFreq = table.Column<double>(type: "double precision", nullable: false),
                    Cpu_MinFreq = table.Column<double>(type: "double precision", nullable: false),
                    Cpu_PhysicalCount = table.Column<int>(type: "integer", nullable: false),
                    Memory_Swap = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Memory_Total = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_MetricsEntries_Machines_MachineId",
                table: "MetricsEntries",
                column: "MachineId",
                principalTable: "Machines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
