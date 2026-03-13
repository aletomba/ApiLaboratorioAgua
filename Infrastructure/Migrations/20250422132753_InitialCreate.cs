using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FechaLLegada = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaAnalisis = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Procedencia = table.Column<string>(type: "TEXT", nullable: false),
                    SitioExtraccion = table.Column<string>(type: "TEXT", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    ColiformesNmp = table.Column<string>(type: "TEXT", nullable: true),
                    ColiformesFecalesNmp = table.Column<string>(type: "TEXT", nullable: true),
                    ColoniasAgar = table.Column<string>(type: "TEXT", nullable: true),
                    ColiFecalesUfc = table.Column<string>(type: "TEXT", nullable: true),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true),
                    MuestraId = table.Column<int>(type: "INTEGER", nullable: true),
                    FisicoQuimico_MuestraId = table.Column<int>(type: "INTEGER", nullable: true),
                    Ph = table.Column<string>(type: "TEXT", nullable: true),
                    Turbidez = table.Column<string>(type: "TEXT", nullable: true),
                    Alcalinidad = table.Column<string>(type: "TEXT", nullable: true),
                    Dureza = table.Column<string>(type: "TEXT", nullable: true),
                    Nitritos = table.Column<string>(type: "TEXT", nullable: true),
                    Cloruros = table.Column<string>(type: "TEXT", nullable: true),
                    Calcio = table.Column<string>(type: "TEXT", nullable: true),
                    Magnesio = table.Column<string>(type: "TEXT", nullable: true),
                    Dbo5 = table.Column<string>(type: "TEXT", nullable: true),
                    LibroDeEntrada_Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Muestras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Procedencia = table.Column<string>(type: "TEXT", nullable: true),
                    NombreMuestreador = table.Column<string>(type: "TEXT", nullable: true),
                    Latitud = table.Column<double>(type: "REAL", nullable: false),
                    Longitud = table.Column<double>(type: "REAL", nullable: false),
                    FechaExtraccion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HoraExtraccion = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    TipoMuestra = table.Column<int>(type: "INTEGER", nullable: false),
                    LibroEntradaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Muestras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Muestras_BaseEntities_LibroEntradaId",
                        column: x => x.LibroEntradaId,
                        principalTable: "BaseEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Muestras_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseEntities_FisicoQuimico_MuestraId",
                table: "BaseEntities",
                column: "FisicoQuimico_MuestraId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BaseEntities_MuestraId",
                table: "BaseEntities",
                column: "MuestraId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Muestras_ClienteId",
                table: "Muestras",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Muestras_LibroEntradaId",
                table: "Muestras",
                column: "LibroEntradaId");

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
                name: "Muestras");

            migrationBuilder.DropTable(
                name: "BaseEntities");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
