using System.Net;
using CursaAqui.App.Models;

namespace CursaAqui.App.Services;

/// <summary>Quais listas recarregar da API.</summary>
[Flags]
public enum Recurso
{
    Nenhum = 0,
    Usuarios = 1,
    Alunos = 2,
    Cursos = 4,
    Matriculas = 8,
    Atendimentos = 16,
    Parametros = 32,
    Permissoes = 64,
    Professores = 128,
    Todos = 255
}

/// <summary>
/// Cache em memória dos dados da CursaAqui-API.
/// As telas continuam lendo as mesmas listas (Cursos, Matriculas...), mas agora quem as preenche é a API.
/// Gravações NÃO acontecem aqui: cada tela chama a API e depois pede RecarregarAsync().
/// O arquivo cursaaqui_db.json deixou de ser usado.
/// </summary>
public class DataRepository
{
    private static DataRepository? _instance;
    public static DataRepository Instance => _instance ??= new DataRepository();

    public List<UsuarioInterno> UsuariosInternos { get; } = new();
    public List<Aluno> Alunos { get; } = new();
    public List<Curso> Cursos { get; } = new();
    public List<Matricula> Matriculas { get; } = new();
    public List<Atendimento> Atendimentos { get; } = new();
    public List<PermissaoSistema> Permissoes { get; } = new();
    public List<ProfessorResumo> Professores { get; } = new();
    public ParametrosSistema Parametros { get; private set; } = new();

    public DateTime UltimaCarga { get; private set; } = DateTime.MinValue;

    private DataRepository()
    {
    }

    public void Limpar()
    {
        UsuariosInternos.Clear();
        Alunos.Clear();
        Cursos.Clear();
        Matriculas.Clear();
        Atendimentos.Clear();
        Permissoes.Clear();
        Professores.Clear();
        Parametros = new ParametrosSistema();
        UltimaCarga = DateTime.MinValue;
    }

    /// <summary>Recarrega só se os dados forem mais velhos que o limite (evita pedir tudo duas vezes seguidas).</summary>
    public Task<string?> RecarregarSeAntigoAsync(TimeSpan idadeMaxima) =>
        DateTime.Now - UltimaCarga < idadeMaxima ? Task.FromResult<string?>(null) : RecarregarAsync();

    /// <summary>
    /// Busca na API as listas pedidas. Retorna null se deu tudo certo, ou a mensagem do primeiro erro.
    /// Se o perfil não tem permissão para uma lista (403), ela fica vazia sem acusar erro.
    /// </summary>
    public async Task<string?> RecarregarAsync(Recurso recursos = Recurso.Todos)
    {
        var auth = AuthService.Instance;
        if (!auth.EstaAutenticado) return null;

        var tarefas = new List<Task<string?>>();

        if (recursos.HasFlag(Recurso.Alunos))
            tarefas.Add(CarregarListaAsync<AlunoApiDto, Aluno>("api/alunos", Alunos, ApiMapper.ToAluno));
        if (recursos.HasFlag(Recurso.Cursos))
            tarefas.Add(CarregarListaAsync<CursoApiDto, Curso>("api/cursos", Cursos, ApiMapper.ToCurso));
        if (recursos.HasFlag(Recurso.Matriculas))
            tarefas.Add(CarregarListaAsync<MatriculaApiDto, Matricula>("api/matriculas", Matriculas, ApiMapper.ToMatricula));
        if (recursos.HasFlag(Recurso.Atendimentos))
            tarefas.Add(CarregarListaAsync<AtendimentoApiDto, Atendimento>("api/atendimentos", Atendimentos, ApiMapper.ToAtendimento));
        if (recursos.HasFlag(Recurso.Professores))
            tarefas.Add(CarregarListaAsync<ProfessorApiDto, ProfessorResumo>("api/professores", Professores, ApiMapper.ToProfessor));
        if (recursos.HasFlag(Recurso.Parametros))
            tarefas.Add(CarregarParametrosAsync());
        if (recursos.HasFlag(Recurso.Permissoes))
            tarefas.Add(auth.CarregarPermissoesAsync());

        // Telas exclusivas do Administrador
        if (auth.EhAdministrador && recursos.HasFlag(Recurso.Usuarios))
            tarefas.Add(CarregarUsuariosAsync());
        if (auth.EhAdministrador && recursos.HasFlag(Recurso.Permissoes))
            tarefas.Add(CarregarListaAsync<PermissaoApiDto, PermissaoSistema>("api/permissoes", Permissoes, ApiMapper.ToPermissao));

        var erros = await Task.WhenAll(tarefas);
        if (recursos == Recurso.Todos) UltimaCarga = DateTime.Now;
        return erros.FirstOrDefault(e => e != null);
    }

    private static async Task<string?> CarregarListaAsync<TApi, TModel>(string url, List<TModel> destino, Func<TApi, TModel> mapear)
    {
        var r = await ApiClient.Instance.GetAsync<List<TApi>>(url);
        if (r.Status == HttpStatusCode.Forbidden)
        {
            destino.Clear(); // sem permissão para esta lista
            return null;
        }
        if (!r.Sucesso) return r.Erro;

        destino.Clear();
        destino.AddRange((r.Dados ?? new List<TApi>()).Select(mapear));
        return null;
    }

    private async Task<string?> CarregarUsuariosAsync()
    {
        var r = await ApiClient.Instance.GetAsync<List<UsuarioApiDto>>("api/usuarios");
        if (r.Status == HttpStatusCode.Forbidden)
        {
            UsuariosInternos.Clear();
            return null;
        }
        if (!r.Sucesso) return r.Erro;

        UsuariosInternos.Clear();
        foreach (var dto in r.Dados ?? new List<UsuarioApiDto>())
        {
            var usuario = ApiMapper.ToUsuario(dto); // só Administrador e Secretaria entram na lista
            if (usuario != null) UsuariosInternos.Add(usuario);
        }
        return null;
    }

    private async Task<string?> CarregarParametrosAsync()
    {
        var r = await ApiClient.Instance.GetAsync<ParametrosApiDto>("api/parametros");
        if (r.Status == HttpStatusCode.Forbidden) return null;
        if (!r.Sucesso || r.Dados == null) return r.Erro;

        Parametros = ApiMapper.ToParametros(r.Dados);
        return null;
    }
}
