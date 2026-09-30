namespace CursaAqui.App.Models;

public class Curso
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int CargaHoraria { get; set; } = 40; // em horas
    public string ProfessorResponsavel { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public int VagasTotais { get; set; } = 50;
    public int VagasOcupadas { get; set; } = 0;
    public string Descricao { get; set; } = string.Empty;
    public double Avaliacao { get; set; } = 5.0;

    public int VagasDisponiveis => Math.Max(0, VagasTotais - VagasOcupadas);

    public override string ToString() => $"{Titulo} ({CargaHoraria}h) - {Categoria}";
}
