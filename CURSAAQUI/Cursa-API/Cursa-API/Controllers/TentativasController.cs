using CursaAqui_API.Data;
using CursaAqui_API.Dtos;
using CursaAqui_API.Infra;
using CursaAqui_API.Models;
using CursaAqui_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Controllers;

[Route("api")]
public class TentativasController(CursaAquiDbContext db) : ApiBase
{
    // ================= ALUNO: ENVIAR PROVA =================

    /// <summary>
    /// Substitui o submitQuiz() do navegador: a API corrige, calcula nota, progresso, XP e nível.
    /// Cada envio é uma nova tentativa (refazer a prova = enviar de novo).
    /// </summary>
    [HttpPost("cursos/{cursoId:int}/tentativas"), Authorize(Roles = "Aluno")]
    public async Task<IActionResult> Enviar(int cursoId, TentativaRequest req)
    {
        var prova = await db.Provas.Include(p => p.Questoes).ThenInclude(q => q.Alternativas).AsSplitQuery()
            .FirstOrDefaultAsync(p => p.CursoId == cursoId && p.Tipo == TipoProva.Avaliacao);
        if (prova == null) return Erro(404, "Este curso ainda não tem avaliação.");

        var matricula = await db.Matriculas.Where(m => m.AlunoId == UserId && m.CursoId == cursoId && m.Status != StatusMatricula.Cancelada)
            .OrderByDescending(m => m.DataMatricula).FirstOrDefaultAsync();
        if (matricula == null) return Erro(409, "Inicie o curso antes de enviar a avaliação.");
        if (matricula.Status == StatusMatricula.Trancada) return Erro(409, "Esta matrícula está trancada.");

        var objetivas = prova.Questoes.Where(q => q.Tipo == TipoQuestao.Objetiva).ToList();
        var respostas = (req.Respostas ?? new()).GroupBy(r => r.QuestaoId).ToDictionary(g => g.Key, g => g.Last().AlternativaEscolhida);
        if (respostas.Keys.Any(k => objetivas.All(q => q.Id != k))) return Erro(400, "Há respostas para questões que não pertencem a esta prova.");

        var dissertativa = prova.Questoes.FirstOrDefault(q => q.Tipo == TipoQuestao.Dissertativa);
        var texto = req.RespostaDissertativa?.Trim();
        if (texto != null && texto.Length > 5000) return Erro(400, "A resposta dissertativa pode ter até 5000 caracteres.");
        if (!string.IsNullOrEmpty(texto) && dissertativa?.MinPalavras != null && Texto.Palavras(texto) < dissertativa.MinPalavras)
            return Erro(400, $"A resposta dissertativa precisa ter ao menos {dissertativa.MinPalavras} palavras.");

        var t = new Tentativa
        {
            AlunoId = UserId, ProvaId = prova.Id, TotalQuestoes = objetivas.Count,
            RespostaDissertativa = string.IsNullOrEmpty(texto) ? null : texto
        };
        int acertos = 0;
        foreach (var q in objetivas)
        {
            respostas.TryGetValue(q.Id, out var escolhida);
            if (escolhida != null && (escolhida < 0 || escolhida >= q.Alternativas.Count))
                return Erro(400, "Alternativa inválida informada.");
            bool correta = escolhida != null && escolhida == q.RespostaCorreta;
            if (correta) acertos++;
            t.Respostas.Add(new RespostaTentativa { QuestaoId = q.Id, AlternativaEscolhida = escolhida, Correta = correta });
        }
        t.Acertos = acertos;
        t.PercentualAcerto = objetivas.Count == 0 ? 0 : Math.Round(acertos * 100m / objetivas.Count, 2);
        t.NotaObtida = objetivas.Count == 0 ? 0 : Math.Round(acertos * prova.NotaMaxima / objetivas.Count, 2);
        t.Aprovada = objetivas.Count > 0 && t.NotaObtida >= prova.NotaMinima;

        var aluno = await db.Alunos.FirstAsync(a => a.Id == UserId);
        var prog = Regras.Atualizar(matricula, aluno, t);
        db.Tentativas.Add(t);
        await db.SaveChangesAsync();

        return Created($"/api/tentativas/{t.Id}", new TentativaResultadoDto(t.Id, prova.Id, cursoId, t.Acertos,
            t.TotalQuestoes, t.PercentualAcerto, t.NotaObtida, t.Aprovada, t.Status, t.DataRealizacao,
            matricula.ProgressoPercentual, prog.XpGanho, aluno.Xp, aluno.Nivel, prog.SubiuNivel, prog.ConcluiuAgora));
    }

    // ================= PROFESSOR: FILA DE CORREÇÃO =================

    private IQueryable<Tentativa> EscopoProfessor()
    {
        var q = db.Tentativas.AsNoTracking();
        if (User.IsInRole("Professor"))
        {
            var uid = UserId;
            q = q.Where(t => t.Prova.Curso.ProfessorId == uid);
        }
        return q;
    }

    [HttpGet("tentativas"), Authorize(Roles = "Administrador,Professor")]
    public async Task<IActionResult> Listar([FromQuery] StatusTentativa? status, [FromQuery] int? cursoId, [FromQuery] string? busca)
    {
        var q = EscopoProfessor();
        if (status != null) q = q.Where(t => t.Status == status.Value);
        if (cursoId != null) q = q.Where(t => t.Prova.CursoId == cursoId.Value);
        if (!string.IsNullOrWhiteSpace(busca))
        {
            var b = busca.Trim();
            q = q.Where(t => t.Aluno.Nome.Contains(b) || t.Aluno.Email.Contains(b) || t.Prova.Curso.Titulo.Contains(b));
        }
        return Ok(await q.OrderByDescending(t => t.DataRealizacao).Select(Consultas.TentativaProj).ToListAsync());
    }

    /// <summary>Detalhe da tentativa (respostas certas/erradas + dissertativa). Aluno só vê as próprias.</summary>
    [HttpGet("tentativas/{id:int}"), Authorize(Roles = "Aluno,Professor,Administrador")]
    public async Task<IActionResult> Detalhe(int id)
    {
        var t = await db.Tentativas.AsNoTracking().Include(x => x.Respostas)
            .Include(x => x.Prova).ThenInclude(p => p.Curso)
            .Include(x => x.Prova).ThenInclude(p => p.Questoes).ThenInclude(q => q.Alternativas)
            .AsSplitQuery().FirstOrDefaultAsync(x => x.Id == id);
        if (t == null) return Erro(404, "Tentativa não encontrada.");
        if (User.IsInRole("Aluno") && t.AlunoId != UserId) return Erro(404, "Tentativa não encontrada.");
        if (User.IsInRole("Professor") && t.Prova.Curso.ProfessorId != UserId) return Erro(403, "Esta tentativa não é de um curso seu.");

        var lista = await db.Tentativas.AsNoTracking().Where(x => x.Id == id).Select(Consultas.TentativaProj).FirstAsync();
        var respostas = t.Respostas.Select(r =>
        {
            var q = t.Prova.Questoes.First(x => x.Id == r.QuestaoId);
            return new RespostaDetalheDto(q.Id, q.Ordem, q.Enunciado,
                q.Alternativas.OrderBy(a => a.Ordem).Select(a => new AlternativaDto(a.Ordem, a.Texto)).ToList(),
                r.AlternativaEscolhida, q.RespostaCorreta, r.Correta);
        }).OrderBy(r => r.Ordem).ToList();

        var diss = t.Prova.Questoes.FirstOrDefault(q => q.Tipo == TipoQuestao.Dissertativa);
        return Ok(new TentativaDetalheDto(lista, t.RespostaDissertativa,
            diss == null || User.IsInRole("Aluno") ? null : ProvasController.ToProf(diss), respostas));
    }

    /// <summary>Parecer do professor (marca a tentativa como Corrigida).</summary>
    [HttpPut("tentativas/{id:int}/correcao"), Authorize(Roles = "Administrador,Professor")]
    public async Task<IActionResult> Corrigir(int id, CorrecaoRequest req)
    {
        var t = await db.Tentativas.Include(x => x.Prova).ThenInclude(p => p.Curso).FirstOrDefaultAsync(x => x.Id == id);
        if (t == null) return Erro(404, "Tentativa não encontrada.");
        if (!PodeGerir(t.Prova.Curso)) return Erro(403, "Esta tentativa não é de um curso seu.");
        if (req.Feedback != null && req.Feedback.Length > 2000) return Erro(400, "O parecer pode ter até 2000 caracteres.");

        t.Status = StatusTentativa.Corrigida;
        t.Feedback = string.IsNullOrWhiteSpace(req.Feedback) ? "Atividade avaliada pelo professor." : req.Feedback.Trim();
        t.ProfessorCorretorId = UserId;
        t.DataCorrecao = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok(await db.Tentativas.AsNoTracking().Where(x => x.Id == id).Select(Consultas.TentativaProj).FirstAsync());
    }

    // ================= TREINO RÁPIDO =================

    /// <summary>Questões aleatórias para treino (só de cursos já liberados pelo nível do aluno). Sem gabarito.</summary>
    [HttpGet("treino"), Authorize(Roles = "Aluno")]
    public async Task<IActionResult> Treino([FromQuery] int? cursoId, [FromQuery] int qtd = 5)
    {
        qtd = Math.Clamp(qtd, 1, 20);
        var aluno = await db.Alunos.AsNoTracking().FirstAsync(a => a.Id == UserId);
        var q = db.Questoes.AsNoTracking().Include(x => x.Alternativas)
            .Where(x => x.Tipo == TipoQuestao.Objetiva && x.Prova.Curso.Ativo && x.Prova.Curso.NivelMinimo <= aluno.Nivel);
        if (cursoId != null) q = q.Where(x => x.Prova.CursoId == cursoId.Value);

        var lista = await q.OrderBy(x => Guid.NewGuid()).Take(qtd).Select(x => new
        {
            x.Id, x.Prova.CursoId, x.Enunciado, Alternativas = x.Alternativas
        }).ToListAsync();

        return Ok(lista.Select(x => new TreinoQuestaoDto(x.Id, x.CursoId, x.Enunciado,
            x.Alternativas.OrderBy(a => a.Ordem).Select(a => new AlternativaDto(a.Ordem, a.Texto)).ToList())));
    }

    /// <summary>Confere a resposta do treino e dá +25 XP por acerto (mesma regra do site).</summary>
    [HttpPost("treino/responder"), Authorize(Roles = "Aluno")]
    public async Task<IActionResult> ResponderTreino(TreinoRespostaRequest req)
    {
        var q = await db.Questoes.Include(x => x.Alternativas).Include(x => x.Prova).ThenInclude(p => p.Curso)
            .FirstOrDefaultAsync(x => x.Id == req.QuestaoId && x.Tipo == TipoQuestao.Objetiva);
        if (q == null) return Erro(404, "Questão não encontrada.");

        var aluno = await db.Alunos.FirstAsync(a => a.Id == UserId);
        if (aluno.Nivel < q.Prova.Curso.NivelMinimo) return Erro(403, "Esta questão pertence a um curso ainda bloqueado.");
        if (req.AlternativaEscolhida < 0 || req.AlternativaEscolhida >= q.Alternativas.Count) return Erro(400, "Alternativa inválida.");

        bool correta = req.AlternativaEscolhida == q.RespostaCorreta;
        int xp = 0;
        bool subiu = false;
        if (correta)
        {
            xp = Regras.XpAcertoTreino;
            subiu = Regras.AdicionarXp(aluno, xp);
            await db.SaveChangesAsync();
        }
        return Ok(new TreinoRespostaDto(correta, q.RespostaCorreta ?? -1, xp, aluno.Xp, aluno.Nivel, subiu));
    }
}
