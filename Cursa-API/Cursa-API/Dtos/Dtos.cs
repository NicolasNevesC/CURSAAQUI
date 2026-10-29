using CursaAqui_API.Models;

namespace CursaAqui_API.Dtos;

// ============ AUTH / USUÁRIOS ============
public record LoginRequest(string Email, string Senha);
public record CadastroRequest(string Nome, string Email, string Senha, string? Cpf, string? Telefone, DateTime? DataNascimento);
public record AuthResponse(string Token, DateTime Expira, UsuarioDto Usuario);
public record AlterarSenhaRequest(string SenhaAtual, string NovaSenha);

public record UsuarioDto(int Id, string Nome, string Email, string Perfil, string? Cpf, string? Telefone,
    DateTime? DataNascimento, bool Ativo, DateTime DataCadastro, string? AvatarUrl, string? Tema, string? Acessibilidade,
    string? NivelAutoridade, DateTime? UltimoAcessoGerencial, string? SetorAtendimento, string? Titulacao,
    string? Endereco, int? Xp, int? Nivel);

public record PerfilUpdateRequest(string? Nome, string? Telefone, string? Cpf, DateTime? DataNascimento,
    string? Endereco, string? AvatarUrl, string? Tema, string? Acessibilidade);

public record UsuarioCreateRequest(PerfilUsuario Perfil, string Nome, string Email, string Senha, string? Cpf,
    string? Telefone, string? NivelAutoridade, string? SetorAtendimento, string? Titulacao);

public record UsuarioUpdateRequest(string? Nome, string? Email, string? Cpf, string? Telefone, string? NovaSenha,
    string? NivelAutoridade, string? SetorAtendimento, string? Titulacao);

public record AtivoRequest(bool Ativo);

// ============ PERMISSÕES / PARÂMETROS ============
public record PermissaoDto(int Id, string ChaveFuncionalidade, string NomeExibicao, string Modulo, string Descricao,
    bool PermitidoAdmin, bool PermitidoSecretaria);
public record PermissaoUpdateRequest(bool PermitidoAdmin, bool PermitidoSecretaria);
public record PermissaoLoteItem(int Id, bool PermitidoAdmin, bool PermitidoSecretaria);

public record ParametrosDto(string NomeInstituicao, string PeriodoLetivoAtual, decimal MediaAprovacaoMinima,
    int CargaHorariaMaximaAluno, int VagasPadraoPorCurso, bool PermitirNovasMatriculas, string EmailInstitucional,
    string TelefoneInstitucional, string EnderecoCampus, DateTime UltimaAtualizacao, string? AtualizadoPor);
public record ParametrosUpdateRequest(string NomeInstituicao, string PeriodoLetivoAtual, decimal MediaAprovacaoMinima,
    int CargaHorariaMaximaAluno, int VagasPadraoPorCurso, bool PermitirNovasMatriculas, string EmailInstitucional,
    string TelefoneInstitucional, string EnderecoCampus);
public record ParametrosPublicosDto(string NomeInstituicao, string PeriodoLetivoAtual, decimal MediaAprovacaoMinima,
    bool PermitirNovasMatriculas, string EmailInstitucional, string TelefoneInstitucional, string EnderecoCampus);

// ============ ALUNOS / PROFESSORES ============
public record AlunoResumoDto(int Id, string Nome, string Email, string? Cpf, string? Telefone, bool Ativo, int Xp,
    int Nivel, DateTime DataCadastro, DateTime? DataNascimento, string? Endereco);
public record HistoricoItemDto(int MatriculaId, int CursoId, string CursoTitulo, string PeriodoLetivo,
    StatusMatricula Status, decimal? NotaFinal, int ProgressoPercentual, DateTime DataMatricula);
public record AlunoFichaDto(UsuarioDto Aluno, List<HistoricoItemDto> Historico);
public record AlunoCreateRequest(string Nome, string Email, string Senha, string? Cpf, string? Telefone,
    DateTime? DataNascimento, string? Endereco);
public record AlunoUpdateRequest(string? Nome, string? Email, string? Cpf, string? Telefone, DateTime? DataNascimento,
    string? Endereco);

public record ProfessorDto(int Id, string Nome, string? Titulacao, int TotalCursos);
public record ProfessorResumoDto(int TotalAtividades, int Pendentes, int Corrigidas, int AlunosDistintos);
public record AlunoDoProfessorDto(int AlunoId, string Nome, string Email, int Atividades, int Corrigidas,
    decimal Aproveitamento);

// ============ CURSOS ============
public record CursoDto(int Id, string Codigo, string Titulo, string Categoria, string? Area, string? Descricao,
    int CargaHoraria, int NivelMinimo, string? ImagemUrl, int? ProfessorId, string? ProfessorNome, bool Ativo,
    int? VagasTotais, int VagasOcupadas, int? VagasDisponiveis, decimal Avaliacao, bool? Bloqueado, int? ProgressoAluno);
public record CursoDetalheDto(CursoDto Curso, List<DisciplinaDto> Disciplinas, List<MaterialDto> Materiais);
public record CursoRequest(string? Codigo, string Titulo, string Categoria, string? Area, string? Descricao,
    int CargaHoraria, int NivelMinimo, string? ImagemUrl, int? ProfessorId, int? VagasTotais, bool? Ativo);

public record DisciplinaDto(int Id, int CursoId, string Nome, string? Conteudo, int Ordem);
public record DisciplinaRequest(string Nome, string? Conteudo, int? Ordem);

public record MaterialDto(int Id, int CursoId, int? DisciplinaId, int? ProfessorId, string Titulo, TipoMaterial Tipo,
    string UrlConteudo, DateTime DataPostagem);
public record MaterialRequest(string Titulo, TipoMaterial Tipo, string UrlConteudo, int? DisciplinaId);
public class UploadMaterialForm
{
    public IFormFile? Arquivo { get; set; }
    public string? Titulo { get; set; }
    public int? DisciplinaId { get; set; }
}

public record AvaliacaoCursoDto(int Id, int AlunoId, string AlunoNome, int Nota, string? Comentario, DateTime Data, DateTime? DataAtualizacao = null);
public record AvaliacaoPublicaDto(int Id, string AlunoNome, int Nota, string? Comentario, DateTime Data, DateTime? DataAtualizacao);
public record AvaliacaoResumoDto(decimal Media, int Quantidade);
public record AvaliacaoCursoRequest(int Nota, string? Comentario);
public record TermoDto(string Chave, string Termo, string Explicacao, string? Tecnica);

// ============ MATRÍCULAS / CERTIFICADOS ============
public record MatriculaDto(int Id, int AlunoId, string AlunoNome, string? AlunoCpf, int CursoId, string CursoTitulo,
    DateTime DataMatricula, StatusMatricula Status, string PeriodoLetivo, decimal? NotaFinal, int ProgressoPercentual,
    string? Observacoes, DateTime? DataConclusao, bool MaterialAcessado, bool EstudoConcluido, bool TemCertificado);
public record MatriculaCreateRequest(int AlunoId, int CursoId, string? PeriodoLetivo);
public record MatriculaStatusRequest(StatusMatricula Status, string? Observacoes);
public record MatriculaLancamentoRequest(decimal? NotaFinal, int? ProgressoPercentual, string? Observacoes);
public record ProgressoRequest(bool? MaterialAcessado, bool? EstudoConcluido, bool? ExerciciosLiberados);
public record CertificadoDto(int Id, string CodigoValidacao, string AlunoNome, int CursoId, string CursoTitulo,
    int CargaHoraria, DateTime DataEmissao, int MatriculaId);

// ============ PROVAS / QUESTÕES / TENTATIVAS ============
public record ProvaDto(int Id, int CursoId, int? DisciplinaId, string Titulo, TipoProva Tipo, decimal NotaMaxima,
    decimal NotaMinima, DateTime DataCriacao, int TotalQuestoes);
public record ProvaRequest(string Titulo, int? DisciplinaId, TipoProva? Tipo, decimal? NotaMaxima, decimal? NotaMinima);
public record AlternativaDto(int Ordem, string Texto);
public record QuestaoProfDto(int Id, int ProvaId, TipoQuestao Tipo, int Ordem, string Enunciado, int? RespostaCorreta,
    List<AlternativaDto> Alternativas, string? Orientacoes, string? PalavrasChave, int? MinPalavras);
public record ProvaProfDto(ProvaDto Prova, List<QuestaoProfDto> Questoes);
public record QuestaoRequest(TipoQuestao Tipo, string Enunciado, int? Ordem, int? RespostaCorreta,
    List<string>? Alternativas, string? Orientacoes, string? PalavrasChave, int? MinPalavras);
public record QuestaoAlunoDto(int Id, TipoQuestao Tipo, int Ordem, string Enunciado, List<AlternativaDto> Alternativas,
    string? Orientacoes, int? MinPalavras);
public record ProvaAlunoDto(int ProvaId, int CursoId, string Titulo, decimal NotaMaxima, decimal NotaMinima,
    List<QuestaoAlunoDto> Questoes);

public record RespostaItemRequest(int QuestaoId, int? AlternativaEscolhida);
public record TentativaRequest(List<RespostaItemRequest> Respostas, string? RespostaDissertativa);
public record TentativaResultadoDto(int Id, int ProvaId, int CursoId, int Acertos, int TotalQuestoes,
    decimal PercentualAcerto, decimal NotaObtida, bool Aprovada, StatusTentativa Status, DateTime DataRealizacao,
    int ProgressoPercentual, int XpGanho, int XpTotal, int Nivel, bool SubiuNivel, bool ConcluiuCurso);
public record TentativaListaDto(int Id, int AlunoId, string AlunoNome, string AlunoEmail, int CursoId, string CursoTitulo,
    int Acertos, int TotalQuestoes, decimal PercentualAcerto, decimal NotaObtida, bool Aprovada, StatusTentativa Status,
    DateTime DataRealizacao, string? Feedback, DateTime? DataCorrecao);
public record RespostaDetalheDto(int QuestaoId, int Ordem, string Enunciado, List<AlternativaDto> Alternativas,
    int? AlternativaEscolhida, int? RespostaCorreta, bool Correta);
public record TentativaDetalheDto(TentativaListaDto Tentativa, string? RespostaDissertativa,
    QuestaoProfDto? QuestaoDissertativa, List<RespostaDetalheDto> Respostas);
public record CorrecaoRequest(string? Feedback);

public record TreinoQuestaoDto(int QuestaoId, int CursoId, string Enunciado, List<AlternativaDto> Alternativas);
public record TreinoRespostaRequest(int QuestaoId, int AlternativaEscolhida);
public record TreinoRespostaDto(bool Correta, int RespostaCorreta, int XpGanho, int XpTotal, int Nivel, bool SubiuNivel);

// ============ ÁREA DO ALUNO ============
public record ConquistaDto(int Id, string Icone, string Titulo, string Descricao, bool Desbloqueada);
public record DashboardCursoDto(int CursoId, string Titulo, int Progresso, StatusMatricula Status);
public record DashboardAlunoDto(string Nome, int Xp, int Nivel, int XpProximoNivel, int CursosEmAndamento,
    int CursosConcluidos, int Certificados, int MetasPendentes, int TentativasPendentes,
    List<DashboardCursoDto> Cursos, List<ConquistaDto> Conquistas);

// ============ CHAT ============
public record MensagemDto(int Id, int CursoId, int AlunoId, int? RemetenteId, string RemetenteNome,
    PerfilUsuario? RemetentePerfil, bool EhSistema, string Texto, DateTime DataEnvio);
public record MensagemRequest(string Texto, int? AlunoId);
public record ChatAlunoDto(int AlunoId, string Nome, string Email, DateTime UltimaMensagem);

// ============ ATENDIMENTOS / RELATÓRIOS / FEEDBACK ============
public record AtendimentoDto(int Id, string Protocolo, TipoSolicitante TipoSolicitante, string SolicitanteNome,
    string? Contato, int? AlunoId, string Assunto, string Descricao, string? Providencia, StatusAtendimento Status,
    DateTime DataAbertura, DateTime? DataFechamento, int AtendenteId, string AtendenteNome);
public record AtendimentoCreateRequest(TipoSolicitante TipoSolicitante, string? SolicitanteNome, string? Contato,
    int? AlunoId, string Assunto, string Descricao, string? Providencia);
public record AtendimentoUpdateRequest(string? SolicitanteNome, string? Contato, string? Assunto, string? Descricao,
    string? Providencia);
public record AtendimentoStatusRequest(StatusAtendimento Status, string? Providencia);

public record RelatorioCursoLinhaDto(int CursoId, string Titulo, int TotalMatriculas, int Ativas, int Concluidas,
    decimal MediaNotas);
public record RelatorioGerencialDto(int TotalAlunos, int CursosAtivos, int MatriculasAtivas, int Concluidas,
    decimal TaxaConclusao, List<RelatorioCursoLinhaDto> PorCurso);
public record RelatorioAtendenteDto(int AtendenteId, string Nome, int Total);
public record RelatorioAtendimentosDto(int Total, Dictionary<string, int> PorStatus,
    List<RelatorioAtendenteDto> PorAtendente, List<AtendimentoDto> Itens);
public record RelatorioPendenciasDto(decimal MediaMinima, int Total, List<MatriculaDto> Itens);

public record FeedbackRequest(TipoFeedback Tipo, string Assunto, string Mensagem);
public record FeedbackDto(int Id, int UsuarioId, string UsuarioNome, TipoFeedback Tipo, string Assunto, string Mensagem,
    StatusFeedback Status, DateTime DataEnvio, string? Resposta, string? RespondidoPorNome, DateTime? DataResposta);
public record FeedbackRespostaRequest(string Resposta);
