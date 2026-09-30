namespace CursaAqui.App.Models;

public class ParametrosSistema
{
    public string NomeInstituicao { get; set; } = "Cursa Aqui - Plataforma Educacional";
    public string PeriodoLetivoAtual { get; set; } = "2026.1";
    public double MediaAprovacaoMinima { get; set; } = 7.0;
    public int CargaHorariaMaximaAluno { get; set; } = 240; // max horas simultâneas
    public int VagasPadraoPorCurso { get; set; } = 60;
    public bool PermitirNovasMatriculas { get; set; } = true;
    public string EmailInstitucional { get; set; } = "contato@cursaaqui.com.br";
    public string TelefoneInstitucional { get; set; } = "(11) 3456-7890";
    public string EnderecoCampus { get; set; } = "Av. Paulista, 1000 - Bela Vista, São Paulo - SP";
    public DateTime UltimaAtualizacao { get; set; } = DateTime.Now;
    public string AtualizadoPor { get; set; } = "admin@cursaaqui.com";
}
