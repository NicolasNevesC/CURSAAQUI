using CursaAqui_API.Data;
using CursaAqui_API.Dtos;
using CursaAqui_API.Infra;
using CursaAqui_API.Models;
using CursaAqui_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Controllers;

[Route("api/auth")]
public class AuthController(CursaAquiDbContext db, TokenService tokens) : ApiBase
{
    [HttpPost("login"), AllowAnonymous, EnableRateLimiting("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Senha))
            return Erro(400, "Informe o e-mail e a senha.");

        var email = Texto.Email(req.Email);
        var u = await db.Usuarios.FirstOrDefaultAsync(x => x.Email == email);
        if (u == null || !BCrypt.Net.BCrypt.Verify(req.Senha, u.SenhaHash))
            return Erro(401, "E-mail ou senha incorretos.");
        if (!u.Ativo) return Erro(403, "Este usuário está inativo. Contate a administração.");

        if (u is Administrador adm)
        {
            adm.UltimoAcessoGerencial = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        var (token, expira) = tokens.Gerar(u);
        return Ok(new AuthResponse(token, expira, UsuarioMapper.ToDto(u)));
    }

    /// <summary>Cadastro público: SEMPRE cria Aluno (o campo "role" do site deixa de existir).</summary>
    [HttpPost("cadastro"), AllowAnonymous, EnableRateLimiting("login")]
    public async Task<IActionResult> Cadastro(CadastroRequest req)
    {
        var erro = Validacoes.NovoUsuario(req.Nome, req.Email, req.Senha, req.Cpf);
        if (erro != null) return Erro(400, erro);

        var email = Texto.Email(req.Email);
        var cpf = Texto.CpfNormalizado(req.Cpf);
        if (await db.Usuarios.AnyAsync(x => x.Email == email)) return Erro(409, "Este e-mail já está cadastrado.");
        if (cpf != null && await db.Usuarios.AnyAsync(x => x.Cpf == cpf)) return Erro(409, "Este CPF já está cadastrado.");

        var aluno = new Aluno
        {
            Nome = req.Nome.Trim(), Email = email, SenhaHash = BCrypt.Net.BCrypt.HashPassword(req.Senha),
            Cpf = cpf, Telefone = req.Telefone?.Trim(), DataNascimento = req.DataNascimento
        };
        db.Alunos.Add(aluno);
        await db.SaveChangesAsync();

        var (token, expira) = tokens.Gerar(aluno);
        return Created("/api/auth/me", new AuthResponse(token, expira, UsuarioMapper.ToDto(aluno)));
    }

    [HttpGet("me"), Authorize]
    public async Task<IActionResult> Me()
    {
        var u = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == UserId);
        return u == null ? Erro(404, "Usuário não encontrado.") : Ok(UsuarioMapper.ToDto(u));
    }

    /// <summary>Mapa funcionalidade -> permitido (o desktop usa para esconder menus). Vazio para Professor/Aluno.</summary>
    [HttpGet("permissoes"), Authorize]
    public async Task<IActionResult> Permissoes()
    {
        if (!EhStaff) return Ok(new Dictionary<string, bool>());
        bool admin = User.IsInRole("Administrador");
        var lista = await db.PermissoesSistema.AsNoTracking().ToListAsync();
        return Ok(lista.ToDictionary(p => p.ChaveFuncionalidade, p => admin ? p.PermitidoAdmin : p.PermitidoSecretaria));
    }

    [HttpPut("senha"), Authorize]
    public async Task<IActionResult> AlterarSenha(AlterarSenhaRequest req)
    {
        if (string.IsNullOrEmpty(req.NovaSenha) || req.NovaSenha.Length < 6)
            return Erro(400, "A nova senha deve ter pelo menos 6 caracteres.");
        var u = await db.Usuarios.FirstAsync(x => x.Id == UserId);
        if (!BCrypt.Net.BCrypt.Verify(req.SenhaAtual ?? "", u.SenhaHash)) return Erro(400, "Senha atual incorreta.");
        u.SenhaHash = BCrypt.Net.BCrypt.HashPassword(req.NovaSenha);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
