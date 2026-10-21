using CursaAqui_API.Data;
using CursaAqui_API.Models;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Infra;

public static class DbExtensoes
{
    /// <summary>Última tentativa da prova oficial do curso (a que define o progresso).</summary>
    public static Task<Tentativa?> UltimaTentativaAsync(this CursaAquiDbContext db, int alunoId, int cursoId) =>
        db.Tentativas
            .Where(t => t.AlunoId == alunoId && t.Prova.CursoId == cursoId && t.Prova.Tipo == TipoProva.Avaliacao)
            .OrderByDescending(t => t.DataRealizacao).ThenByDescending(t => t.Id)
            .FirstOrDefaultAsync();
}
