using CursaAqui_API.Data;
using CursaAqui_API.Dtos;
using CursaAqui_API.Infra;
using CursaAqui_API.Models;
using CursaAqui_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Controllers;

/// <summary>Tudo que o aluno logado vê no seu painel (aluno.html): dashboard, planner, caderno, certificados.</summary>
[Route("api/alunos/me"), Authorize(Roles = "Aluno")]
public class AlunoAreaController(CursaAquiDbContext db) : ApiBase
{
    // ================= DASHBOARD =================

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var aluno = await db.Alunos.AsNoTracking().FirstAsync(a => a.Id == UserId);
        var mats = await db.Matriculas.AsNoTracking().Where(m => m.AlunoId == aluno.Id)
            .Select(m => new { m.CursoId, m.Curso.Titulo, m.ProgressoPercentual, m.Status, m.MaterialAcessado }).ToListAsync();
        var metas = await db.MetasEstudo.AsNoTracking().Where(m => m.AlunoId == aluno.Id).Select(m => m.Concluida).ToListAsync();
        int certificados = await db.Certificados.CountAsync(c => c.AlunoId == aluno.Id);
        int pendentes = await db.Tentativas.CountAsync(t => t.AlunoId == aluno.Id && t.Status == StatusTentativa.Pendente);
        bool pareceres = await db.Tentativas.AnyAsync(t => t.AlunoId == aluno.Id && t.Status == StatusTentativa.Corrigida);

        var conquistas = new List<ConquistaDto>
        {
            new(1, "🌱", "Primeiro Passo", "Iniciou o seu primeiro curso no portal.", mats.Count > 0),
            new(2, "⚡", "Foco Total", "Completou uma meta diária no Planner.", metas.Any(c => c)),
            new(3, "📖", "Leitor Voraz", "Acessou material e apostila em PDF.", mats.Any(m => m.MaterialAcessado)),
            new(4, "🎯", "Mestre do Quiz", "Acertou questões no Treino Rápido.", aluno.Xp >= 300),
            new(5, "🏆", "Primeiro Diploma", "Concluiu 100% de um curso oficial.", mats.Any(m => m.ProgressoPercentual >= 100)),
            new(6, "🚀", "Rumo ao Topo", "Alcançou o Nível 2 ou superior.", aluno.Nivel >= 2),
            new(7, "👑", "Especialista", "Acumulou mais de 500 XP.", aluno.Xp >= 500),
            new(8, "💬", "Aprovado!", "Recebeu parecer do professor.", pareceres)
        };

        return Ok(new DashboardAlunoDto(aluno.Nome, aluno.Xp, aluno.Nivel, Regras.XpParaProximoNivel(aluno.Nivel),
            mats.Count(m => m.Status is StatusMatricula.Ativa or StatusMatricula.Renovada),
            mats.Count(m => m.Status == StatusMatricula.Concluida), certificados, metas.Count(c => !c),
            pendentes,
            mats.Select(m => new DashboardCursoDto(m.CursoId, m.Titulo, m.ProgressoPercentual, m.Status)).ToList(),
            conquistas));
    }

    [HttpGet("matriculas")]
    public async Task<IActionResult> Matriculas() =>
        Ok(await db.Matriculas.AsNoTracking().Where(m => m.AlunoId == UserId).OrderByDescending(m => m.DataMatricula)
            .Select(Consultas.MatriculaProj).ToListAsync());

    /// <summary>"Pareceres dos Professores": todas as tentativas do aluno, com nota e feedback.</summary>
    [HttpGet("tentativas")]
    public async Task<IActionResult> Tentativas([FromQuery] int? cursoId)
    {
        var q = db.Tentativas.AsNoTracking().Where(t => t.AlunoId == UserId);
        if (cursoId != null) q = q.Where(t => t.Prova.CursoId == cursoId.Value);
        return Ok(await q.OrderByDescending(t => t.DataRealizacao).Select(Consultas.TentativaProj).ToListAsync());
    }

    [HttpGet("certificados")]
    public async Task<IActionResult> Certificados() =>
        Ok(await db.Certificados.AsNoTracking().Where(c => c.AlunoId == UserId).OrderByDescending(c => c.DataEmissao)
            .Select(c => new CertificadoDto(c.Id, c.CodigoValidacao, c.Aluno.Nome, c.CursoId, c.Curso.Titulo,
                c.CargaHoraria, c.DataEmissao, c.MatriculaId)).ToListAsync());

    // ================= PLANNER (metas) =================

    private static MetaDto ToDto(MetaEstudo m) => new(m.Id, m.Texto, m.Concluida, m.DataCriacao);

    [HttpGet("metas")]
    public async Task<IActionResult> Metas() =>
        Ok((await db.MetasEstudo.AsNoTracking().Where(m => m.AlunoId == UserId).OrderBy(m => m.Concluida).ThenBy(m => m.Id).ToListAsync())
            .Select(ToDto));

    [HttpPost("metas")]
    public async Task<IActionResult> CriarMeta(MetaRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Texto) || req.Texto.Length > 300) return Erro(400, "Informe a meta (até 300 caracteres).");
        var m = new MetaEstudo { AlunoId = UserId, Texto = req.Texto.Trim() };
        db.MetasEstudo.Add(m);
        await db.SaveChangesAsync();
        return Created("/api/alunos/me/metas", ToDto(m));
    }

    [HttpPut("metas/{id:int}")]
    public async Task<IActionResult> AtualizarMeta(int id, MetaUpdateRequest req)
    {
        var m = await db.MetasEstudo.FirstOrDefaultAsync(x => x.Id == id && x.AlunoId == UserId);
        if (m == null) return Erro(404, "Meta não encontrada.");
        if (req.Texto != null)
        {
            if (string.IsNullOrWhiteSpace(req.Texto) || req.Texto.Length > 300) return Erro(400, "Informe a meta (até 300 caracteres).");
            m.Texto = req.Texto.Trim();
        }
        if (req.Concluida != null) m.Concluida = req.Concluida.Value;
        await db.SaveChangesAsync();
        return Ok(ToDto(m));
    }

    [HttpDelete("metas/{id:int}")]
    public async Task<IActionResult> ExcluirMeta(int id)
    {
        var m = await db.MetasEstudo.FirstOrDefaultAsync(x => x.Id == id && x.AlunoId == UserId);
        if (m == null) return Erro(404, "Meta não encontrada.");
        db.MetasEstudo.Remove(m);
        await db.SaveChangesAsync();
        return NoContent();
    }

    // ================= CADERNO (anotações por curso) =================

    [HttpGet("anotacoes")]
    public async Task<IActionResult> Anotacoes([FromQuery] int? cursoId)
    {
        var q = db.AnotacoesCurso.AsNoTracking().Where(a => a.AlunoId == UserId);
        if (cursoId != null) q = q.Where(a => a.CursoId == cursoId.Value);
        return Ok(await q.Select(a => new AnotacaoDto(a.CursoId, a.Texto, a.AtualizadoEm)).ToListAsync());
    }

    /// <summary>Salva (cria ou substitui) o caderno do aluno para o curso — usado pelo autosave.</summary>
    [HttpPut("anotacoes/{cursoId:int}")]
    public async Task<IActionResult> SalvarAnotacao(int cursoId, AnotacaoRequest req)
    {
        if ((req.Texto ?? "").Length > 20000) return Erro(400, "As anotações podem ter até 20.000 caracteres.");
        if (!await db.Cursos.AnyAsync(c => c.Id == cursoId)) return Erro(404, "Curso não encontrado.");

        var a = await db.AnotacoesCurso.FirstOrDefaultAsync(x => x.AlunoId == UserId && x.CursoId == cursoId);
        if (a == null)
        {
            a = new AnotacaoCurso { AlunoId = UserId, CursoId = cursoId };
            db.AnotacoesCurso.Add(a);
        }
        a.Texto = req.Texto ?? "";
        a.AtualizadoEm = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok(new AnotacaoDto(a.CursoId, a.Texto, a.AtualizadoEm));
    }
}
