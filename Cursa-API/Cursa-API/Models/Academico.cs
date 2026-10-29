namespace CursaAqui_API.Models;

/// <summary>
/// Une Matricula do desktop (gerida pela Secretaria) + "user_courses_progress_{email}" do site
/// (progresso do aluno). Antes eram dois mundos separados; agora é UMA tabela.
/// AlunoNome/AlunoCpf/CursoNome do desktop saíram: vêm dos relacionamentos.
/// </summary>
public class Matricula
{
    public int Id { get; set; }
    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;
    public int CursoId { get; set; }
    public Curso Curso { get; set; } = null!;

    public DateTime DataMatricula { get; set; } = DateTime.UtcNow;
    public StatusMatricula Status { get; set; } = StatusMatricula.Ativa;
    public string PeriodoLetivo { get; set; } = string.Empty; // ex.: "2026.1" (padrão = Parametros)
    public decimal? NotaFinal { get; set; }                   // desktop (0–10)
    public int ProgressoPercentual { get; set; }              // 0..100 (calculado pela API)
    public string? Observacoes { get; set; }
    public DateTime? DataConclusao { get; set; }

    // Etapas do site (course.html) que compõem o progresso: material = 30%, prova aprovada = 70%
    public bool MaterialAcessado { get; set; }
    public bool EstudoConcluido { get; set; }
    public bool ExerciciosLiberados { get; set; }

    // Impede dar os +1000 XP duas vezes (site: "jaConcluiuAntes")
    public bool XpConcedido { get; set; }

    public Certificado? Certificado { get; set; }
}

/// <summary>
/// Site: certificate.js (hoje só um modal, sem registro). Persistir permite validar
/// o certificado por código (o README promete "gerador e validador").
/// </summary>
public class Certificado
{
    public int Id { get; set; }
    public int MatriculaId { get; set; }
    public Matricula Matricula { get; set; } = null!;
    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;
    public int CursoId { get; set; }
    public Curso Curso { get; set; } = null!;

    public string CodigoValidacao { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
    public int CargaHoraria { get; set; } // cópia do curso no momento da emissão
}

/// <summary>
/// Site: localStorage "chat_course_{id}_user_{email}" (dúvidas aluno ↔ professor por curso).
/// AlunoId identifica a CONVERSA (um fio por aluno e curso); RemetenteId é quem escreveu.
/// </summary>
public class MensagemChat
{
    public int Id { get; set; }
    public int CursoId { get; set; }
    public Curso Curso { get; set; } = null!;
    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;
    public int? RemetenteId { get; set; } // nulo quando for mensagem do sistema
    public Usuario? Remetente { get; set; }
    public bool EhSistema { get; set; }   // site: "🤖 Notificação do Sistema"
    public string Texto { get; set; } = string.Empty;
    public DateTime DataEnvio { get; set; } = DateTime.UtcNow;
}
