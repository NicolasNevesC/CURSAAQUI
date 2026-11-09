using System.Text.Json;
using CursaAqui_API.Infra;
using CursaAqui_API.Models;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Data;

/// <summary>
/// Popula o banco com: equipe interna (do desktop) e um aluno de demonstração (só em banco vazio),
/// os professores e os 15 cursos do site (courses.js -> Seed/seed-catalogo.json), inserindo apenas
/// o que ainda não existe. Só roda se Seed:Habilitado = true.
/// </summary>
public static class DbSeeder
{
    internal record SeedArquivo(List<SeedCurso> Cursos, List<SeedTermo> Glossario);
    internal record SeedCurso(int Id, string Titulo, string Categoria, string? Area, string? Professor, int CargaHoraria,
        double Avaliacao, int NivelMinimo, string? ImagemUrl, string? Pdf, List<string> Disciplinas,
        List<SeedQuestao> Questoes, SeedDissertativa? Dissertativa);
    internal record SeedQuestao(string Enunciado, List<string> Alternativas, int Correta);
    internal record SeedDissertativa(string Titulo, string Enunciado, string? Orientacoes, string? PalavrasChave, int? MinPalavras);
    internal record SeedTermo(string Chave, string Termo, string Explicacao, string? Tecnica);

    public static async Task ExecutarAsync(CursaAquiDbContext db, IConfiguration cfg)
    {
        if (!cfg.GetValue<bool>("Seed:Habilitado")) return;

        var senha = cfg["Seed:SenhaPadrao"];
        if (string.IsNullOrWhiteSpace(senha)) return;
        var hash = BCrypt.Net.BCrypt.HashPassword(senha);

        var arquivo = Path.Combine(AppContext.BaseDirectory, "Seed", "seed-catalogo.json");
        SeedArquivo? catalogo = File.Exists(arquivo)
            ? JsonSerializer.Deserialize<SeedArquivo>(await File.ReadAllTextAsync(arquivo),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            : null;

        // ---------- Usuários internos (só em banco vazio) ----------
        if (!await db.Usuarios.AnyAsync())
        {
            db.Administradores.AddRange(
                new Administrador { Nome = "Carlos Eduardo Menezes", Email = "admin@cursaaqui.com", SenhaHash = hash, NivelAutoridade = "Diretoria Acadêmica" },
                new Administrador { Nome = "Mariana Souza Ribeiro", Email = "mariana.admin@cursaaqui.com", SenhaHash = hash, NivelAutoridade = "Coordenação Geral" });
            db.Secretarias.AddRange(
                new Secretaria { Nome = "Fernanda Lima Barbosa", Email = "secretaria@cursaaqui.com", SenhaHash = hash, SetorAtendimento = "Secretaria Acadêmica" },
                new Secretaria { Nome = "Lucas Andrade Ferreira", Email = "lucas.sec@cursaaqui.com", SenhaHash = hash, SetorAtendimento = "Atendimento ao Aluno" });
            db.Alunos.Add(new Aluno { Nome = "Matheus Narvaes", Email = "aluno@cursaaqui.com", SenhaHash = hash, Xp = 350, Nivel = 2 });
            await db.SaveChangesAsync();
        }

        // ---------- Professores do catálogo (cria só os que faltam, pelo e-mail) ----------
        // Chave = nome sem "Prof."/"Dr.", para casar com o campo "professor" do seed.
        var professores = new Dictionary<string, Professor>();
        foreach (var nomeBruto in (catalogo?.Cursos ?? new()).Select(c => c.Professor).Where(p => p != null).Distinct())
        {
            var (nome, titulacao) = SepararTitulo(nomeBruto!);
            var partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var email = nome == "Ricardo Silva"
                ? "ricardo@cursaaqui.com" // professor de demonstração do site
                : $"{Texto.Slug(partes[0])}.{Texto.Slug(partes[^1])}@cursaaqui.com";
            var prof = await db.Professores.FirstOrDefaultAsync(p => p.Email == email);
            if (prof == null)
            {
                prof = new Professor { Nome = nome, Titulacao = titulacao, Email = email, SenhaHash = hash };
                db.Professores.Add(prof);
            }
            professores[nome] = prof;
        }
        await db.SaveChangesAsync();

        // ---------- Catálogo (insere só os cursos que ainda não existem, pelo código) ----------
        if (catalogo != null)
        {
            var codigosExistentes = (await db.Cursos.Select(x => x.Codigo).ToListAsync()).ToHashSet();
            foreach (var c in catalogo.Cursos.OrderBy(x => x.Id))
            {
                var codigo = $"CUR-{c.Id:D3}";
                if (codigosExistentes.Contains(codigo)) continue;

                Professor? prof = c.Professor != null && professores.TryGetValue(SepararTitulo(c.Professor).Nome, out var p) ? p : null;
                var curso = new Curso
                {
                    Codigo = codigo, Titulo = c.Titulo, Categoria = c.Categoria, Area = c.Area,
                    CargaHoraria = c.CargaHoraria, NivelMinimo = c.NivelMinimo, ImagemUrl = c.ImagemUrl,
                    Avaliacao = (decimal)c.Avaliacao, Professor = prof, Ativo = true
                };

                int ordem = 1;
                foreach (var aula in c.Disciplinas)
                    curso.Disciplinas.Add(new Disciplina { Nome = aula, Ordem = ordem++ });

                if (!string.IsNullOrWhiteSpace(c.Pdf))
                    curso.Materiais.Add(new Material
                    {
                        Titulo = $"Apostila - {c.Titulo}", Tipo = TipoMaterial.Pdf, UrlConteudo = c.Pdf, Professor = prof
                    });

                var prova = new Prova { Titulo = $"Avaliação - {c.Titulo}", Tipo = TipoProva.Avaliacao };
                int ordemQ = 1;
                foreach (var q in c.Questoes)
                {
                    var questao = new Questao
                    {
                        Tipo = TipoQuestao.Objetiva, Ordem = ordemQ++,
                        Enunciado = Texto.Sanitizar(q.Enunciado), RespostaCorreta = q.Correta
                    };
                    for (int i = 0; i < q.Alternativas.Count; i++)
                        questao.Alternativas.Add(new Alternativa { Ordem = i, Texto = q.Alternativas[i] });
                    prova.Questoes.Add(questao);
                }
                if (c.Dissertativa != null)
                    prova.Questoes.Add(new Questao
                    {
                        Tipo = TipoQuestao.Dissertativa, Ordem = ordemQ,
                        Enunciado = Texto.Sanitizar(c.Dissertativa.Enunciado),
                        Orientacoes = c.Dissertativa.Orientacoes, PalavrasChave = c.Dissertativa.PalavrasChave,
                        MinPalavras = c.Dissertativa.MinPalavras
                    });
                curso.Provas.Add(prova);

                db.Cursos.Add(curso);
            }
            await db.SaveChangesAsync();
        }

        // ---------- Glossário ----------
        if (catalogo != null && !await db.TermosGlossario.AnyAsync())
        {
            db.TermosGlossario.AddRange(catalogo.Glossario.Select(t => new TermoGlossario
            {
                Chave = t.Chave, Termo = t.Termo, Explicacao = t.Explicacao, Tecnica = t.Tecnica
            }));
            await db.SaveChangesAsync();
        }
    }

    private static (string Nome, string? Titulacao) SepararTitulo(string bruto)
    {
        string? titulacao = bruto.Contains("Dra.") ? "Dra." : bruto.Contains("Dr.") ? "Dr." : null;
        var nome = bruto;
        foreach (var prefixo in new[] { "Profª.", "Prof.", "Dra.", "Dr." })
            nome = nome.Replace(prefixo, "");
        return (string.Join(' ', nome.Split(' ', StringSplitOptions.RemoveEmptyEntries)), titulacao);
    }
}
