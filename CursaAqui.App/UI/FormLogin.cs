using System.Drawing;
using System.Windows.Forms;
using CursaAqui.App.Services;

namespace CursaAqui.App.UI;

public class FormLogin : Form
{
    private readonly TextBox _txtEmail = new() { Width = 360, Font = Theme.FontBody };
    private readonly TextBox _txtSenha = new() { Width = 360, Font = Theme.FontBody, UseSystemPasswordChar = true };
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
        _txtSenha.Text = "123";
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

        // 4. Divisor estilizado
        var lblDivisor = new Label
        {
            Text = "─── ACESSO RÁPIDO PARA TESTES ───",
            Font = Theme.FontSmall,
            ForeColor = Theme.TextMuted,
            Location = new Point(70, 310),
            AutoSize = true
        };

        // 5. Botões de Acesso Rápido Organizados e Bem Posicionados
        var btnFastAdmin = new Button
        {
            Text = "👑  Entrar como Administrador\n(admin@cursaaqui.com)",
            Location = new Point(20, 340),
            Width = 360,
            Height = 52,
            TextAlign = ContentAlignment.MiddleCenter
        };
        Theme.ApplyModernButton(btnFastAdmin, Color.FromArgb(238, 242, 255), Theme.Primary);
        btnFastAdmin.Font = Theme.FontBodyBold;
        btnFastAdmin.Click += (s, e) =>
        {
            _txtEmail.Text = "admin@cursaaqui.com";
            _txtSenha.Text = "123";
            TentarLogin();
        };

        var btnFastSec = new Button
        {
            Text = "📝  Entrar como Secretaria\n(secretaria@cursaaqui.com)",
            Location = new Point(20, 402),
            Width = 360,
            Height = 52,
            TextAlign = ContentAlignment.MiddleCenter
        };
        Theme.ApplyModernButton(btnFastSec, Color.FromArgb(241, 245, 249), Theme.Secondary);
        btnFastSec.Font = Theme.FontBodyBold;
        btnFastSec.Click += (s, e) =>
        {
            _txtEmail.Text = "secretaria@cursaaqui.com";
            _txtSenha.Text = "123";
            TentarLogin();
        };

        var lblAjuda = new Label
        {
            Text = "Dica: Clique em um dos perfis acima para alternar facilmente.",
            Font = Theme.FontSmall,
            ForeColor = Theme.TextMuted,
            Location = new Point(32, 475),
            AutoSize = true
        };

        card.Controls.AddRange(new Control[]
        {
            lblLogo, lblSub, lblDesc,
            lblEmail, _txtEmail,
            lblSenha, _txtSenha,
            _lblMensagem,
            btnEntrar,
            lblDivisor,
            btnFastAdmin,
            btnFastSec,
            lblAjuda
        });

        mainContainer.Controls.Add(card);
        Controls.Add(mainContainer);
    }

    private void TentarLogin()
    {
        _lblMensagem.Text = "";
        var resultado = AuthService.Instance.Login(_txtEmail.Text, _txtSenha.Text);
        if (!resultado.Sucesso)
        {
            _lblMensagem.Text = resultado.Mensagem;
            return;
        }

        Hide();
        var main = new FormMain();
        main.FormClosed += (s, e) => Close();
        main.Show();
    }
}
