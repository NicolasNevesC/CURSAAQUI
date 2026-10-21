namespace CursaAqui.App.Models;

/// <summary>Professor como a API informa (usado na escolha do responsável pelo curso).</summary>
public class ProfessorResumo
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Titulacao { get; set; } = string.Empty;

    public override string ToString() => string.IsNullOrWhiteSpace(Titulacao) ? Nome : $"{Titulacao} {Nome}";
}
