using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cursa_API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMetasAddAtualizacaoAvaliacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnotacoesCurso");

            migrationBuilder.DropTable(
                name: "MetasEstudo");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAtualizacao",
                table: "AvaliacoesCurso",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataAtualizacao",
                table: "AvaliacoesCurso");

            migrationBuilder.CreateTable(
                name: "AnotacoesCurso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<int>(type: "int", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnotacoesCurso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnotacoesCurso_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnotacoesCurso_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MetasEstudo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<int>(type: "int", nullable: false),
                    Concluida = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetasEstudo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetasEstudo_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnotacoesCurso_AlunoId_CursoId",
                table: "AnotacoesCurso",
                columns: new[] { "AlunoId", "CursoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnotacoesCurso_CursoId",
                table: "AnotacoesCurso",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_MetasEstudo_AlunoId",
                table: "MetasEstudo",
                column: "AlunoId");
        }
    }
}
