using System.Drawing;
using System.Windows.Forms;
using CursaAqui.App.Services;

namespace CursaAqui.App.UI;

public class FormLogin : Form
{
    private readonly TextBox _txtEmail = new() { Width = 360, Font = Theme.FontBody };
    private readonly TextBox _txtSenha = new() { Width = 360, Font = Theme.FontBody, UseSystemPasswordChar = true };
    private bool _entrando;
    private readonly Label _lblMensagem = new() { ForeColor = Theme.Danger, Font = Theme.FontSmall, AutoSize = true, Width = 360 };

    public FormLogin()
    {
        Text = "Cursa Aqui - Acesso Interno";
        ClientSize = new Size(460, 600);
        MinimumSize = new Size(460, 600);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = Color.FromArgb(248, 250, 252);

        // Container com AutoScroll para suportar qualquer escala de DPI sem cortar controles
        var mainContainer = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(30, 20, 30, 25)
        };

        // Cartão branco centralizado
        var card = new Panel
        {
            Location = new Point(20, 15),
            Width = 400,
            Height = 540,
            BackColor = Color.White,
            Padding = new Padding(24, 20, 24, 20)
        };
        card.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.BorderColor, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        // 1. Cabeçalho
        var lblLogo = new Label
        {
            Text = "🎓 CURSA AQUI",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Theme.Primary,
            Location = new Point(20, 18),
            AutoSize = true
        };

        var lblSub = new Label
        {
            Text = "Sistema de Gestão - Módulo Interno",
            Font = Theme.FontSubtitle,
            ForeColor = Theme.TextDark,
            Location = new Point(22, 52),
            AutoSize = true
        };

        var lblDesc = new Label
        {
            Text = "Acesso restrito para Administrador e Secretaria",
            Font = Theme.FontSmall,
            ForeColor = Theme.TextMuted,
            Location = new Point(22, 75),
            AutoSize = true
        };

        // 2. Formulário de Credenciais
        var lblEmail = new Label
        {
            Text = "E-mail Institucional:",
            Font = Theme.FontBodyBold,
            ForeColor = Theme.TextDark,
            Location = new Point(20, 110),
            AutoSize = true
        };
        _txtEmail.Location = new Point(20, 132);
        _txtEmail.Text = "admin@cursaaqui.com";

        var lblSenha = new Label
        {
            Text = "Senha de Acesso:",
            Font = Theme.FontBodyBold,
            ForeColor = Theme.TextDark,
            Location = new Point(20, 172),
            AutoSize = true
        };
        _txtSenha.Location = new Point(20, 194);
        _txtSenha.Text = "Trocar@123";
        _txtSenha.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) TentarLogin(); };

        _lblMensagem.Location = new Point(20, 226);

        // 3. Botão Principal
        var btnEntrar = new Button
        {
            Text = "Acessar o Sistema",
            Location = new Point(20, 248),
            Width = 360,
            Height = 42
        };
        Theme.ApplyModernButton(btnEntrar, Theme.Primary, Color.White);
        btnEntrar.Click += (s, e) => TentarLogin();

        card.Controls.AddRange(new Control[]
        {
            lblLogo, lblSub, lblDesc,
            lblEmail, _txtEmail,
            lblSenha, _txtSenha,
            _lblMensagem,
            btnEntrar
        });

        mainContainer.Controls.Add(card);
        Controls.Add(mainContainer);
    }

    private async void TentarLogin()
    {
        if (_entrando) return;
        _entrando = true;
        UseWaitCursor = true;
        _lblMensagem.ForeColor = Theme.TextMuted;
        _lblMensagem.Text = "Conectando à API...";

        try
        {
            var resultado = await AuthService.Instance.LoginAsync(_txtEmail.Text, _txtSenha.Text);
            if (!resultado.Sucesso)
            {
                _lblMensagem.ForeColor = Theme.Danger;
                _lblMensagem.Text = resultado.Mensagem;
                return;
            }

            // Carrega alunos, cursos, matrículas etc. da API antes de abrir o sistema
            var erro = await DataRepository.Instance.RecarregarAsync();
            if (erro != null)
            {
                AuthService.Instance.Logout();
                _lblMensagem.ForeColor = Theme.Danger;
                _lblMensagem.Text = "Não foi possível carregar os dados: " + erro;
                return;
            }
        }
        finally
        {
            _entrando = false;
            UseWaitCursor = false;
        }

        _lblMensagem.Text = "";
        Hide();
        var main = new FormMain();
        main.FormClosed += (s, e) => Close();
        main.Show();
    }
}
