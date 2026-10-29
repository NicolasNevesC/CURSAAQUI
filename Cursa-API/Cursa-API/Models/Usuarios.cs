namespace CursaAqui_API.Models;

// ============================================================================
// ATORES DO SISTEMA
// Hierarquia: Usuario (abstrata) -> Administrador | Secretaria | Professor | Aluno
// No banco vira "Tabela por Tipo" (TPT): Usuarios + uma tabela por filho,
// compartilhando a mesma chave (Id). Visitante NÃO é tabela (ator sem login).
// ============================================================================

/// <summary>
/// Ator base. Une: UsuarioInterno (desktop), User do localStorage (web),
/// "usuário" do UML e "Usuário (Base)" do DER.
/// </summary>
public abstract class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // NUNCA guardar a senha em texto puro (hoje é "123" no desktop e texto puro no web).
    public string SenhaHash { get; set; } = string.Empty;

    // Opcionais: o cadastro do site só pede nome/e-mail/senha,
    // então CPF/telefone/nascimento são completados depois (site ou secretaria).
    public string? Cpf { get; set; }
    public string? Telefone { get; set; }
    public DateTime? DataNascimento { get; set; } // UML: data_nasc

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public bool Ativo { get; set; } = true;
    public PerfilUsuario Perfil { get; set; }

    // Opcionais (README cita avatar/preferências; UML tem "acessibilidade")
    public string? AvatarUrl { get; set; }
    public string? Tema { get; set; }            // "light" | "dark" (hoje em localStorage "theme")
    public string? Acessibilidade { get; set; }  // UML: configurações.acessibilidade
}

public class Administrador : Usuario
{
    public Administrador() { Perfil = PerfilUsuario.Administrador; }

    public string NivelAutoridade { get; set; } = "Total";
    public DateTime? UltimoAcessoGerencial { get; set; } // atualizado a cada login
}

public class Secretaria : Usuario
{
    public Secretaria() { Perfil = PerfilUsuario.Secretaria; }

    public string SetorAtendimento { get; set; } = "Secretaria Acadêmica";
    // AtendimentosRealizadosHoje (desktop) NÃO é coluna: é calculado
    // (COUNT de Atendimentos do dia) para não ficar desatualizado.
}

public class Professor : Usuario
{
    public Professor() { Perfil = PerfilUsuario.Professor; }

    public string? Titulacao { get; set; } // ex.: "Dr.", "Dra." (o site exibe "Prof. Dr. Fulano")

    public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
    public ICollection<Material> Materiais { get; set; } = new List<Material>();
}

public class Aluno : Usuario
{
    public Aluno() { Perfil = PerfilUsuario.Aluno; }

    public string? Endereco { get; set; }
    public int Xp { get; set; } = 0;     // web + desktop
    public int Nivel { get; set; } = 1;  // web + desktop (nível sobe com Nivel*1000 XP)

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    public ICollection<Tentativa> Tentativas { get; set; } = new List<Tentativa>();
    public ICollection<Certificado> Certificados { get; set; } = new List<Certificado>();
    public ICollection<AvaliacaoCurso> Avaliacoes { get; set; } = new List<AvaliacaoCurso>();
}
