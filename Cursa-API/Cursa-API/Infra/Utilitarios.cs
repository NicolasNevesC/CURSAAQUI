using System.Globalization;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text;
using CursaAqui_API.Data;
using CursaAqui_API.Dtos;
using CursaAqui_API.Models;
using CursaAqui_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Infra;

/// <summary>Controller base: atalhos de usuário logado e resposta de erro padronizada { "erro": "..." }.</summary>
[ApiController]
public abstract class ApiBase : ControllerBase
{
    protected int UserId => int.Parse(User.FindFirst("sub")!.Value);
    protected bool EhStaff => User.IsInRole("Administrador") || User.IsInRole("Secretaria");

    protected IActionResult Erro(int status, string mensagem) => StatusCode(status, new { erro = mensagem });

    /// <summary>Administrador gerencia qualquer curso; Professor só os próprios.</summary>
    protected bool PodeGerir(Curso c) =>
        User.IsInRole("Administrador") || (User.IsInRole("Professor") && c.ProfessorId == UserId);
}

/// <summary>
/// Matriz dinâmica do desktop. Vale para Administrador e Secretaria; Professor e Aluno passam direto
/// (o acesso deles é controlado só por [Authorize(Roles = "...")]). Use junto com [Authorize].
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class PermissaoAttribute(string chave) : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext ctx)
    {
        var user = ctx.HttpContext.User;
        if (user.Identity?.IsAuthenticated != true) { ctx.Result = new UnauthorizedResult(); return; }

        bool admin = user.IsInRole("Administrador");
        bool sec = user.IsInRole("Secretaria");
        if (!admin && !sec) return;

        var db = ctx.HttpContext.RequestServices.GetRequiredService<CursaAquiDbContext>();
        var p = await db.PermissoesSistema.AsNoTracking().FirstOrDefaultAsync(x => x.ChaveFuncionalidade == chave);
        bool ok = admin ? (p?.PermitidoAdmin ?? true) : (p?.PermitidoSecretaria ?? false);
        if (!ok)
            ctx.Result = new ObjectResult(new { erro = "Seu perfil não tem permissão para esta funcionalidade." }) { StatusCode = 403 };
    }
}

public static class Arquivos
{
    public static string Pasta(IConfiguration cfg, IWebHostEnvironment env) =>
        Path.GetFullPath(cfg["Uploads:Pasta"] ?? "uploads", env.ContentRootPath);
}

public static class Texto
{
    public static string Digitos(string? s) => new((s ?? "").Where(char.IsDigit).ToArray());
    public static string? CpfNormalizado(string? s) { var d = Digitos(s); return d.Length == 0 ? null : d; }
    public static string Email(string s) => s.Trim().ToLowerInvariant();

    public static bool EmailValido(string s)
    {
        try { return new MailAddress(s).Address == s; } catch { return false; }
    }

    public static bool CpfValido(string cpf)
    {
        if (cpf.Length != 11 || cpf.Distinct().Count() == 1) return false;
        int Dv(int len)
        {
            int soma = 0;
            for (int i = 0; i < len; i++) soma += (cpf[i] - '0') * (len + 1 - i);
            int r = soma * 10 % 11;
            return r == 10 ? 0 : r;
        }
        return Dv(9) == cpf[9] - '0' && Dv(10) == cpf[10] - '0';
    }

    /// <summary>Aceita só &lt;br&gt; (o site usa no enunciado); todo o resto de HTML vira texto.</summary>
    public static string Sanitizar(string s) =>
        s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
         .Replace("&lt;br&gt;", "<br>").Replace("&lt;br/&gt;", "<br>").Replace("&lt;br /&gt;", "<br>");

    public static int Palavras(string s) =>
        s.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

    public static string Slug(string s)
    {
        var d = s.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in d)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark && char.IsLetter(c))
                sb.Append(char.ToLowerInvariant(c));
        return sb.ToString();
    }

    /// <summary>URL relativa, ou http/https. Bloqueia javascript:, data: etc.</summary>
    public static bool UrlSegura(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        if (Uri.TryCreate(url, UriKind.Absolute, out var abs))
            return abs.Scheme == Uri.UriSchemeHttp || abs.Scheme == Uri.UriSchemeHttps;
        return !url.Contains(':') && !url.StartsWith("//");
    }
}

public static class Validacoes
{
    /// <summary>Retorna a mensagem de erro, ou null se estiver tudo certo.</summary>
    public static string? NovoUsuario(string? nome, string? email, string? senha, string? cpf)
    {
        if (string.IsNullOrWhiteSpace(nome)) return "Informe o nome.";
        if (string.IsNullOrWhiteSpace(email) || !Texto.EmailValido(Texto.Email(email))) return "Informe um e-mail válido.";
        if (string.IsNullOrEmpty(senha) || senha.Length < 6) return "A senha deve ter pelo menos 6 caracteres.";
        return Cpf(cpf);
    }

    public static string? Cpf(string? cpf)
    {
        var d = Texto.CpfNormalizado(cpf);
        return d != null && !Texto.CpfValido(d) ? "CPF inválido." : null;
    }
}

public static class UsuarioMapper
{
    public static UsuarioDto ToDto(Usuario u) => new(
        u.Id, u.Nome, u.Email, u.Perfil.ToString(), u.Cpf, u.Telefone, u.DataNascimento, u.Ativo, u.DataCadastro,
        u.AvatarUrl, u.Tema, u.Acessibilidade,
        (u as Administrador)?.NivelAutoridade, (u as Administrador)?.UltimoAcessoGerencial,
        (u as Secretaria)?.SetorAtendimento, (u as Professor)?.Titulacao,
        (u as Aluno)?.Endereco, (u as Aluno)?.Xp, (u as Aluno)?.Nivel);
}

/// <summary>Projeções reutilizadas (viram SELECT direto no SQL, sem trazer entidades inteiras).</summary>
public static class Consultas
{
    public static readonly Expression<Func<Matricula, MatriculaDto>> MatriculaProj = m => new MatriculaDto(
        m.Id, m.AlunoId, m.Aluno.Nome, m.Aluno.Cpf, m.CursoId, m.Curso.Titulo, m.DataMatricula, m.Status,
        m.PeriodoLetivo, m.NotaFinal, m.ProgressoPercentual, m.Observacoes, m.DataConclusao,
        m.MaterialAcessado, m.EstudoConcluido, m.Certificado != null);

    public static readonly Expression<Func<Tentativa, TentativaListaDto>> TentativaProj = t => new TentativaListaDto(
        t.Id, t.AlunoId, t.Aluno.Nome, t.Aluno.Email, t.Prova.CursoId, t.Prova.Curso.Titulo, t.Acertos,
        t.TotalQuestoes, t.PercentualAcerto, t.NotaObtida, t.Aprovada, t.Status, t.DataRealizacao,
        t.Feedback, t.DataCorrecao);

    public static readonly Expression<Func<Atendimento, AtendimentoDto>> AtendimentoProj = a => new AtendimentoDto(
        a.Id, a.Protocolo, a.TipoSolicitante, a.SolicitanteNome, a.Contato, a.AlunoId, a.Assunto, a.Descricao,
        a.Providencia, a.Status, a.DataAbertura, a.DataFechamento, a.AtendenteId, a.Atendente.Nome);

    public static readonly Expression<Func<Feedback, FeedbackDto>> FeedbackProj = f => new FeedbackDto(
        f.Id, f.UsuarioId, f.Usuario.Nome, f.Tipo, f.Assunto, f.Mensagem, f.Status, f.DataEnvio, f.Resposta,
        f.RespondidoPor != null ? f.RespondidoPor.Nome : null, f.DataResposta);

    public static Task<List<HistoricoItemDto>> HistoricoAsync(CursaAquiDbContext db, int alunoId) =>
        db.Matriculas.AsNoTracking().Where(m => m.AlunoId == alunoId).OrderByDescending(m => m.DataMatricula)
            .Select(m => new HistoricoItemDto(m.Id, m.CursoId, m.Curso.Titulo, m.PeriodoLetivo, m.Status,
                m.NotaFinal, m.ProgressoPercentual, m.DataMatricula))
            .ToListAsync();

    /// <summary>Monta CursoDto com vagas ocupadas (calculadas) e, se for aluno, bloqueio por nível e progresso.</summary>
    public static async Task<List<CursoDto>> MontarCursosAsync(CursaAquiDbContext db, List<Curso> cursos, Aluno? aluno)
    {
        var ids = cursos.Select(c => c.Id).ToList();

        var ocupadas = await db.Matriculas.AsNoTracking()
            .Where(m => ids.Contains(m.CursoId) && MatriculaService.StatusOcupamVaga.Contains(m.Status))
            .GroupBy(m => m.CursoId)
            .Select(g => new { CursoId = g.Key, N = g.Count() })
            .ToDictionaryAsync(x => x.CursoId, x => x.N);

        var progresso = new Dictionary<int, int>();
        if (aluno != null)
            progresso = await db.Matriculas.AsNoTracking()
                .Where(m => m.AlunoId == aluno.Id && ids.Contains(m.CursoId))
                .GroupBy(m => m.CursoId)
                .Select(g => new { CursoId = g.Key, P = g.Max(x => x.ProgressoPercentual) })
                .ToDictionaryAsync(x => x.CursoId, x => x.P);

        return cursos.Select(c =>
        {
            int ocup = ocupadas.GetValueOrDefault(c.Id);
            return new CursoDto(c.Id, c.Codigo, c.Titulo, c.Categoria, c.Area, c.Descricao, c.CargaHoraria,
                c.NivelMinimo, c.ImagemUrl, c.ProfessorId, c.Professor?.Nome, c.Ativo, c.VagasTotais, ocup,
                c.VagasTotais == null ? (int?)null : Math.Max(0, c.VagasTotais.Value - ocup),
                c.Avaliacao,
                aluno == null ? (bool?)null : aluno.Nivel < c.NivelMinimo,
                aluno == null ? (int?)null : progresso.GetValueOrDefault(c.Id));
        }).ToList();
    }
}
