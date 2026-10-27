using CursaAqui.App.Models;

namespace CursaAqui.App.Services;

/// <summary>
/// ============================================================================
/// PADRÃO DE PROJETO: SINGLETON & SERVIÇO DE AUTENTICAÇÃO E SESSÃO
/// ============================================================================
/// Agora a autenticação é feita pela CursaAqui-API (JWT):
/// 1. O login envia e-mail e senha para a API, que devolve o token e o perfil
/// 2. O token fica no ApiClient e é enviado em todas as chamadas seguintes
/// 3. As permissões dinâmicas (matriz do Administrador) vêm de /api/auth/permissoes
/// ============================================================================
/// </summary>
public class AuthService
{
    private static AuthService? _instance;
    public static AuthService Instance => _instance ??= new AuthService();

    // Guarda o objeto polimórfico do usuário logado (pode ser Administrador ou Secretaria)
    public UsuarioInterno? UsuarioLogado { get; private set; }

    private Dictionary<string, bool> _permissoes = new();

    public bool EstaAutenticado => UsuarioLogado != null;
    public bool EhAdministrador => UsuarioLogado is Administrador;
    public bool EhSecretaria => UsuarioLogado is Secretaria;

    public event Action? OnUsuarioAlterado;

    private AuthService()
    {
    }

    /// <summary>Autentica na API. Só Administrador e Secretaria podem usar o aplicativo desktop.</summary>
    public async Task<(bool Sucesso, string Mensagem)> LoginAsync(string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            return (false, "Informe o e-mail institucional e a senha.");

        var r = await ApiClient.Instance.PostAsync<AuthRespostaDto>("api/auth/login",
            new { email = email.Trim(), senha = senha.Trim() });

        if (!r.Sucesso || r.Dados == null)
            return (false, r.Erro);

        var usuario = ApiMapper.ToUsuario(r.Dados.Usuario);
        if (usuario == null)
            return (false, "Este aplicativo é exclusivo da equipe interna (Administração e Secretaria).\nAlunos e professores devem usar o portal web.");

        ApiClient.Instance.DefinirToken(r.Dados.Token);
        UsuarioLogado = usuario;

        var erroPermissoes = await CarregarPermissoesAsync();
        if (erroPermissoes != null)
        {
            Logout();
            return (false, erroPermissoes);
        }

        OnUsuarioAlterado?.Invoke();
        return (true, $"Bem-vindo(a), {usuario.Nome}!");
    }

    /// <summary>Busca a matriz de permissões do perfil logado (chave da funcionalidade -> permitido).</summary>
    public async Task<string?> CarregarPermissoesAsync()
    {
        var r = await ApiClient.Instance.GetAsync<Dictionary<string, bool>>("api/auth/permissoes");
        if (!r.Sucesso) return r.Erro;
        _permissoes = r.Dados ?? new Dictionary<string, bool>();
        return null;
    }

    /// <summary>Encerra a sessão: descarta o token e os dados em memória.</summary>
    public void Logout()
    {
        UsuarioLogado = null;
        _permissoes = new Dictionary<string, bool>();
        ApiClient.Instance.DefinirToken(null);
        DataRepository.Instance.Limpar();
        OnUsuarioAlterado?.Invoke();
    }

    /// <summary>Valida se o usuário autenticado pode acessar a funcionalidade (matriz definida pelo Administrador).</summary>
    public bool TemPermissao(string chaveFuncionalidade)
    {
        if (UsuarioLogado == null) return false;
        if (_permissoes.TryGetValue(chaveFuncionalidade, out var permitido)) return permitido;
        return UsuarioLogado.PodeExecutar(chaveFuncionalidade); // sem regra da API: usa o padrão do perfil
    }
}
