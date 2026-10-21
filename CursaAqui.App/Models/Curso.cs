namespace CursaAqui.App.Models;

public class Curso
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int CargaHoraria { get; set; } = 40; // em horas
    public string ProfessorResponsavel { get; set; } = string.Empty; // nome, só para exibição
    public bool Ativo { get; set; } = true;
    public int VagasTotais { get; set; } = 0; // 0 = sem limite de vagas (como os cursos do site)
    public int VagasOcupadas { get; set; } = 0; // calculado pela API (matrículas Ativa/Renovada/Trancada)
    public string Descricao { get; set; } = string.Empty;
    public double Avaliacao { get; set; } = 5.0;

    // Campos que a API guarda e o desktop precisa devolver intactos ao editar um curso
    public int? ProfessorId { get; set; }
    public string Area { get; set; } = string.Empty;
    public int NivelMinimo { get; set; } = 1;
    public string ImagemUrl { get; set; } = string.Empty;

    public bool SemLimiteDeVagas => VagasTotais <= 0;
    public int VagasDisponiveis => SemLimiteDeVagas ? int.MaxValue : Math.Max(0, VagasTotais - VagasOcupadas);

    public override string ToString() => $"{Titulo} ({CargaHoraria}h) - {Categoria}";
}
