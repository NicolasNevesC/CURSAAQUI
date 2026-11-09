import { Stack, useLocalSearchParams } from 'expo-router';
import * as WebBrowser from 'expo-web-browser';
import { useState } from 'react';
import { Pressable, ScrollView, StyleSheet } from 'react-native';

import { BarraProgresso, Estado } from '@/components/estado';
import { Botao, MensagemErro } from '@/components/form';
import { ThemedText } from '@/components/themed-text';
import { ThemedView } from '@/components/themed-view';
import { MaxContentWidth, Spacing } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';
import { api, API_URL } from '@/lib/api';
import type { CursoDetalhe, Material } from '@/lib/types';
import { useApi } from '@/lib/use-api';

export default function CursoScreen() {
  const theme = useTheme();
  const { id } = useLocalSearchParams<{ id: string }>();
  const { dados, erro, recarregar } = useApi<CursoDetalhe>(`/api/cursos/${id}`);
  const [iniciando, setIniciando] = useState(false);
  const [erroAcao, setErroAcao] = useState<string | null>(null);

  if (!dados) return <Estado erro={erro} onTentarNovamente={recarregar} />;

  const { curso, disciplinas, materiais } = dados;
  const matriculado = curso.progressoAluno != null;

  async function iniciar() {
    setErroAcao(null);
    setIniciando(true);
    try {
      await api.post(`/api/cursos/${curso.id}/iniciar`);
      recarregar();
    } catch (e) {
      setErroAcao(e instanceof Error ? e.message : 'Não foi possível iniciar o curso.');
    } finally {
      setIniciando(false);
    }
  }

  function abrirMaterial(m: Material) {
    // Uploads da API vêm como caminho relativo (/uploads/...)
    const url = /^https?:\/\//.test(m.urlConteudo) ? m.urlConteudo : `${API_URL}${m.urlConteudo}`;
    WebBrowser.openBrowserAsync(url);
  }

  return (
    <ThemedView style={styles.container}>
      <Stack.Screen options={{ title: curso.titulo }} />
      <ScrollView contentContainerStyle={styles.conteudo}>
        <ThemedText type="small" themeColor="textSecondary">
          {curso.categoria} · {curso.cargaHoraria}h · Nível mínimo {curso.nivelMinimo}
        </ThemedText>
        <ThemedText type="subtitle">{curso.titulo}</ThemedText>
        {curso.professorNome && <ThemedText themeColor="textSecondary">Prof. {curso.professorNome}</ThemedText>}
        {curso.descricao && <ThemedText>{curso.descricao}</ThemedText>}

        {matriculado ? (
          <ThemedView type="backgroundElement" style={styles.cartao}>
            <ThemedText type="smallBold">Seu progresso: {curso.progressoAluno}%</ThemedText>
            <BarraProgresso valor={curso.progressoAluno ?? 0} cor={theme.primary} fundo={theme.backgroundSelected} />
          </ThemedView>
        ) : (
          <>
            <MensagemErro texto={erroAcao} />
            <Botao titulo="Iniciar curso" onPress={iniciar} carregando={iniciando} />
          </>
        )}

        {disciplinas.length > 0 && (
          <>
            <ThemedText type="smallBold" style={styles.secao}>
              Ementa
            </ThemedText>
            {disciplinas.map((d) => (
              <ThemedView key={d.id} type="backgroundElement" style={styles.cartao}>
                <ThemedText type="smallBold">
                  {d.ordem}. {d.nome}
                </ThemedText>
                {d.conteudo && (
                  <ThemedText type="small" themeColor="textSecondary">
                    {d.conteudo}
                  </ThemedText>
                )}
              </ThemedView>
            ))}
          </>
        )}

        {materiais.length > 0 && (
          <>
            <ThemedText type="smallBold" style={styles.secao}>
              Materiais
            </ThemedText>
            {materiais.map((m) => (
              <Pressable key={m.id} onPress={() => abrirMaterial(m)}>
                <ThemedView type="backgroundElement" style={styles.cartao}>
                  <ThemedText type="smallBold">📄 {m.titulo}</ThemedText>
                  <ThemedText type="small" themeColor="textSecondary">
                    {m.tipo}
                  </ThemedText>
                </ThemedView>
              </Pressable>
            ))}
          </>
        )}
      </ScrollView>
    </ThemedView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1 },
  conteudo: {
    gap: Spacing.three,
    padding: Spacing.four,
    paddingBottom: Spacing.six,
    width: '100%',
    maxWidth: MaxContentWidth,
    alignSelf: 'center',
  },
  cartao: { borderRadius: 16, padding: Spacing.three, gap: Spacing.one },
  secao: { marginTop: Spacing.two },
});
