namespace CursaAqui_API.Models;

/// <summary>
/// Desktop: Atendimento (Secretaria). AtendenteNome saiu: vem do relacionamento.
/// Visitantes não têm cadastro, por isso SolicitanteNome + Contato continuam como texto.
/// </summary>
public class Atendimento
{
    public int Id { get; set; }
    public string Protocolo { get; set; } = string.Empty; // gerado na API: ATEND-{ano}-{sequencial 3 dígitos}

    public TipoSolicitante TipoSolicitante { get; set; } = TipoSolicitante.Aluno;
    public string SolicitanteNome { get; set; } = string.Empty;
    public string Contato { get; set; } = string.Empty; // telefone ou e-mail
    public int? AlunoId { get; set; }                   // opcional: liga ao cadastro quando o solicitante é aluno
    public Aluno? Aluno { get; set; }

    public string Assunto { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? Providencia { get; set; }

    public StatusAtendimento Status { get; set; } = StatusAtendimento.Aberto;
    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    public DateTime? DataFechamento { get; set; }

    public int AtendenteId { get; set; } // usuário logado que abriu o atendimento
    public Usuario Atendente { get; set; } = null!;
}

/// <summary>
/// UML: "suporte/feedback" (dúvidas, reportar problemas) + caso de uso do Administrador
/// "analisar feedback" -> "enviar resposta".
/// </summary>
public class Feedback
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public TipoFeedback Tipo { get; set; } = TipoFeedback.Duvida;
    public string Assunto { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public StatusFeedback Status { get; set; } = StatusFeedback.Aberto;
    public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

    public string? Resposta { get; set; }
    public int? RespondidoPorId { get; set; }
    public Usuario? RespondidoPor { get; set; }
    public DateTime? DataResposta { get; set; }
}
