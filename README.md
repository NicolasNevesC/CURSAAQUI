<div align="center">

# 🎓 Cursa Aqui

**Plataforma Educacional Moderna, Acessível e Gratuita**

Transformando o aprendizado com tecnologia prática, cursos focados no mercado e certificação automática.

[![Status](https://img.shields.io/badge/Status-Ativo-success.svg)](#)
[![Licença](https://img.shields.io/badge/License-MIT-blue.svg)](#)
[![HTML5](https://img.shields.io/badge/HTML5-E34F26?logo=html5&logoColor=white)](#)
[![CSS3](https://img.shields.io/badge/CSS3-1572B6?logo=css3&logoColor=white)](#)
[![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?logo=javascript&logoColor=black)](#)

<br>

[![Acessar Demonstração](https://img.shields.io/badge/🌐%20Acessar%20Demonstração-Online-00C853?style=for-the-badge&logo=googlechrome&logoColor=white)](https://nicolasnevesc.github.io/CURSAAQUI/)

<br>

[🌐 Demonstração Online](https://nicolasnevesc.github.io/CURSAAQUI/) •
[Funcionalidades](#-funcionalidades-principais) •
[Estrutura](#-estrutura-do-repositório) •
[Como Executar](#-como-executar-o-projeto)

---

</div>

## 📌 Sobre o Projeto

O **Cursa Aqui** é uma plataforma educacional gratuita criada para oferecer cursos de alta qualidade, desenvolvidos com foco em impulsionar a carreira profissional dos estudantes. A proposta central é simples: aprender sem limites, com acesso livre a conteúdos, aulas interativas e certificados, tudo em um ambiente intuitivo e moderno.

Ao entrar no site, o usuário encontra uma interface fluida com navegação direta para o catálogo de cursos, perguntas frequentes, suporte e os painéis dedicados para estudantes e professores.

---

## 🌐 Demonstração Online

O site está hospedado e disponível para visualização e testes em tempo real no GitHub Pages:

👉 **[https://nicolasnevesc.github.io/CURSAAQUI/](https://nicolasnevesc.github.io/CURSAAQUI/)**

---

## ✨ Funcionalidades Principais

### 👨‍🎓 Para o Aluno
- **Catálogo Interativo de Cursos:** Busca em tempo real, filtros por categoria, nível de dificuldade e carga horária.
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

A plataforma foi construída priorizando leveza, rapidez e independência de dependências pesadas:

- **HTML5:** Marcação semântica com foco em SEO e acessibilidade.
- **CSS3 Moderno:** Design responsivo, variáveis CSS, temas e transições suaves.
- **JavaScript (ES6+):** Lógica reativa, gerenciamento de estado local (`localStorage`), autenticação e manipulação dinâmica do DOM.

---

## 📂 Estrutura do Repositório

```bash
CursaAqui/
├── .github/workflows/deploy.yml # Pipeline de deploy automatizado no GitHub Pages
├── diagramas/                   # Diagramas de arquitetura e DER do banco de dados
├── protótipo/                   # Wireframes e protótipos de interface
├── site_cursaaqui/              # Código-fonte completo da aplicação web
│   ├── index.html               # Página inicial e apresentação da plataforma
│   ├── cursos.html              # Catálogo completo e filtros de busca
│   ├── course.html              # Visualizador de curso, aulas e questionários
│   ├── aluno.html               # Painel do estudante e emissão de certificados
│   ├── professor.html           # Painel administrativo para instrutores
│   ├── style.css                # Estilização completa e responsividade
│   ├── app.js                   # Lógica geral e interações da interface
│   ├── auth.js                  # Gerenciamento de login, cadastro e sessões
│   ├── courses.js               # Base de dados de cursos, módulos e quizzes
│   ├── dashboard.js             # Lógica do painel de controle do aluno
│   └── certificate.js           # Gerador e validador de certificados
├── index.html                   # Redirecionamento automático para a aplicação
└── README.md
```

---

## 🚀 Como Executar o Projeto Localmente

Como o projeto é construído com tecnologias web puras, você pode rodá-lo instantaneamente sem precisar instalar pacotes via `npm`:

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
   - Ou abra o arquivo `index.html` diretamente no seu navegador (ou via extensão Live Server do VS Code).

4. **Acesse no seu navegador:**
   Abra [http://localhost:3000](http://localhost:3000)

---

## 📄 Licença

Distribuído sob a licença **MIT**.

<div align="center">
Desenvolvido com dedicação para impulsionar a educação e o futuro dos estudantes.
</div>
