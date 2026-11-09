// Formatos devolvidos pela Cursa-API (espelham Cursa-API/Cursa-API/Dtos/Dtos.cs).
// Enums vêm como texto (JsonStringEnumConverter).

export type StatusMatricula = 'Ativa' | 'Renovada' | 'Concluida' | 'Cancelada' | 'Trancada' | string;

export type Usuario = {
  id: number;
  nome: string;
  email: string;
  perfil: string;
  cpf: string | null;
  telefone: string | null;
  avatarUrl: string | null;
  xp: number | null;
  nivel: number | null;
};

export type AuthResponse = {
  token: string;
  expira: string;
  usuario: Usuario;
};

export type CadastroRequest = {
  nome: string;
  email: string;
  senha: string;
  cpf?: string | null;
  telefone?: string | null;
};

export type DashboardCurso = {
  cursoId: number;
  titulo: string;
  progresso: number;
  status: StatusMatricula;
};

export type Conquista = {
  id: number;
  icone: string;
  titulo: string;
  descricao: string;
  desbloqueada: boolean;
};

export type DashboardAluno = {
  nome: string;
  xp: number;
  nivel: number;
  xpProximoNivel: number;
  cursosEmAndamento: number;
  cursosConcluidos: number;
  certificados: number;
  tentativasPendentes: number;
  cursos: DashboardCurso[];
  conquistas: Conquista[];
};

export type Curso = {
  id: number;
  codigo: string;
  titulo: string;
  categoria: string;
  area: string | null;
  descricao: string | null;
  cargaHoraria: number;
  nivelMinimo: number;
  imagemUrl: string | null;
  professorNome: string | null;
  vagasDisponiveis: number | null;
  avaliacao: number;
  bloqueado: boolean | null;
  progressoAluno: number | null;
};

export type Certificado = {
  id: number;
  codigoValidacao: string;
  alunoNome: string;
  cursoId: number;
  cursoTitulo: string;
  cargaHoraria: number;
  dataEmissao: string;
  matriculaId: number;
};

export type Disciplina = {
  id: number;
  cursoId: number;
  nome: string;
  conteudo: string | null;
  ordem: number;
};

export type Material = {
  id: number;
  cursoId: number;
  titulo: string;
  tipo: string;
  urlConteudo: string;
  dataPostagem: string;
};

export type CursoDetalhe = {
  curso: Curso;
  disciplinas: Disciplina[];
  materiais: Material[];
};
