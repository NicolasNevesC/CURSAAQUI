# 🎓 Cursa Aqui - Aplicação Windows Forms (.NET C#)
### Sistema de Gestão Interna (Administrador & Secretaria)

Aplicação desktop desenvolvida em **C# .NET com Windows Forms**, projetada especificamente para atender aos atores internos da instituição de ensino **Cursa Aqui**, conforme a modelagem orientada a objetos e casos de uso do sistema.

---

## 🏛️ Modelagem e Orientação a Objetos

A arquitetura respeita estritamente o diagrama de classes e especialização:

```
                  ┌───────────────────────────────┐
                  │        UsuarioInterno         │  (Classe Abstrata)
                  │ ───────────────────────────── │
                  │ + Id: int                     │
                  │ + Nome: string                │
                  │ + Email: string               │
                  │ + Senha: string               │
                  │ + Cpf: string                 │
                  │ + Ativo: bool                 │
                  │ + TipoPerfil: PerfilUsuario   │
                  │ ───────────────────────────── │
                  │ + ObterDescricaoPerfil()      │
                  │ + PodeExecutar(funcionalidade)│
                  └──────────────┬────────────────┘
                                 │
                 ▲───────────────┴───────────────▲
                 │ (Generalização)               │ (Generalização)
  ┌──────────────┴───────────────┐ ┌─────────────┴───────────────┐
  │        Administrador         │ │          Secretaria         │
  │ ──────────────────────────── │ │ ─────────────────────────── │
  │ + NivelAutoridade: string    │ │ + SetorAtendimento: string  │
  │ + UltimoAcessoGerencial      │ │ + AtendimentosRealizadosHoje│
  │ ──────────────────────────── │ │ ─────────────────────────── │
  │ Governança, Configurações,   │ │ Operação diária, Alunos,    │
  │ Permissões e Relatórios      │ │ Matrículas e Atendimentos   │
  └──────────────────────────────┘ └─────────────────────────────┘
```

---

## 📋 Casos de Uso Implementados

### 👑 Perfil: Administrador
1. **Gerenciar Usuários Internos:**
   - Cadastro de novos administradores e operadores da secretaria.
   - Listagem com busca textual e filtros por perfil.
   - Ativação/Inativação de contas de acesso.
2. **Gerenciar Permissões:**
   - Matriz interativa de privilégios por funcionalidade.
   - Concessão e revogação dinâmica de acesso para cada perfil.
   - Aplicação em tempo de execução e persistência.
3. **Gerenciar Cursos:**
   - Cadastro, edição e consulta do catálogo acadêmico.
   - Configuração de código, título, carga horária, docente, vagas e status.
   - Ativação e inativação de cursos.
4. **Visualizar Relatórios (Gerenciais e Analíticos):**
   - Painel de indicadores com cards de métricas (total de alunos, cursos ativos, conclusões e taxa de sucesso).
   - Gráfico visual de distribuição de matrículas por curso renderizado via GDI+.
   - Tabela analítica com aproveitamento médio por curso.
   - Exportação do relatório gerencial analítico em formato `.txt`.
5. **Configurar Parâmetros do Sistema:**
   - Definição do período letivo vigente (ex: `2026.1`).
   - Média mínima de nota para aprovação (ex: `7.0`).
   - Carga horária máxima simultânea permitida por estudante.
   - Trava global para abertura ou fechamento de novas matrículas.
   - Dados de contato e endereço da instituição.

---

### 📝 Perfil: Secretaria
1. **Gerenciar Matrículas:**
   - Efetivação de novas matrículas vinculando aluno existente a curso ativo.
   - Validação automática de vagas e prevenção de matrícula duplicada no mesmo curso.
   - Alteração de situação acadêmica: **Ativa**, **Renovada**, **Trancada**, **Cancelada** ou **Concluída**.
   - Lançamento de notas e percentual de progresso.
2. **Consultar Cadastro:**
   - Busca instantânea de estudantes por Nome, CPF ou E-mail.
   - Visualização da ficha cadastral completa (dados pessoais, contato e endereço).
   - Histórico escolar do estudante com todos os cursos matriculados, notas e situações.
   - Emissão e exportação da Ficha Cadastral em `.txt`.
3. **Registrar Atendimentos:**
   - Geração automática de protocolo de atendimento (ex: `ATEND-2026-001`).
   - Registro de solicitações de **Alunos** ou **Visitantes**.
   - Acompanhamento de histórico e registro de providências tomadas.
   - Transição de status: **Aberto**, **Em Andamento**, **Concluído** ou **Cancelado**.
4. **Gerar Relatórios Administrativos:**
   - Emissão e visualização operacional formatada:
     - *1. Relação de Alunos Matriculados por Curso / Período.*
     - *2. Relatório de Atendimentos Realizados pela Secretaria.*
     - *3. Relação de Pendências Acadêmicas (notas baixas e matrículas trancadas).*
   - Exportação dos relatórios administrativos em arquivo de texto.

---

## 🔑 Credenciais Pré-cadastradas para Demonstração

Para facilitar a navegação e homologação, a tela de login inclui botões de **Acesso Rápido**:

| Perfil | Nome | E-mail | Senha |
| :--- | :--- | :--- | :--- |
| **Administrador** | Carlos Eduardo Menezes | `admin@cursaaqui.com` | `123` |
| **Administrador** | Mariana Castro | `mariana.admin@cursaaqui.com` | `123` |
| **Secretaria** | Fernanda Oliveira | `secretaria@cursaaqui.com` | `123` |
| **Secretaria** | Lucas Ribeiro | `lucas.sec@cursaaqui.com` | `123` |

---

## 🚀 Como Executar

No terminal, a partir do diretório raiz ou da pasta `CursaAqui.App`:

```bash
cd "CursaAqui.App"
dotnet run
```
