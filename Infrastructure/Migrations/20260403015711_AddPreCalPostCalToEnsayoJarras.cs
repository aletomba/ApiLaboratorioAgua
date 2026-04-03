using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AddPreCalPostCalToEnsayoJarras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PostCal",
                table: "EnsayosJarras",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PreCal",
                table: "EnsayosJarras",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostCal",
                table: "EnsayosJarras");

            migrationBuilder.DropColumn(
                name: "PreCal",
                table: "EnsayosJarras");
        }
    }
}
