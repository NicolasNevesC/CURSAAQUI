using System.Drawing;
using System.Windows.Forms;
using CursaAqui.App.Models;
using CursaAqui.App.Services;
using CursaAqui.App.UI.Controls;

namespace CursaAqui.App.UI;

public class FormMain : Form
{
    private readonly Panel _sidebar;
    private readonly Panel _topbar;
    private readonly Panel _contentArea;
    private readonly Label _lblTituloSecao;
    private readonly Label _lblPeriodo;
    private readonly FlowLayoutPanel _menuContainer;
    private UserControl? _currentControl;

    public FormMain()
    {
        Text = "Cursa Aqui - Módulo de Gestão Interna (Admin & Secretaria)";
        Size = new Size(1280, 800);
        MinimumSize = new Size(1100, 700);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Theme.LightBg;

        // 1. Sidebar à Esquerda
        _sidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 260,
            BackColor = Theme.SidebarBg
        };

        // 1.1 Header da Sidebar
        var brandPanel = new Panel { Dock = DockStyle.Top, Height = 80, Padding = new Padding(20, 15, 20, 10) };
        var lblBrand = new Label
        {
            Text = "🎓 Cursa Aqui",
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(16, 16)
        };
        var lblSub = new Label
        {
            Text = "GESTÃO INTERNA",
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            ForeColor = Color.FromArgb(148, 163, 184),
            AutoSize = true,
            Location = new Point(18, 45)
        };
        brandPanel.Controls.AddRange(new Control[] { lblBrand, lblSub });

        // 1.2 Cartão do Usuário Conectado
        var userPanel = new Panel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(16, 5, 16, 10) };
        var user = AuthService.Instance.UsuarioLogado;

        var lblUserName = new Label
        {
            Text = user?.Nome ?? "Usuário",
            Font = Theme.FontSubtitle,
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(16, 8)
        };

        var roleBadge = new Label
        {
            Text = user is Administrador ? " ADMINISTRADOR " : " SECRETARIA ",
            Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = user is Administrador ? Theme.Primary : Theme.Info,
            AutoSize = true,
            Location = new Point(18, 34),
            Padding = new Padding(4, 2, 4, 2)
        };

        var lblEmail = new Label
        {
            Text = user?.Email ?? "",
            Font = Theme.FontSmall,
            ForeColor = Color.FromArgb(148, 163, 184),
            AutoSize = true,
            Location = new Point(16, 60)
        };

        userPanel.Controls.AddRange(new Control[] { lblUserName, roleBadge, lblEmail });

        // 1.3 Menu de Navegação Dinâmico
        _menuContainer = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(10, 10, 10, 10)
        };

        // 1.4 Rodapé da Sidebar (Logout)
        var footerSidebar = new Panel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(15, 10, 15, 10) };
        var btnLogout = new Button
        {
            Text = "← Sair / Trocar Acesso",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            Font = Theme.FontBodyBold,
            ForeColor = Color.FromArgb(248, 113, 113),
            BackColor = Color.FromArgb(30, 41, 59),
            Cursor = Cursors.Hand
        };
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.Click += (s, e) => Logout();
        footerSidebar.Controls.Add(btnLogout);

        _sidebar.Controls.Add(_menuContainer);
        _sidebar.Controls.Add(footerSidebar);
        _sidebar.Controls.Add(userPanel);
        _sidebar.Controls.Add(brandPanel);

        // 2. Topbar
        _topbar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = Color.White,
            Padding = new Padding(24, 15, 24, 15)
        };

        _lblTituloSecao = new Label
        {
            Text = "Painel Principal",
            Font = Theme.FontTitle,
            ForeColor = Theme.TextDark,
            AutoSize = true,
            Location = new Point(20, 17)
        };

        _lblPeriodo = new Label
        {
            Text = $"Ano Letivo: {DataRepository.Instance.Parametros.PeriodoLetivoAtual} | {DateTime.Now:dd/MM/yyyy}",
            Font = Theme.FontBodyBold,
            ForeColor = Theme.Primary,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(780, 20),
            AutoSize = true
        };

        _topbar.Controls.AddRange(new Control[] { _lblTituloSecao, _lblPeriodo });

        // 3. Área de Conteúdo
        _contentArea = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Theme.LightBg
        };

        Controls.Add(_contentArea);
        Controls.Add(_topbar);
        Controls.Add(_sidebar);

        MontarMenuPorPerfil();
    }

    private void MontarMenuPorPerfil()
    {
        _menuContainer.Controls.Clear();
        var auth = AuthService.Instance;

        if (auth.EhAdministrador)
        {
            AdicionarItemMenu("👥 Gerenciar Usuários", () => NavegarPara(new UcGerenciarUsuarios(), "Gerenciamento de Usuários Internos"), true);
            AdicionarItemMenu("🔑 Gerenciar Permissões", () => NavegarPara(new UcGerenciarPermissoes(), "Matriz de Permissões de Acesso"), auth.TemPermissao("GerenciarPermissoes"));
            AdicionarItemMenu("📚 Gerenciar Cursos", () => NavegarPara(new UcGerenciarCursos(), "Catálogo e Parâmetros de Cursos"), auth.TemPermissao("GerenciarCursos"));
            AdicionarItemMenu("📊 Relatórios Gerenciais", () => NavegarPara(new UcVisualizarRelatorios(), "Relatórios Gerenciais e Indicadores"), auth.TemPermissao("VisualizarRelatorios"));
            AdicionarItemMenu("⚙️ Parâmetros do Sistema", () => NavegarPara(new UcParametrosSistema(), "Configuração Geral do Sistema"), auth.TemPermissao("ConfigurarParametros"));

            // Abre a primeira tela
            NavegarPara(new UcGerenciarCursos(), "Catálogo e Parâmetros de Cursos");
        }
        else if (auth.EhSecretaria)
        {
            AdicionarItemMenu("📝 Gerenciar Matrículas", () => NavegarPara(new UcGerenciarMatriculas(), "Gerenciamento do Ciclo de Matrículas"), auth.TemPermissao("GerenciarMatriculas"));
            AdicionarItemMenu("🔍 Consultar Cadastro", () => NavegarPara(new UcConsultarCadastro(), "Consulta Cadastral e Histórico de Alunos"), auth.TemPermissao("ConsultarCadastro"));
            AdicionarItemMenu("🎧 Registrar Atendimentos", () => NavegarPara(new UcRegistrarAtendimentos(), "Registro e Tratamento de Atendimentos"), auth.TemPermissao("RegistrarAtendimentos"));
            AdicionarItemMenu("📑 Relatórios Administrativos", () => NavegarPara(new UcRelatoriosAdministrativos(), "Emissão de Relatórios da Secretaria"), auth.TemPermissao("GerarRelatoriosAdministrativos"));

            // Abre a primeira tela da secretaria
            NavegarPara(new UcGerenciarMatriculas(), "Gerenciamento do Ciclo de Matrículas");
        }
    }

    private void AdicionarItemMenu(string titulo, Action acao, bool habilitado)
    {
        if (!habilitado) return;

        var btn = new Button
        {
            Text = "  " + titulo,
            Width = 240,
            Height = 44,
            TextAlign = ContentAlignment.MiddleLeft,
            FlatStyle = FlatStyle.Flat,
            Font = Theme.FontBody,
            ForeColor = Color.FromArgb(226, 232, 240),
            BackColor = Theme.SidebarBg,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 0, 6)
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 55, 72);

        btn.Click += (s, e) =>
        {
            // Reset visual dos botões
            foreach (Control c in _menuContainer.Controls)
            {
                if (c is Button b)
                {
                    b.BackColor = Theme.SidebarBg;
                    b.ForeColor = Color.FromArgb(226, 232, 240);
                }
            }

            btn.BackColor = Theme.Primary;
            btn.ForeColor = Color.White;
            acao();
        };

        _menuContainer.Controls.Add(btn);
    }

    private void NavegarPara(UserControl novoControl, string titulo)
    {
        _lblTituloSecao.Text = titulo;
        _lblPeriodo.Text = $"Ano Letivo: {DataRepository.Instance.Parametros.PeriodoLetivoAtual} | {DateTime.Now:dd/MM/yyyy}";

        _contentArea.SuspendLayout();
        _contentArea.Controls.Clear();
        _currentControl?.Dispose();

        _currentControl = novoControl;
        novoControl.Dock = DockStyle.Fill;
        _contentArea.Controls.Add(novoControl);
        _contentArea.ResumeLayout();
    }

    private void Logout()
    {
        if (MessageBox.Show("Deseja realmente encerrar a sessão?", "Confirmação de Saída", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            AuthService.Instance.Logout();
            Hide();
            var login = new FormLogin();
            login.FormClosed += (s, e) => Close();
            login.Show();
        }
    }
}
