using CursaAqui_API.Data;
using CursaAqui_API.Dtos;
using CursaAqui_API.Infra;
using CursaAqui_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Controllers;

[Route("api")]
public class CursosController(CursaAquiDbContext db) : ApiBase
{
    public static DisciplinaDto ToDto(Disciplina d) => new(d.Id, d.CursoId, d.Nome, d.Conteudo, d.Ordem);
    public static MaterialDto ToDto(Material m) =>
        new(m.Id, m.CursoId, m.DisciplinaId, m.ProfessorId, m.Titulo, m.Tipo, m.UrlConteudo, m.DataPostagem);

    private bool Gestor => User.Identity?.IsAuthenticated == true && (EhStaff || User.IsInRole("Professor"));

    private async Task<Aluno?> AlunoLogadoAsync()
    {
        if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Aluno")) return null;
        return await db.Alunos.AsNoTracking().FirstOrDefaultAsync(a => a.Id == UserId);
    }

    // ================= CATÁLOGO =================

    [HttpGet("cursos"), AllowAnonymous]
    public async Task<IActionResult> Listar([FromQuery] string? busca, [FromQuery] string? categoria, [FromQuery] int? nivel,
        [FromQuery] int? cargaMin, [FromQuery] int? cargaMax, [FromQuery] bool? ativo, [FromQuery] int? professorId)
    {
        var q = db.Cursos.AsNoTracking().Include(c => c.Professor).AsQueryable();
        if (!Gestor) q = q.Where(c => c.Ativo);
        else if (ativo != null) q = q.Where(c => c.Ativo == ativo.Value);

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var b = busca.Trim();
            q = q.Where(c => c.Titulo.Contains(b) || c.Categoria.Contains(b) || c.Codigo.Contains(b)
                || (c.Area != null && c.Area.Contains(b)) || (c.Descricao != null && c.Descricao.Contains(b)));
        }
        if (!string.IsNullOrWhiteSpace(categoria)) q = q.Where(c => c.Categoria == categoria);
        if (nivel != null) q = q.Where(c => c.NivelMinimo == nivel.Value);
        if (cargaMin != null) q = q.Where(c => c.CargaHoraria >= cargaMin.Value);
        if (cargaMax != null) q = q.Where(c => c.CargaHoraria <= cargaMax.Value);
        if (professorId != null) q = q.Where(c => c.ProfessorId == professorId.Value);

        var cursos = await q.OrderBy(c => c.Id).ToListAsync();
        return Ok(await Consultas.MontarCursosAsync(db, cursos, await AlunoLogadoAsync()));
    }

    [HttpGet("cursos/categorias"), AllowAnonymous]
    public async Task<IActionResult> Categorias() =>
        Ok(await db.Cursos.AsNoTracking().Where(c => c.Ativo).Select(c => c.Categoria).Distinct().OrderBy(x => x).ToListAsync());

    [HttpGet("cursos/{id:int}"), AllowAnonymous]
    public async Task<IActionResult> Obter(int id)
    {
        var c = await db.Cursos.AsNoTracking().Include(x => x.Professor).FirstOrDefaultAsync(x => x.Id == id);
        if (c == null || (!c.Ativo && !Gestor)) return Erro(404, "Curso não encontrado.");

        var dto = (await Consultas.MontarCursosAsync(db, new List<Curso> { c }, await AlunoLogadoAsync()))[0];
        var disciplinas = (await db.Disciplinas.AsNoTracking().Where(d => d.CursoId == id).OrderBy(d => d.Ordem).ToListAsync())
            .Select(ToDto).ToList();
        var materiais = User.Identity?.IsAuthenticated == true
            ? (await db.Materiais.AsNoTracking().Where(m => m.CursoId == id).OrderBy(m => m.DataPostagem).ToListAsync()).Select(ToDto).ToList()
            : new List<MaterialDto>();
        return Ok(new CursoDetalheDto(dto, disciplinas, materiais));
    }

    // ================= CRUD (Admin / Professor dono) =================

    private async Task<string?> ValidarAsync(CursoRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Titulo)) return "O título é obrigatório.";
        if (string.IsNullOrWhiteSpace(r.Categoria)) return "A categoria é obrigatória.";
        if (r.CargaHoraria < 1 || r.CargaHoraria > 1000) return "A carga horária deve estar entre 1 e 1000 horas.";
        if (r.NivelMinimo < 1) return "O nível mínimo deve ser 1 ou maior.";
        if (r.VagasTotais != null && r.VagasTotais <= 0) return "O total de vagas deve ser maior que zero (ou nulo = sem limite).";
        if (!string.IsNullOrWhiteSpace(r.ImagemUrl) && !Texto.UrlSegura(r.ImagemUrl)) return "URL da imagem inválida.";
        if (r.ProfessorId != null && !await db.Professores.AnyAsync(p => p.Id == r.ProfessorId && p.Ativo))
            return "Professor informado não existe ou está inativo.";
        return null;
    }

    private async Task<string> ProximoCodigoAsync()
    {
        int n = await db.Cursos.CountAsync() + 1;
        string cod;
        do { cod = $"CUR-{n:D3}"; n++; } while (await db.Cursos.AnyAsync(c => c.Codigo == cod));
        return cod;
    }

    [HttpPost("cursos"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> Criar(CursoRequest req)
    {
        var erro = await ValidarAsync(req);
        if (erro != null) return Erro(400, erro);

        var codigo = string.IsNullOrWhiteSpace(req.Codigo) ? await ProximoCodigoAsync() : req.Codigo.Trim().ToUpperInvariant();
        if (await db.Cursos.AnyAsync(c => c.Codigo == codigo)) return Erro(409, "Já existe um curso com este código.");

        var par = await db.ParametrosSistema.AsNoTracking().FirstAsync(p => p.Id == 1);
        var curso = new Curso
        {
            Codigo = codigo, Titulo = req.Titulo.Trim(), Categoria = req.Categoria.Trim(), Area = req.Area?.Trim(),
            Descricao = req.Descricao?.Trim(), CargaHoraria = req.CargaHoraria, NivelMinimo = req.NivelMinimo,
            ImagemUrl = req.ImagemUrl?.Trim(), VagasTotais = req.VagasTotais,
            ProfessorId = User.IsInRole("Professor") ? UserId : req.ProfessorId,
            Ativo = req.Ativo ?? true
        };
        db.Cursos.Add(curso);
        await db.SaveChangesAsync();
        return await RetornarCursoAsync(curso.Id, created: true);
    }

    [HttpPut("cursos/{id:int}"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> Atualizar(int id, CursoRequest req)
    {
        var curso = await db.Cursos.FirstOrDefaultAsync(c => c.Id == id);
        if (curso == null) return Erro(404, "Curso não encontrado.");
        if (!PodeGerir(curso)) return Erro(403, "Você só pode editar os seus cursos.");

        var erro = await ValidarAsync(req);
        if (erro != null) return Erro(400, erro);

        if (!string.IsNullOrWhiteSpace(req.Codigo))
        {
            var codigo = req.Codigo.Trim().ToUpperInvariant();
            if (codigo != curso.Codigo)
            {
                if (await db.Cursos.AnyAsync(c => c.Codigo == codigo && c.Id != id)) return Erro(409, "Já existe um curso com este código.");
                curso.Codigo = codigo;
            }
        }
        curso.Titulo = req.Titulo.Trim();
        curso.Categoria = req.Categoria.Trim();
        curso.Area = req.Area?.Trim();
        curso.Descricao = req.Descricao?.Trim();
        curso.CargaHoraria = req.CargaHoraria;
        curso.NivelMinimo = req.NivelMinimo;
        curso.ImagemUrl = req.ImagemUrl?.Trim();
        curso.VagasTotais = req.VagasTotais;
        if (User.IsInRole("Administrador"))
        {
            curso.ProfessorId = req.ProfessorId;
            if (req.Ativo != null) curso.Ativo = req.Ativo.Value;
        }
        await db.SaveChangesAsync();
        return await RetornarCursoAsync(id, created: false);
    }

    [HttpPatch("cursos/{id:int}/ativo"), Authorize(Roles = "Administrador"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> AlterarAtivo(int id, AtivoRequest req)
    {
        var curso = await db.Cursos.FirstOrDefaultAsync(c => c.Id == id);
        if (curso == null) return Erro(404, "Curso não encontrado.");
        curso.Ativo = req.Ativo;
        await db.SaveChangesAsync();
        return await RetornarCursoAsync(id, created: false);
    }

    private async Task<IActionResult> RetornarCursoAsync(int id, bool created)
    {
        var c = await db.Cursos.AsNoTracking().Include(x => x.Professor).FirstAsync(x => x.Id == id);
        var dto = (await Consultas.MontarCursosAsync(db, new List<Curso> { c }, null))[0];
        return created ? Created($"/api/cursos/{id}", dto) : Ok(dto);
    }

    // ================= DISCIPLINAS (aulas / tópicos) =================

    [HttpGet("cursos/{cursoId:int}/disciplinas"), AllowAnonymous]
    public async Task<IActionResult> ListarDisciplinas(int cursoId) =>
        Ok((await db.Disciplinas.AsNoTracking().Where(d => d.CursoId == cursoId).OrderBy(d => d.Ordem).ToListAsync()).Select(ToDto));

    [HttpPost("cursos/{cursoId:int}/disciplinas"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> CriarDisciplina(int cursoId, DisciplinaRequest req)
    {
        var curso = await db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cursoId);
        if (curso == null) return Erro(404, "Curso não encontrado.");
        if (!PodeGerir(curso)) return Erro(403, "Você só pode editar os seus cursos.");
        if (string.IsNullOrWhiteSpace(req.Nome)) return Erro(400, "Informe o nome da disciplina.");

        int ordem = req.Ordem ?? ((await db.Disciplinas.Where(d => d.CursoId == cursoId).MaxAsync(d => (int?)d.Ordem)) ?? 0) + 1;
        var d = new Disciplina { CursoId = cursoId, Nome = req.Nome.Trim(), Conteudo = req.Conteudo?.Trim(), Ordem = ordem };
        db.Disciplinas.Add(d);
        await db.SaveChangesAsync();
        return Created($"/api/cursos/{cursoId}/disciplinas", ToDto(d));
    }

    [HttpPut("disciplinas/{id:int}"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> AtualizarDisciplina(int id, DisciplinaRequest req)
    {
        var d = await db.Disciplinas.Include(x => x.Curso).FirstOrDefaultAsync(x => x.Id == id);
        if (d == null) return Erro(404, "Disciplina não encontrada.");
        if (!PodeGerir(d.Curso)) return Erro(403, "Você só pode editar os seus cursos.");
        if (string.IsNullOrWhiteSpace(req.Nome)) return Erro(400, "Informe o nome da disciplina.");

        d.Nome = req.Nome.Trim();
        d.Conteudo = req.Conteudo?.Trim();
        if (req.Ordem != null) d.Ordem = req.Ordem.Value;
        await db.SaveChangesAsync();
        return Ok(ToDto(d));
    }

    [HttpDelete("disciplinas/{id:int}"), Authorize(Roles = "Administrador,Professor"), Permissao("GerenciarCursos")]
    public async Task<IActionResult> ExcluirDisciplina(int id)
    {
        var d = await db.Disciplinas.Include(x => x.Curso).FirstOrDefaultAsync(x => x.Id == id);
        if (d == null) return Erro(404, "Disciplina não encontrada.");
        if (!PodeGerir(d.Curso)) return Erro(403, "Você só pode editar os seus cursos.");
        if (await db.Materiais.AnyAsync(m => m.DisciplinaId == id) || await db.Provas.AnyAsync(p => p.DisciplinaId == id))
            return Erro(409, "Disciplina em uso por materiais ou provas. Remova-os antes.");
        db.Disciplinas.Remove(d);
        await db.SaveChangesAsync();
        return NoContent();
    }

    // ================= GLOSSÁRIO =================

    [HttpGet("glossario"), AllowAnonymous]
    public async Task<IActionResult> Glossario() =>
        Ok(await db.TermosGlossario.AsNoTracking().OrderBy(t => t.Termo)
            .Select(t => new TermoDto(t.Chave, t.Termo, t.Explicacao, t.Tecnica)).ToListAsync());
}
