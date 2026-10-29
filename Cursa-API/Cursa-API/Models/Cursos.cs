namespace CursaAqui_API.Models;

/// <summary>
/// Une Curso do desktop + objeto de courses.js do site + Curso do DER.
/// </summary>
public class Curso
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;      // desktop (o site não tem: a API gera)
    public string Titulo { get; set; } = string.Empty;      // site: title
    public string Categoria { get; set; } = string.Empty;   // site/desktop: category
    public string? Area { get; set; }                       // site: area
    public string? Descricao { get; set; }                  // desktop + DER + UML "sinopse"
    public int CargaHoraria { get; set; } = 40;             // horas (site guarda "40h" como texto)
    public int NivelMinimo { get; set; } = 1;               // site: minLevel (1..3) bloqueia o curso
    public string? ImagemUrl { get; set; }                  // site: image

    public int? ProfessorId { get; set; }                   // substitui "ProfessorResponsavel"/"teacher" (texto)
    public Professor? Professor { get; set; }

    public bool Ativo { get; set; } = true;

    // null = sem limite (cursos do site). Quando preenchido, a Secretaria valida vagas.
    public int? VagasTotais { get; set; }

    // Média cacheada das AvaliacaoCurso (recalculada a cada nova avaliação).
    public decimal Avaliacao { get; set; } = 5.0m;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    // VagasOcupadas / VagasDisponiveis NÃO são colunas: calculados pela API
    // (matrículas Ativa/Renovada/Trancada do curso) e devolvidos no DTO.

    public ICollection<Disciplina> Disciplinas { get; set; } = new List<Disciplina>();
    public ICollection<Material> Materiais { get; set; } = new List<Material>();
    public ICollection<Prova> Provas { get; set; } = new List<Prova>();
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    public ICollection<AvaliacaoCurso> Avaliacoes { get; set; } = new List<AvaliacaoCurso>();
}

/// <summary>
/// DER: Disciplina. Equivale às "aulas/tópicos" (courses.js -> lessons[]).
/// </summary>
public class Disciplina
{
    public int Id { get; set; }
    public int CursoId { get; set; }
    public Curso Curso { get; set; } = null!;
    public string Nome { get; set; } = string.Empty;
    public string? Conteudo { get; set; }
    public int Ordem { get; set; }

    public ICollection<Material> Materiais { get; set; } = new List<Material>();
}

/// <summary>
/// DER: Material (professor "posta"). Hoje o site tem 1 PDF por curso (courses.js -> pdf),
/// por isso CursoId é obrigatório e DisciplinaId é opcional.
/// </summary>
public class Material
{
    public int Id { get; set; }
    public int CursoId { get; set; }
    public Curso Curso { get; set; } = null!;
    public int? DisciplinaId { get; set; }
    public Disciplina? Disciplina { get; set; }
    public int? ProfessorId { get; set; } // quem postou
    public Professor? Professor { get; set; }

    public string Titulo { get; set; } = string.Empty;
    public TipoMaterial Tipo { get; set; } = TipoMaterial.Pdf;
    public string UrlConteudo { get; set; } = string.Empty; // DER: URL_Conteudo
    public DateTime DataPostagem { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Site: localStorage "course_reviews" ({courseId, user, rating, comment}).
/// </summary>
public class AvaliacaoCurso
{
    public int Id { get; set; }
    public int CursoId { get; set; }
    public Curso Curso { get; set; } = null!;
    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;
    public int Nota { get; set; } // 1..5
    public string? Comentario { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public DateTime? DataAtualizacao { get; set; }
}

/// <summary>
/// Site: courses.js -> techTermsGlossary (tooltips de termos técnicos).
/// </summary>
public class TermoGlossario
{
    public int Id { get; set; }
    public string Chave { get; set; } = string.Empty;    // ex.: "mvp"
    public string Termo { get; set; } = string.Empty;    // ex.: "MVP (Minimum Viable Product ...)"
    public string Explicacao { get; set; } = string.Empty; // "simple"
    public string? Tecnica { get; set; }                 // "technique" (macete de prova)
}
