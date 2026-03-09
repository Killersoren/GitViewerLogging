using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitViewerLogging.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedisAnonymousEntityTypeEntityNameToLogModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntityName",
                table: "LogEntities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "LogEntities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                table: "LogEntities",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityName",
                table: "LogEntities");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "LogEntities");

            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                table: "LogEntities");
        }
    }
}
