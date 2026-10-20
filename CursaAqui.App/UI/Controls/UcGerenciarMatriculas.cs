using System.Drawing;
using System.Windows.Forms;
using CursaAqui.App.Models;
using CursaAqui.App.Services;

namespace CursaAqui.App.UI.Controls;

public class UcGerenciarMatriculas : UserControl
{
    private readonly DataGridView _grid;
    private readonly TextBox _txtBusca;
    private readonly ComboBox _cbFiltroStatus;
    private readonly Label _lblContador;

    public UcGerenciarMatriculas()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.LightBg;

        var mainPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            Padding = new Padding(24)
        };
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // 1. Cabeçalho
        var headerPanel = new Panel { Dock = DockStyle.Fill };
        var lblTitulo = new Label
        {
            Text = "Gerenciamento do Ciclo de Matrículas",
            Font = Theme.FontTitleLarge,
            ForeColor = Theme.TextDark,
            AutoSize = true,
            Location = new Point(0, 5)
        };
        var lblSub = new Label
        {
            Text = "Realizar nova matrícula, renovar, alterar situação acadêmica e lançar conclusões",
            Font = Theme.FontBody,
            ForeColor = Theme.TextMuted,
            AutoSize = true,
            Location = new Point(0, 35)
        };
        headerPanel.Controls.AddRange(new Control[] { lblTitulo, lblSub });

        // 2. Toolbar Responsiva
        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoSize = true,
            Padding = new Padding(0, 4, 0, 4)
        };

        var lblBuscar = new Label { Text = "Buscar Aluno/Curso:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 6, 0) };
        _txtBusca = new TextBox { Width = 160, Font = Theme.FontBody, Margin = new Padding(0, 5, 12, 0) };
        _txtBusca.TextChanged += (s, e) => Filtrar();

        var lblStatus = new Label { Text = "Situação:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 6, 0) };
        _cbFiltroStatus = new ComboBox { Width = 115, DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontBody, Margin = new Padding(0, 5, 12, 0) };
        _cbFiltroStatus.Items.AddRange(new object[] { "Todas", "Ativa", "Renovada", "Trancada", "Cancelada", "Concluida" });
        _cbFiltroStatus.SelectedIndex = 0;
        _cbFiltroStatus.SelectedIndexChanged += (s, e) => Filtrar();

        var btnNova = new Button { Text = "+ Nova Matrícula", Width = 145, Height = 34, Margin = new Padding(0, 2, 8, 0) };
        Theme.ApplyModernButton(btnNova, Theme.Primary, Color.White);
        btnNova.Click += BtnNova_Click;

        var btnAlterarStatus = new Button { Text = "Alterar Situação", Width = 135, Height = 34, Margin = new Padding(0, 2, 8, 0) };
        Theme.ApplyModernButton(btnAlterarStatus, Theme.Secondary, Color.White);
        btnAlterarStatus.Click += BtnAlterarStatus_Click;

        var btnAtualizarNota = new Button { Text = "Lançar Nota / Progresso", Width = 175, Height = 34, Margin = new Padding(0, 2, 12, 0) };
        Theme.ApplyModernButton(btnAtualizarNota, Theme.Success, Color.White);
        btnAtualizarNota.Click += BtnAtualizarNota_Click;

        _lblContador = new Label { Text = "", AutoSize = true, Font = Theme.FontSmall, ForeColor = Theme.TextMuted, Margin = new Padding(0, 10, 0, 0) };

        toolbar.Controls.AddRange(new Control[] { lblBuscar, _txtBusca, lblStatus, _cbFiltroStatus, btnNova, btnAlterarStatus, btnAtualizarNota, _lblContador });

        // 3. Grid
        _grid = new DataGridView { Dock = DockStyle.Fill };
        Theme.ApplyDataGridStyle(_grid);
        ConfigurarGrid();

        mainPanel.Controls.Add(headerPanel, 0, 0);
        mainPanel.Controls.Add(toolbar, 0, 1);
        mainPanel.Controls.Add(_grid, 0, 2);

        Controls.Add(mainPanel);

        CarregarMatriculas();
    }

    private void ConfigurarGrid()
    {
        _grid.Columns.Clear();
        _grid.Columns.Add("Id", "Matrícula");
        _grid.Columns.Add("Aluno", "Aluno");
        _grid.Columns.Add("Cpf", "CPF");
        _grid.Columns.Add("Curso", "Curso Vinculado");
        _grid.Columns.Add("Periodo", "Período");
        _grid.Columns.Add("Data", "Data Matrícula");
        _grid.Columns.Add("Progresso", "Progresso");
        _grid.Columns.Add("Nota", "Nota Final");
        _grid.Columns.Add("Status", "Situação");

        _grid.Columns["Id"]!.Width = 70;
        _grid.Columns["Periodo"]!.Width = 80;
        _grid.Columns["Progresso"]!.Width = 85;
        _grid.Columns["Nota"]!.Width = 80;
        _grid.Columns["Status"]!.Width = 100;
    }

    private void CarregarMatriculas()
    {
        _grid.Rows.Clear();
        var lista = DataRepository.Instance.Matriculas;

        foreach (var m in lista)
        {
            int rowIndex = _grid.Rows.Add(
                m.Id,
                m.AlunoNome,
                m.AlunoCpf,
                m.CursoNome,
                m.PeriodoLetivo,
                m.DataMatricula.ToString("dd/MM/yyyy"),
                $"{m.ProgressoPercentual}%",
                $"{m.NotaFinal:F1}",
                m.Status.ToString()
            );

            // Cores conforme status
            var row = _grid.Rows[rowIndex];
            if (m.Status == StatusMatricula.Concluida)
                row.Cells["Status"].Style.ForeColor = Theme.Success;
            else if (m.Status == StatusMatricula.Cancelada || m.Status == StatusMatricula.Trancada)
                row.Cells["Status"].Style.ForeColor = Theme.Danger;
            else
                row.Cells["Status"].Style.ForeColor = Theme.Primary;
        }

        _lblContador.Text = $"Total: {lista.Count} matrículas";
    }

    private void Filtrar()
    {
        var termo = _txtBusca.Text.Trim().ToLowerInvariant();
        var filtroStatus = _cbFiltroStatus.SelectedItem?.ToString();

        foreach (DataGridViewRow row in _grid.Rows)
        {
            var aluno = row.Cells["Aluno"].Value?.ToString()?.ToLowerInvariant() ?? "";
            var curso = row.Cells["Curso"].Value?.ToString()?.ToLowerInvariant() ?? "";
            var status = row.Cells["Status"].Value?.ToString() ?? "";

            bool matchTexto = string.IsNullOrEmpty(termo) || aluno.Contains(termo) || curso.Contains(termo);
            bool matchStatus = filtroStatus == "Todas" || status == filtroStatus;

            row.Visible = matchTexto && matchStatus;
        }
    }

    private void BtnNova_Click(object? sender, EventArgs e)
    {
        using var modal = new FormNovaMatricula();
        if (modal.ShowDialog() == DialogResult.OK)
        {
            CarregarMatriculas();
        }
    }

    private void BtnAlterarStatus_Click(object? sender, EventArgs e)
    {
        if (_grid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Selecione uma matrícula para alterar a situação.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int id = Convert.ToInt32(_grid.SelectedRows[0].Cells["Id"].Value);
        var mat = DataRepository.Instance.Matriculas.FirstOrDefault(m => m.Id == id);
        if (mat != null)
        {
            using var modal = new FormAlterarStatusMatricula(mat);
            if (modal.ShowDialog() == DialogResult.OK)
            {
                CarregarMatriculas();
            }
        }
    }

    private void BtnAtualizarNota_Click(object? sender, EventArgs e)
    {
        if (_grid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Selecione uma matrícula para lançar nota/progresso.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int id = Convert.ToInt32(_grid.SelectedRows[0].Cells["Id"].Value);
        var mat = DataRepository.Instance.Matriculas.FirstOrDefault(m => m.Id == id);
        if (mat != null)
        {
            using var modal = new FormLancarNota(mat);
            if (modal.ShowDialog() == DialogResult.OK)
            {
                CarregarMatriculas();
            }
        }
    }
}

public class FormNovaMatricula : Form
{
    private readonly ComboBox _cbAluno = new() { Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontBody };
    private readonly ComboBox _cbCurso = new() { Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontBody };
    private readonly TextBox _txtPeriodo = new() { Width = 340, Font = Theme.FontBody };
    private readonly TextBox _txtObs = new() { Width = 340, Height = 60, Multiline = true, Font = Theme.FontBody };

    public FormNovaMatricula()
    {
        Text = "Realizar Nova Matrícula";
        Size = new Size(410, 480);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        var repo = DataRepository.Instance;

        // Verifica trava global de novas matrículas definida pelo Administrador
        if (!repo.Parametros.PermitirNovasMatriculas)
        {
            MessageBox.Show("As novas matrículas estão temporariamente bloqueadas nas configurações do sistema pelo Administrador.", "Matrículas Fechadas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        foreach (var a in repo.Alunos.Where(al => al.Ativo))
        {
            _cbAluno.Items.Add(a);
        }
        if (_cbAluno.Items.Count > 0) _cbAluno.SelectedIndex = 0;

        foreach (var c in repo.Cursos.Where(cu => cu.Ativo))
        {
            _cbCurso.Items.Add(c);
        }
        if (_cbCurso.Items.Count > 0) _cbCurso.SelectedIndex = 0;

        _txtPeriodo.Text = repo.Parametros.PeriodoLetivoAtual;

        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(24),
            AutoScroll = true
        };

        panel.Controls.Add(new Label { Text = "Selecione o Aluno:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true });
        panel.Controls.Add(_cbAluno);

        panel.Controls.Add(new Label { Text = "Selecione o Curso:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 10, 0, 0), AutoSize = true });
        panel.Controls.Add(_cbCurso);

        panel.Controls.Add(new Label { Text = "Período Letivo:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 10, 0, 0), AutoSize = true });
        panel.Controls.Add(_txtPeriodo);

        panel.Controls.Add(new Label { Text = "Observações da Secretaria:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 10, 0, 0), AutoSize = true });
        panel.Controls.Add(_txtObs);

        var btnConfirmar = new Button { Text = "Efetivar Matrícula", Width = 340, Height = 40, Margin = new Padding(0, 18, 0, 0) };
        Theme.ApplyModernButton(btnConfirmar, Theme.Primary, Color.White);
        btnConfirmar.Click += Confirmar;
        panel.Controls.Add(btnConfirmar);

        Controls.Add(panel);
    }

    private void Confirmar(object? sender, EventArgs e)
    {
        if (_cbAluno.SelectedItem is not Aluno aluno || _cbCurso.SelectedItem is not Curso curso)
        {
            MessageBox.Show("Selecione um aluno e um curso válidos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var repo = DataRepository.Instance;

        // Verifica se aluno já está matriculado no curso
        bool jaMatriculado = repo.Matriculas.Any(m => m.AlunoId == aluno.Id && m.CursoId == curso.Id && m.Status == StatusMatricula.Ativa);
        if (jaMatriculado)
        {
            MessageBox.Show($"O aluno {aluno.Nome} já possui matrícula ATIVA no curso '{curso.Titulo}'.", "Duplicidade de Matrícula", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Verifica vagas
        if (curso.VagasDisponiveis <= 0)
        {
            MessageBox.Show($"O curso '{curso.Titulo}' não possui vagas disponíveis no momento.", "Vagas Esgotadas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int novoId = (repo.Matriculas.MaxBy(m => m.Id)?.Id ?? 0) + 1;
        var novaMatricula = new Matricula
        {
            Id = novoId,
            AlunoId = aluno.Id,
            AlunoNome = aluno.Nome,
            AlunoCpf = aluno.Cpf,
            CursoId = curso.Id,
            CursoNome = curso.Titulo,
            DataMatricula = DateTime.Now,
            Status = StatusMatricula.Ativa,
            NotaFinal = 0.0,
            ProgressoPercentual = 0,
            PeriodoLetivo = _txtPeriodo.Text.Trim(),
            Observacoes = _txtObs.Text.Trim()
        };

        curso.VagasOcupadas++;
        repo.Matriculas.Add(novaMatricula);
        repo.Salvar();

        MessageBox.Show($"Matrícula nº {novaMatricula.Id} efetuada com sucesso para o aluno {aluno.Nome}!", "Matrícula Realizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
        Close();
    }
}

public class FormAlterarStatusMatricula : Form
{
    private readonly Matricula _matricula;
    private readonly ComboBox _cbNovoStatus = new() { Width = 320, DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontBody };
    private readonly TextBox _txtMotivo = new() { Width = 320, Height = 60, Multiline = true, Font = Theme.FontBody };

    public FormAlterarStatusMatricula(Matricula matricula)
    {
        _matricula = matricula;
        Text = $"Situação da Matrícula #{matricula.Id}";
        Size = new Size(390, 360);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        foreach (var s in Enum.GetValues<StatusMatricula>())
        {
            _cbNovoStatus.Items.Add(s);
        }
        _cbNovoStatus.SelectedItem = matricula.Status;

        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(24)
        };

        panel.Controls.Add(new Label { Text = $"Aluno: {matricula.AlunoNome}", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true });
        panel.Controls.Add(new Label { Text = $"Curso: {matricula.CursoNome}", Font = Theme.FontSmall, ForeColor = Theme.TextMuted, AutoSize = true });

        panel.Controls.Add(new Label { Text = "Nova Situação da Matrícula:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 15, 0, 0), AutoSize = true });
        panel.Controls.Add(_cbNovoStatus);

        panel.Controls.Add(new Label { Text = "Justificativa / Motivo da Alteração:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 10, 0, 0), AutoSize = true });
        panel.Controls.Add(_txtMotivo);

        var btnSalvar = new Button { Text = "Confirmar Alteração", Width = 320, Height = 38, Margin = new Padding(0, 15, 0, 0) };
        Theme.ApplyModernButton(btnSalvar, Theme.Primary, Color.White);
        btnSalvar.Click += Salvar;
        panel.Controls.Add(btnSalvar);

        Controls.Add(panel);
    }

    private void Salvar(object? sender, EventArgs e)
    {
        if (_cbNovoStatus.SelectedItem is StatusMatricula novoStatus)
        {
            var antigoStatus = _matricula.Status;
            _matricula.Status = novoStatus;
            if (!string.IsNullOrWhiteSpace(_txtMotivo.Text))
            {
                _matricula.Observacoes += $" [{DateTime.Now:dd/MM HH:mm} Situação alterada de {antigoStatus} para {novoStatus}: {_txtMotivo.Text.Trim()}]";
            }

            DataRepository.Instance.Salvar();
            MessageBox.Show("Situação da matrícula atualizada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

public class FormLancarNota : Form
{
    private readonly Matricula _matricula;
    private readonly NumericUpDown _numNota = new() { Width = 320, DecimalPlaces = 1, Minimum = 0, Maximum = 10, Increment = 0.5m, Font = Theme.FontBody };
    private readonly NumericUpDown _numProgresso = new() { Width = 320, Minimum = 0, Maximum = 100, Increment = 5, Font = Theme.FontBody };

    public FormLancarNota(Matricula matricula)
    {
        _matricula = matricula;
        Text = $"Lançar Desempenho - #{matricula.Id}";
        Size = new Size(380, 310);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        _numNota.Value = (decimal)matricula.NotaFinal;
        _numProgresso.Value = matricula.ProgressoPercentual;

        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(24)
        };

        panel.Controls.Add(new Label { Text = $"Aluno: {matricula.AlunoNome}", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true });
        panel.Controls.Add(new Label { Text = $"Curso: {matricula.CursoNome}", Font = Theme.FontSmall, ForeColor = Theme.TextMuted, AutoSize = true });

        panel.Controls.Add(new Label { Text = "Progresso Concluído (%):", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 12, 0, 0), AutoSize = true });
        panel.Controls.Add(_numProgresso);

        panel.Controls.Add(new Label { Text = "Nota Final Obtida (0 a 10):", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 10, 0, 0), AutoSize = true });
        panel.Controls.Add(_numNota);

        var btnSalvar = new Button { Text = "Salvar Lançamento", Width = 320, Height = 38, Margin = new Padding(0, 16, 0, 0) };
        Theme.ApplyModernButton(btnSalvar, Theme.Success, Color.White);
        btnSalvar.Click += (s, e) =>
        {
            _matricula.NotaFinal = (double)_numNota.Value;
            _matricula.ProgressoPercentual = (int)_numProgresso.Value;

            if (_matricula.ProgressoPercentual >= 100 && _matricula.NotaFinal >= DataRepository.Instance.Parametros.MediaAprovacaoMinima)
            {
                _matricula.Status = StatusMatricula.Concluida;
            }

            DataRepository.Instance.Salvar();
            DialogResult = DialogResult.OK;
            Close();
        };
        panel.Controls.Add(btnSalvar);

        Controls.Add(panel);
    }
}
