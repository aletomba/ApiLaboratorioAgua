using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanillaDiariaEnsayoJarras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseEntities_Muestras_FisicoQuimico_MuestraId",
                table: "BaseEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseEntities_Muestras_MuestraId",
                table: "BaseEntities");

            migrationBuilder.AddColumn<int>(
                name: "PuntoMuestreo",
                table: "Muestras",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SitioExtraccion",
                table: "BaseEntities",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Procedencia",
                table: "BaseEntities",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Cloro",
                table: "BaseEntities",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlanillasDiarias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Operador = table.Column<string>(type: "TEXT", nullable: true),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true),
                    LibroEntradaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanillasDiarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanillasDiarias_BaseEntities_LibroEntradaId",
                        column: x => x.LibroEntradaId,
                        principalTable: "BaseEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EnsayosJarras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Dosis1 = table.Column<double>(type: "REAL", nullable: true),
                    Dosis2 = table.Column<double>(type: "REAL", nullable: true),
                    Dosis3 = table.Column<double>(type: "REAL", nullable: true),
                    Dosis4 = table.Column<double>(type: "REAL", nullable: true),
                    Dosis5 = table.Column<double>(type: "REAL", nullable: true),
                    DosisSeleccionada = table.Column<double>(type: "REAL", nullable: true),
                    UnidadMedida = table.Column<string>(type: "TEXT", nullable: false),
                    PlanillaDiariaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnsayosJarras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnsayosJarras_PlanillasDiarias_PlanillaDiariaId",
                        column: x => x.PlanillaDiariaId,
                        principalTable: "PlanillasDiarias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnsayosJarras_PlanillaDiariaId",
                table: "EnsayosJarras",
                column: "PlanillaDiariaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanillasDiarias_Fecha",
                table: "PlanillasDiarias",
                column: "Fecha",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanillasDiarias_LibroEntradaId",
                table: "PlanillasDiarias",
                column: "LibroEntradaId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseEntities_Muestras_FisicoQuimico_MuestraId",
                table: "BaseEntities",
                column: "FisicoQuimico_MuestraId",
                principalTable: "Muestras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseEntities_Muestras_MuestraId",
                table: "BaseEntities",
                column: "MuestraId",
                principalTable: "Muestras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseEntities_Muestras_FisicoQuimico_MuestraId",
                table: "BaseEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseEntities_Muestras_MuestraId",
                table: "BaseEntities");

            migrationBuilder.DropTable(
                name: "EnsayosJarras");

            migrationBuilder.DropTable(
                name: "PlanillasDiarias");

            migrationBuilder.DropColumn(
                name: "PuntoMuestreo",
                table: "Muestras");

            migrationBuilder.DropColumn(
                name: "Cloro",
                table: "BaseEntities");

            migrationBuilder.AlterColumn<string>(
                name: "SitioExtraccion",
                table: "BaseEntities",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Procedencia",
                table: "BaseEntities",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseEntities_Muestras_FisicoQuimico_MuestraId",
                table: "BaseEntities",
                column: "FisicoQuimico_MuestraId",
                principalTable: "Muestras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseEntities_Muestras_MuestraId",
                table: "BaseEntities",
                column: "MuestraId",
                principalTable: "Muestras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
