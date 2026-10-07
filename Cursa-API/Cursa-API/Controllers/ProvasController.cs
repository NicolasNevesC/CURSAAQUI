using CursaAqui_API.Data;
using CursaAqui_API.Dtos;
using CursaAqui_API.Infra;
using CursaAqui_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Controllers;

/// <summary>Montagem de provas e questões (professor/admin) + a prova vista pelo aluno (sem gabarito).</summary>
[Route("api")]
public class ProvasController(CursaAquiDbContext db) : ApiBase
{
    public static QuestaoProfDto ToProf(Questao q) => new(q.Id, q.ProvaId, q.Tipo, q.Ordem, q.Enunciado, q.RespostaCorreta,
        q.Alternativas.OrderBy(a => a.Ordem).Select(a => new AlternativaDto(a.Ordem, a.Texto)).ToList(),
        q.Orientacoes, q.PalavrasChave, q.MinPalavras);

    private static ProvaDto ToDto(Prova p, int total) => new(p.Id, p.CursoId, p.DisciplinaId, p.Titulo, p.Tipo,
        p.NotaMaxima, p.NotaMinima, p.DataCriacao, total);

    private static string? ValidarProva(ProvaRequest r, out decimal max, out decimal min)
    {
        max = r.NotaMaxima ?? 10m;
        min = r.NotaMinima ?? 6m;
        if (string.IsNullOrWhiteSpace(r.Titulo)) return "Informe o título da prova.";
        if (max <= 0 || max > 100) return "A nota máxima deve ser maior que zero.";
        if (min < 0 || min > max) return "A nota mínima deve estar entre 0 e a nota máxima.";
        return null;
    }

    private static string? ValidarQuestao(QuestaoRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Enunciado)) return "Informe o enunciado.";
        if (r.Tipo == TipoQuestao.Objetiva)
        {
            var alts = r.Alternativas;
            if (alts == null || alts.Count < 2 || alts.Count > 6) return "Informe de 2 a 6 alternativas.";
            if (alts.Any(string.IsNullOrWhiteSpace)) return "As alternativas não podem ficar em branco.";
            if (r.RespostaCorreta == null || r.RespostaCorreta < 0 || r.RespostaCorreta >= alts.Count)
                return "Indique qual alternativa é a correta.";
        }
        else if (r.MinPalavras != null && r.MinPalavras < 0) return "O mínimo de palavras não pode ser negativo.";
        return null;
    }

    // ================= PROFESSOR / ADMIN =================

    [HttpGet("cursos/{cursoId:int}/provas"), Authorize(Roles = "Administrador,Professor")]
    public async Task<IActionResult> Listar(int cursoId)
    {
        var curso = await db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cursoId);
        if (curso == null) return Erro(404, "Curso não encontrado.");
        if (!PodeGerir(curso)) return Erro(403, "Você só acessa as provas dos seus cursos.");
        return Ok(await db.Provas.AsNoTracking().Where(p => p.CursoId == cursoId).OrderBy(p => p.Id)
            .Select(p => new ProvaDto(p.Id, p.CursoId, p.DisciplinaId, p.Titulo, p.Tipo, p.NotaMaxima, p.NotaMinima,
                p.DataCriacao, p.Questoes.Count)).ToListAsync());
    }

    [HttpPost("cursos/{cursoId:int}/provas"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> Criar(int cursoId, ProvaRequest req)
    {
        var curso = await db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cursoId);
        if (curso == null) return Erro(404, "Curso não encontrado.");
        if (!PodeGerir(curso)) return Erro(403, "Você só cria provas nos seus cursos.");
        var erro = ValidarProva(req, out var max, out var min);
        if (erro != null) return Erro(400, erro);

        var tipo = req.Tipo ?? TipoProva.Avaliacao;
        if (tipo == TipoProva.Avaliacao && await db.Provas.AnyAsync(p => p.CursoId == cursoId && p.Tipo == TipoProva.Avaliacao))
            return Erro(409, "Este curso já tem uma prova de avaliação. Edite a existente.");
        if (req.DisciplinaId != null && !await db.Disciplinas.AnyAsync(d => d.Id == req.DisciplinaId && d.CursoId == cursoId))
            return Erro(400, "A disciplina informada não pertence a este curso.");

        var prova = new Prova
        {
            CursoId = cursoId, DisciplinaId = req.DisciplinaId, Titulo = req.Titulo.Trim(), Tipo = tipo,
            NotaMaxima = max, NotaMinima = min
        };
        db.Provas.Add(prova);
        await db.SaveChangesAsync();
        return Created($"/api/provas/{prova.Id}", ToDto(prova, 0));
    }

    /// <summary>Prova COM gabarito (para o professor montar/revisar).</summary>
    [HttpGet("provas/{id:int}"), Authorize(Roles = "Administrador,Professor")]
    public async Task<IActionResult> Obter(int id)
    {
        var p = await db.Provas.AsNoTracking().Include(x => x.Curso)
            .Include(x => x.Questoes).ThenInclude(q => q.Alternativas)
            .AsSplitQuery().FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return Erro(404, "Prova não encontrada.");
        if (!PodeGerir(p.Curso)) return Erro(403, "Você só acessa as provas dos seus cursos.");
        return Ok(new ProvaProfDto(ToDto(p, p.Questoes.Count), p.Questoes.OrderBy(q => q.Ordem).Select(ToProf).ToList()));
    }

    [HttpPut("provas/{id:int}"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> Atualizar(int id, ProvaRequest req)
    {
        var p = await db.Provas.Include(x => x.Curso).FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return Erro(404, "Prova não encontrada.");
        if (!PodeGerir(p.Curso)) return Erro(403, "Você só edita provas dos seus cursos.");
        var erro = ValidarProva(req, out var max, out var min);
        if (erro != null) return Erro(400, erro);
        if (req.DisciplinaId != null && !await db.Disciplinas.AnyAsync(d => d.Id == req.DisciplinaId && d.CursoId == p.CursoId))
            return Erro(400, "A disciplina informada não pertence a este curso.");

        p.Titulo = req.Titulo.Trim();
        p.DisciplinaId = req.DisciplinaId;
        p.NotaMaxima = max;
        p.NotaMinima = min;
        await db.SaveChangesAsync();
        return Ok(ToDto(p, await db.Questoes.CountAsync(q => q.ProvaId == id)));
    }

    [HttpDelete("provas/{id:int}"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> Excluir(int id)
    {
        var p = await db.Provas.Include(x => x.Curso).FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return Erro(404, "Prova não encontrada.");
        if (!PodeGerir(p.Curso)) return Erro(403, "Você só exclui provas dos seus cursos.");
        if (await db.Tentativas.AnyAsync(t => t.ProvaId == id)) return Erro(409, "Esta prova já tem tentativas de alunos e não pode ser excluída.");
        db.Provas.Remove(p);
        await db.SaveChangesAsync();
        return NoContent();
    }

    // ---------- Questões ----------

    [HttpPost("provas/{provaId:int}/questoes"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> CriarQuestao(int provaId, QuestaoRequest req)
    {
        var p = await db.Provas.Include(x => x.Curso).FirstOrDefaultAsync(x => x.Id == provaId);
        if (p == null) return Erro(404, "Prova não encontrada.");
        if (!PodeGerir(p.Curso)) return Erro(403, "Você só edita provas dos seus cursos.");
        var erro = ValidarQuestao(req);
        if (erro != null) return Erro(400, erro);

        int ordem = req.Ordem ?? ((await db.Questoes.Where(x => x.ProvaId == provaId).MaxAsync(x => (int?)x.Ordem)) ?? 0) + 1;
        var q = new Questao { ProvaId = provaId, Tipo = req.Tipo, Ordem = ordem, Enunciado = Texto.Sanitizar(req.Enunciado.Trim()) };
        Preencher(q, req);
        db.Questoes.Add(q);
        await db.SaveChangesAsync();
        return Created($"/api/provas/{provaId}", ToProf(q));
    }

    [HttpPut("questoes/{id:int}"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> AtualizarQuestao(int id, QuestaoRequest req)
    {
        var q = await db.Questoes.Include(x => x.Alternativas).Include(x => x.Prova).ThenInclude(p => p.Curso)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (q == null) return Erro(404, "Questão não encontrada.");
        if (!PodeGerir(q.Prova.Curso)) return Erro(403, "Você só edita questões dos seus cursos.");
        var erro = ValidarQuestao(req);
        if (erro != null) return Erro(400, erro);

        q.Tipo = req.Tipo;
        q.Enunciado = Texto.Sanitizar(req.Enunciado.Trim());
        if (req.Ordem != null) q.Ordem = req.Ordem.Value;
        Preencher(q, req);
        await db.SaveChangesAsync();
        return Ok(ToProf(q));
    }

    [HttpDelete("questoes/{id:int}"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> ExcluirQuestao(int id)
    {
        var q = await db.Questoes.Include(x => x.Prova).ThenInclude(p => p.Curso).FirstOrDefaultAsync(x => x.Id == id);
        if (q == null) return Erro(404, "Questão não encontrada.");
        if (!PodeGerir(q.Prova.Curso)) return Erro(403, "Você só exclui questões dos seus cursos.");
        if (await db.RespostasTentativa.AnyAsync(r => r.QuestaoId == id))
            return Erro(409, "Esta questão já foi respondida por alunos e não pode ser excluída.");
        db.Questoes.Remove(q);
        await db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Copia os campos do request para a questão, atualizando as alternativas "no lugar" (sem violar o índice único).</summary>
    private void Preencher(Questao q, QuestaoRequest req)
    {
        var existentes = q.Alternativas.OrderBy(a => a.Ordem).ToList();
        if (req.Tipo == TipoQuestao.Objetiva)
        {
            var novas = req.Alternativas!.Select(a => a.Trim()).ToList();
            for (int i = 0; i < novas.Count; i++)
            {
                if (i < existentes.Count) existentes[i].Texto = novas[i];
                else q.Alternativas.Add(new Alternativa { Ordem = i, Texto = novas[i] });
            }
            for (int i = novas.Count; i < existentes.Count; i++) db.Alternativas.Remove(existentes[i]);
            q.RespostaCorreta = req.RespostaCorreta;
            q.Orientacoes = null; q.PalavrasChave = null; q.MinPalavras = null;
        }
        else
        {
            foreach (var a in existentes) db.Alternativas.Remove(a);
            q.RespostaCorreta = null;
            q.Orientacoes = req.Orientacoes?.Trim();
            q.PalavrasChave = req.PalavrasChave?.Trim();
            q.MinPalavras = req.MinPalavras;
        }
    }

    // ================= ALUNO =================

    /// <summary>Prova do curso para o aluno responder: NUNCA inclui o gabarito nem as palavras-chave.</summary>
    [HttpGet("cursos/{cursoId:int}/prova"), Authorize(Roles = "Aluno")]
    public async Task<IActionResult> ProvaDoAluno(int cursoId)
    {
        if (!await db.Matriculas.AnyAsync(m => m.AlunoId == UserId && m.CursoId == cursoId && m.Status != StatusMatricula.Cancelada))
            return Erro(409, "Inicie o curso para acessar a avaliação.");

        var p = await db.Provas.AsNoTracking().Include(x => x.Questoes).ThenInclude(q => q.Alternativas)
            .AsSplitQuery().FirstOrDefaultAsync(x => x.CursoId == cursoId && x.Tipo == TipoProva.Avaliacao);
        if (p == null) return Erro(404, "Este curso ainda não tem avaliação.");

        var questoes = p.Questoes.OrderBy(q => q.Ordem).Select(q => new QuestaoAlunoDto(q.Id, q.Tipo, q.Ordem, q.Enunciado,
            q.Alternativas.OrderBy(a => a.Ordem).Select(a => new AlternativaDto(a.Ordem, a.Texto)).ToList(),
            q.Orientacoes, q.MinPalavras)).ToList();
        return Ok(new ProvaAlunoDto(p.Id, p.CursoId, p.Titulo, p.NotaMaxima, p.NotaMinima, questoes));
    }
}
