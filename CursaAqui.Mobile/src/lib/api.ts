/**
 * Cliente HTTP do app: envia o token JWT do login em todas as chamadas,
 * igual ao ApiClient.cs do desktop.
 *
 * Endereço da API: variável EXPO_PUBLIC_API_URL (arquivo .env).
 */

export const API_URL = (process.env.EXPO_PUBLIC_API_URL ?? 'https://cursaaqui-api.azurewebsites.net').replace(/\/+$/, '');

// No plano gratuito a API e o banco "dormem"; a primeira chamada pode levar ~1 min.
const TIMEOUT_MS = 90_000;

export class ApiError extends Error {
  constructor(
    message: string,
    public status: number
  ) {
    super(message);
  }
}

let token: string | null = null;

export function definirToken(novo: string | null) {
  token = novo;
}

async function enviar<T>(metodo: string, caminho: string, corpo?: unknown): Promise<T> {
  const controller = new AbortController();
  const timer = setTimeout(() => controller.abort(), TIMEOUT_MS);

  let resp: Response;
  try {
    resp = await fetch(`${API_URL}${caminho}`, {
      method: metodo,
      headers: {
        Accept: 'application/json',
        ...(corpo !== undefined ? { 'Content-Type': 'application/json' } : {}),
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
      body: corpo !== undefined ? JSON.stringify(corpo) : undefined,
      signal: controller.signal,
    });
  } catch {
    throw new ApiError('Não foi possível conectar ao servidor. Verifique sua internet e tente novamente.', 0);
  } finally {
    clearTimeout(timer);
  }

  const texto = await resp.text();
  const dados = texto ? safeJson(texto) : null;

  if (!resp.ok) {
    // A API devolve { erro: "mensagem" } nos erros tratados
    const erroApi = (dados as { erro?: unknown } | null)?.erro;
    const msg =
      (typeof erroApi === 'string' && erroApi) ||
      (resp.status === 401 ? 'Sessão expirada. Faça login novamente.' : null) ||
      (resp.status === 429 ? 'Muitas tentativas. Aguarde um minuto.' : null) ||
      `Erro ${resp.status} ao falar com o servidor.`;
    throw new ApiError(msg, resp.status);
  }

  return dados as T;
}

function safeJson(texto: string): unknown {
  try {
    return JSON.parse(texto);
  } catch {
    return null;
  }
}

export const api = {
  get: <T>(caminho: string) => enviar<T>('GET', caminho),
  post: <T>(caminho: string, corpo?: unknown) => enviar<T>('POST', caminho, corpo ?? {}),
  put: <T>(caminho: string, corpo?: unknown) => enviar<T>('PUT', caminho, corpo ?? {}),
};
