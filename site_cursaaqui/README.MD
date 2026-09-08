<div align="center">

# 🎓 Cursa Aqui

**Plataforma Educacional Moderna, Acessível e Gratuita**

Transformando o aprendizado com tecnologia prática, cursos focados no mercado e certificação automática.

[![Status](https://img.shields.io/badge/Status-Ativo-success.svg)](#)
[![Licença](https://img.shields.io/badge/License-MIT-blue.svg)](#)
[![HTML5](https://img.shields.io/badge/HTML5-E34F26?logo=html5&logoColor=white)](#)
[![CSS3](https://img.shields.io/badge/CSS3-1572B6?logo=css3&logoColor=white)](#)
[![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?logo=javascript&logoColor=black)](#)

[Demonstração](#-como-executar-o-projeto) •
[Funcionalidades](#-funcionalidades) •
[Estrutura](#-estrutura-do-projeto) •
[Tecnologias](#-tecnologias-utilizadas)

---

</div>

## 📌 Sobre o Projeto

O **Cursa Aqui** é uma plataforma web educacional desenvolvida para democratizar o acesso ao conhecimento prático de tecnologia, design e negócios. Criada com foco em uma experiência fluida e intuitiva, a plataforma permite que estudantes aprendam no seu próprio ritmo, acompanhem seu desenvolvimento através de métricas em tempo real e conquistem certificados de conclusão válidos.

---

## ✨ Funcionalidades Principais

### 👨‍🎓 Para o Aluno
- **Catálogo Interativo de Cursos:** Busca, filtros por categoria, nível de dificuldade e carga horária.
- **Sala de Aula Dinâmica:** Visualização de aulas, ementa detalhada, materiais complementares (PDFs) e questionários de fixação.
- **Dashboard Personalizado:**
  - Acompanhamento de progresso percentual por curso.
  - Histórico de aulas e notas obtidas em testes.
  - Emissão e download de certificado digital ao concluir 100% da carga horária.
- **Área de Perfil:** Configuração de dados pessoais, avatar e preferências de estudo.

### 👨‍🏫 Para o Professor
- **Painel Administrativo:** Visão analítica de alunos matriculados e desempenho geral das turmas.
- **Gestão de Conteúdo:** Criação e estruturação de novos cursos, módulos e lições.
- **Upload de Materiais:** Inserção de apostilas, links úteis e exercícios práticos.

---

## 🛠️ Tecnologias Utilizadas

A plataforma foi construída priorizando leveza, rapidez e independência de frameworks pesados:

- **HTML5:** Marcação semântica com foco em SEO e acessibilidade.
- **CSS3 Moderno:** Design responsivo, variáveis CSS, temas e transições suaves.
- **JavaScript (ES6+):** Lógica reativa, gerenciamento de estado local (`localStorage`), simulação de autenticação e manipulação dinâmica do DOM.

---

## 📂 Estrutura de Arquivos

```bash
site_cursaaqui/
├── index.html        # Página inicial e apresentação da plataforma
├── cursos.html       # Catálogo completo e filtros de busca
├── course.html       # Visualizador de curso, aulas e questionários
├── aluno.html        # Painel do estudante e emissão de certificados
├── professor.html    # Painel administrativo para instrutores
├── style.css         # Estilização completa e responsividade
├── app.js            # Lógica geral e interações da interface
├── auth.js           # Gerenciamento de login, cadastro e sessões
├── courses.js        # Base de dados de cursos, módulos e quizzes
├── dashboard.js      # Lógica do painel de controle do aluno
├── certificate.js    # Gerador e validador de certificados
└── materiais/        # Apostilas e arquivos complementares
```

---

## 🚀 Como Executar o Projeto

Como o projeto é construído com tecnologias web puras, não há necessidade de instalação de dependências complexas ou compilação.

### Pré-requisitos
Qualquer navegador web moderno (Google Chrome, Firefox, Microsoft Edge, Opera, Safari).

### Passo a Passo

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/NicolasNevesC/CURSAAQUI.git
   ```

2. **Acesse a pasta do site:**
   ```bash
   cd CURSAAQUI/site_cursaaqui
   ```

3. **Inicie um servidor local (recomendado):**
   - Com Python:
     ```bash
     python -m http.server 3000
     ```
   - Ou com Node.js (`npx serve`):
     ```bash
     npx serve .
     ```
   - Ou simplesmente utilize a extensão **Live Server** do VS Code abrindo o `index.html`.

4. **Acesse no seu navegador:**
   Abra [http://localhost:3000](http://localhost:3000)

---

## 📄 Licença

Distribuído sob a licença **MIT**. Consulte o arquivo de licença para mais informações.

<div align="center">
Desenvolvido com dedicação para impulsionar a educação e o futuro dos estudantes.
</div>