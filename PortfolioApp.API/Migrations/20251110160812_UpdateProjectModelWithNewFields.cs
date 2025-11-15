using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioApp.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectModelWithNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GithubUrl",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LiveUrl",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Technologies",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GithubUrl",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LiveUrl",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Technologies",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Projects");
        }
    }
}
