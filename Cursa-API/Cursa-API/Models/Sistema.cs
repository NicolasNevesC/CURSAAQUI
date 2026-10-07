namespace CursaAqui_API.Models;

/// <summary>
/// Desktop: ParametrosSistema. Tabela com UMA única linha (Id = 1).
/// "AtualizadoPor" (e-mail em texto) virou chave estrangeira.
/// </summary>
public class ParametrosSistema
{
    public int Id { get; set; } = 1;
    public string NomeInstituicao { get; set; } = "Cursa Aqui - Plataforma Educacional";
    public string PeriodoLetivoAtual { get; set; } = "2026.1";
    public decimal MediaAprovacaoMinima { get; set; } = 7.0m;
    public int CargaHorariaMaximaAluno { get; set; } = 240; // horas simultâneas por aluno
    public int VagasPadraoPorCurso { get; set; } = 60;
    public bool PermitirNovasMatriculas { get; set; } = true; // trava global
    public string EmailInstitucional { get; set; } = "contato@cursaaqui.com.br";
    public string TelefoneInstitucional { get; set; } = "(11) 3456-7890";
    public string EnderecoCampus { get; set; } = "Av. Paulista, 1000 - Bela Vista, São Paulo - SP";
    public DateTime UltimaAtualizacao { get; set; } = DateTime.UtcNow;
    public int? AtualizadoPorId { get; set; }
    public Usuario? AtualizadoPor { get; set; }
}

/// <summary>
/// Desktop: PermissaoSistema (matriz dinâmica da equipe interna).
/// Só vale para Administrador x Secretaria. Professor e Aluno têm acesso FIXO por perfil
/// ([Authorize(Roles = "...")] nos controllers).
/// </summary>
public class PermissaoSistema
{
    public int Id { get; set; }
    public string ChaveFuncionalidade { get; set; } = string.Empty; // ex.: "GerenciarCursos"
    public string NomeExibicao { get; set; } = string.Empty;
    public string Modulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool PermitidoAdmin { get; set; } = true;
    public bool PermitidoSecretaria { get; set; } = false;
}
