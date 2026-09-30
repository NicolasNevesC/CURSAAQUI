namespace CursaAqui.App.Models;

public class PermissaoSistema
{
    public string ChaveFuncionalidade { get; set; } = string.Empty;
    public string NomeExibicao { get; set; } = string.Empty;
    public string Modulo { get; set; } = string.Empty;
    public bool PermitidoAdmin { get; set; } = true;
    public bool PermitidoSecretaria { get; set; } = false;
    public string Descricao { get; set; } = string.Empty;
}
