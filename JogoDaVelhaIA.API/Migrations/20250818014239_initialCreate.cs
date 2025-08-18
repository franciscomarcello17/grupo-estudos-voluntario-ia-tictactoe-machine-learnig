using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JogoDaVelhaIA.API.Migrations
{
    public partial class initialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadosTabuleiro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Estado = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: false),
                    Vitoria = table.Column<bool>(type: "bit", nullable: false),
                    Derrota = table.Column<bool>(type: "bit", nullable: false),
                    Empate = table.Column<bool>(type: "bit", nullable: false),
                    ValorQ = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosTabuleiro", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Partidas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Resultado = table.Column<int>(type: "int", nullable: false),
                    JogadorVencedor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoPartidas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartidaId = table.Column<int>(type: "int", nullable: false),
                    EstadoTabuleiroId = table.Column<int>(type: "int", nullable: false),
                    NumeroJogada = table.Column<int>(type: "int", nullable: false),
                    PosicaoJogada = table.Column<int>(type: "int", nullable: false),
                    SimboloJogador = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoPartidas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoPartidas_EstadosTabuleiro_EstadoTabuleiroId",
                        column: x => x.EstadoTabuleiroId,
                        principalTable: "EstadosTabuleiro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoricoPartidas_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstadosTabuleiro_Estado",
                table: "EstadosTabuleiro",
                column: "Estado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoPartidas_EstadoTabuleiroId",
                table: "HistoricoPartidas",
                column: "EstadoTabuleiroId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoPartidas_PartidaId",
                table: "HistoricoPartidas",
                column: "PartidaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricoPartidas");

            migrationBuilder.DropTable(
                name: "EstadosTabuleiro");

            migrationBuilder.DropTable(
                name: "Partidas");
        }
    }
}
