using CursaAqui_API.Data;
using CursaAqui_API.Dtos;
using CursaAqui_API.Infra;
using CursaAqui_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Controllers;

[Route("api")]
public class MateriaisController(CursaAquiDbContext db, IConfiguration cfg, IWebHostEnvironment env) : ApiBase
{
    private const string PrefixoUrl = "/arquivos/materiais/";
    private static readonly string[] ExtensoesPermitidas = { ".pdf", ".png", ".jpg", ".jpeg", ".mp4", ".docx", ".pptx", ".xlsx" };

    [HttpGet("cursos/{cursoId:int}/materiais"), Authorize]
    public async Task<IActionResult> Listar(int cursoId) =>
        Ok((await db.Materiais.AsNoTracking().Where(m => m.CursoId == cursoId).OrderBy(m => m.DataPostagem).ToListAsync())
            .Select(CursosController.ToDto));

    private async Task<(Curso? Curso, IActionResult? Erro)> CursoGerivelAsync(int cursoId)
    {
        var curso = await db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cursoId);
        if (curso == null) return (null, Erro(404, "Curso não encontrado."));
        if (!PodeGerir(curso)) return (null, Erro(403, "Você só pode postar materiais nos seus cursos."));
        return (curso, null);
    }

    [HttpPost("cursos/{cursoId:int}/materiais"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> Criar(int cursoId, MaterialRequest req)
    {
        var (curso, erro) = await CursoGerivelAsync(cursoId);
        if (erro != null) return erro;
        if (string.IsNullOrWhiteSpace(req.Titulo)) return Erro(400, "Informe o título do material.");
        if (!Texto.UrlSegura(req.UrlConteudo)) return Erro(400, "URL do conteúdo inválida (use http/https ou caminho relativo).");
        if (req.DisciplinaId != null && !await db.Disciplinas.AnyAsync(d => d.Id == req.DisciplinaId && d.CursoId == cursoId))
            return Erro(400, "A disciplina informada não pertence a este curso.");

        var m = new Material
        {
            CursoId = cursoId, DisciplinaId = req.DisciplinaId,
            ProfessorId = User.IsInRole("Professor") ? UserId : curso!.ProfessorId,
            Titulo = req.Titulo.Trim(), Tipo = req.Tipo, UrlConteudo = req.UrlConteudo.Trim()
        };
        db.Materiais.Add(m);
        await db.SaveChangesAsync();
        return Created($"/api/cursos/{cursoId}/materiais", CursosController.ToDto(m));
    }

    /// <summary>Upload de arquivo (multipart/form-data: arquivo, titulo, disciplinaId).</summary>
    [HttpPost("cursos/{cursoId:int}/materiais/upload"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    [RequestSizeLimit(25_000_000)]
    public async Task<IActionResult> Upload(int cursoId, [FromForm] UploadMaterialForm form)
    {
        var (curso, erro) = await CursoGerivelAsync(cursoId);
        if (erro != null) return erro;
        if (form.Arquivo == null || form.Arquivo.Length == 0) return Erro(400, "Envie um arquivo.");
        if (form.Arquivo.Length > 20_000_000) return Erro(400, "O arquivo excede 20 MB.");

        var ext = Path.GetExtension(form.Arquivo.FileName).ToLowerInvariant();
        if (!ExtensoesPermitidas.Contains(ext)) return Erro(400, $"Extensão não permitida. Use: {string.Join(", ", ExtensoesPermitidas)}.");
        if (form.DisciplinaId != null && !await db.Disciplinas.AnyAsync(d => d.Id == form.DisciplinaId && d.CursoId == cursoId))
            return Erro(400, "A disciplina informada não pertence a este curso.");

        var pasta = Path.Combine(Arquivos.Pasta(cfg, env), "materiais");
        Directory.CreateDirectory(pasta);
        var nomeArquivo = $"{Guid.NewGuid():N}{ext}";
        await using (var fs = System.IO.File.Create(Path.Combine(pasta, nomeArquivo)))
            await form.Arquivo.CopyToAsync(fs);

        var m = new Material
        {
            CursoId = cursoId, DisciplinaId = form.DisciplinaId,
            ProfessorId = User.IsInRole("Professor") ? UserId : curso!.ProfessorId,
            Titulo = string.IsNullOrWhiteSpace(form.Titulo) ? Path.GetFileNameWithoutExtension(form.Arquivo.FileName) : form.Titulo.Trim(),
            Tipo = ext == ".pdf" ? TipoMaterial.Pdf : ext == ".mp4" ? TipoMaterial.Video : TipoMaterial.Outro,
            UrlConteudo = PrefixoUrl + nomeArquivo
        };
        db.Materiais.Add(m);
        await db.SaveChangesAsync();
        return Created($"/api/cursos/{cursoId}/materiais", CursosController.ToDto(m));
    }

    [HttpPut("materiais/{id:int}"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> Atualizar(int id, MaterialRequest req)
    {
        var m = await db.Materiais.Include(x => x.Curso).FirstOrDefaultAsync(x => x.Id == id);
        if (m == null) return Erro(404, "Material não encontrado.");
        if (!PodeGerir(m.Curso)) return Erro(403, "Você só pode editar materiais dos seus cursos.");
        if (string.IsNullOrWhiteSpace(req.Titulo)) return Erro(400, "Informe o título do material.");
        if (!Texto.UrlSegura(req.UrlConteudo)) return Erro(400, "URL do conteúdo inválida.");
        if (req.DisciplinaId != null && !await db.Disciplinas.AnyAsync(d => d.Id == req.DisciplinaId && d.CursoId == m.CursoId))
            return Erro(400, "A disciplina informada não pertence a este curso.");

        m.Titulo = req.Titulo.Trim();
        m.Tipo = req.Tipo;
        m.UrlConteudo = req.UrlConteudo.Trim();
        m.DisciplinaId = req.DisciplinaId;
        await db.SaveChangesAsync();
        return Ok(CursosController.ToDto(m));
    }

    [HttpDelete("materiais/{id:int}"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> Excluir(int id)
    {
        var m = await db.Materiais.Include(x => x.Curso).FirstOrDefaultAsync(x => x.Id == id);
        if (m == null) return Erro(404, "Material não encontrado.");
        if (!PodeGerir(m.Curso)) return Erro(403, "Você só pode excluir materiais dos seus cursos.");

        if (m.UrlConteudo.StartsWith(PrefixoUrl))
        {
            var caminho = Path.Combine(Arquivos.Pasta(cfg, env), "materiais", Path.GetFileName(m.UrlConteudo));
            if (System.IO.File.Exists(caminho)) System.IO.File.Delete(caminho);
        }
        db.Materiais.Remove(m);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

[Route("api/cursos/{cursoId:int}/avaliacoes")]
public class AvaliacoesCursoController(CursaAquiDbContext db) : ApiBase
{
    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> Listar(int cursoId) =>
        Ok(await db.AvaliacoesCurso.AsNoTracking().Where(a => a.CursoId == cursoId).OrderByDescending(a => a.Data)
            .Select(a => new AvaliacaoCursoDto(a.Id, a.AlunoId, a.Aluno.Nome, a.Nota, a.Comentario, a.Data)).ToListAsync());

    /// <summary>Cria ou atualiza a avaliação do aluno (1 por curso) e recalcula a média do curso.</summary>
    [HttpPost, Authorize(Roles = "Aluno")]
    public async Task<IActionResult> Avaliar(int cursoId, AvaliacaoCursoRequest req)
    {
        if (req.Nota < 1 || req.Nota > 5) return Erro(400, "A nota deve ser de 1 a 5.");
        if (req.Comentario != null && req.Comentario.Length > 1000) return Erro(400, "O comentário pode ter até 1000 caracteres.");
        var curso = await db.Cursos.FirstOrDefaultAsync(c => c.Id == cursoId);
        if (curso == null) return Erro(404, "Curso não encontrado.");

        var av = await db.AvaliacoesCurso.FirstOrDefaultAsync(a => a.CursoId == cursoId && a.AlunoId == UserId);
        if (av == null)
        {
            av = new AvaliacaoCurso { CursoId = cursoId, AlunoId = UserId };
            db.AvaliacoesCurso.Add(av);
        }
        av.Nota = req.Nota;
        av.Comentario = req.Comentario?.Trim();
        av.Data = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var media = await db.AvaliacoesCurso.Where(a => a.CursoId == cursoId).AverageAsync(a => (double)a.Nota);
        curso.Avaliacao = Math.Round((decimal)media, 2);
        await db.SaveChangesAsync();

        var aluno = await db.Alunos.AsNoTracking().FirstAsync(a => a.Id == UserId);
        return Ok(new AvaliacaoCursoDto(av.Id, av.AlunoId, aluno.Nome, av.Nota, av.Comentario, av.Data));
    }
}
