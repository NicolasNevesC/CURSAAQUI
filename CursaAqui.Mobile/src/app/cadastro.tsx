import { useState } from 'react';
import { KeyboardAvoidingView, Platform, ScrollView, StyleSheet } from 'react-native';

import { Botao, Campo, MensagemErro } from '@/components/form';
import { ThemedView } from '@/components/themed-view';
import { MaxContentWidth, Spacing } from '@/constants/theme';
import { useAuth } from '@/lib/auth';

export default function CadastroScreen() {
  const { cadastrar } = useAuth();
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [cpf, setCpf] = useState('');
  const [telefone, setTelefone] = useState('');
  const [senha, setSenha] = useState('');
  const [erro, setErro] = useState<string | null>(null);
  const [enviando, setEnviando] = useState(false);

  async function onCadastrar() {
    if (!nome.trim() || !email.trim() || !senha) {
      setErro('Preencha nome, e-mail e senha.');
      return;
    }
    if (senha.length < 6) {
      setErro('A senha deve ter pelo menos 6 caracteres.');
      return;
    }
    setErro(null);
    setEnviando(true);
    try {
      // As demais validações (e-mail, CPF, duplicidade) são feitas pela API
      await cadastrar({
        nome: nome.trim(),
        email: email.trim(),
        senha,
        cpf: cpf.trim() || null,
        telefone: telefone.trim() || null,
      });
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Erro ao cadastrar.');
    } finally {
      setEnviando(false);
    }
  }

  return (
    <ThemedView style={styles.container}>
      <KeyboardAvoidingView behavior={Platform.OS === 'ios' ? 'padding' : undefined} style={styles.container}>
        <ScrollView contentContainerStyle={styles.conteudo} keyboardShouldPersistTaps="handled">
          <Campo label="Nome completo" value={nome} onChangeText={setNome} autoComplete="name" />
          <Campo
            label="E-mail"
            value={email}
            onChangeText={setEmail}
            autoCapitalize="none"
            autoComplete="email"
            keyboardType="email-address"
          />
          <Campo label="CPF (opcional)" value={cpf} onChangeText={setCpf} keyboardType="number-pad" />
          <Campo label="Telefone (opcional)" value={telefone} onChangeText={setTelefone} keyboardType="phone-pad" />
          <Campo
            label="Senha"
            value={senha}
            onChangeText={setSenha}
            secureTextEntry
            autoComplete="new-password"
            placeholder="Mínimo de 6 caracteres"
          />

          <MensagemErro texto={erro} />
          <Botao titulo="Criar conta" onPress={onCadastrar} carregando={enviando} />
        </ScrollView>
      </KeyboardAvoidingView>
    </ThemedView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1 },
  conteudo: {
    gap: Spacing.three,
    padding: Spacing.four,
    width: '100%',
    maxWidth: MaxContentWidth / 2,
    alignSelf: 'center',
  },
});
