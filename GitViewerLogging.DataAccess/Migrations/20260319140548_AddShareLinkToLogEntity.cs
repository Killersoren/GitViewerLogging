using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitViewerLogging.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddShareLinkToLogEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ShareLink",
                table: "LogEntities",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShareLink",
                table: "LogEntities");
        }
    }
}
