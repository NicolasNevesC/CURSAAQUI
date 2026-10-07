using CursaAqui_API.Data;
using CursaAqui_API.Dtos;
using CursaAqui_API.Infra;
using CursaAqui_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Controllers;

/// <summary>Dúvidas aluno ↔ professor. Cada conversa é identificada por (curso, aluno).</summary>
[Route("api/cursos/{cursoId:int}/chat")]
public class ChatController(CursaAquiDbContext db) : ApiBase
{
    /// <summary>Descobre de qual aluno é a conversa e valida se o usuário logado pode acessá-la.</summary>
    private async Task<(int AlunoId, IActionResult? Erro)> ResolverAsync(int cursoId, int? alunoIdInformado)
    {
        var curso = await db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cursoId);
        if (curso == null) return (0, Erro(404, "Curso não encontrado."));

        if (User.IsInRole("Aluno"))
        {
            if (!await db.Matriculas.AnyAsync(m => m.AlunoId == UserId && m.CursoId == cursoId))
                return (0, Erro(403, "Inicie o curso para conversar com o professor."));
            return (UserId, null);
        }

        if (!PodeGerir(curso)) return (0, Erro(403, "Você não leciona este curso."));
        if (alunoIdInformado == null) return (0, Erro(400, "Informe o alunoId da conversa."));
        if (!await db.Alunos.AnyAsync(a => a.Id == alunoIdInformado)) return (0, Erro(404, "Aluno não encontrado."));
        return (alunoIdInformado.Value, null);
    }

    [HttpGet, Authorize(Roles = "Aluno,Professor,Administrador")]
    public async Task<IActionResult> Listar(int cursoId, [FromQuery] int? alunoId)
    {
        var (aluno, erro) = await ResolverAsync(cursoId, alunoId);
        if (erro != null) return erro;

        return Ok(await db.MensagensChat.AsNoTracking().Where(m => m.CursoId == cursoId && m.AlunoId == aluno)
            .OrderBy(m => m.DataEnvio).ThenBy(m => m.Id)
            .Select(m => new MensagemDto(m.Id, m.CursoId, m.AlunoId, m.RemetenteId,
                m.Remetente != null ? m.Remetente.Nome : "Sistema",
                m.Remetente != null ? (PerfilUsuario?)m.Remetente.Perfil : null,
                m.EhSistema, m.Texto, m.DataEnvio)).ToListAsync());
    }

    [HttpPost, Authorize(Roles = "Aluno,Professor,Administrador")]
    public async Task<IActionResult> Enviar(int cursoId, MensagemRequest req)
    {
        var (aluno, erro) = await ResolverAsync(cursoId, req.AlunoId);
        if (erro != null) return erro;
        if (string.IsNullOrWhiteSpace(req.Texto) || req.Texto.Length > 2000) return Erro(400, "Escreva a mensagem (até 2000 caracteres).");

        bool primeira = !await db.MensagensChat.AnyAsync(m => m.CursoId == cursoId && m.AlunoId == aluno);
        var msg = new MensagemChat { CursoId = cursoId, AlunoId = aluno, RemetenteId = UserId, Texto = req.Texto.Trim() };
        db.MensagensChat.Add(msg);

        // Mesma "notificação do sistema" que o site mostrava na primeira dúvida do aluno
        if (User.IsInRole("Aluno") && primeira)
            db.MensagensChat.Add(new MensagemChat
            {
                CursoId = cursoId, AlunoId = aluno, EhSistema = true,
                Texto = "Sua dúvida foi enviada! O professor responderá em breve.",
                DataEnvio = DateTime.UtcNow.AddMilliseconds(5)
            });
        await db.SaveChangesAsync();

        var nome = await db.Usuarios.AsNoTracking().Where(u => u.Id == UserId).Select(u => u.Nome).FirstAsync();
        var perfil = Enum.Parse<PerfilUsuario>(User.FindFirst("role")!.Value);
        return Created($"/api/cursos/{cursoId}/chat", new MensagemDto(msg.Id, cursoId, aluno, UserId, nome, perfil, false, msg.Texto, msg.DataEnvio));
    }

    /// <summary>Seletor de alunos do professor: quem já mandou mensagem neste curso.</summary>
    [HttpGet("alunos"), Authorize(Roles = "Professor,Administrador")]
    public async Task<IActionResult> Alunos(int cursoId)
    {
        var curso = await db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cursoId);
        if (curso == null) return Erro(404, "Curso não encontrado.");
        if (!PodeGerir(curso)) return Erro(403, "Você não leciona este curso.");

        return Ok(await db.MensagensChat.AsNoTracking().Where(m => m.CursoId == cursoId)
            .GroupBy(m => new { m.AlunoId, m.Aluno.Nome, m.Aluno.Email })
            .Select(g => new ChatAlunoDto(g.Key.AlunoId, g.Key.Nome, g.Key.Email, g.Max(x => x.DataEnvio)))
            .OrderByDescending(x => x.UltimaMensagem).ToListAsync());
    }
}
