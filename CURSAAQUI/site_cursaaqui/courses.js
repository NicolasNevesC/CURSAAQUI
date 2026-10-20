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
        question: "No contexto de Testes de Software e Garantia da Qualidade (QA), qual a principal diferença conceitual entre 'Verificação' e 'Validação' (V&V)?",
        options: [
          "A. Verificação avalia se estamos construindo o produto corretamente (conforme a especificação), enquanto Validação avalia se estamos construindo o produto certo (atendendo às reais necessidades do usuário).",
          "B. Verificação é realizada exclusivamente por usuários finais, enquanto Validação é executada apenas por compiladores automatizados.",
          "C. Validação diz respeito apenas à sintaxe do código, enquanto Verificação afere custos financeiros do projeto.",
          "D. Não há diferença, ambos são sinônimos para testes de carga e estresse de infraestrutura."
        ],
        correct: 0
      },
      {
        question: "Na dimensão dinâmica do Processo Unificado (RUP), quais são as quatro fases que estruturam o ciclo de vida de um projeto de software?",
        options: [
          "A. Análise, Projeto, Codificação e Manutenção",
          "B. Iniciação (Concepção), Elaboração, Construção e Transição",
          "C. Planejamento, Execução, Monitoramento e Encerramento",
          "D. Levantamento, Modelagem, Implementação e Entrega"
        ],
        correct: 1
      },
      {
        question: "Em POO, o conceito de 'Encapsulamento' visa principalmente:",
        options: [
          "A. Herdar métodos de múltiplas classes simultaneamente sem restrições de acesso.",
          "B. Ocultar os detalhes internos de implementação de uma classe, expondo apenas uma interface pública controlada e protegendo a integridade dos dados.",
          "C. Permitir que variáveis globais sejam acessadas de qualquer ponto do sistema sem restrições.",
          "D. Eliminar a necessidade de interfaces e classes abstratas na modelagem."
        ],
        correct: 1
      },
      {
        question: "Qual das estratégias abaixo representa corretamente uma abordagem de 'Prototipagem Evolutiva' no desenvolvimento de software?",
        options: [
          "A. Criar um protótipo descartável apenas para elicitar requisitos e depois reiniciar do zero.",
          "B. Construir um protótipo inicial e refiná-lo iterativamente com feedback do usuário até que se torne o produto final.",
          "C. Desenvolver toda a documentação técnica antes de qualquer linha de código ser escrita.",
          "D. Utilizar metodologias em cascata rígidas sem ciclos de revisão."
        ],
        correct: 1
      },
      {
        question: "No diagrama de Casos de Uso da UML, o relacionamento '<<include>>' entre dois casos de uso indica:",
        options: [
          "A. Que o caso de uso base pode opcionalmente invocar o comportamento do caso incluído.",
          "B. Que o caso de uso base sempre e obrigatoriamente invoca o comportamento do caso incluído como parte de sua execução.",
          "C. Uma herança entre atores do sistema.",
          "D. Um relacionamento de composição entre classes de domínio."
        ],
        correct: 1
      },
      {
        question: "Qual técnica de teste de caixa-branca analisa o fluxo de controle do código para garantir que todos os caminhos lógicos e desvios condicionais sejam exercitados?",
        options: [
          "A. Particionamento de Equivalência",
          "B. Análise de Valor Limite",
          "C. Teste de Cobertura de Caminhos (Cobertura de Ramos/Branches)",
          "D. Teste de Carga e Estresse"
        ],
        correct: 2
      },
      {
        question: "Em algoritmos e estruturas de dados, a complexidade de tempo O(n log n) é característica de qual classe de algoritmos de ordenação eficientes?",
        options: [
          "A. Bubble Sort e Insertion Sort",
          "B. Merge Sort e Quick Sort (caso médio)",
          "C. Selection Sort",
          "D. Busca Sequencial Linear"
        ],
        correct: 1
      },
      {
        question: "No paradigma de Orientação a Objetos (POO), o mecanismo pelo qual uma classe derivada redefine o comportamento de um método existente na classe base, permitindo que objetos de tipos diferentes respondam à mesma mensagem de formas específicas, denomina-se:",
        options: [
          "A. Encapsulamento estático",
          "B. Polimorfismo de sobreposição (Override)",
          "C. Acoplamento temporal",
          "D. Herança múltipla de atributos privados"
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
      },
      {
        question: "No modelo de responsabilidade compartilhada em Cloud Computing, qual das afirmativas descreve corretamente o modelo IaaS (Infrastructure as a Service)?",
        options: [
          "A. O provedor gerencia tudo, inclusive aplicações e dados do cliente.",
          "B. O cliente gerencia o sistema operacional, middleware, runtime e suas aplicações; o provedor gerencia a infraestrutura física (rede, servidores, virtualização).",
          "C. O cliente só configura parâmetros de negócio, sem acesso à camada de SO.",
          "D. IaaS elimina completamente a necessidade de equipe de TI do cliente."
        ],
        correct: 1
      },
      {
        question: "Qual estratégia de deployment em Kubernetes/Cloud elimina o downtime gradualmente, ao criar réplicas da nova versão enquanto as antigas são removidas progressivamente?",
        options: [
          "A. Recreate Deployment (reinicialização total)",
          "B. Rolling Update (Atualização Progressiva)",
          "C. Canary Deployment (Implantação Canário)",
          "D. Blue-Green Deployment (Deploy Azul-Verde)"
        ],
        correct: 1
      },
      {
        question: "Em DevOps, qual prática garante que mudanças de configuração de servidores sejam rastreáveis, repetíveis e auditáveis, usando código versionado em repositório Git?",
        options: [
          "A. GitOps",
          "B. FTP Manual por Administrador",
          "C. Configuração por painel de controle web sem versionamento",
          "D. Scripts Shell sem controle de versão"
        ],
        correct: 0
      },
      {
        question: "Qual serviço gerenciado da AWS permite executar código em resposta a eventos sem provisionar ou gerenciar servidores (computação serverless)?",
        options: [
          "A. Amazon EC2",
          "B. Amazon S3",
          "C. AWS Lambda",
          "D. Amazon RDS"
        ],
        correct: 2
      },
      {
        question: "Em monitoramento de sistemas distribuídos, qual é a diferença entre 'Métricas', 'Logs' e 'Traces'?",
        options: [
          "A. São termos equivalentes para qualquer dado de observabilidade.",
          "B. Métricas são valores numéricos agregados ao longo do tempo; Logs são registros de eventos textuais; Traces rastreiam o caminho de uma requisição por múltiplos serviços.",
          "C. Logs são coletados apenas em produção, enquanto Métricas funcionam exclusivamente em desenvolvimento.",
          "D. Traces só são úteis em aplicações monolíticas sem microserviços."
        ],
        correct: 1
      },
      {
        question: "O que diferencia o Kubernetes de um simples orquestrador de containers no contexto de alta disponibilidade?",
        options: [
          "A. O Kubernetes não oferece mecanismos de recuperação automática (self-healing).",
          "B. Kubernetes oferece auto-escalonamento, self-healing (reinicia containers com falha), balanceamento de carga e rollback automático, garantindo alta disponibilidade.",
          "C. Kubernetes é uma ferramenta apenas para desenvolvimento local, sem suporte a produção.",
          "D. Kubernetes elimina a necessidade de redes virtuais (VPCs) em ambientes de nuvem."
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
      },
      {
        question: "No React Navigation, qual tipo de navegação é recomendado para fluxos de autenticação (Login → Cadastro → Esqueci Senha) onde o usuário não pode voltar com o botão físico após logar?",
        options: [
          "A. Stack Navigator sem a opção headerShown",
          "B. Stack Navigator com gestureEnabled desabilitado e resets de rota via CommonActions.reset",
          "C. Drawer Navigator com rotas empilhadas",
          "D. Tab Navigator com botão de voltar global"
        ],
        correct: 1
      },
      {
        question: "Qual a principal diferença entre um componente 'Controlado' e 'Não-Controlado' em React?",
        options: [
          "A. Componentes controlados usam refs para acessar o DOM; não-controlados usam estado do React.",
          "B. Componentes controlados têm seu valor gerenciado pelo estado do React via props; não-controlados mantêm seu próprio estado interno no DOM.",
          "C. Não há diferença funcional entre os dois modelos de componente.",
          "D. Componentes não-controlados só funcionam em React Native, nunca em React Web."
        ],
        correct: 1
      },
      {
        question: "Em React Native com Expo, qual API nativa permite solicitar permissões ao dispositivo (câmera, localização, notificações) de forma unificada entre iOS e Android?",
        options: [
          "A. expo-permissions (ou expo-camera, expo-location com módulos específicos)",
          "B. navigator.geolocation do browser",
          "C. window.Notification da Web API",
          "D. document.requestPermission() do DOM"
        ],
        correct: 0
      },
      {
        question: "Para compartilhar estado global entre múltiplas telas em React Native sem prop drilling, qual solução é mais adequada para estados simples e moderados?",
        options: [
          "A. Passar props manualmente através de toda a árvore de componentes",
          "B. Context API com useContext Hook",
          "C. Criar variáveis globais diretamente no arquivo de entrada da aplicação",
          "D. Salvar tudo no AsyncStorage e reler a cada render"
        ],
        correct: 1
      },
      {
        question: "Ao realizar requisições HTTP em React Native para consumo de APIs REST, qual método nativo do JavaScript é amplamente utilizado e funciona nativamente em ambos os ambientes?",
        options: [
          "A. XMLHttpRequest com callbacks aninhados",
          "B. fetch() API (com async/await para melhor legibilidade)",
          "C. axios exclusivamente, pois fetch não existe em React Native",
          "D. $.ajax() do jQuery"
        ],
        correct: 1
      },
      {
        question: "Para publicar um aplicativo React Native (com Expo) na Google Play Store, qual é o formato de arquivo de build exigido para submissão?",
        options: [
          "A. .apk (Android Package) para debug",
          "B. .aab (Android App Bundle) para release em produção",
          "C. .ipa (iOS App Archive)",
          "D. .zip compactado do código-fonte"
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
      },
      {
        question: "Em C#, qual é a finalidade da palavra-chave 'async' combinada com 'await' em métodos?",
        options: [
          "A. Criar threads paralelas que executam simultaneamente de forma sincrona.",
          "B. Permitir que o método seja executado de forma assíncrona sem bloquear a thread principal, liberando-a enquanto aguarda operações de I/O.",
          "C. Forçar a execução síncrona bloqueante em todos os contextos.",
          "D. Declarar métodos que nunca retornam valores ao chamador."
        ],
        correct: 1
      },
      {
        question: "No padrão de arquitetura Clean Architecture aplicado ao .NET, qual a responsabilidade da camada 'Domain' (Domínio)?",
        options: [
          "A. Gerenciar conexões HTTP e configurações de roteamento da API.",
          "B. Conter as regras de negócio centrais, entidades e interfaces, sem dependências de frameworks externos.",
          "C. Realizar a comunicação direta com o banco de dados via SQL raw.",
          "D. Controlar a serialização e desserialização de JSON nas responses."
        ],
        correct: 1
      },
      {
        question: "Para implementar autenticação stateless em APIs ASP.NET Core, qual tecnologia é amplamente adotada pela sua capacidade de carregar claims do usuário de forma autocontida no token?",
        options: [
          "A. Session Cookies com servidor de estado centralizado",
          "B. JSON Web Token (JWT) com validação via chave secreta ou certificado",
          "C. Basic Authentication em texto plano sem criptografia",
          "D. OAuth 1.0 com assinatura HMAC por requisição"
        ],
        correct: 1
      },
      {
        question: "Em testes unitários com xUnit no .NET, qual atributo marca um método como caso de teste parametrizado com múltiplos conjuntos de dados de entrada?",
        options: [
          "A. [Fact]",
          "B. [Theory] com [InlineData]",
          "C. [TestCase]",
          "D. [DataRow]"
        ],
        correct: 1
      },
      {
        question: "O que representa o conceito de 'SOLID' no desenvolvimento orientado a objetos em C#?",
        options: [
          "A. Um conjunto de 5 princípios (Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion) para código manutenível e extensível.",
          "B. Um framework de testes de integração exclusivo do ecossistema .NET.",
          "C. Um padrão de banco de dados relacional para persistência de objetos C#.",
          "D. Uma técnica de compressão de assemblies .NET para redução de tamanho."
        ],
        correct: 0
      },
      {
        question: "No Entity Framework Core, qual é a diferença entre os métodos 'FirstOrDefault()' e 'SingleOrDefault()' em consultas LINQ?",
        options: [
          "A. São equivalentes e podem ser usados de forma intercambiável em qualquer contexto.",
          "B. FirstOrDefault() retorna o primeiro elemento ou null sem verificar duplicatas; SingleOrDefault() lança exceção se houver mais de um elemento correspondente.",
          "C. SingleOrDefault() sempre retorna uma lista, enquanto FirstOrDefault() retorna apenas um objeto.",
          "D. FirstOrDefault() só funciona com chaves primárias, enquanto SingleOrDefault() aceita qualquer predicado."
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
      },
      {
        question: "Qual métrica de negócio mede o custo total para adquirir um novo cliente pagante (incluindo marketing, vendas e onboarding)?",
        options: [
          "A. LTV (Lifetime Value — Valor do Tempo de Vida do Cliente)",
          "B. CAC (Customer Acquisition Cost — Custo de Aquisição de Cliente)",
          "C. MRR (Monthly Recurring Revenue — Receita Recorrente Mensal)",
          "D. NPS (Net Promoter Score — Índice de Promoção Líquida)"
        ],
        correct: 1
      },
      {
        question: "Em startups de TI, o conceito de 'Pivô' (Pivot) significa:",
        options: [
          "A. Encerrar definitivamente as operações da empresa por falta de mercado.",
          "B. Mudar estrategicamente o modelo de negócio, segmento-alvo ou produto com base em aprendizados validados, mantendo o que funciona.",
          "C. Contratar um novo CEO externo para substituir o fundador original.",
          "D. Realizar uma fusão compulsória com uma empresa concorrente."
        ],
        correct: 1
      },
      {
        question: "Qual dos modelos de receita abaixo é característica de empresas SaaS (Software as a Service) no mercado B2B?",
        options: [
          "A. Venda única de licença perpétua com upgrade pago separadamente",
          "B. Assinatura recorrente mensal ou anual com acesso à plataforma na nuvem",
          "C. Modelo freemium apenas para usuários pessoa física",
          "D. Venda de hardware com software embarcado de fábrica"
        ],
        correct: 1
      },
      {
        question: "No contexto de growth hacking em startups digitais, o que é o 'Funil AARRR' (Pirate Metrics) de Dave McClure?",
        options: [
          "A. Um modelo financeiro de projeção de custos operacionais mensais.",
          "B. Um framework de 5 etapas (Acquisition, Activation, Retention, Revenue, Referral) para medir e otimizar o crescimento de usuários.",
          "C. Uma metodologia ágil de desenvolvimento de software para squads de produto.",
          "D. Um sistema de versionamento de código para startups em early-stage."
        ],
        correct: 1
      },
      {
        question: "O que diferencia um 'mercado TAM, SAM e SOM' na análise de oportunidade de negócio para investidores?",
        options: [
          "A. São três departamentos internos de uma startup (Tech, Admin, Marketing).",
          "B. TAM é o mercado total disponível; SAM é o mercado endereçável pelo modelo de negócio; SOM é o mercado alcançável realisticamente nos próximos 3-5 anos.",
          "C. São estágios sequenciais de crescimento: do local para o global.",
          "D. Representam três indicadores de liquidez financeira para análise de balanço."
        ],
        correct: 1
      },
      {
        question: "Qual característica define uma empresa como 'Unicórnio' no ecossistema global de startups?",
        options: [
          "A. Empresa com mais de 1.000 funcionários e presença em 10 países.",
          "B. Startup privada avaliada em pelo menos US$ 1 bilhão sem ter aberto capital na bolsa.",
          "C. Empresa de tecnologia fundada há mais de 20 anos com receita estável.",
          "D. Startup que atingiu breakeven no primeiro ano de operação."
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

// ============================================================
// ENRIQUECIMENTO PEDAGÓGICO: QUESTÕES ABERTAS E EXERCÍCIOS EXTRAS
// ============================================================
const openQuestionsCatalog = {
  1: {
    title: "Estudo de Caso: Arquitetura e Engenharia de Requisitos",
    prompt: "Imagine que você foi encarregado de projetar o sistema de matrículas de uma universidade. Descreva a diferença prática entre um Requisito Funcional e um Requisito Não-Funcional para este sistema, citando um exemplo concreto de cada um e explicando como a prototipagem inicial pode reduzir riscos no levantamento de requisitos.",
    guidelines: "Cite ao menos 1 exemplo de requisito funcional, 1 de não-funcional (ex: tempo de resposta/segurança) e o papel da prototipagem com o usuário.",
    keywords: ["funcional", "requisito", "prototipagem", "usuario", "desempenho", "seguranca", "risco", "validacao"],
    minWords: 20
  },
  2: {
    title: "Estudo de Caso: Migração Monólito para Contêineres e CI/CD",
    prompt: "Explique como a adoção de Docker e pipelines automatizados de CI/CD soluciona o clássico problema 'na minha máquina funciona' e acelera a entrega contínua com segurança em ambientes de nuvem.",
    guidelines: "Aborde padronização de imagens Docker, automação de testes e deploys reprodutíveis em produção.",
    keywords: ["docker", "container", "pipeline", "ci/cd", "ambiente", "deploy", "automacao", "nuvem", "producao"],
    minWords: 20
  },
  3: {
    title: "Estudo de Caso: Defesa em Profundidade e Gestão de Acessos",
    prompt: "Descreva a importância do princípio do Menor Privilégio aliado à Autenticação Multifator (MFA) para mitigar invasões cibernéticas e conter a movimentação lateral de atacantes na rede.",
    guidelines: "Explique como o controle de privilégios limita o raio de impacto e por que senhas simples não são suficientes.",
    keywords: ["privilegio", "mfa", "autenticacao", "seguranca", "acesso", "ataque", "senha", "rede"],
    minWords: 20
  },
  4: {
    title: "Estudo de Caso: Arquitetura RESTful e Gerenciamento de Estado",
    prompt: "Em uma aplicação web Full Stack moderna desenvolvida com React e Node.js, por que é uma boa prática manter as APIs REST stateless (sem estado na sessão do servidor) e como o estado da interface deve ser controlado no cliente?",
    guidelines: "Aborde escalabilidade de servidores sem estado, autenticação via tokens (JWT) e sincronização de estado no frontend.",
    keywords: ["react", "node", "rest", "stateless", "estado", "jwt", "api", "componente", "escalabilidade"],
    minWords: 20
  },
  5: {
    title: "Estudo de Caso: Ciclo de Machine Learning e Prevenção de Overfitting",
    prompt: "Diferencie Aprendizado Supervisionado de Não-Supervisionado e explique o que é o fenômeno do Overfitting (sobreajuste), citando uma técnica prática utilizada para evitá-lo durante o treinamento do modelo.",
    guidelines: "Destaque dados rotulados vs não rotulados e mencione divisão treino/teste, validação cruzada ou regularização.",
    keywords: ["supervisionado", "rotulo", "overfitting", "sobreajuste", "treino", "teste", "validacao", "modelo"],
    minWords: 20
  },
  6: {
    title: "Estudo de Caso: Cerimônias Ágeis e Limitação de Trabalho (WIP)",
    prompt: "Explique o objetivo da reunião de Retrospectiva da Sprint no Scrum e como a aplicação do limite de trabalho em progresso (WIP Limit) no Kanban auxilia o time a evitar gargalos de entrega.",
    guidelines: "Conecte a melhoria contínua do time com o fluxo contínuo e a redução de multitarefas.",
    keywords: ["retrospectiva", "sprint", "kanban", "wip", "gargalo", "melhoria", "fluxo", "time", "scrum"],
    minWords: 20
  },
  7: {
    title: "Estudo de Caso: Modelagem Relacional vs NoSQL e Propriedades ACID",
    prompt: "Discuta em quais cenários práticos você optaria por um banco relacional (ex: PostgreSQL com suporte a ACID) em detrimento de um banco NoSQL documental (ex: MongoDB), destacando a integridade transacional.",
    guidelines: "Cite transações financeiras/críticas vs esquemas flexíveis e alto volume de leitura/escrita.",
    keywords: ["acid", "transacao", "relacional", "nosql", "postgresql", "mongodb", "integridade", "consistencia"],
    minWords: 20
  },
  8: {
    title: "Estudo de Caso: Arquitetura Reativa no Flutter",
    prompt: "Explique a diferença prática entre StatelessWidget e StatefulWidget no Flutter, descrevendo como a renderização reativa da árvore de widgets responde a mudanças no estado da aplicação.",
    guidelines: "Diferencie interfaces estáticas de dinâmicas e mencione o método setState ou gerenciadores de estado.",
    keywords: ["statelesswidget", "statefulwidget", "estado", "widget", "setstate", "arvore", "interface"],
    minWords: 20
  },
  9: {
    title: "Estudo de Caso: Aplicação Prática de Heurísticas de Usabilidade",
    prompt: "Escolha uma das Heurísticas de Usabilidade de Nielsen (ex: Visibilidade do Status do Sistema ou Prevenção de Erros) e demonstre como aplicá-la no fluxo de checkout ou login de uma plataforma digital.",
    guidelines: "Demonstre clareza de feedback ao usuário, redução de carga cognitiva e acessibilidade visual.",
    keywords: ["nielsen", "heuristica", "status", "feedback", "usabilidade", "usuario", "erro", "acessibilidade"],
    minWords: 20
  },
  10: {
    title: "Estudo de Caso: Resiliência em Microsserviços e Padrão Circuit Breaker",
    prompt: "Quais os principais riscos de indisponibilidade em cascata em arquiteturas de microsserviços e como o padrão Circuit Breaker protege o ecossistema quando uma dependência externa falha?",
    guidelines: "Aborde timeouts, falhas em cascata, estados do disjuntor (Fechado, Aberto, Meio-Aberto) e degradação suave.",
    keywords: ["microsservicos", "circuit breaker", "cascata", "resiliencia", "falha", "timeout", "disjuntor"],
    minWords: 20
  },
  11: {
    title: "Estudo de Caso: ITIL - Incidente vs Problema e Continuidade",
    prompt: "No âmbito da biblioteca ITIL 4, qual a distinção essencial entre a Gestão de Incidentes e a Gestão de Problemas, e por que identificar a causa-raiz é crucial para a saúde dos serviços de TI?",
    guidelines: "Diferencie restaurar o serviço rapidamente (incidente) de eliminar causas recorrentes definitivas (problema).",
    keywords: ["itil", "incidente", "problema", "causa", "raiz", "restaurar", "servico", "disponibilidade"],
    minWords: 20
  },
  12: {
    title: "Estudo de Caso: Arquiteturas Modernas de Dados (ETL vs ELT)",
    prompt: "Compare as estratégias de ETL tradicional com a moderna abordagem ELT em Data Lakes/Data Warehouses, explicando por que o processamento distribuído (como Spark) revolucionou a análise de Big Data.",
    guidelines: "Explique onde a transformação ocorre e a capacidade computacional em nuvem para ingestão de dados brutos.",
    keywords: ["etl", "elt", "data warehouse", "spark", "dados", "transformacao", "nuvem", "processamento"],
    minWords: 20
  },
  13: {
    title: "Estudo de Caso: Camada de Transporte (TCP vs UDP)",
    prompt: "Compare detalhadamente os protocolos TCP e UDP quanto a controle de fluxo, confirmação de entrega e latência, justificando por que serviços de streaming em tempo real costumam preferir UDP.",
    guidelines: "Aborde handshake de três vias, retransmissão de pacotes e tolerância a perdas pontuais em prol da baixa latência.",
    keywords: ["tcp", "udp", "latencia", "pacote", "handshake", "confiabilidade", "streaming", "transporte"],
    minWords: 20
  },
  14: {
    title: "Estudo de Caso: Alinhamento com OKRs e Foco em Valor",
    prompt: "Como a definição de OKRs (Objetivos e Resultados-Chave) ajuda equipes que utilizam Scrum ou Kanban a não se tornarem meras 'fábricas de tarefas', conectando o backlog a impactos mensuráveis no negócio?",
    guidelines: "Destaque a transição de entregas baseadas em 'output' (quantidade de tarefas) para 'outcome' (valor gerado).",
    keywords: ["okr", "objetivo", "resultados-chave", "valor", "negocio", "metas", "scrum", "impacto"],
    minWords: 20
  },
  15: {
    title: "Estudo de Caso: Resolução Algorítmica e Estrutura Lógica",
    prompt: "Descreva a lógica passo a passo (em pseudocódigo ou texto explicativo) de um algoritmo para encontrar o maior valor em um vetor numérico de 10 posições, especificando a variável acumuladora e a condição de comparação.",
    guidelines: "Mencione a inicialização com o primeiro elemento, o laço de repetição percorrendo os índices e a atualização do maior.",
    keywords: ["vetor", "maior", "laco", "repeticao", "condicao", "algoritmo", "variavel", "indice"],
    minWords: 20
  }
};

// Questões objetivas complementares para os cursos 6 a 15
const extraQuizzesCatalog = {
  6: [
    {
      question: "No framework Scrum, quem é o único responsável por gerenciar e priorizar os itens do Product Backlog com base no valor de negócio?",
      options: [
        "A. Scrum Master",
        "B. Product Owner (PO)",
        "C. Equipe de Desenvolvedores",
        "D. Gerente de RH"
      ],
      correct: 1
    },
    {
      question: "Qual o principal objetivo do gráfico Burndown em uma Sprint Scrum?",
      options: [
        "A. Acompanhar a quantidade de trabalho restante ao longo do tempo da Sprint.",
        "B. Registrar o salário de cada membro do time.",
        "C. Listar os bugs reportados pelos clientes em tempo real.",
        "D. Calcular o imposto das notas fiscais do projeto."
      ],
      correct: 0
    }
  ],
  7: [
    {
      question: "Em bancos de dados relacionais, o que garante a integridade referencial entre duas tabelas distintas?",
      options: [
        "A. Índice clustered",
        "B. Chave Primária (Primary Key) e Chave Estrangeira (Foreign Key)",
        "C. Comando DROP TABLE em cascata",
        "D. Visão temporária (VIEW)"
      ],
      correct: 1
    },
    {
      question: "Qual cláusula SQL é utilizada para filtrar registros agrupados após a execução da cláusula GROUP BY?",
      options: [
        "A. WHERE",
        "B. HAVING",
        "C. ORDER BY",
        "D. LIMIT"
      ],
      correct: 1
    }
  ],
  8: [
    {
      question: "No desenvolvimento com Flutter, em qual arquivo de configuração do projeto são declaradas as dependências de pacotes e recursos como fontes e imagens?",
      options: [
        "A. package.json",
        "B. pubspec.yaml",
        "C. build.gradle exclusivamente",
        "D. manifest.xml"
      ],
      correct: 1
    },
    {
      question: "Qual linguagem de programação é utilizada pelo Flutter para compilar aplicações nativas com alto desempenho?",
      options: [
        "A. Python",
        "B. Dart",
        "C. Ruby",
        "D. Swift"
      ],
      correct: 1
    }
  ],
  9: [
    {
      question: "No design de interfaces digitais, o que representa um 'Design System'?",
      options: [
        "A. Apenas uma paleta com 3 cores escolhidas aleatoriamente.",
        "B. Um ecossistema padronizado de componentes reutilizáveis, regras visuais e diretrizes de código para manter consistência.",
        "C. Um editor de código fonte alternativo ao VS Code.",
        "D. Um software antivírus para designers."
      ],
      correct: 1
    },
    {
      question: "Qual teste de usabilidade compara duas versões diferentes de uma mesma tela (Versão A e Versão B) para medir qual alcança maior taxa de conversão?",
      options: [
        "A. Teste Unitário",
        "B. Teste A/B",
        "C. Teste de Carga",
        "D. Teste de Regressão"
      ],
      correct: 1
    }
  ],
  10: [
    {
      question: "Qual padrão arquitetural atua como um ponto único de entrada para requisições de clientes externos, roteando chamadas e gerenciando autenticação em microsserviços?",
      options: [
        "A. API Gateway",
        "B. Data Lake",
        "C. Monolito Distribuído",
        "D. Singleton Memory Cache"
      ],
      correct: 0
    },
    {
      question: "O teorema CAP afirma que um sistema distribuído só pode garantir simultaneamente duas das três seguintes propriedades:",
      options: [
        "A. Custo, Acurácia e Precisão",
        "B. Consistência, Disponibilidade e Tolerância a Partições (Consistency, Availability, Partition tolerance)",
        "C. CPU, Armazenamento e Performance",
        "D. Criptografia, Autenticação e Privacidade"
      ],
      correct: 1
    }
  ],
  11: [
    {
      question: "Qual documento formal estabelece as metas negociadas e acordadas de tempo e qualidade do serviço entre o provedor de TI e o cliente?",
      options: [
        "A. SLA (Acordo de Nível de Serviço / Service Level Agreement)",
        "B. Carta de Demissão",
        "C. Recibo de Pagamento",
        "D. Relatório de Logs Brutos"
      ],
      correct: 0
    },
    {
      question: "No ciclo de gerenciamento de mudanças da ITIL, qual comitê é responsável por avaliar o impacto e aprovar alterações de médio e alto risco?",
      options: [
        "A. CAB (Change Advisory Board)",
        "B. DPO (Data Protection Officer)",
        "C. CEO",
        "D. Time de Helpdesk Nível 1"
      ],
      correct: 0
    }
  ],
  12: [
    {
      question: "Qual componente fundamental do ecossistema Hadoop é responsável pelo armazenamento distribuído e tolerante a falhas de blocos de dados em clusters?",
      options: [
        "A. HDFS (Hadoop Distributed File System)",
        "B. Nginx",
        "C. SQLite",
        "D. Redis In-Memory"
      ],
      correct: 0
    },
    {
      question: "No contexto de Big Data, qual das opções melhor resume a definição dos '3 Vs' tradicionais?",
      options: [
        "A. Velocidade, Valor e Variáveis",
        "B. Volume, Velocidade e Variedade",
        "C. Virtualização, Visão e Vendas",
        "D. Validação, Verificação e Versão"
      ],
      correct: 1
    }
  ],
  13: [
    {
      question: "Qual protocolo da camada de aplicação é responsável por traduzir nomes de domínio legíveis (ex: cursaaqui.com.br) para endereços IP numéricos?",
      options: [
        "A. DHCP",
        "B. DNS (Domain Name System)",
        "C. FTP",
        "D. SNMP"
      ],
      correct: 1
    },
    {
      question: "Qual faixa de endereços IPv4 é reservada para testes de loopback local na própria máquina?",
      options: [
        "A. 192.168.1.1",
        "B. 127.0.0.1 (127.0.0.0/8)",
        "C. 10.0.0.1",
        "D. 8.8.8.8"
      ],
      correct: 1
    }
  ],
  14: [
    {
      question: "No framework OKR (Objectives and Key Results), uma característica essencial dos Resultados-Chave (Key Results) é que eles devem ser:",
      options: [
        "A. Vagos e sem prazos para não gerar pressão",
        "B. Estritamente quantitativos, mensuráveis e com prazos definidos",
        "C. Secretos e conhecidos apenas pela diretoria",
        "D. Modificados semanalmente de acordo com a vontade individual"
      ],
      correct: 1
    },
    {
      question: "Qual prática ágil busca manter o código em produção estável integrando alterações com testes automatizados várias vezes ao dia?",
      options: [
        "A. Integração Contínua (CI)",
        "B. Modelo Cascata (Waterfall)",
        "C. Deploy manual sem versionamento",
        "D. Desenvolvimento isolado por 6 meses"
      ],
      correct: 0
    }
  ],
  15: [
    {
      question: "Qual operador lógico resulta em FALSO apenas se ambas as proposições forem falsas?",
      options: [
        "A. Operador AND (E)",
        "B. Operador OR (OU)",
        "C. Operador NOT (NÃO)",
        "D. Operador XOR exclusivo"
      ],
      correct: 1
    },
    {
      question: "O que é um 'Loop Infinito' em programação e por que ele deve ser evitado?",
      options: [
        "A. Uma função que melhora a velocidade do processador.",
        "B. Uma repetição cuja condição de término nunca se torna falsa, travando a execução do programa.",
        "C. Um tipo especial de variável de ponto flutuante.",
        "D. Uma biblioteca de estilos visuais para botões."
      ],
      correct: 1
    }
  ]
};

// ============================================================
// GLOSSÁRIO TÉCNICO DESCOMPLICADO & DICAS PRÁTICAS
// Explicações simples em linguagem humana + macetes para questões
// ============================================================
const techTermsGlossary = {
  "mvp": {
    term: "MVP (Minimum Viable Product / Produto Mínimo Viável)",
    simple: "É a versão mais básica de um aplicativo ou produto que já funciona e entrega valor real ao cliente, sem perfumarias.",
    technique: "💡 Técnica de Prova: Pense no MVP como um 'patinete elétrico' antes de construir um 'carro esportivo'. Serve para testar se as pessoas realmente querem o produto sem gastar rios de dinheiro."
  },
  "stakeholder": {
    term: "Stakeholders (Partes Interessadas)",
    simple: "São todas as pessoas ou grupos que têm interesse, afetam ou são afetados pelo projeto (clientes, chefes, usuários finais, investidores e o próprio time).",
    technique: "💡 Macete: 'Stake' = fatia/interesse, 'Holder' = quem segura. Quem segura uma fatia do interesse no projeto é um Stakeholder."
  },
  "requisito": {
    term: "Requisitos Funcionais vs Não-Funcionais",
    simple: "Funcional é 'o que o sistema FAZ' (ex: fazer login, emitir nota). Não-Funcional é 'COMO ele se comporta' (ex: rapidez, segurança, suportar 1.000 usuários simultâneos).",
    technique: "💡 Técnica: Se começa com verbo de ação do usuário ('calcular', 'cadastrar', 'enviar') = Funcional. Se for adjetivo de qualidade ('rápido', 'seguro', 'responsivo') = Não-Funcional."
  },
  "prototip": {
    term: "Prototipagem de Software",
    simple: "Criar uma simulação visual ou maquete do sistema para mostrar ao cliente antes de programar de verdade.",
    technique: "💡 Técnica: Reduz custos e mal-entendidos. É muito mais barato alterar um desenho na tela do que reprogramar milhares de linhas de código prontas."
  },
  "valida": {
    term: "Verificação vs Validação (V&V)",
    simple: "Verificação: 'Estamos construindo o produto CORRETAMENTE?' (seguiu a especificação técnica?). Validação: 'Estamos construindo o PRODUTO CERTO?' (atende à real necessidade do cliente?).",
    technique: "💡 Macete: Verificação = olhar para o papel/código. Validação = olhar para a cara do cliente para ver se ele ficou satisfeito."
  },
  "polimorf": {
    term: "Polimorfismo (POO)",
    simple: "É a capacidade de objetos diferentes responderem à mesma ordem (método) de maneiras diferentes. Exemplo: a ordem 'TocarSom' faz o Pato 'grasnar' e o Cão 'latir'.",
    technique: "💡 Macete: 'Poli' (muitas) + 'morfo' (formas). O mesmo botão/método assume formas de agir diferentes dependendo de quem é o dono."
  },
  "herança": {
    term: "Herança (POO)",
    simple: "Mecanismo onde uma classe 'filha' herda automaticamente todos os atributos e funções de uma classe 'mãe', evitando duplicar código.",
    technique: "💡 Técnica: Pense no 'Cachorro' herdando características de 'Mamífero' (tem sangue quente, respira oxigênio). Reaproveitamento total."
  },
  "encapsula": {
    term: "Encapsulamento (POO)",
    simple: "Esconder os detalhes internos de funcionamento e proteger os dados para que ninguém mexa neles diretamente de fora sem permissão.",
    technique: "💡 Analogia: Pense em uma cápsula de remédio ou controle remoto. Você só aperta o botão 'Ligar' (interface pública), sem precisar cutucar a fiação elétrica de dentro (privada)."
  },
  "docker": {
    term: "Docker & Contêineres",
    simple: "Empacotar o aplicativo junto com tudo o que ele precisa para rodar em uma 'caixa mágica' padronizada, acabando com a desculpa 'na minha máquina funciona'.",
    technique: "💡 Diferença de VM: A Máquina Virtual carrega um sistema operacional pesado inteiro. O Docker é levíssimo porque compartilha o mesmo motor (kernel) com o computador hospedeiro."
  },
  "ci/cd": {
    term: "CI/CD (Integração Contínua / Entrega Contínua)",
    simple: "Esteira automatizada que testa e publica seu código em produção sozinho toda vez que você envia novidades para o repositório.",
    technique: "💡 Técnica: CI = Testar e juntar tudo automaticamente várias vezes ao dia. CD = Colocar no ar para o usuário sem precisar de intervenção manual demorada."
  },
  "mfa": {
    term: "MFA (Autenticação Multifator / 2FA)",
    simple: "Exigir mais de uma prova de identidade para liberar o acesso (ex: senha digitada + código que chega no celular por SMS ou app).",
    technique: "💡 Regra de Ouro: Baseia-se em combinar: Algo que você SABE (senha) + Algo que você TEM (celular) + Algo que você É (biometria)."
  },
  "privil": {
    term: "Princípio do Menor Privilégio",
    simple: "Dar ao usuário ou programa apenas o nível mínimo de permissão estritamente necessário para ele fazer o trabalho dele, nada a mais.",
    technique: "💡 Técnica: Se um estagiário só precisa ver relatórios, nunca dê acesso de administrador. Se ele for hackeado, o invasor não consegue destruir o banco de dados."
  },
  "rest": {
    term: "APIs REST e Stateless",
    simple: "Arquitetura onde cada pedido enviado ao servidor é independente e já carrega todas as informações necessárias, sem o servidor precisar guardar 'memória' de quem você é na sessão.",
    technique: "💡 Analogia: Um caixa de lanchonete sem memória. Em cada pedido você precisa apresentar seu crachá (token JWT), permitindo que qualquer caixa da lanchonete te atenda rápido."
  },
  "overfit": {
    term: "Overfitting (Sobreajuste na IA)",
    simple: "Quando a IA 'decora' os dados de treino palavra por palavra em vez de realmente 'aprender' a regra geral. Quando chega um dado novo da vida real, ela erra feio.",
    technique: "💡 Macete: É o aluno que decorou o gabarito da prova anterior. Se o professor trocar uma vírgula ou um número, ele não sabe resolver."
  },
  "scrum": {
    term: "Framework Scrum & Papéis Ágeis",
    simple: "Método de trabalho em equipe focado em ciclos curtos (Sprints de 1 a 4 semanas) com reuniões diárias e entregas constantes.",
    technique: "💡 Papéis Rápidos: PO (Product Owner) = define O QUE fazer e o valor. Scrum Master = ajuda o time a remover obstáculos. Dev Team = constrói o produto."
  },
  "kanban": {
    term: "Kanban & Limite de WIP (Work in Progress)",
    simple: "Quadro visual (A Fazer / Fazendo / Feito) com uma regra sagrada: limitar quantas tarefas podem estar 'Fazendo' ao mesmo tempo.",
    technique: "💡 Lema do Kanban: 'Pare de começar e comece a terminar!'. Limitar tarefas em andamento evita gargalos e cansaço da equipe."
  },
  "acid": {
    term: "Propriedades ACID (Bancos de Dados)",
    simple: "Conjunto de 4 garantias que evitam que transações críticas (como transferências de dinheiro) dêem erro pela metade e sumam com seus dados.",
    technique: "💡 Sigla: Atomicidade (tudo ou nada), Consistência (regras respeitadas), Isolamento (uma não atrapalha a outra), Durabilidade (salvou, não perde mais nem se faltar luz)."
  },
  "nosql": {
    term: "SQL (Relacional) vs NoSQL (Não-Relacional)",
    simple: "SQL organiza dados em tabelas rígidas e conectadas (como planilhas que se cruzam). NoSQL guarda em documentos livres (como pastas com arquivos JSON flexíveis).",
    technique: "💡 Quando usar: Transações bancárias e cadastros estritos = SQL (Postgres/MySQL). Alto volume de dados sem formato fixo ou redes sociais = NoSQL (MongoDB)."
  },
  "flutter": {
    term: "Stateless vs StatefulWidget (Flutter)",
    simple: "Stateless: Tela ou componente estático que nunca muda depois de desenhado (ex: um ícone ou título fixo). Stateful: Componente vivo que muda de cor, valor ou texto conforme o usuário clica.",
    technique: "💡 Macete: Tem clique que muda contador ou formulário? É Stateful! É só texto estático ou imagem? É Stateless!"
  },
  "nielsen": {
    term: "Heurísticas de Usabilidade de Nielsen",
    simple: "Regras de bom senso para criar interfaces onde o usuário nunca se sinta perdido, burro ou sem saber o que está acontecendo.",
    technique: "💡 Dica de Ouro: 'Visibilidade do Status' (mostre barra de carregando para ele saber que está processando) e 'Prevenção de Erros' (pergunte 'Tem certeza que deseja apagar?' antes de deletar)."
  },
  "gateway": {
    term: "API Gateway em Microsserviços",
    simple: "É a 'portaria' única de um condomínio de serviços. O aplicativo do usuário só fala com o Gateway, e ele se encarrega de encaminhar a mensagem para o microsserviço certo.",
    technique: "💡 Vantagem: Centraliza login, segurança e bloqueio de ataques em um só lugar, em vez de configurar em 50 servidores separados."
  },
  "circuit": {
    term: "Padrão Circuit Breaker (Disjuntor de Software)",
    simple: "Funciona igual ao disjuntor elétrico da sua casa: se um serviço externo começar a falhar e travar, ele 'desarma' temporariamente para proteger o resto do sistema de cair junto.",
    technique: "💡 Estados: Fechado (passa tudo normal), Aberto (bloqueia requisições para poupar recursos), Meio-Aberto (testa se o serviço lá fora já melhorou)."
  },
  "itil": {
    term: "ITIL: Gestão de Incidentes vs Gestão de Problemas",
    simple: "Incidente: O fogo que você precisa apagar AGORA para fazer o sistema voltar a funcionar. Problema: Investigar o que provocou a faísca para nunca mais pegar fogo.",
    technique: "💡 Macete: Incidente foca em RESTAURAR o serviço rapidamente. Problema foca em descobrir a CAUSA-RAIZ e prevenir."
  },
  "sla": {
    term: "SLA (Service Level Agreement / Acordo de Nível de Serviço)",
    simple: "Contrato formal que promete a qualidade mínima e o prazo de resposta (ex: 'O sistema ficará 99.9% do tempo no ar e chamados serão respondidos em até 2 horas').",
    technique: "💡 Macete: É a promessa com data e hora. Não cumpriu o SLA? A empresa prestadora paga multa."
  },
  "spark": {
    term: "Apache Spark & Processamento Distribuído",
    simple: "Ferramenta monstro para analisar bilhões de dados em segundos, dividindo a conta entre vários computadores ao mesmo tempo na memória RAM.",
    technique: "💡 Diferença: Em vez de ler devagar do disco rígido como antigamente, o Spark faz os cálculos em alta velocidade direto na memória RAM de um cluster de máquinas."
  },
  "tcp": {
    term: "TCP vs UDP (Protocolos de Transporte na Rede)",
    simple: "TCP é a entrega com aviso de recebimento pelos Correios: garante que cada pedaço chegou inteiro, na ordem certa. UDP é o rádio/TV ao vivo: cospe os dados na velocidade máxima, sem ligar se um pacote sumiu no caminho.",
    technique: "💡 Aplicação: Transferência de arquivos e páginas web = TCP (nada pode sumir). Jogos online e chamadas de vídeo ao vivo = UDP (o que importa é não ter atraso/lag)."
  },
  "dns": {
    term: "DNS (Domain Name System)",
    simple: "A lista telefônica da internet. Converte nomes fáceis que os humanos entendem (como cursaaqui.com.br) no endereço numérico real que os computadores usam (ex: 185.199.108.153).",
    technique: "💡 Macete: Sem DNS, você teria que decorar dezenas de números de IP com pontos para entrar nos seus sites favoritos."
  },
  "okr": {
    term: "OKRs (Objectives and Key Results)",
    simple: "Método para definir metas claras. Objetivo = ONDE queremos chegar (inspirador). Resultados-Chave (KRs) = COMO vamos medir se chegamos lá (com números e métricas estritas).",
    technique: "💡 Regra de Ouro: Se um Key Result não tem um número (ex: 'Aumentar de 10% para 40%'), ele não é um KR, é só uma tarefa comum."
  },
  "recursiv": {
    term: "Função Recursiva (Programação)",
    simple: "Uma função que chama a si mesma para resolver pedaços menores do mesmo problema, tendo SEMPRE uma condição de saída (caso base) para não travar.",
    technique: "💡 Analogia: Pense nas bonecas russas (Matrioska), onde você abre uma boneca e encontra outra igual menor dentro, até chegar na última bonequinha sólida que encerra a busca."
  },
  "vetor": {
    term: "Vetor / Array",
    simple: "Uma sequência de caixas numeradas na memória do computador, lado a lado, que guardam dados do mesmo tipo acessíveis por um índice (0, 1, 2...).",
    technique: "💡 Lembrete Universal: Na quase totalidade das linguagens de programação, a contagem de índices começa no ZERO (índice 0 é o primeiro elemento)."
  }
};

// Vincula as questões abertas e exercícios extras aos respectivos cursos
courses.forEach(c => {
  if (openQuestionsCatalog[c.id]) {
    c.openQuestion = openQuestionsCatalog[c.id];
  }
  if (extraQuizzesCatalog[c.id] && c.quiz) {
    // Evita duplicatas se já inserido
    extraQuizzesCatalog[c.id].forEach(extraQ => {
      const alreadyExists = c.quiz.some(q => q.question === extraQ.question);
      if (!alreadyExists) {
        c.quiz.push(extraQ);
      }
    });
  }
});

// Exporta para uso em navegadores e ambientes Node.js
if (typeof window !== 'undefined') {
  window.courses = courses;
  window.techTermsGlossary = techTermsGlossary;
}
if (typeof module !== 'undefined' && module.exports) {
  module.exports = courses;
  module.exports.courses = courses;
  module.exports.techTermsGlossary = techTermsGlossary;
}