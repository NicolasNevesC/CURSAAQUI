using CursaAqui_API.Models;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Data;

public class CursaAquiDbContext : DbContext
{
    public CursaAquiDbContext(DbContextOptions<CursaAquiDbContext> options) : base(options) { }

    // Usuários (TPT): Usuarios + Administradores + Secretarias + Professores + Alunos
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Administrador> Administradores => Set<Administrador>();
    public DbSet<Secretaria> Secretarias => Set<Secretaria>();
    public DbSet<Professor> Professores => Set<Professor>();
    public DbSet<Aluno> Alunos => Set<Aluno>();

    // Conteúdo
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Disciplina> Disciplinas => Set<Disciplina>();
    public DbSet<Material> Materiais => Set<Material>();
    public DbSet<AvaliacaoCurso> AvaliacoesCurso => Set<AvaliacaoCurso>();
    public DbSet<TermoGlossario> TermosGlossario => Set<TermoGlossario>();

    // Avaliação
    public DbSet<Prova> Provas => Set<Prova>();
    public DbSet<Questao> Questoes => Set<Questao>();
    public DbSet<Alternativa> Alternativas => Set<Alternativa>();
    public DbSet<Tentativa> Tentativas => Set<Tentativa>();
    public DbSet<RespostaTentativa> RespostasTentativa => Set<RespostaTentativa>();

    // Acadêmico
    public DbSet<Matricula> Matriculas => Set<Matricula>();
    public DbSet<Certificado> Certificados => Set<Certificado>();
    public DbSet<MensagemChat> MensagensChat => Set<MensagemChat>();

    // Operacional / sistema
    public DbSet<Atendimento> Atendimentos => Set<Atendimento>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<ParametrosSistema> ParametrosSistema => Set<ParametrosSistema>();
    public DbSet<PermissaoSistema> PermissoesSistema => Set<PermissaoSistema>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // Regra geral: NÃO apagar em cascata (o SQL Server recusa "multiple cascade paths"
        // e o sistema já trabalha com inativação via campo Ativo). Abaixo liberamos
        // cascata só onde o filho não existe sem o pai.
        foreach (var fk in mb.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        // ---------------- USUÁRIOS (Tabela por Tipo) ----------------
        mb.Entity<Usuario>(e =>
        {
            e.UseTptMappingStrategy();
            e.ToTable("Usuarios");
            e.Property(u => u.Nome).HasMaxLength(120).IsRequired();
            e.Property(u => u.Email).HasMaxLength(150).IsRequired();
            e.Property(u => u.SenhaHash).HasMaxLength(200).IsRequired();
            e.Property(u => u.Cpf).HasMaxLength(11);        // somente dígitos
            e.Property(u => u.Telefone).HasMaxLength(20);
            e.Property(u => u.AvatarUrl).HasMaxLength(300);
            e.Property(u => u.Tema).HasMaxLength(20);
            e.Property(u => u.Acessibilidade).HasMaxLength(100);
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.Cpf).IsUnique().HasFilter("[Cpf] IS NOT NULL"); // CPF é opcional
        });
        mb.Entity<Administrador>().ToTable("Administradores");
        mb.Entity<Secretaria>().ToTable("Secretarias");
        mb.Entity<Professor>().ToTable("Professores");
        mb.Entity<Aluno>().ToTable("Alunos");

        mb.Entity<Administrador>().Property(a => a.NivelAutoridade).HasMaxLength(60);
        mb.Entity<Secretaria>().Property(s => s.SetorAtendimento).HasMaxLength(80);
        mb.Entity<Professor>().Property(p => p.Titulacao).HasMaxLength(30);
        mb.Entity<Aluno>().Property(a => a.Endereco).HasMaxLength(250);

        // ---------------- CONTEÚDO ----------------
        mb.Entity<Curso>(e =>
        {
            e.Property(c => c.Codigo).HasMaxLength(20).IsRequired();
            e.Property(c => c.Titulo).HasMaxLength(200).IsRequired();
            e.Property(c => c.Categoria).HasMaxLength(100).IsRequired();
            e.Property(c => c.Area).HasMaxLength(150);
            e.Property(c => c.ImagemUrl).HasMaxLength(500);
            e.Property(c => c.Avaliacao).HasPrecision(3, 2);
            e.HasIndex(c => c.Codigo).IsUnique();
        });

        mb.Entity<Disciplina>(e =>
        {
            e.Property(d => d.Nome).HasMaxLength(250).IsRequired();
            e.HasIndex(d => new { d.CursoId, d.Ordem });
        });

        mb.Entity<Material>(e =>
        {
            e.Property(m => m.Titulo).HasMaxLength(200).IsRequired();
            e.Property(m => m.UrlConteudo).HasMaxLength(500).IsRequired();
        });

        mb.Entity<AvaliacaoCurso>(e =>
        {
            e.Property(a => a.Comentario).HasMaxLength(1000);
            e.HasIndex(a => new { a.AlunoId, a.CursoId }).IsUnique(); // 1 avaliação por aluno/curso
        });

        mb.Entity<TermoGlossario>(e =>
        {
            e.Property(t => t.Chave).HasMaxLength(60).IsRequired();
            e.Property(t => t.Termo).HasMaxLength(200).IsRequired();
            e.HasIndex(t => t.Chave).IsUnique();
        });

        // ---------------- AVALIAÇÃO ----------------
        mb.Entity<Prova>(e =>
        {
            e.Property(p => p.Titulo).HasMaxLength(200).IsRequired();
            e.Property(p => p.NotaMaxima).HasPrecision(4, 2);
            e.Property(p => p.NotaMinima).HasPrecision(4, 2);
        });

        mb.Entity<Questao>(e =>
        {
            e.Property(q => q.Enunciado).IsRequired();
            e.Property(q => q.PalavrasChave).HasMaxLength(500);
            e.HasOne(q => q.Prova).WithMany(p => p.Questoes)
                .HasForeignKey(q => q.ProvaId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<Alternativa>(e =>
        {
            e.Property(a => a.Texto).HasMaxLength(1000).IsRequired();
            e.HasIndex(a => new { a.QuestaoId, a.Ordem }).IsUnique();
            e.HasOne(a => a.Questao).WithMany(q => q.Alternativas)
                .HasForeignKey(a => a.QuestaoId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<Tentativa>(e =>
        {
            e.Property(t => t.PercentualAcerto).HasPrecision(5, 2);
            e.Property(t => t.NotaObtida).HasPrecision(4, 2);
            e.HasIndex(t => new { t.AlunoId, t.ProvaId });
            e.HasIndex(t => t.Status); // fila de correção do professor
        });

        mb.Entity<RespostaTentativa>(e =>
        {
            e.HasOne(r => r.Tentativa).WithMany(t => t.Respostas)
                .HasForeignKey(r => r.TentativaId).OnDelete(DeleteBehavior.Cascade);
            // Questao -> Restrict (já definido acima) evita o caminho de cascata duplo.
        });

        // ---------------- ACADÊMICO ----------------
        mb.Entity<Matricula>(e =>
        {
            e.Property(m => m.PeriodoLetivo).HasMaxLength(10).IsRequired();
            e.Property(m => m.NotaFinal).HasPrecision(4, 2);
            e.Property(m => m.Observacoes).HasMaxLength(500);
            // Impede matrícula duplicada no mesmo curso/período (regra do desktop)
            e.HasIndex(m => new { m.AlunoId, m.CursoId, m.PeriodoLetivo }).IsUnique();
            e.HasIndex(m => m.Status);
            e.HasOne(m => m.Certificado).WithOne(c => c.Matricula)
                .HasForeignKey<Certificado>(c => c.MatriculaId);
        });

        mb.Entity<Certificado>(e =>
        {
            e.Property(c => c.CodigoValidacao).HasMaxLength(40).IsRequired();
            e.HasIndex(c => c.CodigoValidacao).IsUnique();
            e.HasIndex(c => c.MatriculaId).IsUnique();
        });

        mb.Entity<MensagemChat>(e =>
        {
            e.Property(m => m.Texto).HasMaxLength(2000).IsRequired();
            e.HasIndex(m => new { m.CursoId, m.AlunoId, m.DataEnvio });
        });

        // ---------------- OPERACIONAL / SISTEMA ----------------
        mb.Entity<Atendimento>(e =>
        {
            e.Property(a => a.Protocolo).HasMaxLength(20).IsRequired();
            e.Property(a => a.SolicitanteNome).HasMaxLength(120).IsRequired();
            e.Property(a => a.Contato).HasMaxLength(150);
            e.Property(a => a.Assunto).HasMaxLength(200).IsRequired();
            e.HasIndex(a => a.Protocolo).IsUnique();
            e.HasIndex(a => a.Status);
        });

        mb.Entity<Feedback>(e =>
        {
            e.Property(f => f.Assunto).HasMaxLength(200).IsRequired();
            e.Property(f => f.Mensagem).HasMaxLength(2000).IsRequired();
            e.HasIndex(f => f.Status);
        });

        mb.Entity<ParametrosSistema>(e =>
        {
            e.Property(p => p.Id).ValueGeneratedNever(); // sempre Id = 1
            e.Property(p => p.NomeInstituicao).HasMaxLength(150);
            e.Property(p => p.PeriodoLetivoAtual).HasMaxLength(10);
            e.Property(p => p.MediaAprovacaoMinima).HasPrecision(4, 2);
            e.Property(p => p.EmailInstitucional).HasMaxLength(150);
            e.Property(p => p.TelefoneInstitucional).HasMaxLength(30);
            e.Property(p => p.EnderecoCampus).HasMaxLength(250);
            e.HasData(new ParametrosSistema
            {
                Id = 1,
                UltimaAtualizacao = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        });

        mb.Entity<PermissaoSistema>(e =>
        {
            e.Property(p => p.ChaveFuncionalidade).HasMaxLength(60).IsRequired();
            e.HasIndex(p => p.ChaveFuncionalidade).IsUnique();
            e.HasData(
                new PermissaoSistema { Id = 1, ChaveFuncionalidade = "GerenciarUsuarios", NomeExibicao = "Gerenciar Usuários Internos", Modulo = "Administração", Descricao = "Cadastrar, editar e inativar contas de Administradores e da Secretaria.", PermitidoAdmin = true, PermitidoSecretaria = false },
                new PermissaoSistema { Id = 2, ChaveFuncionalidade = "GerenciarPermissoes", NomeExibicao = "Gerenciar Permissões de Acesso", Modulo = "Administração", Descricao = "Definir, alterar e revogar níveis de acesso aos recursos do sistema.", PermitidoAdmin = true, PermitidoSecretaria = false },
                new PermissaoSistema { Id = 3, ChaveFuncionalidade = "GerenciarCursos", NomeExibicao = "Gerenciar Catálogo de Cursos", Modulo = "Acadêmico", Descricao = "Cadastrar, editar, inativar cursos, cargas horárias e vagas.", PermitidoAdmin = true, PermitidoSecretaria = false },
                new PermissaoSistema { Id = 4, ChaveFuncionalidade = "VisualizarRelatorios", NomeExibicao = "Visualizar Relatórios Gerenciais", Modulo = "Inteligência & Gestão", Descricao = "Consultar indicadores estratégicos, matrículas por curso e taxas analíticas.", PermitidoAdmin = true, PermitidoSecretaria = false },
                new PermissaoSistema { Id = 5, ChaveFuncionalidade = "ConfigurarParametros", NomeExibicao = "Configurar Parâmetros do Sistema", Modulo = "Governança", Descricao = "Ajustar período letivo vigente, notas mínimas e travas de negócio.", PermitidoAdmin = true, PermitidoSecretaria = false },
                new PermissaoSistema { Id = 6, ChaveFuncionalidade = "GerenciarMatriculas", NomeExibicao = "Gerenciar Matrículas", Modulo = "Secretaria", Descricao = "Realizar, alterar, renovar e cancelar matrículas de alunos.", PermitidoAdmin = true, PermitidoSecretaria = true },
                new PermissaoSistema { Id = 7, ChaveFuncionalidade = "ConsultarCadastro", NomeExibicao = "Consultar Cadastro de Alunos", Modulo = "Secretaria", Descricao = "Localizar dados cadastrais, histórico e cursos matriculados.", PermitidoAdmin = true, PermitidoSecretaria = true },
                new PermissaoSistema { Id = 8, ChaveFuncionalidade = "RegistrarAtendimentos", NomeExibicao = "Registrar e Tratar Atendimentos", Modulo = "Secretaria", Descricao = "Gerar protocolos de atendimento, providências e histórico de contatos.", PermitidoAdmin = true, PermitidoSecretaria = true },
                new PermissaoSistema { Id = 9, ChaveFuncionalidade = "GerarRelatoriosAdministrativos", NomeExibicao = "Gerar Relatórios Administrativos", Modulo = "Secretaria", Descricao = "Emitir listas operacionais de matriculados, atendimentos e pendências.", PermitidoAdmin = true, PermitidoSecretaria = true }
            );
        });
    }
}
