import { Link } from 'expo-router';
import { Pressable, RefreshControl, ScrollView, StyleSheet } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

import { BarraProgresso, Estado } from '@/components/estado';
import { ThemedText } from '@/components/themed-text';
import { ThemedView } from '@/components/themed-view';
import { BottomTabInset, MaxContentWidth, Spacing } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';
import type { DashboardAluno } from '@/lib/types';
import { useApi } from '@/lib/use-api';

export default function DashboardScreen() {
  const theme = useTheme();
  const { dados, erro, carregando, recarregar } = useApi<DashboardAluno>('/api/alunos/me/dashboard');

  if (!dados) return <Estado erro={erro} onTentarNovamente={recarregar} />;

  const xpPct = dados.xpProximoNivel > 0 ? (dados.xp / dados.xpProximoNivel) * 100 : 100;

  return (
    <ThemedView style={styles.container}>
      <SafeAreaView style={styles.container} edges={['top']}>
        <ScrollView
          contentContainerStyle={styles.conteudo}
          refreshControl={<RefreshControl refreshing={carregando} onRefresh={recarregar} />}>
          <ThemedText type="subtitle">Olá, {dados.nome.split(' ')[0]}!</ThemedText>

          {/* Nível e XP */}
          <ThemedView type="backgroundElement" style={styles.cartao}>
            <ThemedText type="smallBold">
              Nível {dados.nivel} · {dados.xp} / {dados.xpProximoNivel} XP
            </ThemedText>
            <BarraProgresso valor={xpPct} cor={theme.primary} fundo={theme.backgroundSelected} />
          </ThemedView>

          {/* Resumo */}
          <ThemedView style={styles.linha}>
            <Resumo numero={dados.cursosEmAndamento} rotulo="Em andamento" />
            <Resumo numero={dados.cursosConcluidos} rotulo="Concluídos" />
            <Resumo numero={dados.certificados} rotulo="Certificados" />
          </ThemedView>

          {/* Meus cursos */}
          <ThemedText type="smallBold" style={styles.secao}>
            Meus cursos
          </ThemedText>
          {dados.cursos.length === 0 ? (
            <ThemedText themeColor="textSecondary">
              Você ainda não iniciou nenhum curso. Veja o catálogo na aba Cursos.
            </ThemedText>
          ) : (
            dados.cursos.map((c) => (
              <Link key={c.cursoId} href={{ pathname: '/curso/[id]', params: { id: c.cursoId } }} asChild>
                <Pressable>
                  <ThemedView type="backgroundElement" style={styles.cartao}>
                    <ThemedText type="smallBold">{c.titulo}</ThemedText>
                    <BarraProgresso valor={c.progresso} cor={theme.primary} fundo={theme.backgroundSelected} />
                    <ThemedText type="small" themeColor="textSecondary">
                      {c.progresso}% · {c.status}
                    </ThemedText>
                  </ThemedView>
                </Pressable>
              </Link>
            ))
          )}

          {/* Conquistas */}
          <ThemedText type="smallBold" style={styles.secao}>
            Conquistas
          </ThemedText>
          <ThemedView style={styles.grade}>
            {dados.conquistas.map((q) => (
              <ThemedView
                key={q.id}
                type="backgroundElement"
                style={[styles.conquista, !q.desbloqueada && { opacity: 0.4 }]}>
                <ThemedText style={styles.icone}>{q.icone}</ThemedText>
                <ThemedText type="small" style={styles.centro}>
                  {q.titulo}
                </ThemedText>
              </ThemedView>
            ))}
          </ThemedView>
        </ScrollView>
      </SafeAreaView>
    </ThemedView>
  );
}

function Resumo({ numero, rotulo }: { numero: number; rotulo: string }) {
  return (
    <ThemedView type="backgroundElement" style={[styles.cartao, styles.resumo]}>
      <ThemedText type="subtitle">{numero}</ThemedText>
      <ThemedText type="small" themeColor="textSecondary" style={styles.centro}>
        {rotulo}
      </ThemedText>
    </ThemedView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1 },
  conteudo: {
    gap: Spacing.three,
    padding: Spacing.four,
    paddingBottom: BottomTabInset + Spacing.four,
    width: '100%',
    maxWidth: MaxContentWidth,
    alignSelf: 'center',
  },
  cartao: { borderRadius: 16, padding: Spacing.three, gap: Spacing.two },
  linha: { flexDirection: 'row', gap: Spacing.two },
  resumo: { flex: 1, alignItems: 'center' },
  secao: { marginTop: Spacing.two },
  grade: { flexDirection: 'row', flexWrap: 'wrap', gap: Spacing.two },
  conquista: { width: '23%', minWidth: 72, alignItems: 'center', borderRadius: 12, padding: Spacing.two, gap: Spacing.one },
  icone: { fontSize: 28, lineHeight: 34 },
  centro: { textAlign: 'center' },
});
