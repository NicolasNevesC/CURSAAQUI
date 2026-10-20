using CursaAqui_API.Data;
using CursaAqui_API.Dtos;
using CursaAqui_API.Infra;
using CursaAqui_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CursaAqui_API.Controllers;

[Route("api/permissoes"), Authorize(Roles = "Administrador"), Permissao("GerenciarPermissoes")]
public class PermissoesController(CursaAquiDbContext db) : ApiBase
{
    private static PermissaoDto ToDto(PermissaoSistema p) => new(p.Id, p.ChaveFuncionalidade, p.NomeExibicao,
        p.Modulo, p.Descricao, p.PermitidoAdmin, p.PermitidoSecretaria);

    [HttpGet]
    public async Task<IActionResult> Listar() =>
        Ok((await db.PermissoesSistema.AsNoTracking().OrderBy(p => p.Modulo).ThenBy(p => p.Id).ToListAsync()).Select(ToDto));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, PermissaoUpdateRequest req)
    {
        var p = await db.PermissoesSistema.FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return Erro(404, "Permissão não encontrada.");
        if (p.ChaveFuncionalidade == "GerenciarPermissoes" && !req.PermitidoAdmin)
            return Erro(400, "O Administrador não pode perder o acesso à gestão de permissões.");
        p.PermitidoAdmin = req.PermitidoAdmin;
        p.PermitidoSecretaria = req.PermitidoSecretaria;
        await db.SaveChangesAsync();
        return Ok(ToDto(p));
    }

    /// <summary>Botão "Salvar" da matriz inteira do desktop.</summary>
    [HttpPut]
    public async Task<IActionResult> AtualizarLote(List<PermissaoLoteItem> itens)
    {
        var ids = itens.Select(i => i.Id).ToList();
        var permissoes = await db.PermissoesSistema.Where(p => ids.Contains(p.Id)).ToListAsync();
        if (permissoes.Count != ids.Distinct().Count()) return Erro(404, "Alguma permissão informada não existe.");

        foreach (var item in itens)
        {
            var p = permissoes.First(x => x.Id == item.Id);
            if (p.ChaveFuncionalidade == "GerenciarPermissoes" && !item.PermitidoAdmin)
                return Erro(400, "O Administrador não pode perder o acesso à gestão de permissões.");
            p.PermitidoAdmin = item.PermitidoAdmin;
            p.PermitidoSecretaria = item.PermitidoSecretaria;
        }
        await db.SaveChangesAsync();
        return Ok(permissoes.OrderBy(p => p.Modulo).ThenBy(p => p.Id).Select(ToDto));
    }
}

[Route("api/parametros")]
public class ParametrosController(CursaAquiDbContext db) : ApiBase
{
    private async Task<ParametrosSistema> CarregarAsync()
    {
        var p = await db.ParametrosSistema.Include(x => x.AtualizadoPor).FirstOrDefaultAsync(x => x.Id == 1);
        if (p == null)
        {
            p = new ParametrosSistema();
            db.ParametrosSistema.Add(p);
            await db.SaveChangesAsync();
        }
        return p;
    }

    private static ParametrosDto ToDto(ParametrosSistema p) => new(p.NomeInstituicao, p.PeriodoLetivoAtual,
        p.MediaAprovacaoMinima, p.CargaHorariaMaximaAluno, p.VagasPadraoPorCurso, p.PermitirNovasMatriculas,
        p.EmailInstitucional, p.TelefoneInstitucional, p.EnderecoCampus, p.UltimaAtualizacao, p.AtualizadoPor?.Nome);

    [HttpGet, Authorize(Roles = "Administrador,Secretaria")]
    public async Task<IActionResult> Obter() => Ok(ToDto(await CarregarAsync()));

    /// <summary>Dados institucionais para o site (rodapé, período letivo, trava de matrícula).</summary>
    [HttpGet("publico"), AllowAnonymous]
    public async Task<IActionResult> Publico()
    {
        var p = await CarregarAsync();
        return Ok(new ParametrosPublicosDto(p.NomeInstituicao, p.PeriodoLetivoAtual, p.MediaAprovacaoMinima,
            p.PermitirNovasMatriculas, p.EmailInstitucional, p.TelefoneInstitucional, p.EnderecoCampus));
    }

    [HttpPut, Authorize(Roles = "Administrador"), Permissao("ConfigurarParametros")]
    public async Task<IActionResult> Atualizar(ParametrosUpdateRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.NomeInstituicao)) return Erro(400, "Informe o nome da instituição.");
        if (!Regex.IsMatch(req.PeriodoLetivoAtual ?? "", @"^\d{4}\.[12]$")) return Erro(400, "Período letivo deve ser no formato AAAA.1 ou AAAA.2.");
        if (req.MediaAprovacaoMinima < 0 || req.MediaAprovacaoMinima > 10) return Erro(400, "A média mínima deve estar entre 0 e 10.");
        if (req.CargaHorariaMaximaAluno <= 0) return Erro(400, "A carga horária máxima deve ser maior que zero.");
        if (req.VagasPadraoPorCurso <= 0) return Erro(400, "O padrão de vagas deve ser maior que zero.");
        if (!Texto.EmailValido(Texto.Email(req.EmailInstitucional ?? ""))) return Erro(400, "E-mail institucional inválido.");

        var p = await CarregarAsync();
        p.NomeInstituicao = req.NomeInstituicao.Trim();
        p.PeriodoLetivoAtual = req.PeriodoLetivoAtual!;
        p.MediaAprovacaoMinima = req.MediaAprovacaoMinima;
        p.CargaHorariaMaximaAluno = req.CargaHorariaMaximaAluno;
        p.VagasPadraoPorCurso = req.VagasPadraoPorCurso;
        p.PermitirNovasMatriculas = req.PermitirNovasMatriculas;
        p.EmailInstitucional = Texto.Email(req.EmailInstitucional!);
        p.TelefoneInstitucional = req.TelefoneInstitucional?.Trim() ?? "";
        p.EnderecoCampus = req.EnderecoCampus?.Trim() ?? "";
        p.UltimaAtualizacao = DateTime.UtcNow;
        p.AtualizadoPorId = UserId;
        await db.SaveChangesAsync();

        return Ok(ToDto(await CarregarAsync()));
    }
}
