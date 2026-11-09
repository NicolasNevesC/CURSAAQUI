import { createContext, useContext, useEffect, useState, type PropsWithChildren } from 'react';

import { api, ApiError, definirToken } from '@/lib/api';
import { apagarToken, lerToken, salvarToken } from '@/lib/token-storage';
import type { AuthResponse, CadastroRequest, Usuario } from '@/lib/types';

type AuthState = {
  usuario: Usuario | null;
  carregando: boolean;
  entrar: (email: string, senha: string) => Promise<void>;
  cadastrar: (dados: CadastroRequest) => Promise<void>;
  sair: () => Promise<void>;
};

const AuthContext = createContext<AuthState | null>(null);

export function AuthProvider({ children }: PropsWithChildren) {
  const [usuario, setUsuario] = useState<Usuario | null>(null);
  const [carregando, setCarregando] = useState(true);

  // Ao abrir o app, retoma a sessão salva (se o token ainda for válido)
  useEffect(() => {
    (async () => {
      try {
        const salvo = await lerToken();
        if (!salvo) return;
        definirToken(salvo);
        setUsuario(await api.get<Usuario>('/api/auth/me'));
      } catch (e) {
        if (e instanceof ApiError && (e.status === 401 || e.status === 404)) await limpar();
        else definirToken(null);
      } finally {
        setCarregando(false);
      }
    })();
  }, []);

  async function limpar() {
    definirToken(null);
    await apagarToken();
    setUsuario(null);
  }

  async function aplicar(resp: AuthResponse) {
    // O app mobile é a área do aluno; os demais perfis usam o desktop
    if (resp.usuario.perfil !== 'Aluno') {
      throw new ApiError('O app é exclusivo para alunos. Professores e administração usam o sistema desktop.', 403);
    }
    definirToken(resp.token);
    await salvarToken(resp.token);
    setUsuario(resp.usuario);
  }

  const valor: AuthState = {
    usuario,
    carregando,
    entrar: async (email, senha) => aplicar(await api.post<AuthResponse>('/api/auth/login', { email, senha })),
    cadastrar: async (dados) => aplicar(await api.post<AuthResponse>('/api/auth/cadastro', dados)),
    sair: limpar,
  };

  return <AuthContext.Provider value={valor}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth precisa estar dentro de <AuthProvider>.');
  return ctx;
}
