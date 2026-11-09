import { useCallback, useEffect, useState } from 'react';

import { api } from '@/lib/api';

type Estado<T> = { dados: T | null; erro: string | null; carregando: boolean };

/** Busca um GET da API e expõe dados, erro, carregando e recarregar (pull-to-refresh). */
export function useApi<T>(caminho: string) {
  const [estado, setEstado] = useState<Estado<T>>({ dados: null, erro: null, carregando: true });
  const [versao, setVersao] = useState(0);

  useEffect(() => {
    let ativo = true;
    api
      .get<T>(caminho)
      .then((dados) => ativo && setEstado({ dados, erro: null, carregando: false }))
      .catch(
        (e) =>
          ativo &&
          setEstado((s) => ({ ...s, erro: e instanceof Error ? e.message : 'Erro inesperado.', carregando: false }))
      );
    return () => {
      ativo = false;
    };
  }, [caminho, versao]);

  const recarregar = useCallback(() => {
    setEstado((s) => ({ ...s, erro: null, carregando: true }));
    setVersao((v) => v + 1);
  }, []);

  return { ...estado, recarregar };
}
