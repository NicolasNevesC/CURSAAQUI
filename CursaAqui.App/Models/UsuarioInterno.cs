namespace CursaAqui.App.Models;

/// <summary>
/// ============================================================================
/// CONCEITO DE POO: CLASSE ABSTRATA & GENERALIZAÇÃO
/// ============================================================================
/// 'UsuarioInterno' é o ator base/abstrato modelado no diagrama UML.
/// 
/// Por que é abstrata?
/// 1. Não pode ser instanciada diretamente no sistema (ninguém é apenas "Usuário Interno").
/// 2. Define os atributos e comportamentos comuns que são herdados pelas especializações:
///    - Administrador (Gestão estratégica e governança)
///    - Secretaria (Operação acadêmica e atendimento ao público)
/// ============================================================================
/// </summary>
public abstract class UsuarioInterno
{
    // Identificador único do usuário no banco de dados
    public int Id { get; set; }

    // Dados cadastrais e de identificação pessoal
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;

    // Metadados de auditoria e segurança
    public DateTime DataCadastro { get; set; } = DateTime.Now;
    public bool Ativo { get; set; } = true;

    // Enum indicando se a instância é Administrador ou Secretaria
    public PerfilUsuario TipoPerfil { get; set; }

    // Construtor protegido: obriga as classes filhas a informarem seu perfil específico
    protected UsuarioInterno(PerfilUsuario tipoPerfil)
    {
        TipoPerfil = tipoPerfil;
    }

    /// <summary>
    /// MÉTODO ABSTRATO (POLIMORFISMO):
    /// Cada classe filha (Administrador / Secretaria) é OBRIGADA a implementar
    /// e detalhar a descrição das suas próprias atribuições no sistema.
    /// </summary>
    public abstract string ObterDescricaoPerfil();

    /// <summary>
    /// MÉTODO ABSTRATO DE SEGURANÇA:
    /// Verifica se este perfil tem permissão nativa para executar a funcionalidade solicitada.
    /// </summary>
    public abstract bool PodeExecutar(string funcionalidade);

    public override string ToString()
    {
        return $"{Nome} ({TipoPerfil})";
    }
}
