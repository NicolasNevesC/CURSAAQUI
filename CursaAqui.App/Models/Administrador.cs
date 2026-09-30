namespace CursaAqui.App.Models;

/// <summary>
/// ============================================================================
/// CONCEITO DE POO: HERANÇA / ESPECIALIZAÇÃO (Administrador)
/// ============================================================================
/// 'Administrador' herda tudo de 'UsuarioInterno' (Nome, Email, Senha, CPF, etc.)
/// e adiciona atributos e responsabilidades de alto nível:
/// 
/// CASOS DE USO ATENDIDOS:
/// 1. Gerenciar permissões de acesso
/// 2. Gerenciar cursos (catálogo institucional)
/// 3. Visualizar relatórios gerenciais e analíticos
/// 4. Configurar parâmetros do sistema (regras globais)
/// 5. Gerenciar usuários internos (adicionar/inativar contas)
/// ============================================================================
/// </summary>
public class Administrador : UsuarioInterno
{
    // Atributos exclusivos do perfil Administrador
    public string NivelAutoridade { get; set; } = "Total";
    public DateTime? UltimoAcessoGerencial { get; set; }

    // Construtor: invoca o construtor base passando o tipo PerfilUsuario.Administrador
    public Administrador() : base(PerfilUsuario.Administrador)
    {
    }

    /// <summary>
    /// Implementação polimórfica da descrição do perfil.
    /// </summary>
    public override string ObterDescricaoPerfil()
    {
        return "Administrador do Sistema: responsável pela governança, cursos, permissões, parâmetros e relatórios analíticos.";
    }

    /// <summary>
    /// Verificação de permissões do Administrador (acesso a módulos executivos).
    /// </summary>
    public override bool PodeExecutar(string funcionalidade)
    {
        return funcionalidade switch
        {
            "GerenciarUsuarios" => true,
            "GerenciarPermissoes" => true,
            "GerenciarCursos" => true,
            "VisualizarRelatorios" => true,
            "ConfigurarParametros" => true,
            _ => true
        };
    }
}
