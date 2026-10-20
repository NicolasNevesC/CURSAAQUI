namespace CursaAqui_API.Models;

/// <summary>
/// DER: Prova. A prova oficial do curso (Tipo = Avaliacao) e o banco de questões
/// do "Treino Rápido" (Tipo = Treino) usam a mesma estrutura.
/// </summary>
public class Prova
{
    public int Id { get; set; }
    public int CursoId { get; set; }
    public Curso Curso { get; set; } = null!;
    public int? DisciplinaId { get; set; } // DER liga a Disciplina; o site hoje tem 1 prova por curso
    public Disciplina? Disciplina { get; set; }

    public string Titulo { get; set; } = string.Empty;
    public TipoProva Tipo { get; set; } = TipoProva.Avaliacao;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    // ATENÇÃO (decisão do grupo): o site aprova com 60% e o desktop usa média 0–10 (7.0).
    // Aqui: nota 0–10, aprovação em NotaMinima (padrão 6.0 = os 60% do site).
    public decimal NotaMaxima { get; set; } = 10m;
    public decimal NotaMinima { get; set; } = 6m;

    public ICollection<Questao> Questoes { get; set; } = new List<Questao>();
    public ICollection<Tentativa> Tentativas { get; set; } = new List<Tentativa>();
}

/// <summary>
/// DER: Questão. Cobre as objetivas (courses.js -> quiz[]) e a dissertativa
/// (courses.js -> openQuestionsCatalog), diferenciadas por Tipo.
/// </summary>
public class Questao
{
    public int Id { get; set; }
    public int ProvaId { get; set; }
    public Prova Prova { get; set; } = null!;
    public TipoQuestao Tipo { get; set; } = TipoQuestao.Objetiva;
    public int Ordem { get; set; }

    // O site permite HTML no enunciado (<br>): sanitizar antes de devolver/exibir.
    public string Enunciado { get; set; } = string.Empty;

    // Objetiva: índice (0-based) da alternativa correta (site: "correct"). Nulo na dissertativa.
    public int? RespostaCorreta { get; set; }

    // Somente dissertativa (site: guidelines, keywords, minWords)
    public string? Orientacoes { get; set; }
    public string? PalavrasChave { get; set; } // separadas por vírgula
    public int? MinPalavras { get; set; }

    public ICollection<Alternativa> Alternativas { get; set; } = new List<Alternativa>();
}

/// <summary>
/// Site: quiz[].options[] ("A. ...", "B. ..."). Nunca expor "RespostaCorreta" ao aluno
/// antes de ele enviar a prova.
/// </summary>
public class Alternativa
{
    public int Id { get; set; }
    public int QuestaoId { get; set; }
    public Questao Questao { get; set; } = null!;
    public int Ordem { get; set; } // 0 = A, 1 = B ...
    public string Texto { get; set; } = string.Empty;
}

/// <summary>
/// DER: Tentativa/Resultado + site: "teacher_activity_submissions" (envio do aluno + parecer do professor).
/// Cada vez que o aluno refaz a prova nasce uma nova Tentativa (histórico preservado).
/// </summary>
public class Tentativa
{
    public int Id { get; set; }
    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;
    public int ProvaId { get; set; }
    public Prova Prova { get; set; } = null!;

    public DateTime DataRealizacao { get; set; } = DateTime.UtcNow; // site: submittedAt

    // Correção AUTOMÁTICA das objetivas (feita na API, não mais no navegador)
    public int Acertos { get; set; }            // site: score
    public int TotalQuestoes { get; set; }      // site: totalQuestions
    public decimal PercentualAcerto { get; set; } // site: scorePercent
    public decimal NotaObtida { get; set; }     // DER: Nota_Obtida (0–10)
    public bool Aprovada { get; set; }          // site: quizPassed

    public string? RespostaDissertativa { get; set; } // site: openAnswer

    // Parecer do PROFESSOR (site: status/feedback/gradedAt)
    public StatusTentativa Status { get; set; } = StatusTentativa.Pendente;
    public string? Feedback { get; set; }
    public int? ProfessorCorretorId { get; set; }
    public Professor? ProfessorCorretor { get; set; }
    public DateTime? DataCorrecao { get; set; }

    public ICollection<RespostaTentativa> Respostas { get; set; } = new List<RespostaTentativa>();
}

/// <summary>
/// Site: submission.answers = { 0: 2, 1: 1, ... } (questão -> alternativa escolhida).
/// </summary>
public class RespostaTentativa
{
    public int Id { get; set; }
    public int TentativaId { get; set; }
    public Tentativa Tentativa { get; set; } = null!;
    public int QuestaoId { get; set; }
    public Questao Questao { get; set; } = null!;
    public int? AlternativaEscolhida { get; set; } // índice 0-based; nulo = em branco
    public bool Correta { get; set; }
}
