namespace CursaAqui.App.Models;

public enum PerfilUsuario
{
    Administrador = 1,
    Secretaria = 2
}

public enum StatusMatricula
{
    Ativa = 1,
    Renovada = 2,
    Trancada = 3,
    Cancelada = 4,
    Concluida = 5
}

public enum StatusAtendimento
{
    Aberto = 1,
    EmAndamento = 2,
    Concluido = 3,
    Cancelado = 4
}

public enum TipoSolicitante
{
    Aluno = 1,
    Visitante = 2,
    Outro = 3
}
