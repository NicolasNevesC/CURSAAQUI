using CursaAqui_API.Data;
using CursaAqui_API.Dtos;
using CursaAqui_API.Infra;
using CursaAqui_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Controllers;

[Route("api/atendimentos"), Authorize(Roles = "Administrador,Secretaria")]
public class AtendimentosController(CursaAquiDbContext db) : ApiBase
{
    private Task<AtendimentoDto?> DtoAsync(int id) =>
        db.Atendimentos.AsNoTracking().Where(a => a.Id == id).Select(Consultas.AtendimentoProj).FirstOrDefaultAsync();

    /// <summary>Lista usada por Registrar Atendimentos e pelo Relatório de Atendimentos do desktop.</summary>
    [HttpGet, Permissao("RegistrarAtendimentos", "GerarRelatoriosAdministrativos")]
    public async Task<IActionResult> Listar([FromQuery] StatusAtendimento? status, [FromQuery] TipoSolicitante? tipo,
        [FromQuery] string? busca, [FromQuery] DateTime? de, [FromQuery] DateTime? ate)
    {
        var q = db.Atendimentos.AsNoTracking();
        if (status != null) q = q.Where(a => a.Status == status.Value);
        if (tipo != null) q = q.Where(a => a.TipoSolicitante == tipo.Value);
        if (de != null) q = q.Where(a => a.DataAbertura >= de.Value.Date);
        if (ate != null) q = q.Where(a => a.DataAbertura < ate.Value.Date.AddDays(1));
        if (!string.IsNullOrWhiteSpace(busca))
        {
            var b = busca.Trim();
            q = q.Where(a => a.Protocolo.Contains(b) || a.SolicitanteNome.Contains(b) || a.Assunto.Contains(b));
        }
        return Ok(await q.OrderByDescending(a => a.DataAbertura).Select(Consultas.AtendimentoProj).ToListAsync());
    }

    [HttpGet("{id:int}"), Permissao("RegistrarAtendimentos", "GerarRelatoriosAdministrativos")]
    public async Task<IActionResult> Obter(int id)
    {
        var a = await DtoAsync(id);
        return a == null ? Erro(404, "Atendimento não encontrado.") : Ok(a);
    }

    /// <summary>Protocolo ATEND-{ano}-{sequencial}. Gerado no servidor para não repetir entre atendentes.</summary>
    private async Task<string> NovoProtocoloAsync()
    {
        var prefixo = $"ATEND-{DateTime.UtcNow.Year}-";
        var ultimo = await db.Atendimentos.Where(a => a.Protocolo.StartsWith(prefixo))
            .OrderByDescending(a => a.Protocolo.Length).ThenByDescending(a => a.Protocolo)
            .Select(a => a.Protocolo).FirstOrDefaultAsync();
        int n = ultimo != null && int.TryParse(ultimo[prefixo.Length..], out var x) ? x + 1 : 1;
        return $"{prefixo}{n:D3}";
    }

    [HttpPost, Permissao("RegistrarAtendimentos")]
    public async Task<IActionResult> Criar(AtendimentoCreateRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Assunto)) return Erro(400, "Informe o assunto.");
        if (string.IsNullOrWhiteSpace(req.Descricao)) return Erro(400, "Descreva o atendimento.");

        var nome = req.SolicitanteNome?.Trim();
        if (req.AlunoId != null)
        {
            var aluno = await db.Alunos.AsNoTracking().FirstOrDefaultAsync(a => a.Id == req.AlunoId);
            if (aluno == null) return Erro(404, "Aluno informado não encontrado.");
            if (string.IsNullOrWhiteSpace(nome)) nome = aluno.Nome;
        }
        if (string.IsNullOrWhiteSpace(nome)) return Erro(400, "Informe o nome do solicitante.");

        for (int tentativa = 0; tentativa < 3; tentativa++)
        {
            var at = new Atendimento
            {
                Protocolo = await NovoProtocoloAsync(), TipoSolicitante = req.TipoSolicitante, SolicitanteNome = nome,
                Contato = req.Contato?.Trim() ?? "", AlunoId = req.AlunoId, Assunto = req.Assunto.Trim(),
                Descricao = req.Descricao.Trim(), Providencia = req.Providencia?.Trim(), AtendenteId = UserId
            };
            db.Atendimentos.Add(at);
            try
            {
                await db.SaveChangesAsync();
                return Created($"/api/atendimentos/{at.Id}", await DtoAsync(at.Id));
            }
            catch (DbUpdateException)
            {
                db.Entry(at).State = EntityState.Detached; // protocolo repetido por concorrência: gera outro
            }
        }
        return Erro(409, "Não foi possível gerar o protocolo. Tente novamente.");
    }

    [HttpPut("{id:int}"), Permissao("RegistrarAtendimentos")]
    public async Task<IActionResult> Atualizar(int id, AtendimentoUpdateRequest req)
    {
        var a = await db.Atendimentos.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return Erro(404, "Atendimento não encontrado.");

        if (req.SolicitanteNome != null)
        {
            if (string.IsNullOrWhiteSpace(req.SolicitanteNome)) return Erro(400, "Informe o nome do solicitante.");
            a.SolicitanteNome = req.SolicitanteNome.Trim();
        }
        if (req.Assunto != null)
        {
            if (string.IsNullOrWhiteSpace(req.Assunto)) return Erro(400, "Informe o assunto.");
            a.Assunto = req.Assunto.Trim();
        }
        if (req.Descricao != null)
        {
            if (string.IsNullOrWhiteSpace(req.Descricao)) return Erro(400, "Descreva o atendimento.");
            a.Descricao = req.Descricao.Trim();
        }
        if (req.Contato != null) a.Contato = req.Contato.Trim();
        if (req.Providencia != null) a.Providencia = req.Providencia.Trim();
        await db.SaveChangesAsync();
        return Ok(await DtoAsync(id));
    }

    [HttpPatch("{id:int}/status"), Permissao("RegistrarAtendimentos")]
    public async Task<IActionResult> AlterarStatus(int id, AtendimentoStatusRequest req)
    {
        var a = await db.Atendimentos.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return Erro(404, "Atendimento não encontrado.");
        a.Status = req.Status;
        a.DataFechamento = req.Status is StatusAtendimento.Concluido or StatusAtendimento.Cancelado
            ? DateTime.UtcNow : null; // reabrir limpa a data de fechamento
        if (req.Providencia != null) a.Providencia = req.Providencia.Trim();
        await db.SaveChangesAsync();
        return Ok(await DtoAsync(id));
    }
}

[Route("api/relatorios")]
public class RelatoriosController(CursaAquiDbContext db) : ApiBase
{
    /// <summary>Cards e tabela da tela "Relatórios Gerenciais" do desktop.</summary>
    [HttpGet("gerencial"), Authorize(Roles = "Administrador"), Permissao("VisualizarRelatorios")]
    public async Task<IActionResult> Gerencial()
    {
        int totalAlunos = await db.Alunos.CountAsync();
        int cursosAtivos = await db.Cursos.CountAsync(c => c.Ativo);
        int totalMat = await db.Matriculas.CountAsync();
        int ativas = await db.Matriculas.CountAsync(m => m.Status == StatusMatricula.Ativa);
        int concluidas = await db.Matriculas.CountAsync(m => m.Status == StatusMatricula.Concluida);
        decimal taxa = totalMat > 0 ? Math.Round(concluidas * 100m / totalMat, 1) : 0;

        var linhas = await db.Cursos.AsNoTracking().Select(c => new
        {
            c.Id, c.Titulo,
            Total = c.Matriculas.Count,
            Ativas = c.Matriculas.Count(m => m.Status == StatusMatricula.Ativa),
            Concluidas = c.Matriculas.Count(m => m.Status == StatusMatricula.Concluida),
            Media = c.Matriculas.Average(m => m.NotaFinal)
        }).ToListAsync();

        var porCurso = linhas.OrderByDescending(l => l.Total).ThenBy(l => l.Titulo)
            .Select(l => new RelatorioCursoLinhaDto(l.Id, l.Titulo, l.Total, l.Ativas, l.Concluidas, Math.Round(l.Media ?? 0m, 2)))
            .ToList();

        return Ok(new RelatorioGerencialDto(totalAlunos, cursosAtivos, ativas, concluidas, taxa, porCurso));
    }

    [HttpGet("matriculados"), Authorize(Roles = "Administrador,Secretaria"), Permissao("GerarRelatoriosAdministrativos")]
    public async Task<IActionResult> Matriculados([FromQuery] int? cursoId, [FromQuery] string? periodo)
    {
        var q = db.Matriculas.AsNoTracking();
        if (cursoId != null) q = q.Where(m => m.CursoId == cursoId.Value);
        if (!string.IsNullOrWhiteSpace(periodo)) q = q.Where(m => m.PeriodoLetivo == periodo);
        return Ok(await q.OrderBy(m => m.Curso.Titulo).ThenBy(m => m.Aluno.Nome).Select(Consultas.MatriculaProj).ToListAsync());
    }

    [HttpGet("atendimentos"), Authorize(Roles = "Administrador,Secretaria"), Permissao("GerarRelatoriosAdministrativos")]
    public async Task<IActionResult> Atendimentos([FromQuery] DateTime? de, [FromQuery] DateTime? ate)
    {
        var q = db.Atendimentos.AsNoTracking();
        if (de != null) q = q.Where(a => a.DataAbertura >= de.Value.Date);
        if (ate != null) q = q.Where(a => a.DataAbertura < ate.Value.Date.AddDays(1));

        var itens = await q.OrderByDescending(a => a.DataAbertura).Select(Consultas.AtendimentoProj).ToListAsync();
        var porStatus = itens.GroupBy(i => i.Status).ToDictionary(g => g.Key.ToString(), g => g.Count());
        var porAtendente = itens.GroupBy(i => new { i.AtendenteId, i.AtendenteNome })
            .Select(g => new RelatorioAtendenteDto(g.Key.AtendenteId, g.Key.AtendenteNome, g.Count()))
            .OrderByDescending(x => x.Total).ToList();
        return Ok(new RelatorioAtendimentosDto(itens.Count, porStatus, porAtendente, itens));
    }

    /// <summary>Alunos em risco: matrícula Ativa com nota abaixo da média mínima, ou Trancada.</summary>
    [HttpGet("pendencias"), Authorize(Roles = "Administrador,Secretaria"), Permissao("GerarRelatoriosAdministrativos")]
    public async Task<IActionResult> Pendencias()
    {
        var media = (await db.ParametrosSistema.AsNoTracking().FirstAsync(p => p.Id == 1)).MediaAprovacaoMinima;
        var itens = await db.Matriculas.AsNoTracking()
            .Where(m => (m.Status == StatusMatricula.Ativa && m.NotaFinal != null && m.NotaFinal < media)
                        || m.Status == StatusMatricula.Trancada)
            .OrderBy(m => m.Aluno.Nome).Select(Consultas.MatriculaProj).ToListAsync();
        return Ok(new RelatorioPendenciasDto(media, itens.Count, itens));
    }
}

[Route("api/feedbacks")]
public class FeedbacksController(CursaAquiDbContext db) : ApiBase
{
    /// <summary>UML: suporte/feedback (dúvidas e reportar problemas) — qualquer usuário logado.</summary>
    [HttpPost, Authorize]
    public async Task<IActionResult> Enviar(FeedbackRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Assunto) || req.Assunto.Length > 200) return Erro(400, "Informe o assunto (até 200 caracteres).");
        if (string.IsNullOrWhiteSpace(req.Mensagem) || req.Mensagem.Length > 2000) return Erro(400, "Escreva a mensagem (até 2000 caracteres).");

        var f = new Feedback { UsuarioId = UserId, Tipo = req.Tipo, Assunto = req.Assunto.Trim(), Mensagem = req.Mensagem.Trim() };
        db.Feedbacks.Add(f);
        await db.SaveChangesAsync();
        return Created("/api/feedbacks/meus", await db.Feedbacks.AsNoTracking().Where(x => x.Id == f.Id).Select(Consultas.FeedbackProj).FirstAsync());
    }

    [HttpGet("meus"), Authorize]
    public async Task<IActionResult> Meus() =>
        Ok(await db.Feedbacks.AsNoTracking().Where(f => f.UsuarioId == UserId).OrderByDescending(f => f.DataEnvio)
            .Select(Consultas.FeedbackProj).ToListAsync());

    [HttpGet, Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Listar([FromQuery] StatusFeedback? status, [FromQuery] TipoFeedback? tipo)
    {
        var q = db.Feedbacks.AsNoTracking();
        if (status != null) q = q.Where(f => f.Status == status.Value);
        if (tipo != null) q = q.Where(f => f.Tipo == tipo.Value);
        return Ok(await q.OrderBy(f => f.Status).ThenByDescending(f => f.DataEnvio).Select(Consultas.FeedbackProj).ToListAsync());
    }

    [HttpPut("{id:int}/resposta"), Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Responder(int id, FeedbackRespostaRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Resposta) || req.Resposta.Length > 2000) return Erro(400, "Escreva a resposta (até 2000 caracteres).");
        var f = await db.Feedbacks.FirstOrDefaultAsync(x => x.Id == id);
        if (f == null) return Erro(404, "Feedback não encontrado.");

        f.Resposta = req.Resposta.Trim();
        f.Status = StatusFeedback.Respondido;
        f.RespondidoPorId = UserId;
        f.DataResposta = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok(await db.Feedbacks.AsNoTracking().Where(x => x.Id == id).Select(Consultas.FeedbackProj).FirstAsync());
    }
}
