import { Link } from 'expo-router';
import { useMemo, useState } from 'react';
import { FlatList, Pressable, RefreshControl, StyleSheet, TextInput } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

import { Estado } from '@/components/estado';
import { ThemedText } from '@/components/themed-text';
import { ThemedView } from '@/components/themed-view';
import { BottomTabInset, MaxContentWidth, Spacing } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';
import type { Curso } from '@/lib/types';
import { useApi } from '@/lib/use-api';

export default function CursosScreen() {
  const theme = useTheme();
  const { dados, erro, carregando, recarregar } = useApi<Curso[]>('/api/cursos');
  const [busca, setBusca] = useState('');

  const filtrados = useMemo(() => {
    const b = busca.trim().toLowerCase();
    if (!dados || !b) return dados ?? [];
    return dados.filter((c) => `${c.titulo} ${c.categoria} ${c.area ?? ''}`.toLowerCase().includes(b));
  }, [dados, busca]);

  if (!dados) return <Estado erro={erro} onTentarNovamente={recarregar} />;

  return (
    <ThemedView style={styles.container}>
      <SafeAreaView style={styles.container} edges={['top']}>
        <FlatList
          data={filtrados}
          keyExtractor={(c) => String(c.id)}
          contentContainerStyle={styles.conteudo}
          refreshControl={<RefreshControl refreshing={carregando} onRefresh={recarregar} />}
          ListHeaderComponent={
            <ThemedView style={styles.cabecalho}>
              <ThemedText type="subtitle">Catálogo</ThemedText>
              <TextInput
                value={busca}
                onChangeText={setBusca}
                placeholder="Buscar por curso, categoria ou área"
                placeholderTextColor={theme.textSecondary}
                style={[styles.busca, { color: theme.text, backgroundColor: theme.backgroundElement }]}
              />
            </ThemedView>
          }
          ListEmptyComponent={<ThemedText themeColor="textSecondary">Nenhum curso encontrado.</ThemedText>}
          renderItem={({ item: c }) => (
            <Link href={{ pathname: '/curso/[id]', params: { id: c.id } }} asChild>
              <Pressable>
                <ThemedView type="backgroundElement" style={styles.cartao}>
                  <ThemedText type="small" themeColor="textSecondary">
                    {c.categoria} · {c.cargaHoraria}h · Nível {c.nivelMinimo}
                  </ThemedText>
                  <ThemedText type="smallBold">{c.titulo}</ThemedText>
                  <ThemedText type="small" themeColor="textSecondary">
                    {c.avaliacao > 0 ? `★ ${c.avaliacao.toFixed(1)}  ` : ''}
                    {c.progressoAluno != null ? `Seu progresso: ${c.progressoAluno}%` : ''}
                    {c.bloqueado ? '🔒 Nível insuficiente' : ''}
                  </ThemedText>
                </ThemedView>
              </Pressable>
            </Link>
          )}
        />
      </SafeAreaView>
    </ThemedView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1 },
  conteudo: {
    gap: Spacing.two,
    padding: Spacing.four,
    paddingBottom: BottomTabInset + Spacing.four,
    width: '100%',
    maxWidth: MaxContentWidth,
    alignSelf: 'center',
  },
  cabecalho: { gap: Spacing.three, marginBottom: Spacing.two },
  busca: { borderRadius: 12, paddingHorizontal: Spacing.three, paddingVertical: 12, fontSize: 16 },
  cartao: { borderRadius: 16, padding: Spacing.three, gap: Spacing.one },
});
