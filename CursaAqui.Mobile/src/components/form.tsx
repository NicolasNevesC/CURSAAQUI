import { ActivityIndicator, Pressable, StyleSheet, TextInput, type TextInputProps } from 'react-native';

import { ThemedText } from '@/components/themed-text';
import { ThemedView } from '@/components/themed-view';
import { Spacing } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';

export function Campo({ label, style, ...rest }: TextInputProps & { label: string }) {
  const theme = useTheme();
  return (
    <ThemedView style={styles.campo}>
      <ThemedText type="smallBold">{label}</ThemedText>
      <TextInput
        placeholderTextColor={theme.textSecondary}
        style={[styles.input, { color: theme.text, backgroundColor: theme.backgroundElement }, style]}
        {...rest}
      />
    </ThemedView>
  );
}

type BotaoProps = {
  titulo: string;
  onPress: () => void;
  carregando?: boolean;
  variante?: 'primario' | 'secundario';
};

export function Botao({ titulo, onPress, carregando, variante = 'primario' }: BotaoProps) {
  const theme = useTheme();
  const primario = variante === 'primario';
  return (
    <Pressable
      onPress={onPress}
      disabled={carregando}
      style={({ pressed }) => [
        styles.botao,
        { backgroundColor: primario ? theme.primary : theme.backgroundElement },
        (pressed || carregando) && { opacity: 0.7 },
      ]}>
      {carregando ? (
        <ActivityIndicator color={primario ? '#ffffff' : theme.text} />
      ) : (
        <ThemedText style={{ color: primario ? '#ffffff' : theme.text }} type="smallBold">
          {titulo}
        </ThemedText>
      )}
    </Pressable>
  );
}

export function MensagemErro({ texto }: { texto: string | null }) {
  if (!texto) return null;
  return (
    <ThemedText type="small" themeColor="danger">
      {texto}
    </ThemedText>
  );
}

const styles = StyleSheet.create({
  campo: {
    gap: Spacing.one,
  },
  input: {
    borderRadius: 12,
    paddingHorizontal: Spacing.three,
    paddingVertical: 12,
    fontSize: 16,
  },
  botao: {
    borderRadius: 12,
    minHeight: 48,
    alignItems: 'center',
    justifyContent: 'center',
    paddingHorizontal: Spacing.three,
  },
});
