import { RefreshControl, ScrollView, StyleSheet } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

import { Botao } from '@/components/form';
import { ThemedText } from '@/components/themed-text';
import { ThemedView } from '@/components/themed-view';
import { BottomTabInset, MaxContentWidth, Spacing } from '@/constants/theme';
import { useAuth } from '@/lib/auth';
import type { Certificado } from '@/lib/types';
import { useApi } from '@/lib/use-api';

export default function PerfilScreen() {
  const { usuario, sair } = useAuth();
  const { dados: certificados, erro, carregando, recarregar } = useApi<Certificado[]>('/api/alunos/me/certificados');

  return (
    <ThemedView style={styles.container}>
      <SafeAreaView style={styles.container} edges={['top']}>
        <ScrollView
          contentContainerStyle={styles.conteudo}
          refreshControl={<RefreshControl refreshing={carregando} onRefresh={recarregar} />}>
          <ThemedText type="subtitle">Perfil</ThemedText>

          <ThemedView type="backgroundElement" style={styles.cartao}>
            <ThemedText type="smallBold">{usuario?.nome}</ThemedText>
            <ThemedText type="small" themeColor="textSecondary">
              {usuario?.email}
            </ThemedText>
            {usuario?.telefone && (
              <ThemedText type="small" themeColor="textSecondary">
                {usuario.telefone}
              </ThemedText>
            )}
          </ThemedView>

          <ThemedText type="smallBold" style={styles.secao}>
            Certificados
          </ThemedText>
          {erro && (
            <ThemedText type="small" themeColor="danger">
              {erro}
            </ThemedText>
          )}
          {certificados?.length === 0 && (
            <ThemedText themeColor="textSecondary">Conclua um curso para emitir seu primeiro certificado.</ThemedText>
          )}
          {certificados?.map((c) => (
            <ThemedView key={c.id} type="backgroundElement" style={styles.cartao}>
              <ThemedText type="smallBold">🏆 {c.cursoTitulo}</ThemedText>
              <ThemedText type="small" themeColor="textSecondary">
                {c.cargaHoraria}h · emitido em {new Date(c.dataEmissao).toLocaleDateString('pt-BR')}
              </ThemedText>
              <ThemedText type="code">Código: {c.codigoValidacao}</ThemedText>
            </ThemedView>
          ))}

          <ThemedView style={styles.secao}>
            <Botao titulo="Sair" variante="secundario" onPress={sair} />
          </ThemedView>
        </ScrollView>
      </SafeAreaView>
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
  cartao: { borderRadius: 16, padding: Spacing.three, gap: Spacing.one },
  secao: { marginTop: Spacing.two },
});
