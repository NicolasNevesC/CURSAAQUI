using System.Drawing;
using System.Windows.Forms;
using CursaAqui.App.Models;
using CursaAqui.App.Services;

namespace CursaAqui.App.UI.Controls;

public class UcParametrosSistema : UserControl
{
    private readonly TextBox _txtNomeInst = new() { Width = 450, Font = Theme.FontBody };
    private readonly TextBox _txtPeriodo = new() { Width = 220, Font = Theme.FontBody };
    private readonly NumericUpDown _numNotaMinima = new() { Width = 220, DecimalPlaces = 1, Minimum = 0, Maximum = 10, Increment = 0.5m, Font = Theme.FontBody };
    private readonly NumericUpDown _numCargaMax = new() { Width = 220, Minimum = 20, Maximum = 1000, Increment = 10, Font = Theme.FontBody };
    private readonly NumericUpDown _numVagasPadrao = new() { Width = 220, Minimum = 10, Maximum = 500, Increment = 5, Font = Theme.FontBody };
    private readonly CheckBox _chkPermitirNovas = new() { Text = "Permitir novas matrículas no período vigente", AutoSize = true, Font = Theme.FontBodyBold };
    private readonly TextBox _txtEmail = new() { Width = 450, Font = Theme.FontBody };
    private readonly TextBox _txtTelefone = new() { Width = 450, Font = Theme.FontBody };
    private readonly TextBox _txtEndereco = new() { Width = 450, Font = Theme.FontBody };
    private readonly Label _lblUltimaModificacao = new() { Font = Theme.FontSmall, ForeColor = Theme.TextMuted, AutoSize = true };

    public UcParametrosSistema()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.LightBg;

        var mainPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Padding = new Padding(24)
        };
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // 1. Cabeçalho
        var headerPanel = new Panel { Dock = DockStyle.Fill };
        var lblTitulo = new Label
        {
            Text = "Configuração de Parâmetros do Sistema",
            Font = Theme.FontTitleLarge,
            ForeColor = Theme.TextDark,
            AutoSize = true,
            Location = new Point(0, 5)
        };
        var lblSub = new Label
        {
            Text = "Definição de regras de negócio, períodos acadêmicos e parâmetros institucionais",
            Font = Theme.FontBody,
            ForeColor = Theme.TextMuted,
            AutoSize = true,
            Location = new Point(0, 35)
        };
        headerPanel.Controls.AddRange(new Control[] { lblTitulo, lblSub });

        // 2. Formulário Centralizado em Card Branco
        var scrollPanel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true
        };

        var card = new Panel
        {
            Location = new Point(0, 10),
            Width = 650,
            AutoSize = true,
            BackColor = Color.White,
            Padding = new Padding(24)
        };

        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };

        // Seção 1: Identidade Institucional
        layout.Controls.Add(new Label { Text = "DADOS DA INSTITUIÇÃO", Font = Theme.FontSubtitle, ForeColor = Theme.Primary, AutoSize = true });
        layout.Controls.Add(new Label { Text = "Nome da Instituição:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 8, 0, 0), AutoSize = true });
        layout.Controls.Add(_txtNomeInst);

        layout.Controls.Add(new Label { Text = "E-mail Institucional Oficial:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 8, 0, 0), AutoSize = true });
        layout.Controls.Add(_txtEmail);

        layout.Controls.Add(new Label { Text = "Telefone Geral da Secretaria:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 8, 0, 0), AutoSize = true });
        layout.Controls.Add(_txtTelefone);

        layout.Controls.Add(new Label { Text = "Endereço do Campus Principal:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 8, 0, 0), AutoSize = true });
        layout.Controls.Add(_txtEndereco);

        // Seção 2: Regras Acadêmicas
        layout.Controls.Add(new Label { Text = "REGRAS E PARÂMETROS ACADÊMICOS", Font = Theme.FontSubtitle, ForeColor = Theme.Primary, Margin = new Padding(0, 20, 0, 0), AutoSize = true });

        layout.Controls.Add(new Label { Text = "Período Letivo Vigente (Ex: 2026.1):", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 8, 0, 0), AutoSize = true });
        layout.Controls.Add(_txtPeriodo);

        layout.Controls.Add(new Label { Text = "Média Mínima para Aprovação (0 a 10):", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 8, 0, 0), AutoSize = true });
        layout.Controls.Add(_numNotaMinima);

        layout.Controls.Add(new Label { Text = "Carga Horária Máxima Permitida por Aluno (horas):", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 8, 0, 0), AutoSize = true });
        layout.Controls.Add(_numCargaMax);

        layout.Controls.Add(new Label { Text = "Vagas Padrão ao Criar Novos Cursos:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 8, 0, 0), AutoSize = true });
        layout.Controls.Add(_numVagasPadrao);

        layout.Controls.Add(new Panel { Height = 10, Width = 10 });
        layout.Controls.Add(_chkPermitirNovas);

        layout.Controls.Add(new Panel { Height = 15, Width = 10 });
        var btnSalvar = new Button { Text = "Salvar Parâmetros Institucionais", Width = 320, Height = 40 };
        Theme.ApplyModernButton(btnSalvar, Theme.Primary, Color.White);
        btnSalvar.Click += SalvarParametros;
        layout.Controls.Add(btnSalvar);

        layout.Controls.Add(new Panel { Height = 10, Width = 10 });
        layout.Controls.Add(_lblUltimaModificacao);

        card.Controls.Add(layout);
        scrollPanel.Controls.Add(card);

        mainPanel.Controls.Add(headerPanel, 0, 0);
        mainPanel.Controls.Add(scrollPanel, 0, 1);

        Controls.Add(mainPanel);

        CarregarParametros();
    }

    private void CarregarParametros()
    {
        var p = DataRepository.Instance.Parametros;
        _txtNomeInst.Text = p.NomeInstituicao;
        _txtPeriodo.Text = p.PeriodoLetivoAtual;
        _numNotaMinima.Value = (decimal)p.MediaAprovacaoMinima;
        _numCargaMax.Value = p.CargaHorariaMaximaAluno;
        _numVagasPadrao.Value = p.VagasPadraoPorCurso;
        _chkPermitirNovas.Checked = p.PermitirNovasMatriculas;
        _txtEmail.Text = p.EmailInstitucional;
        _txtTelefone.Text = p.TelefoneInstitucional;
        _txtEndereco.Text = p.EnderecoCampus;
        _lblUltimaModificacao.Text = $"Última alteração: {p.UltimaAtualizacao:dd/MM/yyyy HH:mm} por {p.AtualizadoPor}";
    }

    private void SalvarParametros(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtNomeInst.Text) || string.IsNullOrWhiteSpace(_txtPeriodo.Text))
        {
            MessageBox.Show("Informe o nome da instituição e o período letivo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var p = DataRepository.Instance.Parametros;
        p.NomeInstituicao = _txtNomeInst.Text.Trim();
        p.PeriodoLetivoAtual = _txtPeriodo.Text.Trim();
        p.MediaAprovacaoMinima = (double)_numNotaMinima.Value;
        p.CargaHorariaMaximaAluno = (int)_numCargaMax.Value;
        p.VagasPadraoPorCurso = (int)_numVagasPadrao.Value;
        p.PermitirNovasMatriculas = _chkPermitirNovas.Checked;
        p.EmailInstitucional = _txtEmail.Text.Trim();
        p.TelefoneInstitucional = _txtTelefone.Text.Trim();
        p.EnderecoCampus = _txtEndereco.Text.Trim();
        p.UltimaAtualizacao = DateTime.Now;
        p.AtualizadoPor = AuthService.Instance.UsuarioLogado?.Email ?? "administrador";

        DataRepository.Instance.Salvar();
        CarregarParametros();

        MessageBox.Show("Parâmetros do sistema atualizados com sucesso e vigentes para toda a instituição!", "Parâmetros Salvos", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
