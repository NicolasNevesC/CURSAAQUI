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
public class MatriculasController(CursaAquiDbContext db, MatriculaService svc) : ApiBase
{
    private Task<MatriculaDto?> DtoAsync(int id) =>
        db.Matriculas.AsNoTracking().Where(m => m.Id == id).Select(Consultas.MatriculaProj).FirstOrDefaultAsync();

    // ================= SECRETARIA =================

    [HttpGet("matriculas"), Authorize(Roles = "Administrador,Secretaria"), Permissao("GerenciarMatriculas")]
    public async Task<IActionResult> Listar([FromQuery] int? alunoId, [FromQuery] int? cursoId,
        [FromQuery] StatusMatricula? status, [FromQuery] string? periodo, [FromQuery] string? busca)
    {
        var q = db.Matriculas.AsNoTracking();
        if (alunoId != null) q = q.Where(m => m.AlunoId == alunoId.Value);
        if (cursoId != null) q = q.Where(m => m.CursoId == cursoId.Value);
        if (status != null) q = q.Where(m => m.Status == status.Value);
        if (!string.IsNullOrWhiteSpace(periodo)) q = q.Where(m => m.PeriodoLetivo == periodo);
        if (!string.IsNullOrWhiteSpace(busca))
        {
            var b = busca.Trim();
            var d = Texto.Digitos(b);
            q = q.Where(m => m.Aluno.Nome.Contains(b) || m.Curso.Titulo.Contains(b)
                || (d != "" && m.Aluno.Cpf != null && m.Aluno.Cpf.Contains(d)));
        }
        return Ok(await q.OrderByDescending(m => m.DataMatricula).Select(Consultas.MatriculaProj).ToListAsync());
    }

    [HttpGet("matriculas/{id:int}"), Authorize(Roles = "Administrador,Secretaria"), Permissao("GerenciarMatriculas")]
    public async Task<IActionResult> Obter(int id)
    {
        var m = await DtoAsync(id);
        return m == null ? Erro(404, "Matrícula não encontrada.") : Ok(m);
    }

    [HttpPost("matriculas"), Authorize(Roles = "Administrador,Secretaria"), Permissao("GerenciarMatriculas")]
    public async Task<IActionResult> Criar(MatriculaCreateRequest req)
    {
        var r = await svc.IniciarAsync(req.AlunoId, req.CursoId, req.PeriodoLetivo, ignorarNivel: true);
        if (r.Matricula == null) return Erro(r.Status, r.Erro!);
        if (!r.Criada) return Erro(409, "O aluno já está matriculado neste curso/período.");
        return Created($"/api/matriculas/{r.Matricula.Id}", await DtoAsync(r.Matricula.Id));
    }

    [HttpPatch("matriculas/{id:int}/status"), Authorize(Roles = "Administrador,Secretaria"), Permissao("GerenciarMatriculas")]
    public async Task<IActionResult> AlterarStatus(int id, MatriculaStatusRequest req)
    {
        var m = await db.Matriculas.FirstOrDefaultAsync(x => x.Id == id);
        if (m == null) return Erro(404, "Matrícula não encontrada.");
        m.Status = req.Status;
        if (req.Status == StatusMatricula.Concluida) m.DataConclusao ??= DateTime.UtcNow;
        if (req.Observacoes != null) m.Observacoes = req.Observacoes.Trim();
        await db.SaveChangesAsync();
        return Ok(await DtoAsync(id));
    }

    /// <summary>Lançamento de nota final e progresso (desktop: "Lançar Notas"). Professor só nos cursos dele.</summary>
    [HttpPatch("matriculas/{id:int}"), Authorize(Roles = "Administrador,Secretaria,Professor"), Permissao("GerenciarMatriculas")]
    public async Task<IActionResult> Lancar(int id, MatriculaLancamentoRequest req)
    {
        var m = await db.Matriculas.Include(x => x.Curso).FirstOrDefaultAsync(x => x.Id == id);
        if (m == null) return Erro(404, "Matrícula não encontrada.");
        if (User.IsInRole("Professor") && m.Curso.ProfessorId != UserId) return Erro(403, "Você só lança notas nos seus cursos.");
        if (req.NotaFinal != null && (req.NotaFinal < 0 || req.NotaFinal > 10)) return Erro(400, "A nota deve estar entre 0 e 10.");
        if (req.ProgressoPercentual != null && (req.ProgressoPercentual < 0 || req.ProgressoPercentual > 100))
            return Erro(400, "O progresso deve estar entre 0 e 100.");

        if (req.NotaFinal != null) m.NotaFinal = req.NotaFinal;
        if (req.ProgressoPercentual != null) m.ProgressoPercentual = req.ProgressoPercentual.Value;
        if (req.Observacoes != null) m.Observacoes = req.Observacoes.Trim();

        // Mesma regra do desktop: 100% + média mínima = Concluída
        var media = (await db.ParametrosSistema.AsNoTracking().FirstAsync(p => p.Id == 1)).MediaAprovacaoMinima;
        if (m.ProgressoPercentual >= 100 && m.NotaFinal != null && m.NotaFinal >= media)
        {
            m.Status = StatusMatricula.Concluida;
            m.DataConclusao ??= DateTime.UtcNow;
        }
        await db.SaveChangesAsync();
        return Ok(await DtoAsync(id));
    }

    // ================= ALUNO (site) =================

    /// <summary>Botão "Iniciar curso": cria a matrícula (valida nível mínimo, vagas e trava global).</summary>
    [HttpPost("cursos/{cursoId:int}/iniciar"), Authorize(Roles = "Aluno")]
    public async Task<IActionResult> Iniciar(int cursoId)
    {
        var r = await svc.IniciarAsync(UserId, cursoId, null, ignorarNivel: false);
        if (r.Matricula == null) return Erro(r.Status, r.Erro!);
        var dto = await DtoAsync(r.Matricula.Id);
        return r.Criada ? Created($"/api/matriculas/{r.Matricula.Id}", dto) : Ok(dto);
    }

    /// <summary>Marca material acessado / estudo concluído e recalcula o progresso.</summary>
    [HttpPut("matriculas/{id:int}/progresso"), Authorize(Roles = "Aluno")]
    public async Task<IActionResult> AtualizarProgresso(int id, ProgressoRequest req)
    {
        var m = await db.Matriculas.FirstOrDefaultAsync(x => x.Id == id && x.AlunoId == UserId);
        if (m == null) return Erro(404, "Matrícula não encontrada.");
        if (m.Status is StatusMatricula.Trancada or StatusMatricula.Cancelada)
            return Erro(409, "Esta matrícula está trancada ou cancelada.");

        if (req.MaterialAcessado != null) m.MaterialAcessado = req.MaterialAcessado.Value;
        if (req.EstudoConcluido != null) m.EstudoConcluido = req.EstudoConcluido.Value;
        if (req.ExerciciosLiberados != null) m.ExerciciosLiberados = req.ExerciciosLiberados.Value;

        var aluno = await db.Alunos.FirstAsync(a => a.Id == UserId);
        Regras.Atualizar(m, aluno, await db.UltimaTentativaAsync(aluno.Id, m.CursoId));
        await db.SaveChangesAsync();
        return Ok(await DtoAsync(id));
    }
}

[Route("api")]
public class CertificadosController(CursaAquiDbContext db) : ApiBase
{
    private static CertificadoDto ToDto(Certificado c, string alunoNome, string cursoTitulo) =>
        new(c.Id, c.CodigoValidacao, alunoNome, c.CursoId, cursoTitulo, c.CargaHoraria, c.DataEmissao, c.MatriculaId);

    /// <summary>Emite o certificado (ou devolve o já emitido). Só com curso concluído (100% + aprovado).</summary>
    [HttpPost("matriculas/{id:int}/certificado"), Authorize(Roles = "Aluno,Administrador,Secretaria")]
    public async Task<IActionResult> Emitir(int id)
    {
        var m = await db.Matriculas.Include(x => x.Aluno).Include(x => x.Curso).Include(x => x.Certificado)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (m == null || (User.IsInRole("Aluno") && m.AlunoId != UserId)) return Erro(404, "Matrícula não encontrada.");

        if (m.Certificado != null) return Ok(ToDto(m.Certificado, m.Aluno.Nome, m.Curso.Titulo));
        if (m.Status != StatusMatricula.Concluida || m.ProgressoPercentual < 100)
            return Erro(409, "Conclua 100% do curso e seja aprovado na avaliação para liberar o certificado.");

        var cert = new Certificado
        {
            MatriculaId = m.Id, AlunoId = m.AlunoId, CursoId = m.CursoId, CargaHoraria = m.Curso.CargaHoraria
        };
        db.Certificados.Add(cert);
        await db.SaveChangesAsync();
        return Created($"/api/certificados/{cert.CodigoValidacao}", ToDto(cert, m.Aluno.Nome, m.Curso.Titulo));
    }

    /// <summary>Validação pública do certificado pelo código.</summary>
    [HttpGet("certificados/{codigo}"), AllowAnonymous]
    public async Task<IActionResult> Validar(string codigo)
    {
        var c = await db.Certificados.AsNoTracking().Include(x => x.Aluno).Include(x => x.Curso)
            .FirstOrDefaultAsync(x => x.CodigoValidacao == codigo);
        return c == null
            ? Erro(404, "Certificado não encontrado. Verifique o código informado.")
            : Ok(ToDto(c, c.Aluno.Nome, c.Curso.Titulo));
    }
}
