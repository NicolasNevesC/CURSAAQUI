using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CursaAqui.App.Services;

/// <summary>
/// Endereço da API. Ordem de busca: variável de ambiente CURSAAQUI_API_URL,
/// arquivo api.config.json ao lado do .exe ({ "BaseUrl": "http://..." }) e, por fim, o padrão local.
/// </summary>
public static class ApiConfig
{
    public const string UrlPadrao = "http://localhost:5035";
    public static string BaseUrl { get; } = Resolver();

    private static string Resolver()
    {
        var env = Environment.GetEnvironmentVariable("CURSAAQUI_API_URL");
        if (!string.IsNullOrWhiteSpace(env)) return env.Trim();

        try
        {
            var caminho = Path.Combine(AppContext.BaseDirectory, "api.config.json");
            if (File.Exists(caminho))
            {
                using var doc = JsonDocument.Parse(File.ReadAllText(caminho));
                if (doc.RootElement.TryGetProperty("BaseUrl", out var v) && !string.IsNullOrWhiteSpace(v.GetString()))
                    return v.GetString()!.Trim();
            }
        }
        catch
        {
            // arquivo inválido: usa o padrão
        }
        return UrlPadrao;
    }
}

/// <summary>Resposta de uma chamada: Sucesso, dados (quando houver) e a mensagem de erro já pronta para exibir.</summary>
public record ApiResultado<T>(bool Sucesso, T? Dados, string Erro, HttpStatusCode Status);

/// <summary>Usado como T quando a resposta da API não interessa (só importa se deu certo).</summary>
public class Vazio { }

/// <summary>
/// Cliente HTTP único do app: guarda o token JWT do login e envia em todas as chamadas.
/// Enums trafegam como texto e os nomes em camelCase, como a API espera.
/// </summary>
public class ApiClient
{
    private static ApiClient? _instance;
    public static ApiClient Instance => _instance ??= new ApiClient();

    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _http;

    private ApiClient()
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(ApiConfig.BaseUrl.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromSeconds(20)
        };
    }

    public void DefinirToken(string? token) =>
        _http.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);

    public Task<ApiResultado<T>> GetAsync<T>(string url) => EnviarAsync<T>(HttpMethod.Get, url, null);
    public Task<ApiResultado<T>> PostAsync<T>(string url, object? corpo) => EnviarAsync<T>(HttpMethod.Post, url, corpo);
    public Task<ApiResultado<T>> PutAsync<T>(string url, object? corpo) => EnviarAsync<T>(HttpMethod.Put, url, corpo);
    public Task<ApiResultado<T>> PatchAsync<T>(string url, object? corpo) => EnviarAsync<T>(HttpMethod.Patch, url, corpo);

    private async Task<ApiResultado<T>> EnviarAsync<T>(HttpMethod metodo, string url, object? corpo)
    {
        try
        {
            using var req = new HttpRequestMessage(metodo, url);
            if (corpo != null)
                req.Content = JsonContent.Create(corpo, corpo.GetType(), null, Json);

            using var resp = await _http.SendAsync(req);

            if (resp.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(Vazio) || resp.StatusCode == HttpStatusCode.NoContent)
                    return new ApiResultado<T>(true, default, "", resp.StatusCode);

                var dados = await resp.Content.ReadFromJsonAsync<T>(Json);
                return new ApiResultado<T>(true, dados, "", resp.StatusCode);
            }

            var texto = await resp.Content.ReadAsStringAsync();
            return new ApiResultado<T>(false, default, ExtrairErro(texto, resp.StatusCode), resp.StatusCode);
        }
        catch (HttpRequestException)
        {
            return new ApiResultado<T>(false, default,
                $"Não foi possível conectar à API em {ApiConfig.BaseUrl}.\nConfirme se a API está em execução.",
                HttpStatusCode.ServiceUnavailable);
        }
        catch (TaskCanceledException)
        {
            return new ApiResultado<T>(false, default, "A API demorou demais para responder. Tente novamente.", HttpStatusCode.RequestTimeout);
        }
        catch (JsonException)
        {
            return new ApiResultado<T>(false, default, "A resposta da API veio em um formato inesperado.", HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>A API devolve erros como { "erro": "mensagem" } (ou ProblemDetails nas validações automáticas).</summary>
    private static string ExtrairErro(string corpo, HttpStatusCode status)
    {
        try
        {
            using var doc = JsonDocument.Parse(corpo);
            var raiz = doc.RootElement;
            if (raiz.ValueKind == JsonValueKind.Object)
            {
                if (raiz.TryGetProperty("erro", out var e) && e.ValueKind == JsonValueKind.String)
                    return e.GetString()!;

                if (raiz.TryGetProperty("errors", out var errs) && errs.ValueKind == JsonValueKind.Object)
                    foreach (var campo in errs.EnumerateObject())
                        if (campo.Value.ValueKind == JsonValueKind.Array && campo.Value.GetArrayLength() > 0)
                            return campo.Value[0].GetString() ?? "Dados inválidos.";

                if (raiz.TryGetProperty("title", out var t) && t.ValueKind == JsonValueKind.String)
                    return t.GetString()!;
            }
        }
        catch (JsonException)
        {
            // corpo vazio ou que não é JSON: cai nas mensagens padrão abaixo
        }

        return status switch
        {
            HttpStatusCode.Unauthorized => "Sessão expirada ou não autenticada. Faça login novamente.",
            HttpStatusCode.Forbidden => "Seu perfil não tem permissão para esta operação.",
            HttpStatusCode.NotFound => "Recurso não encontrado na API.",
            HttpStatusCode.TooManyRequests => "Muitas tentativas. Aguarde um minuto e tente novamente.",
            _ => $"A API respondeu com erro {(int)status}."
        };
    }
}
