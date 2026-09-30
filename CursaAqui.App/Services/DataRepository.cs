using System.Text.Json;
using CursaAqui.App.Models;

namespace CursaAqui.App.Services;

public class DataContainer
{
    public List<AdminDataDto> Administradores { get; set; } = new();
    public List<SecretariaDataDto> Secretarias { get; set; } = new();
    public List<Aluno> Alunos { get; set; } = new();
    public List<Curso> Cursos { get; set; } = new();
    public List<Matricula> Matriculas { get; set; } = new();
    public List<Atendimento> Atendimentos { get; set; } = new();
    public ParametrosSistema Parametros { get; set; } = new();
    public List<PermissaoSistema> Permissoes { get; set; } = new();
}

public class AdminDataDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
    public string NivelAutoridade { get; set; } = "Total";
}

public class SecretariaDataDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
    public string SetorAtendimento { get; set; } = "Secretaria Acadêmica";
}

public class DataRepository
{
    private static DataRepository? _instance;
    public static DataRepository Instance => _instance ??= new DataRepository();

    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public List<UsuarioInterno> UsuariosInternos { get; private set; } = new();
    public List<Aluno> Alunos { get; private set; } = new();
    public List<Curso> Cursos { get; private set; } = new();
    public List<Matricula> Matriculas { get; private set; } = new();
    public List<Atendimento> Atendimentos { get; private set; } = new();
    public ParametrosSistema Parametros { get; set; } = new();
    public List<PermissaoSistema> Permissoes { get; private set; } = new();

    private DataRepository()
    {
        var appData = AppDomain.CurrentDomain.BaseDirectory;
        _filePath = Path.Combine(appData, "cursaaqui_db.json");
        CarregarOuInicializar();
    }

    public void Salvar()
    {
        try
        {
            var container = new DataContainer
            {
                Alunos = Alunos,
                Cursos = Cursos,
                Matriculas = Matriculas,
                Atendimentos = Atendimentos,
                Parametros = Parametros,
                Permissoes = Permissoes
            };

            foreach (var u in UsuariosInternos)
            {
                if (u is Administrador admin)
                {
                    container.Administradores.Add(new AdminDataDto
                    {
                        Id = admin.Id,
                        Nome = admin.Nome,
                        Email = admin.Email,
                        Senha = admin.Senha,
                        Cpf = admin.Cpf,
                        Telefone = admin.Telefone,
                        DataCadastro = admin.DataCadastro,
                        Ativo = admin.Ativo,
                        NivelAutoridade = admin.NivelAutoridade
                    });
                }
                else if (u is Secretaria sec)
                {
                    container.Secretarias.Add(new SecretariaDataDto
                    {
                        Id = sec.Id,
                        Nome = sec.Nome,
                        Email = sec.Email,
                        Senha = sec.Senha,
                        Cpf = sec.Cpf,
                        Telefone = sec.Telefone,
                        DataCadastro = sec.DataCadastro,
                        Ativo = sec.Ativo,
                        SetorAtendimento = sec.SetorAtendimento
                    });
                }
            }

            var json = JsonSerializer.Serialize(container, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception)
        {
            // Tratamento silencioso para garantir estabilidade em ambiente de leitura restrita
        }
    }

    private void CarregarOuInicializar()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                var container = JsonSerializer.Deserialize<DataContainer>(json, _jsonOptions);
                if (container != null && (container.Administradores.Count > 0 || container.Secretarias.Count > 0))
                {
                    Alunos = container.Alunos;
                    Cursos = container.Cursos;
                    Matriculas = container.Matriculas;
                    Atendimentos = container.Atendimentos;
                    Parametros = container.Parametros ?? new ParametrosSistema();
                    Permissoes = container.Permissoes;

                    UsuariosInternos = new List<UsuarioInterno>();
                    foreach (var a in container.Administradores)
                    {
                        UsuariosInternos.Add(new Administrador
                        {
                            Id = a.Id,
                            Nome = a.Nome,
                            Email = a.Email,
                            Senha = a.Senha,
                            Cpf = a.Cpf,
                            Telefone = a.Telefone,
                            DataCadastro = a.DataCadastro,
                            Ativo = a.Ativo,
                            NivelAutoridade = a.NivelAutoridade
                        });
                    }

                    foreach (var s in container.Secretarias)
                    {
                        UsuariosInternos.Add(new Secretaria
                        {
                            Id = s.Id,
                            Nome = s.Nome,
                            Email = s.Email,
                            Senha = s.Senha,
                            Cpf = s.Cpf,
                            Telefone = s.Telefone,
                            DataCadastro = s.DataCadastro,
                            Ativo = s.Ativo,
                            SetorAtendimento = s.SetorAtendimento
                        });
                    }
                    return;
                }
            }
            catch
            {
                // Se falhar a leitura, carrega dados semente
            }
        }

        InicializarDadosPadrao();
        Salvar();
    }

    private void InicializarDadosPadrao()
    {
        UsuariosInternos = new List<UsuarioInterno>
        {
            new Administrador
            {
                Id = 1,
                Nome = "Carlos Eduardo Menezes",
                Email = "admin@cursaaqui.com",
                Senha = "123",
                Cpf = "111.222.333-44",
                Telefone = "(11) 98111-2233",
                DataCadastro = DateTime.Now.AddMonths(-12),
                Ativo = true,
                NivelAutoridade = "Diretoria Acadêmica"
            },
            new Administrador
            {
                Id = 2,
                Nome = "Mariana Castro",
                Email = "mariana.admin@cursaaqui.com",
                Senha = "123",
                Cpf = "222.333.444-55",
                Telefone = "(11) 98222-3344",
                DataCadastro = DateTime.Now.AddMonths(-6),
                Ativo = true,
                NivelAutoridade = "Gestão de TI"
            },
            new Secretaria
            {
                Id = 3,
                Nome = "Fernanda Oliveira",
                Email = "secretaria@cursaaqui.com",
                Senha = "123",
                Cpf = "333.444.555-66",
                Telefone = "(11) 98333-4455",
                DataCadastro = DateTime.Now.AddMonths(-8),
                Ativo = true,
                SetorAtendimento = "Atendimento ao Aluno"
            },
            new Secretaria
            {
                Id = 4,
                Nome = "Lucas Ribeiro",
                Email = "lucas.sec@cursaaqui.com",
                Senha = "123",
                Cpf = "444.555.666-77",
                Telefone = "(11) 98444-5566",
                DataCadastro = DateTime.Now.AddMonths(-3),
                Ativo = true,
                SetorAtendimento = "Secretaria Geral"
            }
        };

        Alunos = new List<Aluno>
        {
            new Aluno
            {
                Id = 101,
                Nome = "Matheus Narvaes",
                Email = "aluno@cursaaqui.com",
                Cpf = "555.666.777-88",
                Telefone = "(11) 98765-4321",
                DataNascimento = new DateTime(2002, 5, 14),
                Endereco = "Rua das Flores, 120 - Vila Mariana, São Paulo - SP",
                DataCadastro = DateTime.Now.AddMonths(-6),
                Ativo = true,
                Xp = 450,
                Nivel = 2
            },
            new Aluno
            {
                Id = 102,
                Nome = "Beatriz Lima Santos",
                Email = "beatriz.santos@email.com",
                Cpf = "666.777.888-99",
                Telefone = "(11) 97654-3210",
                DataNascimento = new DateTime(2001, 8, 22),
                Endereco = "Av. Brasil, 450 - Pinheiros, São Paulo - SP",
                DataCadastro = DateTime.Now.AddMonths(-5),
                Ativo = true,
                Xp = 820,
                Nivel = 4
            },
            new Aluno
            {
                Id = 103,
                Nome = "Rodrigo Albuquerque",
                Email = "rodrigo.alb@email.com",
                Cpf = "777.888.999-00",
                Telefone = "(21) 99876-1234",
                DataNascimento = new DateTime(1999, 11, 30),
                Endereco = "Rua do Catete, 88 - Catete, Rio de Janeiro - RJ",
                DataCadastro = DateTime.Now.AddMonths(-4),
                Ativo = true,
                Xp = 210,
                Nivel = 1
            },
            new Aluno
            {
                Id = 104,
                Nome = "Juliana Mendes Rocha",
                Email = "juliana.mendes@email.com",
                Cpf = "888.999.000-11",
                Telefone = "(31) 98888-5544",
                DataNascimento = new DateTime(2000, 3, 10),
                Endereco = "Av. Afonso Pena, 1500 - Centro, Belo Horizonte - MG",
                DataCadastro = DateTime.Now.AddMonths(-7),
                Ativo = true,
                Xp = 1200,
                Nivel = 5
            },
            new Aluno
            {
                Id = 105,
                Nome = "Gabriel Silveira",
                Email = "gabriel.silv@email.com",
                Cpf = "999.000.111-22",
                Telefone = "(41) 99123-4567",
                DataNascimento = new DateTime(2003, 1, 18),
                Endereco = "Rua XV de Novembro, 700 - Centro, Curitiba - PR",
                DataCadastro = DateTime.Now.AddMonths(-2),
                Ativo = true,
                Xp = 95,
                Nivel = 1
            },
            new Aluno
            {
                Id = 106,
                Nome = "Camila Duarte",
                Email = "camila.duarte@email.com",
                Cpf = "123.456.789-01",
                Telefone = "(19) 98234-5678",
                DataNascimento = new DateTime(1998, 7, 5),
                Endereco = "Rua Barão de Jaguara, 310 - Cambuí, Campinas - SP",
                DataCadastro = DateTime.Now.AddMonths(-3),
                Ativo = true,
                Xp = 950,
                Nivel = 4
            }
        };

        Cursos = new List<Curso>
        {
            new Curso
            {
                Id = 1,
                Codigo = "CRS-ADS01",
                Titulo = "Análise e Desenvolvimento de Sistemas - Manual Acadêmico",
                Categoria = "Engenharia de Software",
                CargaHoraria = 40,
                ProfessorResponsavel = "Coordenação Acadêmica",
                Ativo = true,
                VagasTotais = 60,
                VagasOcupadas = 2,
                Descricao = "Fundamentos de requisitos, ciclo de vida de software, prototipagem, POO e modelagem conceitual.",
                Avaliacao = 5.0
            },
            new Curso
            {
                Id = 2,
                Codigo = "CRS-CLD02",
                Titulo = "Cloud Computing e DevOps",
                Categoria = "Infraestrutura & Cloud",
                CargaHoraria = 60,
                ProfessorResponsavel = "Prof. Ricardo Silva",
                Ativo = true,
                VagasTotais = 50,
                VagasOcupadas = 2,
                Descricao = "Arquitetura em nuvem AWS/Azure, containers Docker, orquestração Kubernetes e pipelines CI/CD.",
                Avaliacao = 4.9
            },
            new Curso
            {
                Id = 3,
                Codigo = "CRS-SEC03",
                Titulo = "Segurança da Informação e LGPD",
                Categoria = "Cibersegurança",
                CargaHoraria = 45,
                ProfessorResponsavel = "Prof. Roberto Lima",
                Ativo = true,
                VagasTotais = 40,
                VagasOcupadas = 1,
                Descricao = "Proteção de dados, conformidade com a LGPD, criptografia e boas práticas de defesa cibernética.",
                Avaliacao = 4.8
            },
            new Curso
            {
                Id = 4,
                Codigo = "CRS-AIM04",
                Titulo = "Inteligência Artificial e Machine Learning",
                Categoria = "Ciência de Dados",
                CargaHoraria = 80,
                ProfessorResponsavel = "Profa. Aline Prado",
                Ativo = true,
                VagasTotais = 45,
                VagasOcupadas = 1,
                Descricao = "Algoritmos de aprendizado supervisionado, redes neurais artificiais e processamento com Python.",
                Avaliacao = 4.9
            },
            new Curso
            {
                Id = 5,
                Codigo = "CRS-DSG05",
                Titulo = "Design de Interfaces e UX/UI Moderno",
                Categoria = "Design Digital",
                CargaHoraria = 35,
                ProfessorResponsavel = "Profa. Paula Ferreira",
                Ativo = true,
                VagasTotais = 50,
                VagasOcupadas = 1,
                Descricao = "Design thinking, wireframes de alta fidelidade no Figma, testes de usabilidade e heurísticas.",
                Avaliacao = 5.0
            },
            new Curso
            {
                Id = 6,
                Codigo = "CRS-AGL06",
                Titulo = "Gestão Ágil de Projetos com Scrum e Kanban",
                Categoria = "Gestão & Negócios",
                CargaHoraria = 30,
                ProfessorResponsavel = "Prof. Marcelo Dias",
                Ativo = true,
                VagasTotais = 55,
                VagasOcupadas = 1,
                Descricao = "Papéis do Scrum, cerimônias ágeis, métricas de fluxo e aplicação prática em equipes modernas.",
                Avaliacao = 4.7
            }
        };

        Matriculas = new List<Matricula>
        {
            new Matricula
            {
                Id = 1,
                AlunoId = 101,
                AlunoNome = "Matheus Narvaes",
                AlunoCpf = "555.666.777-88",
                CursoId = 1,
                CursoNome = "Análise e Desenvolvimento de Sistemas - Manual Acadêmico",
                DataMatricula = DateTime.Now.AddMonths(-4),
                Status = StatusMatricula.Ativa,
                NotaFinal = 8.5,
                ProgressoPercentual = 85,
                PeriodoLetivo = "2026.1",
                Observacoes = "Aluno destaque na disciplina de requisitos."
            },
            new Matricula
            {
                Id = 2,
                AlunoId = 101,
                AlunoNome = "Matheus Narvaes",
                AlunoCpf = "555.666.777-88",
                CursoId = 2,
                CursoNome = "Cloud Computing e DevOps",
                DataMatricula = DateTime.Now.AddMonths(-3),
                Status = StatusMatricula.Concluida,
                NotaFinal = 9.2,
                ProgressoPercentual = 100,
                PeriodoLetivo = "2026.1",
                Observacoes = "Curso concluído com emissão de certificado digital."
            },
            new Matricula
            {
                Id = 3,
                AlunoId = 101,
                AlunoNome = "Matheus Narvaes",
                AlunoCpf = "555.666.777-88",
                CursoId = 3,
                CursoNome = "Segurança da Informação e LGPD",
                DataMatricula = DateTime.Now.AddMonths(-2),
                Status = StatusMatricula.Ativa,
                NotaFinal = 7.0,
                ProgressoPercentual = 40,
                PeriodoLetivo = "2026.1",
                Observacoes = "Em andamento regular."
            },
            new Matricula
            {
                Id = 4,
                AlunoId = 102,
                AlunoNome = "Beatriz Lima Santos",
                AlunoCpf = "666.777.888-99",
                CursoId = 1,
                CursoNome = "Análise e Desenvolvimento de Sistemas - Manual Acadêmico",
                DataMatricula = DateTime.Now.AddMonths(-3),
                Status = StatusMatricula.Ativa,
                NotaFinal = 8.0,
                ProgressoPercentual = 70,
                PeriodoLetivo = "2026.1",
                Observacoes = "Entregou todas as atividades do módulo 1."
            },
            new Matricula
            {
                Id = 5,
                AlunoId = 103,
                AlunoNome = "Rodrigo Albuquerque",
                AlunoCpf = "777.888.999-00",
                CursoId = 2,
                CursoNome = "Cloud Computing e DevOps",
                DataMatricula = DateTime.Now.AddMonths(-1),
                Status = StatusMatricula.Ativa,
                NotaFinal = 6.5,
                ProgressoPercentual = 30,
                PeriodoLetivo = "2026.1",
                Observacoes = "Aguardando entrega de laboratório prático."
            },
            new Matricula
            {
                Id = 6,
                AlunoId = 104,
                AlunoNome = "Juliana Mendes Rocha",
                AlunoCpf = "888.999.000-11",
                CursoId = 5,
                CursoNome = "Design de Interfaces e UX/UI Moderno",
                DataMatricula = DateTime.Now.AddMonths(-4),
                Status = StatusMatricula.Concluida,
                NotaFinal = 10.0,
                ProgressoPercentual = 100,
                PeriodoLetivo = "2026.1",
                Observacoes = "Projeto final de prototipagem avaliado com excelência."
            },
            new Matricula
            {
                Id = 7,
                AlunoId = 105,
                AlunoNome = "Gabriel Silveira",
                AlunoCpf = "999.000.111-22",
                CursoId = 4,
                CursoNome = "Inteligência Artificial e Machine Learning",
                DataMatricula = DateTime.Now.AddMonths(-2),
                Status = StatusMatricula.Trancada,
                NotaFinal = 5.0,
                ProgressoPercentual = 20,
                PeriodoLetivo = "2026.1",
                Observacoes = "Solicitação formal de trancamento aprovada pela secretaria."
            },
            new Matricula
            {
                Id = 8,
                AlunoId = 106,
                AlunoNome = "Camila Duarte",
                AlunoCpf = "123.456.789-01",
                CursoId = 6,
                CursoNome = "Gestão Ágil de Projetos com Scrum e Kanban",
                DataMatricula = DateTime.Now.AddMonths(-2),
                Status = StatusMatricula.Ativa,
                NotaFinal = 9.0,
                ProgressoPercentual = 90,
                PeriodoLetivo = "2026.1",
                Observacoes = "Fase de conclusão de estudos de caso."
            }
        };

        Atendimentos = new List<Atendimento>
        {
            new Atendimento
            {
                Id = 1,
                Protocolo = "ATEND-2026-001",
                SolicitanteNome = "Matheus Narvaes",
                TipoSolicitante = TipoSolicitante.Aluno,
                Contato = "(11) 98765-4321 / aluno@cursaaqui.com",
                Assunto = "Emissão e Validação de Certificado Digital",
                Descricao = "Aluno finalizou 100% da carga horária de Cloud Computing e solicitou verificação formal do código hash de validação.",
                Providencia = "Certificado validado no sistema e enviado arquivo assinado em PDF com chave de autenticidade.",
                DataAbertura = DateTime.Now.AddDays(-10),
                DataFechamento = DateTime.Now.AddDays(-9),
                AtendenteId = 3,
                AtendenteNome = "Fernanda Oliveira",
                Status = StatusAtendimento.Concluido
            },
            new Atendimento
            {
                Id = 2,
                Protocolo = "ATEND-2026-002",
                SolicitanteNome = "Beatriz Lima Santos",
                TipoSolicitante = TipoSolicitante.Aluno,
                Contato = "(11) 97654-3210",
                Assunto = "Dúvida sobre Calendário de Rematrícula 2026.2",
                Descricao = "Aluna procurou a secretaria presencialmente solicitando informações do edital para o próximo período letivo.",
                Providencia = "Prestada orientação das datas oficiais e fornecido folheto do calendário acadêmico.",
                DataAbertura = DateTime.Now.AddDays(-5),
                DataFechamento = DateTime.Now.AddDays(-5),
                AtendenteId = 3,
                AtendenteNome = "Fernanda Oliveira",
                Status = StatusAtendimento.Concluido
            },
            new Atendimento
            {
                Id = 3,
                Protocolo = "ATEND-2026-003",
                SolicitanteNome = "Julio Cesar Mendes",
                TipoSolicitante = TipoSolicitante.Visitante,
                Contato = "julio.mendes@gmail.com",
                Assunto = "Informações sobre Novos Cursos de Tecnologia",
                Descricao = "Visitante compareceu à instituição com interesse em saber pré-requisitos e formato dos cursos online gratuitos.",
                Providencia = "Apresentada a plataforma web Cursa Aqui e efetuado pré-cadastro de interesse.",
                DataAbertura = DateTime.Now.AddDays(-2),
                DataFechamento = null,
                AtendenteId = 4,
                AtendenteNome = "Lucas Ribeiro",
                Status = StatusAtendimento.EmAndamento
            },
            new Atendimento
            {
                Id = 4,
                Protocolo = "ATEND-2026-004",
                SolicitanteNome = "Gabriel Silveira",
                TipoSolicitante = TipoSolicitante.Aluno,
                Contato = "(41) 99123-4567",
                Assunto = "Pedido de Trancamento Provisório",
                Descricao = "Solicitou trancamento da matrícula CRS-AIM04 por viagem a trabalho durante 60 dias.",
                Providencia = "Registrado protocolo e encaminhado termo de trancamento para assinatura digital.",
                DataAbertura = DateTime.Now.AddHours(-18),
                DataFechamento = null,
                AtendenteId = 3,
                AtendenteNome = "Fernanda Oliveira",
                Status = StatusAtendimento.Aberto
            }
        };

        Parametros = new ParametrosSistema
        {
            NomeInstituicao = "Cursa Aqui - Plataforma Educacional",
            PeriodoLetivoAtual = "2026.1",
            MediaAprovacaoMinima = 7.0,
            CargaHorariaMaximaAluno = 240,
            VagasPadraoPorCurso = 60,
            PermitirNovasMatriculas = true,
            EmailInstitucional = "secretaria@cursaaqui.com.br",
            TelefoneInstitucional = "(11) 3456-7890",
            EnderecoCampus = "Av. Paulista, 1000 - Bela Vista, São Paulo - SP",
            UltimaAtualizacao = DateTime.Now,
            AtualizadoPor = "admin@cursaaqui.com"
        };

        Permissoes = new List<PermissaoSistema>
        {
            new PermissaoSistema
            {
                ChaveFuncionalidade = "GerenciarUsuarios",
                NomeExibicao = "Gerenciar Usuários Internos",
                Modulo = "Administração",
                PermitidoAdmin = true,
                PermitidoSecretaria = false,
                Descricao = "Cadastrar, editar e inativar contas de Administradores e da Secretaria."
            },
            new PermissaoSistema
            {
                ChaveFuncionalidade = "GerenciarPermissoes",
                NomeExibicao = "Gerenciar Permissões de Acesso",
                Modulo = "Administração",
                PermitidoAdmin = true,
                PermitidoSecretaria = false,
                Descricao = "Definir, alterar e revogar níveis de acesso aos recursos do sistema."
            },
            new PermissaoSistema
            {
                ChaveFuncionalidade = "GerenciarCursos",
                NomeExibicao = "Gerenciar Catálogo de Cursos",
                Modulo = "Acadêmico",
                PermitidoAdmin = true,
                PermitidoSecretaria = false,
                Descricao = "Cadastrar, editar, inativar cursos, cargas horárias e vagas."
            },
            new PermissaoSistema
            {
                ChaveFuncionalidade = "VisualizarRelatorios",
                NomeExibicao = "Visualizar Relatórios Gerenciais",
                Modulo = "Inteligência & Gestão",
                PermitidoAdmin = true,
                PermitidoSecretaria = false,
                Descricao = "Consultar indicadores estratégicos, matrículas por curso e taxas analíticas."
            },
            new PermissaoSistema
            {
                ChaveFuncionalidade = "ConfigurarParametros",
                NomeExibicao = "Configurar Parâmetros do Sistema",
                Modulo = "Governança",
                PermitidoAdmin = true,
                PermitidoSecretaria = false,
                Descricao = "Ajustar período letivo vigente, notas mínimas e travas de negócio."
            },
            new PermissaoSistema
            {
                ChaveFuncionalidade = "GerenciarMatriculas",
                NomeExibicao = "Gerenciar Matrículas",
                Modulo = "Secretaria",
                PermitidoAdmin = true,
                PermitidoSecretaria = true,
                Descricao = "Realizar, alterar, renovar e cancelar matrículas de alunos."
            },
            new PermissaoSistema
            {
                ChaveFuncionalidade = "ConsultarCadastro",
                NomeExibicao = "Consultar Cadastro de Alunos",
                Modulo = "Secretaria",
                PermitidoAdmin = true,
                PermitidoSecretaria = true,
                Descricao = "Localizar dados cadastrais, histórico e cursos matriculados."
            },
            new PermissaoSistema
            {
                ChaveFuncionalidade = "RegistrarAtendimentos",
                NomeExibicao = "Registrar e Tratar Atendimentos",
                Modulo = "Secretaria",
                PermitidoAdmin = true,
                PermitidoSecretaria = true,
                Descricao = "Gerar protocolos de atendimento, providências e histórico de contatos."
            },
            new PermissaoSistema
            {
                ChaveFuncionalidade = "GerarRelatoriosAdministrativos",
                NomeExibicao = "Gerar Relatórios Administrativos",
                Modulo = "Secretaria",
                PermitidoAdmin = true,
                PermitidoSecretaria = true,
                Descricao = "Emitir listas operacionais de matriculados, atendimentos e pendências."
            }
        };
    }
}
