import { ActivityIndicator, StyleSheet } from 'react-native';

import { Botao } from '@/components/form';
import { ThemedText } from '@/components/themed-text';
import { ThemedView } from '@/components/themed-view';
import { Spacing } from '@/constants/theme';

/** Tela de carregamento / erro usada enquanto um GET da API não termina. */
export function Estado({ erro, onTentarNovamente }: { erro: string | null; onTentarNovamente: () => void }) {
  return (
    <ThemedView style={styles.container}>
      {erro ? (
        <>
          <ThemedText themeColor="danger" style={styles.texto}>
            {erro}
          </ThemedText>
          <Botao titulo="Tentar novamente" variante="secundario" onPress={onTentarNovamente} />
        </>
      ) : (
        <>
          <ActivityIndicator size="large" />
          <ThemedText type="small" themeColor="textSecondary" style={styles.texto}>
            Carregando… a primeira conexão pode demorar alguns segundos.
          </ThemedText>
        </>
      )}
    </ThemedView>
  );
}

export function BarraProgresso({ valor, cor, fundo }: { valor: number; cor: string; fundo: string }) {
  const pct = Math.max(0, Math.min(100, valor));
  return (
    <ThemedView style={[styles.barra, { backgroundColor: fundo }]}>
      <ThemedView style={{ width: `${pct}%`, height: '100%', backgroundColor: cor, borderRadius: 4 }} />
    </ThemedView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    gap: Spacing.three,
    padding: Spacing.four,
  },
  texto: { textAlign: 'center' },
  barra: { height: 8, borderRadius: 4, overflow: 'hidden' },
});
