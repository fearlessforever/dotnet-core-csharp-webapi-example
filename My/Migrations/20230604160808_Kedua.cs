using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace My.Migrations
{
    /// <inheritdoc />
    public partial class Kedua : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.CreateTable(
            name: "Tickets2",
            columns: table => new
            {
                TicketId = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                ProjectId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tickets2", x => x.TicketId);
                table.ForeignKey(
                    name: "FK_Tickets2_Projects_ProjectId",
                    column: x => x.ProjectId,
                    principalTable: "Projects",
                    principalColumn: "ProjectId",
                    onDelete: ReferentialAction.Restrict);
            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.DropTable( name: "Tickets2");
        }
    }
}
