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
        int certificados = await db.Certificados.CountAsync(c => c.AlunoId == aluno.Id);
        int pendentes = await db.Tentativas.CountAsync(t => t.AlunoId == aluno.Id && t.Status == StatusTentativa.Pendente);
        bool pareceres = await db.Tentativas.AnyAsync(t => t.AlunoId == aluno.Id && t.Status == StatusTentativa.Corrigida);

        var conquistas = new List<ConquistaDto>
        {
            new(1, "🌱", "Primeiro Passo", "Iniciou o seu primeiro curso no portal.", mats.Count > 0),
            new(2, "⚡", "Foco Total", "Alcançou 50% de aproveitamento em um curso.", mats.Any(m => m.ProgressoPercentual >= 50)),
            new(3, "📖", "Leitor Voraz", "Acessou material e apostila em PDF.", mats.Any(m => m.MaterialAcessado)),
            new(4, "🎯", "Mestre do Quiz", "Acertou questões no Treino Rápido.", aluno.Xp >= 300),
            new(5, "🏆", "Primeiro Diploma", "Concluiu 100% de um curso oficial.", mats.Any(m => m.ProgressoPercentual >= 100)),
            new(6, "🚀", "Rumo ao Topo", "Alcançou o Nível 2 ou superior.", aluno.Nivel >= 2),
            new(7, "👑", "Especialista", "Acumulou mais de 500 XP.", aluno.Xp >= 500),
            new(8, "💬", "Aprovado!", "Recebeu parecer do professor.", pareceres)
        };

        return Ok(new DashboardAlunoDto(aluno.Nome, aluno.Xp, aluno.Nivel, Regras.XpParaProximoNivel(aluno.Nivel),
            mats.Count(m => m.Status is StatusMatricula.Ativa or StatusMatricula.Renovada),
            mats.Count(m => m.Status == StatusMatricula.Concluida), certificados, 0,
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
}