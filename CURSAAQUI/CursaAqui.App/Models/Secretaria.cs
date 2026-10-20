namespace CursaAqui.App.Models;

/// <summary>
/// ============================================================================
/// CONCEITO DE POO: HERANÇA / ESPECIALIZAÇÃO (Secretaria)
/// ============================================================================
/// 'Secretaria' herda tudo de 'UsuarioInterno' e foca na operação cotidiana
/// de atendimento, documentação e acompanhamento acadêmico dos estudantes:
/// 
/// CASOS DE USO ATENDIDOS:
/// 1. Gerenciar matrículas (novas, renovações, trancamentos, notas)
/// 2. Consultar cadastro de alunos (ficha cadastral e histórico)
/// 3. Registrar atendimentos (geração de protocolo e providências)
/// 4. Gerar relatórios administrativos (listas operacionais de rotina)
/// ============================================================================
/// </summary>
public class Secretaria : UsuarioInterno
{
    // Atributos exclusivos do perfil da Secretaria
    public string SetorAtendimento { get; set; } = "Secretaria Acadêmica";
    public int AtendimentosRealizadosHoje { get; set; } = 0;

    // Construtor: invoca o construtor base passando o tipo PerfilUsuario.Secretaria
    public Secretaria() : base(PerfilUsuario.Secretaria)
    {
    }

    /// <summary>
    /// Implementação polimórfica da descrição do perfil.
    /// </summary>
    public override string ObterDescricaoPerfil()
    {
        return "Secretaria Acadêmica: responsável pelo ciclo de matrículas, cadastro de alunos, atendimento e relatórios operacionais.";
    }

    /// <summary>
    /// Regra de acesso padrão: a secretaria acessa funções operacionais,
    /// mas não tem acesso nativo a parâmetros globais ou gestão de outros usuários.
    /// </summary>
    public override bool PodeExecutar(string funcionalidade)
    {
        return funcionalidade switch
        {
            "GerenciarMatriculas" => true,
            "ConsultarCadastro" => true,
            "RegistrarAtendimentos" => true,
            "GerarRelatoriosAdministrativos" => true,
            _ => false
        };
    }
}
