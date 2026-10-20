using CursaAqui.App.Models;

namespace CursaAqui.App.Services;

/// <summary>Converte os DTOs da API nos models que as telas do desktop já usam.</summary>
public static class ApiMapper
{
    public static string? NullSeVazio(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    // A API devolve datas em UTC (às vezes sem o "Z"): converte para o horário local do computador.
    public static DateTime Local(DateTime d) =>
        d.Kind == DateTimeKind.Local ? d : DateTime.SpecifyKind(d, DateTimeKind.Utc).ToLocalTime();

    public static DateTime? Local(DateTime? d) => d.HasValue ? Local(d.Value) : null;

    // A API guarda só os 11 dígitos; o desktop sempre exibiu formatado.
    public static string FormatarCpf(string? cpf)
    {
        var d = new string((cpf ?? "").Where(char.IsDigit).ToArray());
        return d.Length == 11 ? $"{d[..3]}.{d[3..6]}.{d[6..9]}-{d[9..]}" : (cpf ?? "");
    }

    public static UsuarioInterno? ToUsuario(UsuarioApiDto u)
    {
        UsuarioInterno? r = u.Perfil switch
        {
            "Administrador" => new Administrador
            {
                NivelAutoridade = u.NivelAutoridade ?? "Total",
                UltimoAcessoGerencial = Local(u.UltimoAcessoGerencial)
            },
            "Secretaria" => new Secretaria { SetorAtendimento = u.SetorAtendimento ?? "Secretaria Acadêmica" },
            _ => null // professores e alunos não são "usuários internos" do desktop
        };
        if (r == null) return null;

        r.Id = u.Id;
        r.Nome = u.Nome;
        r.Email = u.Email;
        r.Senha = string.Empty; // a API nunca devolve senha
        r.Cpf = FormatarCpf(u.Cpf);
        r.Telefone = u.Telefone ?? string.Empty;
        r.DataCadastro = Local(u.DataCadastro);
        r.Ativo = u.Ativo;
        return r;
    }

    public static Aluno ToAluno(AlunoApiDto a) => new()
    {
        Id = a.Id,
        Nome = a.Nome,
        Email = a.Email,
        Cpf = FormatarCpf(a.Cpf),
        Telefone = a.Telefone ?? string.Empty,
        DataNascimento = a.DataNascimento ?? default,
        Endereco = a.Endereco ?? string.Empty,
        DataCadastro = Local(a.DataCadastro),
        Ativo = a.Ativo,
        Xp = a.Xp,
        Nivel = a.Nivel
    };

    public static Curso ToCurso(CursoApiDto c) => new()
    {
        Id = c.Id,
        Codigo = c.Codigo,
        Titulo = c.Titulo,
        Categoria = c.Categoria,
        Area = c.Area ?? string.Empty,
        CargaHoraria = c.CargaHoraria,
        NivelMinimo = c.NivelMinimo,
        ImagemUrl = c.ImagemUrl ?? string.Empty,
        ProfessorId = c.ProfessorId,
        ProfessorResponsavel = c.ProfessorNome ?? "Sem professor definido",
        Ativo = c.Ativo,
        VagasTotais = c.VagasTotais ?? 0, // 0 = sem limite de vagas
        VagasOcupadas = c.VagasOcupadas,
        Descricao = c.Descricao ?? string.Empty,
        Avaliacao = (double)c.Avaliacao
    };

    public static Matricula ToMatricula(MatriculaApiDto m) => new()
    {
        Id = m.Id,
        AlunoId = m.AlunoId,
        AlunoNome = m.AlunoNome,
        AlunoCpf = FormatarCpf(m.AlunoCpf),
        CursoId = m.CursoId,
        CursoNome = m.CursoTitulo,
        DataMatricula = Local(m.DataMatricula),
        Status = m.Status,
        NotaFinal = (double)(m.NotaFinal ?? 0m),
        ProgressoPercentual = m.ProgressoPercentual,
        PeriodoLetivo = m.PeriodoLetivo,
        Observacoes = m.Observacoes ?? string.Empty
    };

    public static Atendimento ToAtendimento(AtendimentoApiDto a) => new()
    {
        Id = a.Id,
        Protocolo = a.Protocolo,
        SolicitanteNome = a.SolicitanteNome,
        TipoSolicitante = a.TipoSolicitante,
        Contato = a.Contato ?? string.Empty,
        Assunto = a.Assunto,
        Descricao = a.Descricao,
        Providencia = a.Providencia ?? string.Empty,
        DataAbertura = Local(a.DataAbertura),
        DataFechamento = Local(a.DataFechamento),
        AtendenteId = a.AtendenteId,
        AtendenteNome = a.AtendenteNome,
        Status = a.Status
    };

    public static ParametrosSistema ToParametros(ParametrosApiDto p) => new()
    {
        NomeInstituicao = p.NomeInstituicao,
        PeriodoLetivoAtual = p.PeriodoLetivoAtual,
        MediaAprovacaoMinima = (double)p.MediaAprovacaoMinima,
        CargaHorariaMaximaAluno = p.CargaHorariaMaximaAluno,
        VagasPadraoPorCurso = p.VagasPadraoPorCurso,
        PermitirNovasMatriculas = p.PermitirNovasMatriculas,
        EmailInstitucional = p.EmailInstitucional,
        TelefoneInstitucional = p.TelefoneInstitucional,
        EnderecoCampus = p.EnderecoCampus,
        UltimaAtualizacao = Local(p.UltimaAtualizacao),
        AtualizadoPor = p.AtualizadoPor ?? "-"
    };

    public static PermissaoSistema ToPermissao(PermissaoApiDto p) => new()
    {
        Id = p.Id,
        ChaveFuncionalidade = p.ChaveFuncionalidade,
        NomeExibicao = p.NomeExibicao,
        Modulo = p.Modulo,
        Descricao = p.Descricao,
        PermitidoAdmin = p.PermitidoAdmin,
        PermitidoSecretaria = p.PermitidoSecretaria
    };

    public static ProfessorResumo ToProfessor(ProfessorApiDto p) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Titulacao = p.Titulacao ?? string.Empty
    };
}
