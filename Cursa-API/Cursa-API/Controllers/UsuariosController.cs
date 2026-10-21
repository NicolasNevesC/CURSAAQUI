using CursaAqui_API.Data;
using CursaAqui_API.Dtos;
using CursaAqui_API.Infra;
using CursaAqui_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Controllers;

/// <summary>Gestão da equipe (desktop: Gerenciar Usuários) + consulta de qualquer usuário (UML: consultar usuário).</summary>
[Route("api/usuarios"), Authorize(Roles = "Administrador"), Permissao("GerenciarUsuarios")]
public class UsuariosController(CursaAquiDbContext db) : ApiBase
{
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? busca, [FromQuery] PerfilUsuario? perfil, [FromQuery] bool? ativo)
    {
        var q = db.Usuarios.AsNoTracking();
        if (perfil != null) q = q.Where(u => u.Perfil == perfil.Value);
        if (ativo != null) q = q.Where(u => u.Ativo == ativo.Value);
        if (!string.IsNullOrWhiteSpace(busca))
        {
            var b = busca.Trim();
            var d = Texto.Digitos(b);
            q = q.Where(u => u.Nome.Contains(b) || u.Email.Contains(b) || (d != "" && u.Cpf != null && u.Cpf.Contains(d)));
        }
        var lista = await q.OrderBy(u => u.Nome).ToListAsync();
        return Ok(lista.Select(UsuarioMapper.ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
    {
        var u = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return u == null ? Erro(404, "Usuário não encontrado.") : Ok(UsuarioMapper.ToDto(u));
    }

    /// <summary>Cria Administrador, Secretaria ou Professor. Aluno nasce em POST /api/alunos ou /api/auth/cadastro.</summary>
    [HttpPost]
    public async Task<IActionResult> Criar(UsuarioCreateRequest req)
    {
        if (req.Perfil == PerfilUsuario.Aluno) return Erro(400, "Para criar alunos use POST /api/alunos.");
        var erro = Validacoes.NovoUsuario(req.Nome, req.Email, req.Senha, req.Cpf);
        if (erro != null) return Erro(400, erro);

        var email = Texto.Email(req.Email);
        var cpf = Texto.CpfNormalizado(req.Cpf);
        if (await db.Usuarios.AnyAsync(x => x.Email == email)) return Erro(409, "Este e-mail já está cadastrado.");
        if (cpf != null && await db.Usuarios.AnyAsync(x => x.Cpf == cpf)) return Erro(409, "Este CPF já está cadastrado.");

        Usuario novo = req.Perfil switch
        {
            PerfilUsuario.Administrador => new Administrador { NivelAutoridade = req.NivelAutoridade ?? "Total" },
            PerfilUsuario.Secretaria => new Secretaria { SetorAtendimento = req.SetorAtendimento ?? "Secretaria Acadêmica" },
            PerfilUsuario.Professor => new Professor { Titulacao = req.Titulacao },
            _ => throw new ArgumentOutOfRangeException()
        };
        novo.Nome = req.Nome.Trim();
        novo.Email = email;
        novo.Cpf = cpf;
        novo.Telefone = req.Telefone?.Trim();
        novo.SenhaHash = BCrypt.Net.BCrypt.HashPassword(req.Senha);

        db.Usuarios.Add(novo);
        await db.SaveChangesAsync();
        return Created($"/api/usuarios/{novo.Id}", UsuarioMapper.ToDto(novo));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, UsuarioUpdateRequest req)
    {
        var u = await db.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return Erro(404, "Usuário não encontrado.");

        if (req.Nome != null)
        {
            if (string.IsNullOrWhiteSpace(req.Nome)) return Erro(400, "Informe o nome.");
            u.Nome = req.Nome.Trim();
        }
        if (req.Email != null)
        {
            var email = Texto.Email(req.Email);
            if (!Texto.EmailValido(email)) return Erro(400, "Informe um e-mail válido.");
            if (await db.Usuarios.AnyAsync(x => x.Email == email && x.Id != id)) return Erro(409, "Este e-mail já está cadastrado.");
            u.Email = email;
        }
        if (req.Cpf != null)
        {
            var erro = Validacoes.Cpf(req.Cpf);
            if (erro != null) return Erro(400, erro);
            var cpf = Texto.CpfNormalizado(req.Cpf);
            if (cpf != null && await db.Usuarios.AnyAsync(x => x.Cpf == cpf && x.Id != id)) return Erro(409, "Este CPF já está cadastrado.");
            u.Cpf = cpf;
        }
        if (req.Telefone != null) u.Telefone = req.Telefone.Trim();
        if (!string.IsNullOrEmpty(req.NovaSenha))
        {
            if (req.NovaSenha.Length < 6) return Erro(400, "A senha deve ter pelo menos 6 caracteres.");
            u.SenhaHash = BCrypt.Net.BCrypt.HashPassword(req.NovaSenha);
        }

        switch (u)
        {
            case Administrador a when req.NivelAutoridade != null: a.NivelAutoridade = req.NivelAutoridade; break;
            case Secretaria s when req.SetorAtendimento != null: s.SetorAtendimento = req.SetorAtendimento; break;
            case Professor p when req.Titulacao != null: p.Titulacao = req.Titulacao; break;
        }

        await db.SaveChangesAsync();
        return Ok(UsuarioMapper.ToDto(u));
    }

    [HttpPatch("{id:int}/ativo")]
    public async Task<IActionResult> AlterarAtivo(int id, AtivoRequest req)
    {
        if (id == UserId && !req.Ativo) return Erro(400, "Não é permitido inativar o próprio usuário logado.");
        var u = await db.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return Erro(404, "Usuário não encontrado.");
        u.Ativo = req.Ativo;
        await db.SaveChangesAsync();
        return Ok(UsuarioMapper.ToDto(u));
    }

    /// <summary>UML "acessar cursos": matrículas (se aluno) e cursos lecionados (se professor).</summary>
    [HttpGet("{id:int}/cursos")]
    public async Task<IActionResult> Cursos(int id)
    {
        if (!await db.Usuarios.AnyAsync(x => x.Id == id)) return Erro(404, "Usuário não encontrado.");
        var historico = await Consultas.HistoricoAsync(db, id);
        var lecionados = await db.Cursos.AsNoTracking().Where(c => c.ProfessorId == id)
            .Select(c => new { c.Id, c.Codigo, c.Titulo, c.Ativo }).ToListAsync();
        return Ok(new { historico, lecionados });
    }

    /// <summary>UML "acessar notas": tentativas de prova do usuário (aluno).</summary>
    [HttpGet("{id:int}/notas")]
    public async Task<IActionResult> Notas(int id)
    {
        var lista = await db.Tentativas.AsNoTracking().Where(t => t.AlunoId == id)
            .OrderByDescending(t => t.DataRealizacao).Select(Consultas.TentativaProj).ToListAsync();
        return Ok(lista);
    }
}
