namespace CursaAqui.App.Models;

public class Matricula
{
    public int Id { get; set; }
    public int AlunoId { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public string AlunoCpf { get; set; } = string.Empty;
    public int CursoId { get; set; }
    public string CursoNome { get; set; } = string.Empty;
    public DateTime DataMatricula { get; set; } = DateTime.Now;
    public StatusMatricula Status { get; set; } = StatusMatricula.Ativa;
    public double NotaFinal { get; set; } = 0.0;
    public int ProgressoPercentual { get; set; } = 0;
    public string PeriodoLetivo { get; set; } = "2026.1";
    public string Observacoes { get; set; } = string.Empty;
}
