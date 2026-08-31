const courses = [
  {
    id: 1,
    title: "Análise e Desenvolvimento de Sistemas - Manual Acadêmico",
    category: "Engenharia de Software",
    teacher: "Coordenação Acadêmica",
    duration: "40h",
    rating: 5.0,
    progress: 0,
    image: "https://images.unsplash.com/photo-1498050108023-c5249f4df085",
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
        correct: 2 // Alternativa C
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
        correct: 1 // Alternativa B
      },
      {
        question: "Um analista de sistemas ficou responsável por mapear o fluxo de navegação de um usuário em um sistema de e-commerce. O diagrama abaixo descreve a sequência de passos desde o login até a finalização da compra.<br><br><img src='imagem questão 3.png' alt='Diagrama de Navegação E-commerce' class='quiz-img'><br>Considerando o diagrama apresentado e as boas práticas de modelagem de processos, assinale a alternativa que descreve corretamente a transição entre o estado de 'Validação de Dados' e 'Confirmação de Pagamento':",
        options: [
          "A. Ocorre de forma estritamente assíncrona, dependendo de um callback externo.",
          "B. É condicional, avançando apenas se os dados forem validados com sucesso.",
          "C. Representa um loop infinito caso o cartão seja rejeitado mais de três vezes.",
          "D. É disparada automaticamente por um timer de sessão do usuário.",
          "E. Depende obrigatoriamente da intervenção manual do administrador do sistema."
        ],
        correct: 1 // Alternativa B
      },
      {
        question: "Analise a especificação do diagrama de classes UML abaixo, o qual modela parte de um subsistema corporativo de Recursos Humanos:<br><br><img src='imagem questão 4.png' alt='Diagrama de Classes RH' class='quiz-img'><br>Com base nas relações de associação, agregação e herança indicadas na imagem, assinale a afirmação correta:",
        options: [
          "A. A classe Funcionario é uma subclasse de Departamento.",
          "B. Se a instância de Departamento for destruída, todas as instâncias de Projeto associadas serão eliminadas imediatamente por composição.",
          "C. Um Gerente herda todos os atributos e métodos públicos/protegidos da classe Funcionario.",
          "D. A relação entre Funcionario e Projeto indica que um funcionário só pode trabalhar em exatamente um projeto por vez.",
          "E. O método calcularSalario() não pode ser sobrescrito pelas subclasses."
        ],
        correct: 2 // Alternativa C
      },
      {
        question: "No contexto da Orientação a Objetos, considere os conceitos de Coesão e Acoplamento, que são fundamentais para assegurar a manutenibilidade e a evolução de um ecossistema de software de grande porte. Uma arquitetura ideal busca alcançar:",
        options: [
          "A. Baixa Coesão e Alto Acoplamento.",
          "B. Alta Coesão e Baixo Acoplamento.",
          "C. Alta Coesão e Alto Acoplamento.",
          "D. Baixa Coesão e Baixo Acoplamento.",
          "E. Coesão Nula e Acoplamento Infinito."
        ],
        correct: 1 // Alternativa B
      },
      {
        question: "Um engenheiro de software está projetando uma rotina crítica de processamento financeiro e precisa decidir entre uma abordagem puramente Iterativa (utilizando laços como for/while) ou uma abordagem Recursiva (onde a função chama a si mesma até atingir uma condição de parada).<br><br>Do ponto de vista de arquitetura de hardware, consumo de memória e gerenciamento de processos, assinale a alternativa correta:",
        options: [
          "A. A recursão sempre consome menos memória que a iteração, pois reaproveita o mesmo frame de pilha (Stack Frame) indefinidamente.",
          "B. Cada chamada recursiva adiciona um novo registro na Pilha de Execução (Call Stack), o que pode levar ao erro de Stack Overflow caso a condição de parada falhe ou a profundidade seja excessiva.",
          "C. Laços de repetição tradicionais (iteração) são eliminados pelo compilador e convertidos em tabelas de distribuição hash.",
          "D. O consumo de memória de uma função recursiva é alocado exclusivamente na memória Heap dinâmica global de longo prazo.",
          "E. Sistemas operacionais modernos proíbem o uso de funções recursivas em código de modo usuário."
        ],
        correct: 1 // Alternativa B
      },
      {
        question: "A figura abaixo apresenta um diagrama de sequência UML que ilustra as interações de troca de mensagens entre os objetos de um sistema de controle de estoque de uma distribuidora de bebidas:<br><br><img src='imagem questão 7.png' alt='Diagrama de Sequência Estoque' class='quiz-img'><br>Considerando o ciclo de vida dos objetos e a ordem cronológica das mensagens (de cima para baixo) mostradas no diagrama, assinale a opção correta:",
        options: [
          "A. O objeto 'Interface' cria uma nova instância de 'Gerenciador' por meio de uma mensagem assíncrona logo no início da execução.",
          "B. A mensagem 'verificarDisponibilidade()' retorna um valor booleano diretamente para o ator antes do término do método.",
          "C. O diagrama demonstra que o método 'atualizarSaldo()' é executado de forma concorrente e sem travas de banco de dados.",
          "D. A seta pontilhada de retorno indica o encerramento do foco de controle da mensagem correspondente, devolvendo o fluxo ao remetente.",
          "E. O objeto 'Estoque' é destruído explicitamente por uma mensagem de 'destroy' ao final da operação."
        ],
        correct: 3 // Alternativa D
      },
      {
        question: "Em uma modelagem conceitual UML de um sistema de nutrição, analisa-se a classe Refeição e suas subclasses (Café, Almoço, Jantar). Nota: Esta questão foi oficialmente anulada no certame Enade devido à falta de combinação de gabarito técnico, mas os itens corretos afirmam que o método montarCardápioDiário() é sobrescrito e que as subclasses herdam o cálculo base da superclasse. Selecione a alternativa que representa este grupo correto (II, III e V):",
        options: [
          "A. Itens I e II estão corretos.",
          "B. Itens I e IV estão corretos.",
          "C. Itens II, III e V estão corretos.",
          "D. Itens III e V estão corretos apenas.",
          "E. Itens IV e V estão corretos."
        ],
        correct: 2 // Alternativa C
      },
      {
        question: "Considere o diagrama de transição de estados (Statechart) abaixo, que modela o comportamento da conexão de um socket de rede em um protocolo de comunicação customizado de uma aplicação distribuída:<br><br><img src='imagem questão 9.png' alt='Diagrama de Estados Socket' class='quiz-img'><br>Ao analisar os estados (retângulos de cantos arredondados) e as transições disparadas por eventos/condições, conclui-se corretamente que:",
        options: [
          "A. O estado 'Escutando' (Listening) só pode transicionar diretamente para o estado 'Fechado' (Closed).",
          "B. O gatilho 'timeout' faz o sistema retornar imediatamente ao estado inicial 'Iniciado' sem passar por limpezas.",
          "C. A partir do estado 'Conectado', o recebimento do evento 'desconectar' leva o socket ao estado 'Aguardando Fechamento'.",
          "D. O socket pode permanecer indefinidamente no estado 'Erro' sem possibilidade de reinicialização.",
          "E. Não existem condições de guarda (guards) mapeadas nas transições deste diagrama."
        ],
        correct: 2 // Alternativa C
      },
      {
        question: "No desenvolvimento de rotinas computacionais e estruturas de dados, a tipologia correta impacta o tempo de resposta do sistema. Considere uma estrutura de dados de uma Fila (FIFO - First In, First Out) e uma Pilha (LIFO - Last In, First Out). Ao inserirmos consecutivamente as cargas de dados A, B e C, quais serão respectivamente os primeiros elementos removidos na fila e na pilha?",
        options: [
          "A. A e C",
          "B. C e A",
          "C. A e A",
          "D. C e C",
          "E. B e B"
        ],
        correct: 0 // Alternativa A
      }
    ]
  }
];

if (typeof module !== 'undefined' && module.exports) {
  module.exports = courses;
}