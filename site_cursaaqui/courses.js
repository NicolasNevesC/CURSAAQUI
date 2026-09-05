// ============================================================
// courses.js - Catálogo de Cursos da Plataforma
//
// Este arquivo exporta o array global 'courses', onde cada
// objeto representa um curso com as seguintes propriedades:
//
//   id           → identificador único numérico do curso
//   title        → nome completo exibido na plataforma
//   category     → área de conhecimento (usado nos filtros)
//   teacher      → nome do professor responsável
//   duration     → carga horária estimada (ex: "40h")
//   rating       → avaliação média de 0 a 5
//   progress     → percentual de conclusão do usuário (0 a 100)
//   image        → URL da imagem de capa (Unsplash)
//   lessons      → array de strings com os títulos das aulas
//   quiz         → array de objetos de perguntas do quiz:
//                    question → enunciado da questão (aceita HTML)
//                    options  → array de strings com as alternativas
//                    correct  → índice (0-based) da alternativa correta
//
// O array também é exposto em window.courses para acesso global
// e exportado via module.exports para compatibilidade com Node.js.
// ============================================================
var courses = [
  {
    id: 1,
    title: "Análise e Desenvolvimento de Sistemas - Manual Acadêmico",
    category: "Engenharia de Software",
    area: "Engenharia de Software & Programação",
    teacher: "Coordenação Acadêmica",
    duration: "40h",
    rating: 5.0,
    progress: 0,
    minLevel: 1,
    image: "https://images.unsplash.com/photo-1498050108023-c5249f4df085?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/ads_manual_academico.pdf",
    lessons: [
      "1. Requisitos, Ciclos de Vida e Restrições de Projeto",
      "1.1. Restrições de Projeto",
      "1.2. O Papel da Prototipagem",
      "1.3. Processos Iterativos e Incrementais",
      "2. Princípios Fundamentais do Paradigma de Orientação a Objetos (POO)",
      "4. Verificação, Validação e Testes de Software",
      "5. Processo Unificado (RUP) e Dimensão Dinâmica",
      "10. Estruturas de Algoritmos e Tipologia de Dados"
    ],
    quiz: [
      {
        question: "Um analista foi contratado para desenvolver um sistema de pesquisa de DVDs em lojas virtuais. O sistema deverá solicitar ao usuário um título de DVD, que será usado para realizar a pesquisa nas bases de dados das lojas conveniadas. Ao detectar a disponibilidade do DVD solicitado, o sistema armazenará temporariamente os dados das lojas (nome, preço, data prevista para entrega do produto) e exibirá as informações ordenadas de forma crescente pelo preço do produto. O cliente poderá, então, selecionar uma das opções exibidas para efetuar a compra do DVD desejado através do redirecionamento para o site da loja correspondente.<br><br>Considerando as boas práticas de Engenharia de Requisitos para a modelagem conceitual do sistema, assinale a alternativa correta sobre as restrições de projeto e os requisitos funcionais descritos:",
        options: [
          "A. O armazenamento temporário dos dados de preço e o redirecionamento ao site externo constituem requisitos não-funcionais de desempenho.",
          "B. A exigência de ordenação crescente pelo preço do produto atua estritamente como uma restrição de hardware legado.",
          "C. Solicitar um título de DVD, realizar a pesquisa nas bases de dados e exibir as informações ordenadas são Requisitos Funcionais do sistema.",
          "D. A especificação do protocolo de comunicação segura (HTTPS) para o redirecionamento é classificada como um Requisito de Negócio e não técnico.",
          "E. O sistema de busca por título de DVD é classificado exclusivamente como um Atributo de Qualidade de Portabilidade."
        ],
        correct: 2
      },
      {
        question: "Durante o levantamento de requisitos de um sistema acadêmico, o cliente manifestou o forte desejo de que a interface gráfica fosse intuitiva, elegante e que respondesse aos cliques em menos de 2 segundos. Contudo, a equipe técnica identificou que os servidores legados da instituição utilizam uma versão antiga de banco de dados que limita o tempo de resposta mínimo para 3 segundos em consultas complexas.<br><br>Com base nos conceitos de Metas de Design, Restrições de Projeto e Requisitos de Qualidade, analise o cenário e assinale o parecer correto:",
        options: [
          "A. O desejo do cliente por uma interface intuitiva e elegante deve ser mapeado estritamente como um Requisito Funcional obrigatório no backlog.",
          "B. O tempo de resposta de 2 segundos desejado pelo cliente constitui uma Meta de Usuário, enquanto o limite técnico de 3 segundos imposto pelo servidor legado atua como uma Restrição de Projeto.",
          "C. As restrições técnicas do banco de dados legado devem ser completamente ignoradas no documento de arquitetura final.",
          "D. O requisito de elegância da interface substitui a necessidade de validação dos testes de carga do banco de dados.",
          "E. A limitação dos servidores de banco de dados é classificada como um caso de uso secundário assíncrono."
        ],
        correct: 1
      }
    ]
  },
  {
    id: 2,
    title: "CLOUD COMPUTING E DEVOPS",
    category: "Infraestrutura & Nuvem",
    area: "Infraestrutura, Nuvem & Cibersegurança",
    teacher: "Prof. Ricardo Silva",
    duration: "60h",
    rating: 4.9,
    progress: 0,
    minLevel: 3,
    image: "https://images.unsplash.com/photo-1451187580459-43490279c0fa?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/cloud_computing_devops.pdf",
    lessons: [
      "1. Introdução à Computação em Nuvem (AWS, Azure, GCP)",
      "2. Containers e Orquestração com Docker e Kubernetes",
      "3. Integração e Entrega Contínua (CI/CD Pipeline)",
      "4. Infraestrutura como Código (IaC com Terraform)",
      "5. Monitoramento, Logs e Segurança em DevOps"
    ],
    quiz: [
      {
        question: "Qual o principal benefício do uso de Containers (ex: Docker) em relação a Máquinas Virtuais tradicionais em ambientes DevOps?",
        options: [
          "A. Containers virtualizam o sistema operacional inteiro com hardware dedicado.",
          "B. Containers compartilham o kernel do SO hospedeiro, sendo extremamente leves e com rápida inicialização.",
          "C. Containers não necessitam de rede ou portas configuradas.",
          "D. Containers eliminam a necessidade de utilizar repositórios Git."
        ],
        correct: 1
      }
    ]
  },
  {
    id: 3,
    title: "DESENV MOBILE JAVASCRIPT(OPT)",
    category: "Desenvolvimento Mobile",
    area: "Engenharia de Software & Programação",
    teacher: "Profª. Amanda Costa",
    duration: "50h",
    rating: 4.8,
    progress: 0,
    minLevel: 2,
    image: "https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/desenv_mobile_js.pdf",
    lessons: [
      "1. Fundamentos de React Native e Expo Framework",
      "2. Componentes e Estilização Flexbox para Mobile",
      "3. Gerenciamento de Estado e Navegação (React Navigation)",
      "4. Consumo de APIs RESTful e Armazenamento Local",
      "5. Publicação de Apps na App Store e Google Play"
    ],
    quiz: [
      {
        question: "No React Native, qual componente é utilizado para exibir elementos em lista de forma otimizada para alto desempenho?",
        options: [
          "A. ScrollView",
          "B. FlatList",
          "C. ViewContainer",
          "D. ListViewBox"
        ],
        correct: 1
      }
    ]
  },
  {
    id: 4,
    title: "DESENVOLVIMENTO WEB COM .NET",
    category: "Desenvolvimento Web",
    area: "Engenharia de Software & Programação",
    teacher: "Prof. Carlos Eduardo",
    duration: "55h",
    rating: 4.9,
    progress: 0,
    minLevel: 3,
    image: "https://images.unsplash.com/photo-1555066931-4365d14bab8c?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/desenv_web_dotnet.pdf",
    lessons: [
      "1. C# Avançado e Paradigmas Orientados a Objetos",
      "2. Arquitetura ASP.NET Core Web API",
      "3. Mapeamento Objeto-Relacional com Entity Framework Core",
      "4. Autenticação e Autorização com JWT e Identity",
      "5. Testes Unitários e Boas Práticas (Clean Code)"
    ],
    quiz: [
      {
        question: "No Entity Framework Core, qual ferramenta é utilizada para aplicar alterações de modelo ao banco de dados?",
        options: [
          "A. Database Query Builder",
          "B. Migrations (dotnet ef migrations add)",
          "C. JSON Schema Serializer",
          "D. SQL Server Direct Injection"
        ],
        correct: 1
      }
    ]
  },
  {
    id: 5,
    title: "EMPREENDEDORISMO EM TI",
    category: "Gestão & Negócios",
    area: "Negócios, Sociedade & Humanidades",
    teacher: "Profª. Patricia Lima",
    duration: "30h",
    rating: 4.7,
    progress: 0,
    minLevel: 1,
    image: "https://images.unsplash.com/photo-1522071820081-009f0129c71c?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/empreendedorismo_ti.pdf",
    lessons: [
      "1. Modelos de Negócio Inovadores e Canvas",
      "2. Validação de Ideias e Construção do MVP",
      "3. Pitching, Captação de Recursos e Venture Capital",
      "4. Gestão Ágil de Produtos (Scrum & Kanban)",
      "5. Marketing Digital e Aquisição de Clientes"
    ],
    quiz: [
      {
        question: "O termo MVP (Minimum Viable Product) no ecossistema de startups de TI refere-se a:",
        options: [
          "A. O produto final com todas as funcionalidades complexas concluídas.",
          "B. A versão mais simples de um produto que permite validar a hipótese de negócio com usuários reais.",
          "C. Um protótipo em papel sem qualquer interação digital.",
          "D. A estimativa financeira mínima exigida por investidores anjo."
        ],
        correct: 1
      }
    ]
  },
  {
    id: 6,
    title: "RELACOES ETNICO-RACIAIS E AFRODESCENDENCIA",
    category: "Ciências Humanas & Sociais",
    area: "Negócios, Sociedade & Humanidades",
    teacher: "Prof. Dr. Marcos Oliveira",
    duration: "40h",
    rating: 5.0,
    progress: 0,
    minLevel: 1,
    image: "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/relacoes_etnico_raciais.pdf",
    lessons: [
      "1. História e Cultura Afro-Brasileira e Africana",
      "2. Análise Crítica das Relações Étnico-Raciais no Brasil",
      "3. Políticas de Ações Afirmativas e Direitos Humanos",
      "4. Diversidade, Inclusão e Equidade no Ambiente Corporativo",
      "5. Desafios Contemporâneos e Antirracismo em Prática"
    ],
    quiz: [
      {
        question: "Qual o marco legislativo fundamental no Brasil que tornou obrigatório o ensino da história e cultura afro-brasileira nas escolas?",
        options: [
          "A. Lei nº 10.639/2003",
          "B. Lei Áurea de 1888",
          "C. Código Civil de 2002",
          "D. Marco Civil da Internet"
        ],
        correct: 0
      }
    ]
  },
  {
    id: 7,
    title: "REDES",
    category: "Redes & Segurança",
    area: "Infraestrutura, Nuvem & Cibersegurança",
    teacher: "Prof. Fernando Santos",
    duration: "45h",
    rating: 4.8,
    progress: 0,
    minLevel: 2,
    image: "https://images.unsplash.com/photo-1544197150-b99a580bb7a8?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/redes_computadores.pdf",
    lessons: [
      "1. Modelo OSI e Arquitetura de Protocolos TCP/IP",
      "2. Endereçamento IPv4/IPv6 e Roteamento",
      "3. Serviços de Rede (DNS, DHCP, HTTP, FTP, SSH)",
      "4. Segurança em Redes: Firewalls, VPNs e Criptografia",
      "5. Monitoramento e Análise de Pacotes com Wireshark"
    ],
    quiz: [
      {
        question: "No modelo de referência OSI de 7 camadas, em qual camada operam os roteadores para realizar o encaminhamento de pacotes por IP?",
        options: [
          "A. Camada 2 (Enlace de Dados)",
          "B. Camada 3 (Rede)",
          "C. Camada 4 (Transporte)",
          "D. Camada 7 (Aplicação)"
        ],
        correct: 1
      }
    ]
  },
  {
    id: 8,
    title: "Inteligência Artificial e Machine Learning com Python",
    category: "Data Science & IA",
    area: "Dados & Inteligência Artificial",
    teacher: "Profª. Dra. Helena Zhang",
    duration: "65h",
    rating: 4.9,
    progress: 0,
    minLevel: 3,
    image: "https://images.unsplash.com/photo-1677442136019-21780efad99a?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/ia_machine_learning_python.pdf",
    lessons: [
      "1. Introdução à IA, Ciência de Dados e ecossistema Python",
      "2. Aprendizado Supervisicionado: Regressão e Classificação",
      "3. Aprendizado Não-Supervisionado: Clustering e K-Means",
      "4. Redes Neurais Artificiais e Deep Learning com TensorFlow",
      "5. Modelos de Linguagem (LLMs) e IA Generativa"
    ],
    quiz: [
      {
        question: "Qual biblioteca em Python é amplamente utilizada para manipulação e análise de dados estruturados em DataFrames?",
        options: [
          "A. NumPy",
          "B. Pandas",
          "C. Matplotlib",
          "D. Scikit-Learn"
        ],
        correct: 1
      }
    ]
  },
  {
    id: 9,
    title: "Cybersecurity e Pentest Prático",
    category: "Segurança da Informação",
    area: "Infraestrutura, Nuvem & Cibersegurança",
    teacher: "Prof. Gabriel Vianna",
    duration: "50h",
    rating: 5.0,
    progress: 0,
    minLevel: 3,
    image: "https://images.unsplash.com/photo-1563986768609-322da13575f3?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/cybersecurity_pentest.pdf",
    lessons: [
      "1. Fundamentos de Cibersegurança e Etapas do Pentest",
      "2. Análise de Vulnerabilidades e Reconhecimento de Redes",
      "3. Exploração de Vulnerabilidades Web (OWASP Top 10)",
      "4. Engenharia Social e Defesa contra Phishing",
      "5. Relatórios Técnicos e Remediação de Segurança"
    ],
    quiz: [
      {
        question: "Qual das opções representa uma vulnerabilidade crítica listada no OWASP Top 10 onde dados não sanitizados executam comandos no banco de dados?",
        options: [
          "A. SQL Injection (SQLi)",
          "B. Cross-Site Scripting (XSS)",
          "C. Broken Authentication",
          "D. Insecure Direct Object References (IDOR)"
        ],
        correct: 0
      }
    ]
  },
  {
    id: 10,
    title: "UI/UX Design e Prototipagem no Figma",
    category: "Design & Experiência do Usuário",
    area: "Design, Produto & Gestão Ágil",
    teacher: "Profª. Beatriz Mendes",
    duration: "35h",
    rating: 4.8,
    progress: 0,
    minLevel: 1,
    image: "https://images.unsplash.com/photo-1581291518633-83b4ebd1d83e?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/ui_ux_design_figma.pdf",
    lessons: [
      "1. Fundamentos de UX: Pesquisa de Usuário e Personas",
      "2. Arquitetura de Informação e Wireframing",
      "3. Design System, Cores e Tipografia Responsiva",
      "4. Prototipagem Interativa Avançada no Figma",
      "5. Testes de Usabilidade e Entrega para Desenvolvedores (Hand-off)"
    ],
    quiz: [
      {
        question: "No processo de UX Design, o que é um Wireframe?",
        options: [
          "A. O código HTML e CSS final do site pronto para produção.",
          "B. Um esboço visual de baixa fidelidade que estrutura o layout e a hierarquia da interface.",
          "C. Um teste automatizado de carga no servidor web.",
          "D. A paleta de cores definitiva do aplicativo."
        ],
        correct: 1
      }
    ]
  },
  {
    id: 11,
    title: "Banco de Dados SQL & NoSQL (PostgreSQL e MongoDB)",
    category: "Banco de Dados",
    area: "Dados & Inteligência Artificial",
    teacher: "Prof. Lucas Albuquerque",
    duration: "45h",
    rating: 4.9,
    progress: 0,
    minLevel: 2,
    image: "https://images.unsplash.com/photo-1544383835-bda2bc66a55d?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/banco_de_dados_sql_nosql.pdf",
    lessons: [
      "1. Modelagem Relacional e Normalização de Dados",
      "2. Consultas SQL Avançadas (JOINs, Subqueries e Indexação)",
      "3. Transações ACID e Integridade Referencial",
      "4. Introdução ao NoSQL e Modelagem Orientada a Documentos com MongoDB",
      "5. Otimização de Performance e Cache com Redis"
    ],
    quiz: [
      {
        question: "Qual comando SQL é utilizado para combinar registros de duas tabelas com base em uma coluna relacionada?",
        options: [
          "A. GROUP BY",
          "B. JOIN",
          "C. UNION",
          "D. MERGE"
        ],
        correct: 1
      }
    ]
  },
  {
    id: 12,
    title: "Arquitetura de Microserviços com Node.js e Docker",
    category: "Backend",
    area: "Infraestrutura, Nuvem & Cibersegurança",
    teacher: "Prof. Thiago Martins",
    duration: "55h",
    rating: 4.9,
    progress: 0,
    minLevel: 3,
    image: "https://images.unsplash.com/photo-1618401471353-b98afee0b2eb?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/microservicos_nodejs_docker.pdf",
    lessons: [
      "1. Monolito vs Microserviços: Conceitos e Desafios",
      "2. Comunicação entre Serviços (REST vs gRPC vs RabbitMQ)",
      "3. API Gateway, Service Discovery e Autenticação Centralizada",
      "4. Containerização de Serviços com Docker Compose",
      "5. Monitoramento Distribuído com Jaeger e Prometheus"
    ],
    quiz: [
      {
        question: "Qual a função principal de um API Gateway em uma arquitetura de microserviços?",
        options: [
          "A. Atuar como ponto único de entrada para os clientes, roteando requisições e gerenciando autenticação.",
          "B. Substituir todos os bancos de dados dos microserviços por uma única tabela.",
          "C. Executar compilação de código nativo no navegador do usuário.",
          "D. Eliminar a necessidade de protocolo HTTP nas comunicações."
        ],
        correct: 0
      }
    ]
  },
  {
    id: 13,
    title: "Desenvolvimento Frontend com React e Next.js",
    category: "Frontend",
    area: "Engenharia de Software & Programação",
    teacher: "Profª. Camilla Rocha",
    duration: "50h",
    rating: 5.0,
    progress: 0,
    minLevel: 2,
    image: "https://images.unsplash.com/photo-1633356122544-f134324a6cee?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/frontend_react_nextjs.pdf",
    lessons: [
      "1. React 18: Hooks, Componentes e Estado Global",
      "2. Renderização no Servidor (SSR) e Geração Estática (SSG) no Next.js",
      "3. Roteamento baseado em App Router",
      "4. Estilização com TailwindCSS e CSS Modules",
      "5. Otimização de Performance e SEO para Aplicações Web"
    ],
    quiz: [
      {
        question: "No Next.js (App Router), qual o benefício da Renderização no Servidor (SSR)?",
        options: [
          "A. Melhora o tempo de carregamento inicial e o SEO da aplicação enviando o HTML pronto do servidor.",
          "B. Impede o navegador de interpretar arquivos JavaScript.",
          "C. Substitui o uso de HTML5 por imagens PNG estáticas.",
          "D. Exige que o usuário instale extensões no navegador."
        ],
        correct: 0
      }
    ]
  },
  {
    id: 14,
    title: "Gestão Ágil de Projetos com Scrum e Kanban",
    category: "Metodologias Ágeis",
    area: "Design, Produto & Gestão Ágil",
    teacher: "Prof. Rodrigo Nogueira",
    duration: "30h",
    rating: 4.7,
    progress: 0,
    minLevel: 2,
    image: "https://images.unsplash.com/photo-1531403009284-440f080d1e12?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/gestao_agil_scrum_kanban.pdf",
    lessons: [
      "1. Manifesto Ágil e Princípios do Agile",
      "2. O Framework Scrum: Papéis (PO, Scrum Master, Dev Team)",
      "3. Cerimônias do Scrum (Sprint Planning, Daily, Review, Retrospective)",
      "4. O Método Kanban: Limite de WIP e Fluxo de Valor",
      "5. Métricas Ágeis (Velocity, Cumulative Flow Diagram, Burn-down)"
    ],
    quiz: [
      {
        question: "No Scrum, qual evento tem o objetivo de inspecionar o trabalho realizado durante a Sprint e adaptar o Product Backlog?",
        options: [
          "A. Daily Scrum",
          "B. Sprint Review",
          "C. Sprint Planning",
          "D. Backlog Refinement"
        ],
        correct: 1
      }
    ]
  },
  {
    id: 15,
    title: "Lógica de Programação e Algoritmos para Iniciantes",
    category: "Fundamentos de TI",
    area: "Engenharia de Software & Programação",
    teacher: "Prof. Marcelo Moreira",
    duration: "40h",
    rating: 4.9,
    progress: 0,
    minLevel: 1,
    image: "https://images.unsplash.com/photo-1515879218367-8466d910aaa4?auto=format&fit=crop&w=600&q=80",
    pdf: "materiais/logica_programacao_algoritmos.pdf",
    lessons: [
      "1. Conceitos de Algoritmo, Variáveis e Tipos de Dados",
      "2. Operadores Aritméticos, Relacionais e Lógicos",
      "3. Estruturas Condicionais (SE, SENÃO, ESCOLHA)",
      "4. Estruturas de Repetição (ENQUANTO, PARA, REPITA)",
      "5. Vetores, Matrizes e Resolução de Problemas Práticos"
    ],
    quiz: [
      {
        question: "Em lógica de programação, qual estrutura é usada para executar um bloco de código repetidamente enquanto uma condição for verdadeira?",
        options: [
          "A. Estrutura Condicional (SE/SENÃO)",
          "B. Estrutura de Repetição (ENQUANTO / WHILE)",
          "C. Estrutura de Atribuição",
          "D. Declaração de Constante"
        ],
        correct: 1
      }
    ]
  }
];

// Exporta para uso em navegadores e ambientes Node.js
if (typeof window !== 'undefined') {
  window.courses = courses;
}
if (typeof module !== 'undefined' && module.exports) {
  module.exports = courses;
}