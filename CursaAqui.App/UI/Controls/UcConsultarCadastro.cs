using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CursaAqui.App.Models;
using CursaAqui.App.Services;

namespace CursaAqui.App.UI.Controls;

public class UcConsultarCadastro : UserControl
{
    private readonly TextBox _txtBusca;
    private readonly ListBox _lstAlunos;
    private readonly Panel _panelFicha;
    private readonly Label _lblNome;
    private readonly Label _lblCpf;
    private readonly Label _lblEmail;
    private readonly Label _lblTelefone;
    private readonly Label _lblDataNasc;
    private readonly Label _lblEndereco;
    private readonly Label _lblDataCadastro;
    private readonly Label _lblStatus;
    private readonly DataGridView _gridCursosAluno;
    private Aluno? _alunoSelecionado;

    public UcConsultarCadastro()
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
            Text = "Consulta Cadastral de Alunos",
            Font = Theme.FontTitleLarge,
            ForeColor = Theme.TextDark,
            AutoSize = true,
            Location = new Point(0, 5)
        };
        var lblSub = new Label
        {
            Text = "Localização de dados cadastrais, histórico escolar e cursos matriculados",
            Font = Theme.FontBody,
            ForeColor = Theme.TextMuted,
            AutoSize = true,
            Location = new Point(0, 35)
        };
        headerPanel.Controls.AddRange(new Control[] { lblTitulo, lblSub });

        // 2. Conteúdo dividido: Lista de busca à esquerda, Ficha Cadastral à direita
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 320,
            BackColor = Theme.BorderColor
        };

        // Painel Esquerdo: Busca e Lista
        var leftPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12) };
        var lblBusca = new Label { Text = "Buscar Aluno (Nome, CPF, E-mail):", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Location = new Point(12, 10) };
        _txtBusca = new TextBox { Location = new Point(12, 32), Width = 280, Font = Theme.FontBody };
        _txtBusca.TextChanged += (s, e) => FiltrarAlunos();

        _lstAlunos = new ListBox
        {
            Location = new Point(12, 65),
            Width = 280,
            Height = 420,
            Font = Theme.FontBody,
            BorderStyle = BorderStyle.FixedSingle,
            IntegralHeight = false
        };
        _lstAlunos.SelectedIndexChanged += LstAlunos_SelectedIndexChanged;

        leftPanel.Controls.AddRange(new Control[] { lblBusca, _txtBusca, _lstAlunos });
        split.Panel1.Controls.Add(leftPanel);

        // Painel Direito: Ficha Cadastral e Cursos
        _panelFicha = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20), AutoScroll = true };

        var lblFichaTitle = new Label { Text = "FICHA CADASTRAL DO ESTUDANTE", Font = Theme.FontSubtitle, ForeColor = Theme.Primary, Location = new Point(20, 15), AutoSize = true };

        var btnExportarFicha = new Button
        {
            Text = "Exportar Ficha Cadastral (.txt)",
            Location = new Point(400, 10),
            Width = 220,
            Height = 32
        };
        Theme.ApplyModernButton(btnExportarFicha, Theme.Secondary, Color.White);
        btnExportarFicha.Click += ExportarFicha;

        var panelDados = new Panel { Location = new Point(20, 50), Width = 600, Height = 190, BackColor = Color.FromArgb(248, 250, 252), Padding = new Padding(15) };

        _lblNome = new Label { Text = "Nome: -", Font = Theme.FontSubtitle, ForeColor = Theme.TextDark, Location = new Point(15, 12), AutoSize = true };
        _lblCpf = new Label { Text = "CPF: -", Font = Theme.FontBody, ForeColor = Theme.TextDark, Location = new Point(15, 40), AutoSize = true };
        _lblEmail = new Label { Text = "E-mail: -", Font = Theme.FontBody, ForeColor = Theme.TextDark, Location = new Point(15, 65), AutoSize = true };
        _lblTelefone = new Label { Text = "Telefone: -", Font = Theme.FontBody, ForeColor = Theme.TextDark, Location = new Point(320, 40), AutoSize = true };
        _lblDataNasc = new Label { Text = "Nascimento: -", Font = Theme.FontBody, ForeColor = Theme.TextDark, Location = new Point(320, 65), AutoSize = true };
        _lblEndereco = new Label { Text = "Endereço: -", Font = Theme.FontBody, ForeColor = Theme.TextDark, Location = new Point(15, 95), AutoSize = true };
        _lblDataCadastro = new Label { Text = "Cadastrado em: -", Font = Theme.FontSmall, ForeColor = Theme.TextMuted, Location = new Point(15, 125), AutoSize = true };
        _lblStatus = new Label { Text = "Situação Cadastral: -", Font = Theme.FontBodyBold, ForeColor = Theme.Success, Location = new Point(15, 150), AutoSize = true };

        panelDados.Controls.AddRange(new Control[] { _lblNome, _lblCpf, _lblEmail, _lblTelefone, _lblDataNasc, _lblEndereco, _lblDataCadastro, _lblStatus });

        var lblHistorico = new Label { Text = "CURSOS E MATRÍCULAS DO ALUNO", Font = Theme.FontSubtitle, ForeColor = Theme.TextDark, Location = new Point(20, 255), AutoSize = true };

        _gridCursosAluno = new DataGridView { Location = new Point(20, 285), Width = 600, Height = 210, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        Theme.ApplyDataGridStyle(_gridCursosAluno);
        ConfigurarGridCursos();

        _panelFicha.Controls.AddRange(new Control[] { lblFichaTitle, btnExportarFicha, panelDados, lblHistorico, _gridCursosAluno });
        split.Panel2.Controls.Add(_panelFicha);

        mainPanel.Controls.Add(headerPanel, 0, 0);
        mainPanel.Controls.Add(split, 0, 1);

        Controls.Add(mainPanel);

        CarregarAlunos();
    }

    private void ConfigurarGridCursos()
    {
        _gridCursosAluno.Columns.Clear();
        _gridCursosAluno.Columns.Add("Curso", "Curso");
        _gridCursosAluno.Columns.Add("Periodo", "Período");
        _gridCursosAluno.Columns.Add("Progresso", "Progresso");
        _gridCursosAluno.Columns.Add("Nota", "Nota Final");
        _gridCursosAluno.Columns.Add("Status", "Situação");

        _gridCursosAluno.Columns["Periodo"]!.Width = 80;
        _gridCursosAluno.Columns["Progresso"]!.Width = 85;
        _gridCursosAluno.Columns["Nota"]!.Width = 80;
        _gridCursosAluno.Columns["Status"]!.Width = 100;
    }

    private void CarregarAlunos()
    {
        _lstAlunos.Items.Clear();
        foreach (var a in DataRepository.Instance.Alunos)
        {
            _lstAlunos.Items.Add(a);
        }

        if (_lstAlunos.Items.Count > 0)
        {
            _lstAlunos.SelectedIndex = 0;
        }
    }

    private void FiltrarAlunos()
    {
        var termo = _txtBusca.Text.Trim().ToLowerInvariant();
        _lstAlunos.Items.Clear();

        var filtrados = DataRepository.Instance.Alunos.Where(a =>
            string.IsNullOrEmpty(termo) ||
            a.Nome.ToLowerInvariant().Contains(termo) ||
            a.Cpf.Contains(termo) ||
            a.Email.ToLowerInvariant().Contains(termo)
        ).ToList();

        foreach (var a in filtrados)
        {
            _lstAlunos.Items.Add(a);
        }

        if (_lstAlunos.Items.Count > 0)
        {
            _lstAlunos.SelectedIndex = 0;
        }
        else
        {
            LimparFicha();
        }
    }

    private void LstAlunos_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_lstAlunos.SelectedItem is Aluno aluno)
        {
            _alunoSelecionado = aluno;
            ExibirAluno(aluno);
        }
    }

    private void ExibirAluno(Aluno aluno)
    {
        _lblNome.Text = $"Nome: {aluno.Nome}";
        _lblCpf.Text = $"CPF: {aluno.Cpf}";
        _lblEmail.Text = $"E-mail: {aluno.Email}";
        _lblTelefone.Text = $"Telefone: {aluno.Telefone}";
        _lblDataNasc.Text = $"Nascimento: {aluno.DataNascimento:dd/MM/yyyy}";
        _lblEndereco.Text = $"Endereço: {aluno.Endereco}";
        _lblDataCadastro.Text = $"Cadastrado no Sistema em: {aluno.DataCadastro:dd/MM/yyyy HH:mm}";
        _lblStatus.Text = aluno.Ativo ? "Situação Cadastral: REGULAR (ATIVO)" : "Situação Cadastral: SUSPENSO / INATIVO";
        _lblStatus.ForeColor = aluno.Ativo ? Theme.Success : Theme.Danger;

        _gridCursosAluno.Rows.Clear();
        var matriculas = DataRepository.Instance.Matriculas.Where(m => m.AlunoId == aluno.Id).ToList();

        foreach (var m in matriculas)
        {
            _gridCursosAluno.Rows.Add(
                m.CursoNome,
                m.PeriodoLetivo,
                $"{m.ProgressoPercentual}%",
                $"{m.NotaFinal:F1}",
                m.Status.ToString()
            );
        }
    }

    private void LimparFicha()
    {
        _alunoSelecionado = null;
        _lblNome.Text = "Nome: -";
        _lblCpf.Text = "CPF: -";
        _lblEmail.Text = "E-mail: -";
        _lblTelefone.Text = "Telefone: -";
        _lblDataNasc.Text = "Nascimento: -";
        _lblEndereco.Text = "Endereço: -";
        _lblDataCadastro.Text = "Cadastrado em: -";
        _lblStatus.Text = "Situação Cadastral: -";
        _gridCursosAluno.Rows.Clear();
    }

    private void ExportarFicha(object? sender, EventArgs e)
    {
        if (_alunoSelecionado == null)
        {
            MessageBox.Show("Nenhum aluno selecionado para exportação.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("==========================================================================");
        sb.AppendLine("                 CURSA AQUI - FICHA CADASTRAL DO ESTUDANTE                ");
        sb.AppendLine("==========================================================================");
        sb.AppendLine($"Data de Consulta: {DateTime.Now:dd/MM/yyyy HH:mm}");
        sb.AppendLine($"Consultado por: {AuthService.Instance.UsuarioLogado?.Nome} (Secretaria)");
        sb.AppendLine();
        sb.AppendLine($"Nome Completo: {_alunoSelecionado.Nome}");
        sb.AppendLine($"CPF:           {_alunoSelecionado.Cpf}");
        sb.AppendLine($"E-mail:        {_alunoSelecionado.Email}");
        sb.AppendLine($"Telefone:      {_alunoSelecionado.Telefone}");
        sb.AppendLine($"Nascimento:    {_alunoSelecionado.DataNascimento:dd/MM/yyyy}");
        sb.AppendLine($"Endereço:      {_alunoSelecionado.Endereco}");
        sb.AppendLine($"Situação:      {(_alunoSelecionado.Ativo ? "Regular" : "Inativo")}");
        sb.AppendLine($"XP Acumulado:  {_alunoSelecionado.Xp} XP (Nível {_alunoSelecionado.Nivel})");
        sb.AppendLine();
        sb.AppendLine("--- HISTÓRICO DE CURSOS E MATRÍCULAS ---");
        var mats = DataRepository.Instance.Matriculas.Where(m => m.AlunoId == _alunoSelecionado.Id).ToList();

        if (mats.Count == 0)
        {
            sb.AppendLine("Nenhuma matrícula registrada.");
        }
        else
        {
            foreach (var m in mats)
            {
                sb.AppendLine($"- Curso: {m.CursoNome}");
                sb.AppendLine($"  Período: {m.PeriodoLetivo} | Data: {m.DataMatricula:dd/MM/yyyy} | Situação: {m.Status}");
                sb.AppendLine($"  Progresso: {m.ProgressoPercentual}% | Nota Final: {m.NotaFinal:F1}");
                if (!string.IsNullOrEmpty(m.Observacoes)) sb.AppendLine($"  Obs: {m.Observacoes}");
                sb.AppendLine();
            }
        }

        sb.AppendLine("==========================================================================");

        using var saveDialog = new SaveFileDialog
        {
            Filter = "Arquivo de Texto (*.txt)|*.txt",
            FileName = $"Ficha_{_alunoSelecionado.Nome.Replace(" ", "_")}.txt"
        };

        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            File.WriteAllText(saveDialog.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show("Ficha cadastral exportada com sucesso!", "Exportação Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
