using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JogoDaVelhaIA.API.Migrations
{
    public partial class CriarTabelaJogadasBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JogadasAprendidas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstadoTabuleiro = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PosicaoEscolhida = table.Column<int>(type: "int", nullable: false),
                    Resultado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JogadasAprendidas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JogadasBase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstadoTabuleiro = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PosicaoEscolhida = table.Column<int>(type: "int", nullable: false),
                    Peso = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JogadasBase", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JogadasAprendidas_EstadoTabuleiro",
                table: "JogadasAprendidas",
                column: "EstadoTabuleiro");

            migrationBuilder.CreateIndex(
                name: "IX_JogadasBase_EstadoTabuleiro",
                table: "JogadasBase",
                column: "EstadoTabuleiro");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JogadasAprendidas");

            migrationBuilder.DropTable(
                name: "JogadasBase");
        }
    }
}
