using System.Security.Claims;
using System.Text;
using CursaAqui_API.Data;
using CursaAqui_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CursaAqui_API.Services;

public class TokenService(IConfiguration cfg)
{
    public (string Token, DateTime Expira) Gerar(Usuario u)
    {
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!));
        var expira = DateTime.UtcNow.AddHours(double.Parse(cfg["Jwt:HorasExpiracao"] ?? "8"));
        var desc = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("sub", u.Id.ToString()),
                new Claim("name", u.Nome),
                new Claim("email", u.Email),
                new Claim("role", u.Perfil.ToString())
            }),
            Expires = expira,
            Issuer = cfg["Jwt:Issuer"],
            Audience = cfg["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256)
        };
        return (new JsonWebTokenHandler().CreateToken(desc), expira);
    }
}

public record ResultadoMatricula(Matricula? Matricula, bool Criada, int Status, string? Erro);
public record ResultadoProgresso(bool ConcluiuAgora, bool SubiuNivel, int XpGanho);

/// <summary>Regras que hoje rodam no navegador (course.html / aluno.html) e agora ficam na API.</summary>
public static class Regras
{
    public const int XpConclusaoCurso = 1000;
    public const int XpAcertoTreino = 25;

    public static int XpParaProximoNivel(int nivel) => nivel * 1000;

    /// <summary>Soma XP e sobe de nível enquanto Xp >= Nivel x 1000 (sobra vira XP do novo nível).</summary>
    public static bool AdicionarXp(Aluno aluno, int xp)
    {
        aluno.Xp += xp;
        bool subiu = false;
        while (aluno.Xp >= XpParaProximoNivel(aluno.Nivel))
        {
            aluno.Xp -= XpParaProximoNivel(aluno.Nivel);
            aluno.Nivel++;
            subiu = true;
        }
        return subiu;
    }

    /// <summary>Material/estudo = 30%; prova aprovada = 70%; reprovado = proporcional (máx. 35) e teto de 65%.</summary>
    public static int CalcularProgresso(Matricula m, Tentativa? ultima)
    {
        int pesoMaterial = (m.MaterialAcessado || m.EstudoConcluido) ? 30 : 0;
        int pesoProva = 0;
        if (ultima != null)
            pesoProva = ultima.Aprovada
                ? 70
                : (int)Math.Round((double)ultima.PercentualAcerto / 100 * 35, MidpointRounding.AwayFromZero);
        int total = pesoMaterial + pesoProva;
        return ultima is { Aprovada: true } ? Math.Min(total, 100) : Math.Min(total, 65);
    }

    /// <summary>Recalcula o progresso; ao chegar a 100% aprovado, conclui o curso e dá +1000 XP uma única vez.</summary>
    public static ResultadoProgresso Atualizar(Matricula m, Aluno aluno, Tentativa? ultima)
    {
        if (m.XpConcedido) return new(false, false, 0); // curso concluído não regride
        m.ProgressoPercentual = CalcularProgresso(m, ultima);
        if (m.ProgressoPercentual >= 100 && ultima is { Aprovada: true })
        {
            m.XpConcedido = true;
            m.Status = StatusMatricula.Concluida;
            m.DataConclusao = DateTime.UtcNow;
            m.NotaFinal ??= ultima.NotaObtida;
            bool subiu = AdicionarXp(aluno, XpConclusaoCurso);
            return new(true, subiu, XpConclusaoCurso);
        }
        return new(false, false, 0);
    }
}

public class MatriculaService(CursaAquiDbContext db)
{
    // Vagas ocupadas = matrículas Ativa, Renovada e Trancada (trancada ainda guarda a vaga)
    public static readonly StatusMatricula[] StatusOcupamVaga =
        { StatusMatricula.Ativa, StatusMatricula.Renovada, StatusMatricula.Trancada };

    /// <summary>
    /// Cria (ou devolve a existente) a matrícula de um aluno. Mesmas travas do desktop:
    /// matrículas liberadas, curso ativo, vaga disponível e sem duplicidade; mais o nível mínimo do site.
    /// </summary>
    public async Task<ResultadoMatricula> IniciarAsync(int alunoId, int cursoId, string? periodo, bool ignorarNivel)
    {
        var aluno = await db.Alunos.FirstOrDefaultAsync(a => a.Id == alunoId);
        if (aluno == null) return new(null, false, 404, "Aluno não encontrado.");
        if (!aluno.Ativo) return new(null, false, 409, "Aluno inativo.");

        var curso = await db.Cursos.FirstOrDefaultAsync(c => c.Id == cursoId);
        if (curso == null) return new(null, false, 404, "Curso não encontrado.");
        if (!curso.Ativo) return new(null, false, 409, "Curso inativo.");

        var par = await db.ParametrosSistema.AsNoTracking().FirstAsync(p => p.Id == 1);
        periodo = string.IsNullOrWhiteSpace(periodo) ? par.PeriodoLetivoAtual : periodo.Trim();

        var existente = await db.Matriculas.FirstOrDefaultAsync(m =>
            m.AlunoId == alunoId && m.CursoId == cursoId && m.PeriodoLetivo == periodo);
        if (existente != null)
            return existente.Status == StatusMatricula.Cancelada
                ? new(null, false, 409, "Matrícula cancelada neste período. Procure a Secretaria.")
                : new(existente, false, 200, null);

        if (!par.PermitirNovasMatriculas) return new(null, false, 409, "As novas matrículas estão suspensas no momento.");
        if (!ignorarNivel && aluno.Nivel < curso.NivelMinimo)
            return new(null, false, 403, $"Curso bloqueado: é preciso alcançar o nível {curso.NivelMinimo}.");

        if (curso.VagasTotais != null)
        {
            int ocupadas = await db.Matriculas.CountAsync(m => m.CursoId == cursoId && StatusOcupamVaga.Contains(m.Status));
            if (ocupadas >= curso.VagasTotais) return new(null, false, 409, "Não há vagas disponíveis neste curso.");
        }

        var nova = new Matricula { AlunoId = alunoId, CursoId = cursoId, PeriodoLetivo = periodo, Status = StatusMatricula.Ativa };
        db.Matriculas.Add(nova);
        await db.SaveChangesAsync();
        return new(nova, true, 201, null);
    }
}
