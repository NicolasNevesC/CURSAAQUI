using CursaAqui.App.Models;

namespace CursaAqui.App.Services;

/// <summary>
/// ============================================================================
/// PADRÃO DE PROJETO: SINGLETON & SERVIÇO DE AUTENTICAÇÃO E SESSÃO
/// ============================================================================
/// Responsável por:
/// 1. Gerenciar a sessão do usuário interno logado (Administrador ou Secretaria)
/// 2. Validar credenciais (e-mail institucional e senha)
/// 3. Aplicar o princípio de menor privilégio (RBAC - Role-Based Access Control)
/// 4. Consultar a matriz de permissões dinâmicas configuradas pelo Administrador
/// ============================================================================
/// </summary>
public class AuthService
{
    // Instância única global (Singleton)
    private static AuthService? _instance;
    public static AuthService Instance => _instance ??= new AuthService();

    // Guarda o objeto polimórfico do usuário logado (pode ser Administrador ou Secretaria)
    public UsuarioInterno? UsuarioLogado { get; private set; }

    // Propriedades auxiliares de verificação de estado
    public bool EstaAutenticado => UsuarioLogado != null;
    public bool EhAdministrador => UsuarioLogado is Administrador;
    public bool EhSecretaria => UsuarioLogado is Secretaria;

    public event Action? OnUsuarioAlterado;

    private AuthService()
    {
    }

    /// <summary>
    /// Realiza a autenticação do usuário interno buscando no repositório.
    /// </summary>
    public (bool Sucesso, string Mensagem) Login(string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            return (false, "Informe o e-mail institucional e a senha.");

        var repo = DataRepository.Instance;
        var usuario = repo.UsuariosInternos.FirstOrDefault(u => 
            u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));

        if (usuario == null)
            return (false, "Usuário interno não encontrado. Verifique as credenciais.");

        if (!usuario.Ativo)
            return (false, "Este usuário interno está inativo. Contate a administração.");

        if (usuario.Senha != senha.Trim())
            return (false, "Senha incorreta. Tente novamente.");

        // Define a sessão ativa
        UsuarioLogado = usuario;

        // Se for administrador, atualiza timestamp de último acesso gerencial
        if (usuario is Administrador admin)
        {
            admin.UltimoAcessoGerencial = DateTime.Now;
            repo.Salvar();
        }

        OnUsuarioAlterado?.Invoke();
        return (true, $"Bem-vindo(a), {usuario.Nome}!");
    }

    /// <summary>
    /// Encerra a sessão atual com segurança.
    /// </summary>
    public void Logout()
    {
        UsuarioLogado = null;
        OnUsuarioAlterado?.Invoke();
    }

    /// <summary>
    /// Valida se o usuário autenticado pode acessar a funcionalidade solicitada.
    /// Consulta tanto o perfil base quanto a matriz de permissões configurável.
    /// </summary>
    public bool TemPermissao(string chaveFuncionalidade)
    {
        if (UsuarioLogado == null) return false;

        var repo = DataRepository.Instance;
        var perm = repo.Permissoes.FirstOrDefault(p => p.ChaveFuncionalidade == chaveFuncionalidade);

        if (UsuarioLogado is Administrador)
        {
            return perm?.PermitidoAdmin ?? true;
        }
        else if (UsuarioLogado is Secretaria)
        {
            return perm?.PermitidoSecretaria ?? false;
        }

        return false;
    }
}
