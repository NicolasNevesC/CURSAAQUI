import * as SecureStore from 'expo-secure-store';
import { Platform } from 'react-native';

// Token guardado no armazenamento seguro do aparelho (Keychain / Keystore).
// No navegador (expo web) o SecureStore não existe, então usa localStorage.
const CHAVE = 'cursaaqui_token';

export async function lerToken(): Promise<string | null> {
  if (Platform.OS === 'web') return globalThis.localStorage?.getItem(CHAVE) ?? null;
  return SecureStore.getItemAsync(CHAVE);
}

export async function salvarToken(token: string): Promise<void> {
  if (Platform.OS === 'web') return globalThis.localStorage?.setItem(CHAVE, token);
  await SecureStore.setItemAsync(CHAVE, token);
}

export async function apagarToken(): Promise<void> {
  if (Platform.OS === 'web') return globalThis.localStorage?.removeItem(CHAVE);
  await SecureStore.deleteItemAsync(CHAVE);
}
