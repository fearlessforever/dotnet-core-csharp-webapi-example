using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace My.Migrations
{
    /// <inheritdoc />
    public partial class Ketiga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects2",
                columns: table => new { ProjectId = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true) },
                constraints: table => { table.PrimaryKey("PK_Projects", x => x.ProjectId); }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable( name: "Projects2");
        }
    }
}
