using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cursa_API.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PermissoesSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChaveFuncionalidade = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    NomeExibicao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermitidoAdmin = table.Column<bool>(type: "bit", nullable: false),
                    PermitidoSecretaria = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissoesSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TermosGlossario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Chave = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Termo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Explicacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tecnica = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermosGlossario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Perfil = table.Column<int>(type: "int", nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Tema = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Acessibilidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NivelAutoridade = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    UltimoAcessoGerencial = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Administradores_Usuarios_Id",
                        column: x => x.Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alunos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Xp = table.Column<int>(type: "int", nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alunos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alunos_Usuarios_Id",
                        column: x => x.Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Assunto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Resposta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RespondidoPorId = table.Column<int>(type: "int", nullable: true),
                    DataResposta = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Usuarios_RespondidoPorId",
                        column: x => x.RespondidoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParametrosSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NomeInstituicao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PeriodoLetivoAtual = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MediaAprovacaoMinima = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    CargaHorariaMaximaAluno = table.Column<int>(type: "int", nullable: false),
                    VagasPadraoPorCurso = table.Column<int>(type: "int", nullable: false),
                    PermitirNovasMatriculas = table.Column<bool>(type: "bit", nullable: false),
                    EmailInstitucional = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TelefoneInstitucional = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EnderecoCampus = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UltimaAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoPorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrosSistema", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParametrosSistema_Usuarios_AtualizadoPorId",
                        column: x => x.AtualizadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Professores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Titulacao = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Professores_Usuarios_Id",
                        column: x => x.Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Secretarias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SetorAtendimento = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secretarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Secretarias_Usuarios_Id",
                        column: x => x.Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Atendimentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Protocolo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoSolicitante = table.Column<int>(type: "int", nullable: false),
                    SolicitanteNome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Contato = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AlunoId = table.Column<int>(type: "int", nullable: true),
                    Assunto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Providencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFechamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AtendenteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atendimentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Atendimentos_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Atendimentos_Usuarios_AtendenteId",
                        column: x => x.AtendenteId,
                        principalTable: "Usuarios",
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
                    Texto = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Concluida = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Area = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CargaHoraria = table.Column<int>(type: "int", nullable: false),
                    NivelMinimo = table.Column<int>(type: "int", nullable: false),
                    ImagemUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProfessorId = table.Column<int>(type: "int", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    VagasTotais = table.Column<int>(type: "int", nullable: true),
                    Avaliacao = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cursos_Professores_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnotacoesCurso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<int>(type: "int", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "AvaliacoesCurso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    AlunoId = table.Column<int>(type: "int", nullable: false),
                    Nota = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvaliacoesCurso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvaliacoesCurso_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AvaliacoesCurso_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Disciplinas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Conteudo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ordem = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplinas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Disciplinas_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matriculas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<int>(type: "int", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    DataMatricula = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PeriodoLetivo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NotaFinal = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: true),
                    ProgressoPercentual = table.Column<int>(type: "int", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaterialAcessado = table.Column<bool>(type: "bit", nullable: false),
                    EstudoConcluido = table.Column<bool>(type: "bit", nullable: false),
                    ExerciciosLiberados = table.Column<bool>(type: "bit", nullable: false),
                    XpConcedido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matriculas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matriculas_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matriculas_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MensagensChat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    AlunoId = table.Column<int>(type: "int", nullable: false),
                    RemetenteId = table.Column<int>(type: "int", nullable: true),
                    EhSistema = table.Column<bool>(type: "bit", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensagensChat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensagensChat_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MensagensChat_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MensagensChat_Usuarios_RemetenteId",
                        column: x => x.RemetenteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Materiais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    DisciplinaId = table.Column<int>(type: "int", nullable: true),
                    ProfessorId = table.Column<int>(type: "int", nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    UrlConteudo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DataPostagem = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materiais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materiais_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Materiais_Disciplinas_DisciplinaId",
                        column: x => x.DisciplinaId,
                        principalTable: "Disciplinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Materiais_Professores_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Provas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    DisciplinaId = table.Column<int>(type: "int", nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotaMaxima = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    NotaMinima = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Provas_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Provas_Disciplinas_DisciplinaId",
                        column: x => x.DisciplinaId,
                        principalTable: "Disciplinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Certificados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatriculaId = table.Column<int>(type: "int", nullable: false),
                    AlunoId = table.Column<int>(type: "int", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    CodigoValidacao = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CargaHoraria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificados_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificados_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificados_Matriculas_MatriculaId",
                        column: x => x.MatriculaId,
                        principalTable: "Matriculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Questoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProvaId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Enunciado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RespostaCorreta = table.Column<int>(type: "int", nullable: true),
                    Orientacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PalavrasChave = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinPalavras = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questoes_Provas_ProvaId",
                        column: x => x.ProvaId,
                        principalTable: "Provas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tentativas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<int>(type: "int", nullable: false),
                    ProvaId = table.Column<int>(type: "int", nullable: false),
                    DataRealizacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Acertos = table.Column<int>(type: "int", nullable: false),
                    TotalQuestoes = table.Column<int>(type: "int", nullable: false),
                    PercentualAcerto = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    NotaObtida = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    Aprovada = table.Column<bool>(type: "bit", nullable: false),
                    RespostaDissertativa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfessorCorretorId = table.Column<int>(type: "int", nullable: true),
                    DataCorrecao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tentativas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tentativas_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tentativas_Professores_ProfessorCorretorId",
                        column: x => x.ProfessorCorretorId,
                        principalTable: "Professores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tentativas_Provas_ProvaId",
                        column: x => x.ProvaId,
                        principalTable: "Provas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alternativas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestaoId = table.Column<int>(type: "int", nullable: false),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alternativas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alternativas_Questoes_QuestaoId",
                        column: x => x.QuestaoId,
                        principalTable: "Questoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RespostasTentativa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TentativaId = table.Column<int>(type: "int", nullable: false),
                    QuestaoId = table.Column<int>(type: "int", nullable: false),
                    AlternativaEscolhida = table.Column<int>(type: "int", nullable: true),
                    Correta = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespostasTentativa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespostasTentativa_Questoes_QuestaoId",
                        column: x => x.QuestaoId,
                        principalTable: "Questoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RespostasTentativa_Tentativas_TentativaId",
                        column: x => x.TentativaId,
                        principalTable: "Tentativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ParametrosSistema",
                columns: new[] { "Id", "AtualizadoPorId", "CargaHorariaMaximaAluno", "EmailInstitucional", "EnderecoCampus", "MediaAprovacaoMinima", "NomeInstituicao", "PeriodoLetivoAtual", "PermitirNovasMatriculas", "TelefoneInstitucional", "UltimaAtualizacao", "VagasPadraoPorCurso" },
                values: new object[] { 1, null, 240, "contato@cursaaqui.com.br", "Av. Paulista, 1000 - Bela Vista, São Paulo - SP", 7.0m, "Cursa Aqui - Plataforma Educacional", "2026.1", true, "(11) 3456-7890", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 60 });

            migrationBuilder.InsertData(
                table: "PermissoesSistema",
                columns: new[] { "Id", "ChaveFuncionalidade", "Descricao", "Modulo", "NomeExibicao", "PermitidoAdmin", "PermitidoSecretaria" },
                values: new object[,]
                {
                    { 1, "GerenciarUsuarios", "Cadastrar, editar e inativar contas de Administradores e da Secretaria.", "Administração", "Gerenciar Usuários Internos", true, false },
                    { 2, "GerenciarPermissoes", "Definir, alterar e revogar níveis de acesso aos recursos do sistema.", "Administração", "Gerenciar Permissões de Acesso", true, false },
                    { 3, "GerenciarCursos", "Cadastrar, editar, inativar cursos, cargas horárias e vagas.", "Acadêmico", "Gerenciar Catálogo de Cursos", true, false },
                    { 4, "VisualizarRelatorios", "Consultar indicadores estratégicos, matrículas por curso e taxas analíticas.", "Inteligência & Gestão", "Visualizar Relatórios Gerenciais", true, false },
                    { 5, "ConfigurarParametros", "Ajustar período letivo vigente, notas mínimas e travas de negócio.", "Governança", "Configurar Parâmetros do Sistema", true, false },
                    { 6, "GerenciarMatriculas", "Realizar, alterar, renovar e cancelar matrículas de alunos.", "Secretaria", "Gerenciar Matrículas", true, true },
                    { 7, "ConsultarCadastro", "Localizar dados cadastrais, histórico e cursos matriculados.", "Secretaria", "Consultar Cadastro de Alunos", true, true },
                    { 8, "RegistrarAtendimentos", "Gerar protocolos de atendimento, providências e histórico de contatos.", "Secretaria", "Registrar e Tratar Atendimentos", true, true },
                    { 9, "GerarRelatoriosAdministrativos", "Emitir listas operacionais de matriculados, atendimentos e pendências.", "Secretaria", "Gerar Relatórios Administrativos", true, true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alternativas_QuestaoId_Ordem",
                table: "Alternativas",
                columns: new[] { "QuestaoId", "Ordem" },
                unique: true);

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
                name: "IX_Atendimentos_AlunoId",
                table: "Atendimentos",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_Atendimentos_AtendenteId",
                table: "Atendimentos",
                column: "AtendenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Atendimentos_Protocolo",
                table: "Atendimentos",
                column: "Protocolo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Atendimentos_Status",
                table: "Atendimentos",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesCurso_AlunoId_CursoId",
                table: "AvaliacoesCurso",
                columns: new[] { "AlunoId", "CursoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesCurso_CursoId",
                table: "AvaliacoesCurso",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificados_AlunoId",
                table: "Certificados",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificados_CodigoValidacao",
                table: "Certificados",
                column: "CodigoValidacao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificados_CursoId",
                table: "Certificados",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificados_MatriculaId",
                table: "Certificados",
                column: "MatriculaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_Codigo",
                table: "Cursos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_ProfessorId",
                table: "Cursos",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Disciplinas_CursoId_Ordem",
                table: "Disciplinas",
                columns: new[] { "CursoId", "Ordem" });

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_RespondidoPorId",
                table: "Feedbacks",
                column: "RespondidoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_Status",
                table: "Feedbacks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UsuarioId",
                table: "Feedbacks",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiais_CursoId",
                table: "Materiais",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiais_DisciplinaId",
                table: "Materiais",
                column: "DisciplinaId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiais_ProfessorId",
                table: "Materiais",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_AlunoId_CursoId_PeriodoLetivo",
                table: "Matriculas",
                columns: new[] { "AlunoId", "CursoId", "PeriodoLetivo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_CursoId",
                table: "Matriculas",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_Status",
                table: "Matriculas",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MensagensChat_AlunoId",
                table: "MensagensChat",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_MensagensChat_CursoId_AlunoId_DataEnvio",
                table: "MensagensChat",
                columns: new[] { "CursoId", "AlunoId", "DataEnvio" });

            migrationBuilder.CreateIndex(
                name: "IX_MensagensChat_RemetenteId",
                table: "MensagensChat",
                column: "RemetenteId");

            migrationBuilder.CreateIndex(
                name: "IX_MetasEstudo_AlunoId",
                table: "MetasEstudo",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_ParametrosSistema_AtualizadoPorId",
                table: "ParametrosSistema",
                column: "AtualizadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissoesSistema_ChaveFuncionalidade",
                table: "PermissoesSistema",
                column: "ChaveFuncionalidade",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provas_CursoId",
                table: "Provas",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Provas_DisciplinaId",
                table: "Provas",
                column: "DisciplinaId");

            migrationBuilder.CreateIndex(
                name: "IX_Questoes_ProvaId",
                table: "Questoes",
                column: "ProvaId");

            migrationBuilder.CreateIndex(
                name: "IX_RespostasTentativa_QuestaoId",
                table: "RespostasTentativa",
                column: "QuestaoId");

            migrationBuilder.CreateIndex(
                name: "IX_RespostasTentativa_TentativaId",
                table: "RespostasTentativa",
                column: "TentativaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tentativas_AlunoId_ProvaId",
                table: "Tentativas",
                columns: new[] { "AlunoId", "ProvaId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tentativas_ProfessorCorretorId",
                table: "Tentativas",
                column: "ProfessorCorretorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tentativas_ProvaId",
                table: "Tentativas",
                column: "ProvaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tentativas_Status",
                table: "Tentativas",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TermosGlossario_Chave",
                table: "TermosGlossario",
                column: "Chave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Cpf",
                table: "Usuarios",
                column: "Cpf",
                unique: true,
                filter: "[Cpf] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "Alternativas");

            migrationBuilder.DropTable(
                name: "AnotacoesCurso");

            migrationBuilder.DropTable(
                name: "Atendimentos");

            migrationBuilder.DropTable(
                name: "AvaliacoesCurso");

            migrationBuilder.DropTable(
                name: "Certificados");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Materiais");

            migrationBuilder.DropTable(
                name: "MensagensChat");

            migrationBuilder.DropTable(
                name: "MetasEstudo");

            migrationBuilder.DropTable(
                name: "ParametrosSistema");

            migrationBuilder.DropTable(
                name: "PermissoesSistema");

            migrationBuilder.DropTable(
                name: "RespostasTentativa");

            migrationBuilder.DropTable(
                name: "Secretarias");

            migrationBuilder.DropTable(
                name: "TermosGlossario");

            migrationBuilder.DropTable(
                name: "Matriculas");

            migrationBuilder.DropTable(
                name: "Questoes");

            migrationBuilder.DropTable(
                name: "Tentativas");

            migrationBuilder.DropTable(
                name: "Alunos");

            migrationBuilder.DropTable(
                name: "Provas");

            migrationBuilder.DropTable(
                name: "Disciplinas");

            migrationBuilder.DropTable(
                name: "Cursos");

            migrationBuilder.DropTable(
                name: "Professores");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
