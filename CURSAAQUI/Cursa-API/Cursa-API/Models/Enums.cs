namespace CursaAqui_API.Models;

// Os 4 primeiros enums mantêm EXATAMENTE os valores do app desktop
// (CursaAqui.App/Models/Enums.cs) para não quebrar dados/telas existentes.

public enum PerfilUsuario
{
    Administrador = 1, // desktop
    Secretaria = 2,    // desktop
    Professor = 3,     // web (role "teacher") + UML/DER
    Aluno = 4          // web (role "student") + UML/DER
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

// ---- novos (vindos do site web / UML) ----

public enum TipoProva
{
    Avaliacao = 1, // prova oficial do curso (course.html)
    Treino = 2     // "Treino Rápido" do aluno.html (extraQuizzesCatalog)
}

public enum TipoQuestao
{
    Objetiva = 1,
    Dissertativa = 2 // openQuestionsCatalog do courses.js
}

public enum StatusTentativa
{
    Pendente = 1,  // web: "Pendente" (aguardando correção do professor)
    Corrigida = 2  // web: "Corrigido"
}

public enum TipoMaterial
{
    Pdf = 1,
    Link = 2,
    Video = 3,
    Outro = 4
}

public enum TipoFeedback
{
    Duvida = 1,           // UML: suporte/feedback -> dúvidas()
    ReportarProblema = 2  // UML: suporte/feedback -> reportar_problemas()
}

public enum StatusFeedback
{
    Aberto = 1,
    Respondido = 2
}
