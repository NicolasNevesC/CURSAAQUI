using CursaAqui_API.Data;
using CursaAqui_API.Dtos;
using CursaAqui_API.Infra;
using CursaAqui_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Controllers;

[Route("api/alunos")]
public class AlunosController(CursaAquiDbContext db) : ApiBase
{
    /// <summary>Staff vê todos; Professor só os alunos matriculados nos cursos dele.</summary>
    private IQueryable<Aluno> Escopo()
    {
        var q = db.Alunos.AsNoTracking();
        if (User.IsInRole("Professor"))
        {
            var uid = UserId;
            q = q.Where(a => a.Matriculas.Any(m => m.Curso.ProfessorId == uid));
        }
        return q;
    }

    /// <summary>Lista usada por Consultar Cadastro, Gerenciar Matrículas e pelos relatórios do desktop.</summary>
    [HttpGet, Authorize(Roles = "Administrador,Secretaria,Professor"), Permissao("ConsultarCadastro", "GerenciarMatriculas", "VisualizarRelatorios", "GerarRelatoriosAdministrativos")]
    public async Task<IActionResult> Listar([FromQuery] string? busca, [FromQuery] bool? ativo)
    {
        var q = Escopo();
        if (ativo != null) q = q.Where(a => a.Ativo == ativo.Value);
        if (!string.IsNullOrWhiteSpace(busca))
        {
            var b = busca.Trim();
            var d = Texto.Digitos(b);
            q = q.Where(a => a.Nome.Contains(b) || a.Email.Contains(b) || (d != "" && a.Cpf != null && a.Cpf.Contains(d)));
        }
        var lista = await q.OrderBy(a => a.Nome)
            .Select(a => new AlunoResumoDto(a.Id, a.Nome, a.Email, a.Cpf, a.Telefone, a.Ativo, a.Xp, a.Nivel, a.DataCadastro, a.DataNascimento, a.Endereco))
            .ToListAsync();
        return Ok(lista);
    }

    [HttpGet("{id:int}"), Authorize(Roles = "Administrador,Secretaria,Professor"), Permissao("ConsultarCadastro")]
    public async Task<IActionResult> Obter(int id)
    {
        var a = await Escopo().FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return Erro(404, "Aluno não encontrado.");
        return Ok(new AlunoFichaDto(UsuarioMapper.ToDto(a), await Consultas.HistoricoAsync(db, id)));
    }

    [HttpGet("{id:int}/historico"), Authorize(Roles = "Administrador,Secretaria,Professor"), Permissao("ConsultarCadastro")]
    public async Task<IActionResult> Historico(int id)
    {
        if (!await Escopo().AnyAsync(x => x.Id == id)) return Erro(404, "Aluno não encontrado.");
        return Ok(await Consultas.HistoricoAsync(db, id));
    }

    /// <summary>Cadastro presencial feito pela Secretaria.</summary>
    [HttpPost, Authorize(Roles = "Administrador,Secretaria"), Permissao("GerenciarMatriculas")]
    public async Task<IActionResult> Criar(AlunoCreateRequest req)
    {
        var erro = Validacoes.NovoUsuario(req.Nome, req.Email, req.Senha, req.Cpf);
        if (erro != null) return Erro(400, erro);
        var email = Texto.Email(req.Email);
        var cpf = Texto.CpfNormalizado(req.Cpf);
        if (await db.Usuarios.AnyAsync(x => x.Email == email)) return Erro(409, "Este e-mail já está cadastrado.");
        if (cpf != null && await db.Usuarios.AnyAsync(x => x.Cpf == cpf)) return Erro(409, "Este CPF já está cadastrado.");

        var aluno = new Aluno
        {
            Nome = req.Nome.Trim(), Email = email, SenhaHash = BCrypt.Net.BCrypt.HashPassword(req.Senha), Cpf = cpf,
            Telefone = req.Telefone?.Trim(), DataNascimento = req.DataNascimento, Endereco = req.Endereco?.Trim()
        };
        db.Alunos.Add(aluno);
        await db.SaveChangesAsync();
        return Created($"/api/alunos/{aluno.Id}", UsuarioMapper.ToDto(aluno));
    }

    [HttpPut("{id:int}"), Authorize(Roles = "Administrador,Secretaria"), Permissao("GerenciarMatriculas")]
    public async Task<IActionResult> Atualizar(int id, AlunoUpdateRequest req)
    {
        var a = await db.Alunos.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return Erro(404, "Aluno não encontrado.");

        if (req.Nome != null)
        {
            if (string.IsNullOrWhiteSpace(req.Nome)) return Erro(400, "Informe o nome.");
            a.Nome = req.Nome.Trim();
        }
        if (req.Email != null)
        {
            var email = Texto.Email(req.Email);
            if (!Texto.EmailValido(email)) return Erro(400, "Informe um e-mail válido.");
            if (await db.Usuarios.AnyAsync(x => x.Email == email && x.Id != id)) return Erro(409, "Este e-mail já está cadastrado.");
            a.Email = email;
        }
        if (req.Cpf != null)
        {
            var erro = Validacoes.Cpf(req.Cpf);
            if (erro != null) return Erro(400, erro);
            var cpf = Texto.CpfNormalizado(req.Cpf);
            if (cpf != null && await db.Usuarios.AnyAsync(x => x.Cpf == cpf && x.Id != id)) return Erro(409, "Este CPF já está cadastrado.");
            a.Cpf = cpf;
        }
        if (req.Telefone != null) a.Telefone = req.Telefone.Trim();
        if (req.DataNascimento != null) a.DataNascimento = req.DataNascimento;
        if (req.Endereco != null) a.Endereco = req.Endereco.Trim();

        await db.SaveChangesAsync();
        return Ok(UsuarioMapper.ToDto(a));
    }

    [HttpPatch("{id:int}/ativo"), Authorize(Roles = "Administrador,Secretaria"), Permissao("GerenciarMatriculas")]
    public async Task<IActionResult> AlterarAtivo(int id, AtivoRequest req)
    {
        var a = await db.Alunos.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return Erro(404, "Aluno não encontrado.");
        a.Ativo = req.Ativo;
        await db.SaveChangesAsync();
        return Ok(UsuarioMapper.ToDto(a));
    }
}

[Route("api/professores")]
public class ProfessoresController(CursaAquiDbContext db) : ApiBase
{
    [HttpGet, Authorize]
    public async Task<IActionResult> Listar() =>
        Ok(await db.Professores.AsNoTracking().Where(p => p.Ativo).OrderBy(p => p.Nome)
            .Select(p => new ProfessorDto(p.Id, p.Nome, p.Titulacao, p.Cursos.Count)).ToListAsync());

    /// <summary>Cards da "Visão Geral" do painel do professor.</summary>
    [HttpGet("me/resumo"), Authorize(Roles = "Professor")]
    public async Task<IActionResult> Resumo()
    {
        var uid = UserId;
        var q = db.Tentativas.AsNoTracking().Where(t => t.Prova.Curso.ProfessorId == uid);
        return Ok(new ProfessorResumoDto(
            await q.CountAsync(),
            await q.CountAsync(t => t.Status == StatusTentativa.Pendente),
            await q.CountAsync(t => t.Status == StatusTentativa.Corrigida),
            await q.Select(t => t.AlunoId).Distinct().CountAsync()));
    }

    /// <summary>"Alunos Cadastrados" do professor: matriculados nos cursos dele + desempenho nas atividades.</summary>
    [HttpGet("me/alunos"), Authorize(Roles = "Professor")]
    public async Task<IActionResult> Alunos()
    {
        var uid = UserId;
        var alunos = await db.Alunos.AsNoTracking()
            .Where(a => a.Matriculas.Any(m => m.Curso.ProfessorId == uid))
            .OrderBy(a => a.Nome).Select(a => new { a.Id, a.Nome, a.Email }).ToListAsync();

        var stats = (await db.Tentativas.AsNoTracking().Where(t => t.Prova.Curso.ProfessorId == uid)
            .GroupBy(t => t.AlunoId)
            .Select(g => new
            {
                AlunoId = g.Key,
                Atividades = g.Count(),
                Corrigidas = g.Count(x => x.Status == StatusTentativa.Corrigida),
                Media = g.Average(x => x.PercentualAcerto)
            }).ToListAsync()).ToDictionary(x => x.AlunoId);

        return Ok(alunos.Select(a =>
        {
            stats.TryGetValue(a.Id, out var s);
            return new AlunoDoProfessorDto(a.Id, a.Nome, a.Email, s?.Atividades ?? 0, s?.Corrigidas ?? 0,
                s == null ? 0 : Math.Round(s.Media, 1));
        }));
    }
}
