using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monte.Migrations
{
    /// <inheritdoc />
    public partial class AgentDisplayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Agents",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE "Agents" dst
                SET "DisplayName" = src."Name" || ' #' || src."OrdinalNumber"
                FROM "Agents" AS src
                WHERE dst."Id" = src."Id"
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Agents");
        }
    }
}
