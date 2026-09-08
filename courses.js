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
      },
      {
        question: "No paradigma de Orientação a Objetos (POO), o mecanismo pelo qual uma classe derivada redefine o comportamento de um método existente na classe base, permitindo que objetos de tipos diferentes respondam à mesma mensagem de formas específicas, denomina-se:",
        options: [
          "A. Encapsulamento estático",
          "B. Polimorfismo de sobreposição (Override)",
          "C. Acoplamento temporal",
          "D. Herança múltipla de atributos privados",
          "E. Coesão fraca"
        ],
        correct: 1
      },
      {
        question: "No contexto de Testes de Software e Garantia da Qualidade (QA), qual a principal diferença conceitual entre 'Verificação' e 'Validação' (V&V)?",
        options: [
          "A. Verificação avalia se estamos construindo o produto corretamente (conforme a especificação), enquanto Validação avalia se estamos construindo o produto certo (atendendo às reais necessidades do usuário).",
          "B. Verificação é realizada exclusivamente por usuários finais, enquanto Validação é executada apenas por compiladores automatizados.",
          "C. Validação diz respeito apenas à sintaxe do código, enquanto Verificação afere custos financeiros do projeto.",
          "D. Não há diferença, ambos são sinônimos para testes de carga e estresse de infraestrutura."
        ],
        correct: 0
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
      },
      {
        question: "No Kubernetes, qual é a menor unidade computacional implantável que encapsula um ou mais containers compartilhando rede e armazenamento?",
        options: [
          "A. Service",
          "B. Pod",
          "C. Deployment",
          "D. Ingress Controller"
        ],
        correct: 1
      },
      {
        question: "Ao implementar Infraestrutura como Código (IaC) com ferramentas declarativas como Terraform, qual é a principal característica dessa abordagem?",
        options: [
          "A. Descreve-se o estado final desejado da infraestrutura, e a ferramenta calcula as mudanças necessárias para atingi-lo.",
          "B. Escreve-se uma sequência passo a passo de comandos manuais no terminal bash.",
          "C. O código precisa ser executado manualmente dentro de cada máquina virtual criada.",
          "D. O Terraform só permite provisionar servidores físicos locais, sem suporte a provedores de nuvem pública."
        ],
        correct: 0
      },
      {
        question: "Em uma esteira de CI/CD (Continuous Integration / Continuous Delivery), qual etapa garante que novos commits não quebrem funcionalidades existentes antes do merge?",
        options: [
          "A. Provisionamento de faturamento da nuvem",
          "B. Execução automatizada de testes unitários e de integração (Test Automation)",
          "C. Envio de newsletter aos usuários cadastrados",
          "D. Desligamento do banco de dados de homologação"
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
        question: "No React Native, qual componente é utilizado para exibir elementos em lista de forma otimizada para alto desempenho com reciclagem de memória?",
        options: [
          "A. ScrollView",
          "B. FlatList",
          "C. ViewContainer",
          "D. ListViewBox"
        ],
        correct: 1
      },
      {
        question: "Qual Hook nativo do React é utilizado para executar efeitos colaterais em componentes funcionais, como buscar dados de uma API ao montar a tela?",
        options: [
          "A. useState",
          "B. useEffect",
          "C. useReducer",
          "D. useMemo"
        ],
        correct: 1
      },
      {
        question: "Como o sistema de layout do React Native organiza os elementos por padrão no componente View através do Flexbox?",
        options: [
          "A. flexDirection: 'row' (em linha horizontal)",
          "B. flexDirection: 'column' (em coluna vertical)",
          "C. display: 'table' (em grade de tabela)",
          "D. float: 'left' (flutuante à esquerda)"
        ],
        correct: 1
      },
      {
        question: "Para persistir dados simples localmente no dispositivo do usuário de forma assíncrona no ecossistema React Native / Expo, recomenda-se:",
        options: [
          "A. AsyncStorage (ou bibliotecas como react-native-mmkv)",
          "B. sessionStorage do navegador desktop",
          "C. Criar arquivos .txt no diretório raiz do sistema operacional sem permissões",
          "D. Variáveis globais do escopo window"
        ],
        correct: 0
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
          "B. Migrations (dotnet ef migrations add / update)",
          "C. JSON Schema Serializer",
          "D. SQL Server Direct Injection"
        ],
        correct: 1
      },
      {
        question: "No ASP.NET Core, qual padrão arquitetural nativo é configurado no arquivo Program.cs para gerenciar o ciclo de vida de serviços (Transient, Scoped e Singleton)?",
        options: [
          "A. Injeção de Dependência (Dependency Injection)",
          "B. Factory Method Manual",
          "C. Active Record Pattern",
          "D. Observer Remoto"
        ],
        correct: 0
      },
      {
        question: "Em APIs RESTful desenvolvidas com ASP.NET Core, qual atributo de anotação indica que um parâmetro do método de ação deve ser desserializado a partir do corpo da requisição HTTP?",
        options: [
          "A. [FromQuery]",
          "B. [FromHeader]",
          "C. [FromBody]",
          "D. [FromRoute]"
        ],
        correct: 2
      },
      {
        question: "Qual o papel do Middleware no pipeline de requisições do ASP.NET Core?",
        options: [
          "A. Compilar o código C# em tempo de execução no cliente.",
          "B. Interceptar requisições e respostas HTTP para executar autenticação, tratamento de erros e logging sequencialmente.",
          "C. Substituir o uso de bancos de dados relacionais.",
          "D. Converter páginas HTML em PDFs para impressão direta."
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
          "B. A versão mais simples de um produto que permite validar a hipótese de negócio com usuários reais gerando aprendizado validado.",
          "C. Um protótipo em papel sem qualquer interação digital.",
          "D. A estimativa financeira mínima exigida por investidores anjo."
        ],
        correct: 1
      },
      {
        question: "No Business Model Canvas (Osterwalder), qual quadrante é considerado o núcleo da proposta de valor?",
        options: [
          "A. Estrutura de Custos",
          "B. Proposta de Valor (o benefício e solução entregue para resolver a dor do cliente)",
          "C. Fontes de Receita",
          "D. Parcerias Principais"
        ],
        correct: 1
      },
      {
        question: "O ciclo iterativo fundamental preconizado pela metodologia Lean Startup (Eric Ries) é composto por quais etapas?",
        options: [
          "A. Planejar - Executar - Cobrar - Finalizar",
          "B. Construir - Medir - Aprender (Build - Measure - Learn)",
          "C. Codificar - Testar - Cancelar - Reiniciar",
          "D. Contratar - Investir - Vender - Demitir"
        ],
        correct: 1
      },
      {
        question: "O que caracteriza a fase de investimento 'Seed' (Semente) em uma startup de tecnologia?",
        options: [
          "A. Ocorre quando a empresa abre capital na bolsa de valores (IPO).",
          "B. Financiamento inicial destinado a validar o produto no mercado, aprimorar o MVP e conquistar os primeiros clientes.",
          "C. É o processo de liquidação judicial da empresa em falência.",
          "D. Trata-se de um empréstimo bancário tradicional garantido por bens imóveis."
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
        question: "Qual o marco legislativo fundamental no Brasil que alterou a LDB e tornou obrigatório o ensino da história e cultura afro-brasileira nas escolas?",
        options: [
          "A. Lei nº 10.639/2003",
          "B. Lei Áurea de 1888",
          "C. Código Civil de 2002",
          "D. Marco Civil da Internet"
        ],
        correct: 0
      },
      {
        question: "Segundo as teorias sociológicas contemporâneas (como as de Silvio Almeida), o que define o 'Racismo Estrutural'?",
        options: [
          "A. Um desvio de conduta ou preconceito puramente individual e isolado.",
          "B. Uma dimensão constitutiva das relações sociais, políticas e econômicas históricas, reproduzida institucionalmente de modo sistêmico.",
          "C. Um fenômeno superado no início do século XX com a industrialização.",
          "D. Apenas ofensas verbais diretas tipificadas no código penal."
        ],
        correct: 1
      },
      {
        question: "Qual o objetivo primordial das políticas de Ações Afirmativas (como o sistema de cotas no ensino superior e concursos públicos)?",
        options: [
          "A. Substituir os critérios acadêmicos por sorteio aleatório.",
          "B. Promover a reparação histórica e a equidade material de oportunidades para grupos historicamente marginalizados.",
          "C. Criar segregação social obrigatória em instituições de ensino.",
          "D. Eliminar vestibulares e processos seletivos públicos."
        ],
        correct: 1
      },
      {
        question: "Como o conceito de 'Interseccionalidade' (cunhado por Kimberlé Crenshaw) contribui para o debate étnico-racial?",
        options: [
          "A. Analisa como diferentes marcadores sociais (como raça, gênero e classe) se interconectam e potencializam situações de opressão ou vulnerabilidade.",
          "B. Afirma que todas as pessoas no mundo têm exatamente as mesmas experiências sociais.",
          "C. Trata exclusivamente de questões geográficas e fronteiras territoriais.",
          "D. Defende que raça e gênero não possuem qualquer impacto no mercado de trabalho de tecnologia."
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
        question: "No modelo de referência OSI de 7 camadas, em qual camada operam os roteadores para realizar o encaminhamento de pacotes baseado em endereços lógicos (IP)?",
        options: [
          "A. Camada 2 (Enlace de Dados)",
          "B. Camada 3 (Rede)",
          "C. Camada 4 (Transporte)",
          "D. Camada 7 (Aplicação)"
        ],
        correct: 1
      },
      {
        question: "Qual a principal diferença entre os protocolos de transporte TCP e UDP?",
        options: [
          "A. TCP é orientado a conexão, confiável e com controle de fluxo; UDP é não orientado a conexão, mais rápido e sem garantia de entrega sequencial.",
          "B. UDP é mais seguro e utiliza criptografia quântica nativa em todos os pacotes.",
          "C. TCP não permite tráfego de dados web (HTTP), operando somente para e-mails.",
          "D. UDP só funciona em conexões de fibra óptica submarina."
        ],
        correct: 0
      },
      {
        question: "Qual protocolo é responsável por atribuir dinamicamente configurações de IP, máscara de sub-rede e gateway padrão aos dispositivos que entram em uma rede?",
        options: [
          "A. DNS (Domain Name System)",
          "B. DHCP (Dynamic Host Configuration Protocol)",
          "C. SNMP (Simple Network Management Protocol)",
          "D. ARP (Address Resolution Protocol)"
        ],
        correct: 1
      },
      {
        question: "Em uma rede com máscara de sub-rede 255.255.255.0 (/24), quantos endereços de hosts válidos e utilizáveis podem ser atribuídos a computadores?",
        options: [
          "A. 256",
          "B. 254 (pois o primeiro é endereço de rede e o último é de broadcast)",
          "C. 512",
          "D. 128"
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
      "2. Aprendizado Supervisionado: Regressão e Classificação",
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
      },
      {
        question: "O que é o fenômeno de 'Overfitting' (sobreajuste) no treinamento de modelos de Machine Learning?",
        options: [
          "A. O modelo tem desempenho perfeito em dados novos e erra nos dados de treino.",
          "B. O modelo decora os dados de treino com seus ruídos, tendo excelente acurácia no treino, mas péssima capacidade de generalização para dados inéditos.",
          "C. O modelo não consegue aprender nem os padrões básicos dos dados de treino.",
          "D. É a falta de memória RAM suficiente no servidor de processamento."
        ],
        correct: 1
      },
      {
        question: "Em tarefas de Aprendizado Supervisionado, qual a diferença entre problemas de 'Classificação' e 'Regressão'?",
        options: [
          "A. Classificação prevê classes ou categorias discretas (ex: spam ou não spam); Regressão prevê valores numéricos contínuos (ex: preço de imóvel).",
          "B. Regressão só funciona com texto e Classificação só com imagens.",
          "C. Classificação não utiliza dados rotulados no treinamento.",
          "D. São termos sinônimos para a mesma função matemática de clusterização."
        ],
        correct: 0
      },
      {
        question: "Qual função de ativação é amplamente utilizada nas camadas ocultas de Redes Neurais Profundas para introduzir não-linearidade evitando o desvanecimento do gradiente?",
        options: [
          "A. ReLU (Rectified Linear Unit)",
          "B. Função Degrau de Heaviside",
          "C. Média Aritmética Simples",
          "D. Matriz de Confusão"
        ],
        correct: 0
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
      },
      {
        question: "Em testes de invasão (Pentest), qual a diferença entre as abordagens 'Black Box' e 'White Box'?",
        options: [
          "A. Black Box o pentester não possui informações prévias sobre o alvo; White Box ele possui acesso completo ao código-fonte, arquitetura e credenciais.",
          "B. White Box é sempre ilegal e Black Box é contratado por empresas.",
          "C. Black Box testa apenas impressoras e hardware físico.",
          "D. Não há diferença metodológica entre os dois tipos."
        ],
        correct: 0
      },
      {
        question: "O que caracteriza uma vulnerabilidade de Cross-Site Scripting (XSS)?",
        options: [
          "A. Injeção de scripts maliciosos (normalmente JavaScript) em páginas web legítimas, executados no navegador de outros usuários.",
          "B. Queda de energia física nos servidores de hospedagem.",
          "C. Ataque de força bruta contra senhas de roteadores Wi-Fi.",
          "D. Interceptação de cabos de rede com grampo telefônico."
        ],
        correct: 0
      },
      {
        question: "O conceito de 'Zero Trust' em arquitetura de cibersegurança fundamenta-se em qual premissa?",
        options: [
          "A. Confie em todos os dispositivos que estejam fisicamente dentro da rede local da empresa.",
          "B. 'Nunca confie, sempre verifique': todo acesso deve ser explicitamente autenticado, autorizado e criptografado, independente de estar dentro ou fora do perímetro.",
          "C. Senhas não precisam ser trocadas caso o usuário utilize biometria.",
          "D. Deixar todas as portas de firewall abertas para facilitar a manutenção remota."
        ],
        correct: 1
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
          "B. Um esboço visual de baixa ou média fidelidade que estrutura o layout e a hierarquia da interface.",
          "C. Um teste automatizado de carga no servidor web.",
          "D. A paleta de cores definitiva do aplicativo."
        ],
        correct: 1
      },
      {
        question: "Qual das alternativas corresponde a uma das 10 Heurísticas de Usabilidade de Jakob Nielsen?",
        options: [
          "A. Visibilidade do status do sistema (o sistema deve sempre manter os usuários informados sobre o que está acontecendo).",
          "B. Obrigatoriedade de utilizar tons de azul em formulários.",
          "C. Bloquear o botão de voltar no navegador em todas as telas.",
          "D. Utilizar apenas imagens em alta resolução acima de 10MB."
        ],
        correct: 0
      },
      {
        question: "No Figma, qual recurso permite criar layouts responsivos onde botões e cartões redimensionam e adaptam seus espaçamentos automaticamente conforme o conteúdo cresce?",
        options: [
          "A. Mask Group",
          "B. Auto Layout",
          "C. Pen Tool",
          "D. Flatten Selection"
        ],
        correct: 1
      },
      {
        question: "O que é um 'Design System' em equipes de produto digital?",
        options: [
          "A. Um conjunto único de ilustrações 3D sem padrões definidos.",
          "B. Uma biblioteca padronizada de componentes visuais, padrões de código, tokens de design e diretrizes de estilo compartilhada entre designers e desenvolvedores.",
          "C. Um software de edição de fotos alternativo ao Photoshop.",
          "D. Um contrato financeiro firmado com agências de publicidade."
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
          "B. JOIN (ex: INNER JOIN, LEFT JOIN)",
          "C. UNION",
          "D. MERGE"
        ],
        correct: 1
      },
      {
        question: "O que representam as propriedades ACID em Sistemas Gerenciadores de Bancos de Dados Relacionais?",
        options: [
          "A. Atomicidade, Consistência, Isolamento e Durabilidade.",
          "B. Acesso, Conexão, Indexação e Distribuição.",
          "C. Autenticação, Criptografia, Integridade e Desempenho.",
          "D. Aplicação, Camada, Interface e Deploy."
        ],
        correct: 0
      },
      {
        question: "No MongoDB, banco de dados NoSQL orientado a documentos, em qual formato os dados são estruturados e armazenados?",
        options: [
          "A. Linhas e Colunas rígidas com esquema relacional fixo",
          "B. Documentos BSON (Binary JSON) organizados em Collections flexíveis",
          "C. Planilhas CSV descompactadas em memória",
          "D. Arquivos XML somente para leitura"
        ],
        correct: 1
      },
      {
        question: "Qual a função de um Índice (INDEX) em uma tabela de banco de dados relacional?",
        options: [
          "A. Aumentar o tamanho do arquivo para ocupar espaço reservado no disco.",
          "B. Acelerar significativamente o tempo de busca e recuperação de registros em consultas SELECT.",
          "C. Impedir que novos registros sejam inseridos na tabela.",
          "D. Criptografar as senhas dos usuários automaticamente."
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
          "A. Atuar como ponto único de entrada para os clientes, roteando requisições e gerenciando autenticação e limitação de taxa.",
          "B. Substituir todos os bancos de dados dos microserviços por uma única tabela.",
          "C. Executar compilação de código nativo no navegador do usuário.",
          "D. Eliminar a necessidade de protocolo HTTP nas comunicações."
        ],
        correct: 0
      },
      {
        question: "Como é tratada a persistência de dados no padrão de design para microserviços 'Database per Service'?",
        options: [
          "A. Todos os serviços compartilham compulsoriamente a mesma instância e tabelas do banco de dados.",
          "B. Cada microserviço possui seu próprio banco de dados isolado, garantindo baixo acoplamento e independência de deploy.",
          "C. Microserviços nunca persistem dados, gravando apenas em memória temporária.",
          "D. O banco de dados só pode ser acessado via SSH manual pelo administrador."
        ],
        correct: 1
      },
      {
        question: "Qual padrão de resiliência em microserviços interrompe temporariamente o envio de requisições a um serviço em falha, evitando sobrecarga em cascata no sistema?",
        options: [
          "A. Circuit Breaker",
          "B. Lazy Loading",
          "C. Singleton Pattern",
          "D. Injeção de SQL"
        ],
        correct: 0
      },
      {
        question: "Em uma comunicação assíncrona orientada a eventos entre microserviços, qual tecnologia atua como message broker garantindo a entrega de mensagens?",
        options: [
          "A. RabbitMQ ou Apache Kafka",
          "B. Nginx atuando como servidor de arquivos estáticos",
          "C. Protocolo FTP tradicional",
          "D. LocalStorage do navegador"
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
        question: "No Next.js (App Router), qual o principal benefício da Renderização no Servidor (SSR)?",
        options: [
          "A. Melhora o tempo de carregamento inicial e a indexação em mecanismos de busca (SEO), enviando o HTML pré-renderizado pelo servidor.",
          "B. Impede o navegador de interpretar arquivos JavaScript.",
          "C. Substitui o uso de HTML5 por imagens PNG estáticas.",
          "D. Exige que o usuário instale extensões adicionais no navegador."
        ],
        correct: 0
      },
      {
        question: "No React, o que é o 'Virtual DOM' e qual a sua utilidade para o desempenho da aplicação?",
        options: [
          "A. É uma representação leve do DOM real em memória; o React compara as alterações (diffing) e atualiza apenas os nós modificados no DOM real.",
          "B. É uma máquina virtual Java embutida no navegador.",
          "C. Trata-se de um banco de dados NoSQL local.",
          "D. É uma tecnologia obsoleta substituída pelo jQuery."
        ],
        correct: 0
      },
      {
        question: "No Next.js com App Router, qual diretiva é necessária no topo de um arquivo para transformar um componente em um 'Client Component' com suporte a interatividade e Hooks?",
        options: [
          "A. 'use client'",
          "B. 'use server'",
          "C. 'enable hooks'",
          "D. 'client:only'"
        ],
        correct: 0
      },
      {
        question: "Qual o Hook do React utilizado para compartilhar valores (como tema ou dados do usuário logado) em toda a árvore de componentes sem precisar passar props manualmente em cada nível?",
        options: [
          "A. useContext",
          "B. useRef",
          "C. useLayoutEffect",
          "D. useTransition"
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
        question: "No Scrum, qual evento tem o objetivo de inspecionar o trabalho realizado durante a Sprint e adaptar o Product Backlog com feedback dos stakeholders?",
        options: [
          "A. Daily Scrum",
          "B. Sprint Review",
          "C. Sprint Planning",
          "D. Backlog Refinement"
        ],
        correct: 1
      },
      {
        question: "Qual é a responsabilidade primordial do Scrum Master em uma equipe de desenvolvimento?",
        options: [
          "A. Atuar como líder servidor, facilitando os eventos, removendo impedimentos para o time e disseminando os valores e práticas do Scrum.",
          "B. Definir os prazos e cobrar horas trabalhadas individualmente de cada programador.",
          "C. Definir e priorizar sozinho os itens do Product Backlog com o cliente final.",
          "D. Escrever toda a documentação de arquitetura de software."
        ],
        correct: 0
      },
      {
        question: "No método Kanban, qual a finalidade da prática de estabelecer limites de Trabalho em Progresso (WIP Limits)?",
        options: [
          "A. Evitar sobrecarga de tarefas simultâneas, reduzir gargalos e otimizar o fluxo contínuo de entrega de valor.",
          "B. Impedir que a equipe inicie novas tarefas antes do final do ano fiscal.",
          "C. Forçar todos os desenvolvedores a trabalharem no mesmo computador ao mesmo tempo.",
          "D. Aumentar artificialmente o número de cartões parados na coluna 'Fazendo'."
        ],
        correct: 0
      },
      {
        question: "Qual métrica ágil expressa a quantidade de trabalho (geralmente em Story Points) que uma equipe Scrum consegue entregar como 'Done' em uma Sprint média?",
        options: [
          "A. Velocidade da Equipe (Velocity)",
          "B. Retorno sobre Investimento (ROI)",
          "C. Taxa de Rotatividade (Churn Rate)",
          "D. Índice de Liquidez Corrente"
        ],
        correct: 0
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
      },
      {
        question: "Considerando as operações lógicas booleanas, qual será o resultado da expressão: (5 > 2) AND (3 == 4)?",
        options: [
          "A. Verdadeiro (True)",
          "B. Falso (False), pois na operação lógica AND (E) ambas as condições precisam ser verdadeiras.",
          "C. Erro de compilação por tipos incompatíveis",
          "D. Nulo (Null)"
        ],
        correct: 1
      },
      {
        question: "Qual estrutura de dados linear e homogênea armazena uma sequência de elementos do mesmo tipo acessíveis por um índice numérico posicional?",
        options: [
          "A. Vetor (Array unidimensional)",
          "B. Grafo acíclico complexo",
          "C. Árvore binária de busca balanceada",
          "D. Variável booleana simples"
        ],
        correct: 0
      },
      {
        question: "Em ciência da computação, o que caracteriza uma função 'Recursiva'?",
        options: [
          "A. Uma função que nunca termina e trava o computador obrigatoriamente.",
          "B. Uma função que chama a si mesma durante sua execução, possuindo uma condição de parada (caso base) para encerrar as chamadas.",
          "C. Uma função escrita exclusivamente em linguagem Assembly.",
          "D. Uma função que só aceita números negativos como argumentos."
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