using CursaAqui_API.Data;
using CursaAqui_API.Dtos;
using CursaAqui_API.Infra;
using CursaAqui_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Controllers;

[Route("api/perfil"), Authorize]
public class PerfilController(CursaAquiDbContext db) : ApiBase
{
    [HttpGet]
    public async Task<IActionResult> Obter()
    {
        var u = await db.Usuarios.AsNoTracking().FirstAsync(x => x.Id == UserId);
        return Ok(UsuarioMapper.ToDto(u));
    }

    [HttpPut]
    public async Task<IActionResult> Atualizar(PerfilUpdateRequest req)
    {
        var u = await db.Usuarios.FirstAsync(x => x.Id == UserId);

        if (req.Nome != null)
        {
            if (string.IsNullOrWhiteSpace(req.Nome)) return Erro(400, "Informe o nome.");
            u.Nome = req.Nome.Trim();
        }
        if (req.Cpf != null)
        {
            var erro = Validacoes.Cpf(req.Cpf);
            if (erro != null) return Erro(400, erro);
            var cpf = Texto.CpfNormalizado(req.Cpf);
            if (cpf != null && await db.Usuarios.AnyAsync(x => x.Cpf == cpf && x.Id != u.Id))
                return Erro(409, "Este CPF já está cadastrado.");
            u.Cpf = cpf;
        }
        if (req.Telefone != null) u.Telefone = req.Telefone.Trim();
        if (req.DataNascimento != null) u.DataNascimento = req.DataNascimento;
        if (req.AvatarUrl != null)
        {
            if (req.AvatarUrl != "" && !Texto.UrlSegura(req.AvatarUrl)) return Erro(400, "URL de avatar inválida.");
            u.AvatarUrl = req.AvatarUrl == "" ? null : req.AvatarUrl;
        }
        if (req.Tema != null)
        {
            if (req.Tema != "light" && req.Tema != "dark") return Erro(400, "Tema deve ser 'light' ou 'dark'.");
            u.Tema = req.Tema;
        }
        if (req.Acessibilidade != null) u.Acessibilidade = req.Acessibilidade;
        if (req.Endereco != null && u is Aluno a) a.Endereco = req.Endereco.Trim();

        await db.SaveChangesAsync();
        return Ok(UsuarioMapper.ToDto(u));
    }
}
