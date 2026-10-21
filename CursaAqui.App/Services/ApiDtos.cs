using CursaAqui.App.Models;

namespace CursaAqui.App.Services;

// Formatos que a API devolve. Os nomes batem (sem diferenciar maiúsculas) com os DTOs da CursaAqui-API.

public record UsuarioApiDto(int Id, string Nome, string Email, string Perfil, string? Cpf, string? Telefone,
    DateTime? DataNascimento, bool Ativo, DateTime DataCadastro, string? NivelAutoridade,
    DateTime? UltimoAcessoGerencial, string? SetorAtendimento, string? Titulacao, string? Endereco, int? Xp, int? Nivel);

public record AuthRespostaDto(string Token, DateTime Expira, UsuarioApiDto Usuario);

public record AlunoApiDto(int Id, string Nome, string Email, string? Cpf, string? Telefone, bool Ativo, int Xp, int Nivel,
    DateTime DataCadastro, DateTime? DataNascimento, string? Endereco);

public record CursoApiDto(int Id, string Codigo, string Titulo, string Categoria, string? Area, string? Descricao,
    int CargaHoraria, int NivelMinimo, string? ImagemUrl, int? ProfessorId, string? ProfessorNome, bool Ativo,
    int? VagasTotais, int VagasOcupadas, int? VagasDisponiveis, decimal Avaliacao);

public record MatriculaApiDto(int Id, int AlunoId, string AlunoNome, string? AlunoCpf, int CursoId, string CursoTitulo,
    DateTime DataMatricula, StatusMatricula Status, string PeriodoLetivo, decimal? NotaFinal, int ProgressoPercentual,
    string? Observacoes, DateTime? DataConclusao);

public record AtendimentoApiDto(int Id, string Protocolo, TipoSolicitante TipoSolicitante, string SolicitanteNome,
    string? Contato, int? AlunoId, string Assunto, string Descricao, string? Providencia, StatusAtendimento Status,
    DateTime DataAbertura, DateTime? DataFechamento, int AtendenteId, string AtendenteNome);

public record ParametrosApiDto(string NomeInstituicao, string PeriodoLetivoAtual, decimal MediaAprovacaoMinima,
    int CargaHorariaMaximaAluno, int VagasPadraoPorCurso, bool PermitirNovasMatriculas, string EmailInstitucional,
    string TelefoneInstitucional, string EnderecoCampus, DateTime UltimaAtualizacao, string? AtualizadoPor);

public record PermissaoApiDto(int Id, string ChaveFuncionalidade, string NomeExibicao, string Modulo, string Descricao,
    bool PermitidoAdmin, bool PermitidoSecretaria);

public record ProfessorApiDto(int Id, string Nome, string? Titulacao, int TotalCursos);
