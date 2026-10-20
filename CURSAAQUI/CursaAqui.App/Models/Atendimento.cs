namespace CursaAqui.App.Models;

public class Atendimento
{
    public int Id { get; set; }
    public string Protocolo { get; set; } = string.Empty;
    public string SolicitanteNome { get; set; } = string.Empty;
    public TipoSolicitante TipoSolicitante { get; set; } = TipoSolicitante.Aluno;
    public string Contato { get; set; } = string.Empty; // Telefone ou E-mail
    public string Assunto { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Providencia { get; set; } = string.Empty;
    public DateTime DataAbertura { get; set; } = DateTime.Now;
    public DateTime? DataFechamento { get; set; }
    public int AtendenteId { get; set; }
    public string AtendenteNome { get; set; } = string.Empty;
    public StatusAtendimento Status { get; set; } = StatusAtendimento.Aberto;
}
