using System.Text.Json;
using CursaAqui_API.Infra;
using CursaAqui_API.Models;
using Microsoft.EntityFrameworkCore;

namespace CursaAqui_API.Data;

/// <summary>
/// Popula o banco vazio com: equipe interna (do desktop), um aluno de demonstração, os professores
/// e os 15 cursos do site (courses.js -> Seed/seed-catalogo.json). Só roda se Seed:Habilitado = true.
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
        // Conserto de bancos que receberam senhas em texto direto (SQL manual): converte para hash BCrypt.
        if (cfg.GetValue<bool>("Seed:ConverterSenhasEmTexto"))
            await ConverterSenhasEmTextoAsync(db);

        if (!cfg.GetValue<bool>("Seed:Habilitado")) return;

        var senha = cfg["Seed:SenhaPadrao"];
        if (string.IsNullOrWhiteSpace(senha)) return;
        var hash = BCrypt.Net.BCrypt.HashPassword(senha);

        var arquivo = Path.Combine(AppContext.BaseDirectory, "Seed", "seed-catalogo.json");
        SeedArquivo? catalogo = File.Exists(arquivo)
            ? JsonSerializer.Deserialize<SeedArquivo>(await File.ReadAllTextAsync(arquivo),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            : null;

        // ---------- Usuários ----------
        var professores = new Dictionary<string, Professor>();
        if (!await db.Usuarios.AnyAsync())
        {
            db.Administradores.AddRange(
                new Administrador { Nome = "Carlos Eduardo Menezes", Email = "admin@cursaaqui.com", SenhaHash = hash, NivelAutoridade = "Diretoria Acadêmica" },
                new Administrador { Nome = "Mariana Souza Ribeiro", Email = "mariana.admin@cursaaqui.com", SenhaHash = hash, NivelAutoridade = "Coordenação Geral" });
            db.Secretarias.AddRange(
                new Secretaria { Nome = "Fernanda Lima Barbosa", Email = "secretaria@cursaaqui.com", SenhaHash = hash, SetorAtendimento = "Secretaria Acadêmica" },
                new Secretaria { Nome = "Lucas Andrade Ferreira", Email = "lucas.sec@cursaaqui.com", SenhaHash = hash, SetorAtendimento = "Atendimento ao Aluno" });
            db.Alunos.Add(new Aluno { Nome = "Matheus Narvaes", Email = "aluno@cursaaqui.com", SenhaHash = hash, Xp = 350, Nivel = 2 });

            foreach (var nomeBruto in (catalogo?.Cursos ?? new()).Select(c => c.Professor).Where(p => p != null).Distinct())
            {
                var (nome, titulacao) = SepararTitulo(nomeBruto!);
                var partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var email = nome == "Ricardo Silva"
                    ? "ricardo@cursaaqui.com" // professor de demonstração do site
                    : $"{Texto.Slug(partes[0])}.{Texto.Slug(partes[^1])}@cursaaqui.com";
                var prof = new Professor { Nome = nome, Titulacao = titulacao, Email = email, SenhaHash = hash };
                professores[nomeBruto!] = prof;
                db.Professores.Add(prof);
            }
            await db.SaveChangesAsync();
        }
        else
        {
            foreach (var p in await db.Professores.ToListAsync())
                professores[p.Nome] = p;
        }

        // ---------- Catálogo ----------
        if (catalogo != null && !await db.Cursos.AnyAsync())
        {
            foreach (var c in catalogo.Cursos.OrderBy(x => x.Id))
            {
                Professor? prof = c.Professor != null && professores.TryGetValue(c.Professor, out var p) ? p : null;
                var curso = new Curso
                {
                    Codigo = $"CUR-{c.Id:D3}", Titulo = c.Titulo, Categoria = c.Categoria, Area = c.Area,
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

    /// <summary>
    /// Usuários cuja SenhaHash NÃO é um hash BCrypt (60 caracteres começando com "$2") tiveram a senha gravada em
    /// texto no banco. Esse valor é a senha de verdade: aqui ele é transformado em hash, e o login passa a funcionar.
    /// Use só uma vez (Seed:ConverterSenhasEmTexto = true) e depois volte para false.
    /// </summary>
    private static async Task ConverterSenhasEmTextoAsync(CursaAquiDbContext db)
    {
        var candidatos = await db.Usuarios
            .Where(u => u.SenhaHash.Length != 60 || !u.SenhaHash.StartsWith("$2"))
            .ToListAsync();

        foreach (var u in candidatos)
            u.SenhaHash = BCrypt.Net.BCrypt.HashPassword(u.SenhaHash);

        if (candidatos.Count > 0)
            await db.SaveChangesAsync();

        Console.WriteLine($"[Seed] Senhas em texto convertidas para hash: {candidatos.Count}");
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
