using System.Drawing;
using System.Windows.Forms;
using CursaAqui.App.Models;
using CursaAqui.App.Services;

namespace CursaAqui.App.UI.Controls;

public class UcGerenciarCursos : UserControl
{
    private readonly DataGridView _grid;
    private readonly TextBox _txtBusca;
    private readonly ComboBox _cbFiltroStatus;
    private readonly Label _lblResumo;

    public UcGerenciarCursos()
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
            Text = "Gerenciamento do Catálogo de Cursos",
            Font = Theme.FontTitleLarge,
            ForeColor = Theme.TextDark,
            AutoSize = true,
            Location = new Point(0, 5)
        };
        var lblSub = new Label
        {
            Text = "Cadastro, edição, parametrização de carga horária e inativação de cursos",
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

        var lblBuscar = new Label { Text = "Buscar:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 6, 0) };
        _txtBusca = new TextBox { Width = 180, Font = Theme.FontBody, Margin = new Padding(0, 5, 12, 0) };
        _txtBusca.TextChanged += (s, e) => Filtrar();

        var lblStatus = new Label { Text = "Status:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 6, 0) };
        _cbFiltroStatus = new ComboBox { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontBody, Margin = new Padding(0, 5, 12, 0) };
        _cbFiltroStatus.Items.AddRange(new object[] { "Todos", "Ativos", "Inativos" });
        _cbFiltroStatus.SelectedIndex = 0;
        _cbFiltroStatus.SelectedIndexChanged += (s, e) => Filtrar();

        var btnNovo = new Button { Text = "+ Novo Curso", Width = 130, Height = 34, Margin = new Padding(0, 2, 8, 0) };
        Theme.ApplyModernButton(btnNovo, Theme.Primary, Color.White);
        btnNovo.Click += BtnNovo_Click;

        var btnEditar = new Button { Text = "Editar Curso", Width = 120, Height = 34, Margin = new Padding(0, 2, 8, 0) };
        Theme.ApplyModernButton(btnEditar, Theme.Secondary, Color.White);
        btnEditar.Click += BtnEditar_Click;

        var btnToggleStatus = new Button { Text = "Ativar / Inativar", Width = 135, Height = 34, Margin = new Padding(0, 2, 12, 0) };
        Theme.ApplyModernButton(btnToggleStatus, Theme.Warning, Color.Black);
        btnToggleStatus.Click += BtnToggleStatus_Click;

        _lblResumo = new Label { Text = "", AutoSize = true, Font = Theme.FontSmall, ForeColor = Theme.TextMuted, Margin = new Padding(0, 10, 0, 0) };

        toolbar.Controls.AddRange(new Control[] { lblBuscar, _txtBusca, lblStatus, _cbFiltroStatus, btnNovo, btnEditar, btnToggleStatus, _lblResumo });

        // 3. Grid
        _grid = new DataGridView { Dock = DockStyle.Fill };
        Theme.ApplyDataGridStyle(_grid);
        ConfigurarGrid();

        mainPanel.Controls.Add(headerPanel, 0, 0);
        mainPanel.Controls.Add(toolbar, 0, 1);
        mainPanel.Controls.Add(_grid, 0, 2);

        Controls.Add(mainPanel);

        CarregarCursos();
    }

    private void ConfigurarGrid()
    {
        _grid.Columns.Clear();
        _grid.Columns.Add("Id", "ID");
        _grid.Columns.Add("Codigo", "Código");
        _grid.Columns.Add("Titulo", "Título do Curso");
        _grid.Columns.Add("Categoria", "Categoria / Eixo");
        _grid.Columns.Add("CargaHoraria", "Carga Horária");
        _grid.Columns.Add("Professor", "Docente / Responsável");
        _grid.Columns.Add("Vagas", "Vagas (Totais/Ocupadas)");
        _grid.Columns.Add("Status", "Situação");

        _grid.Columns["Id"]!.Width = 50;
        _grid.Columns["Codigo"]!.Width = 110;
        _grid.Columns["CargaHoraria"]!.Width = 110;
        _grid.Columns["Vagas"]!.Width = 140;
        _grid.Columns["Status"]!.Width = 90;
    }

    private void CarregarCursos()
    {
        _grid.Rows.Clear();
        var lista = DataRepository.Instance.Cursos;

        foreach (var c in lista)
        {
            var vagasStr = $"{c.VagasTotais} / {c.VagasOcupadas} ({c.VagasDisponiveis} disp.)";
            var statusStr = c.Ativo ? "Ativo" : "Inativo";

            int rowIndex = _grid.Rows.Add(c.Id, c.Codigo, c.Titulo, c.Categoria, $"{c.CargaHoraria}h", c.ProfessorResponsavel, vagasStr, statusStr);
            if (!c.Ativo)
            {
                _grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Theme.TextMuted;
            }
        }

        _lblResumo.Text = $"Total: {lista.Count} cursos ({lista.Count(c => c.Ativo)} ativos)";
    }

    private void Filtrar()
    {
        var termo = _txtBusca.Text.Trim().ToLowerInvariant();
        var filtroStatus = _cbFiltroStatus.SelectedIndex; // 0=Todos, 1=Ativos, 2=Inativos

        foreach (DataGridViewRow row in _grid.Rows)
        {
            var titulo = row.Cells["Titulo"].Value?.ToString()?.ToLowerInvariant() ?? "";
            var codigo = row.Cells["Codigo"].Value?.ToString()?.ToLowerInvariant() ?? "";
            var cat = row.Cells["Categoria"].Value?.ToString()?.ToLowerInvariant() ?? "";
            var status = row.Cells["Status"].Value?.ToString() ?? "";

            bool matchTexto = string.IsNullOrEmpty(termo) || titulo.Contains(termo) || codigo.Contains(termo) || cat.Contains(termo);
            bool matchStatus = filtroStatus == 0 || (filtroStatus == 1 && status == "Ativo") || (filtroStatus == 2 && status == "Inativo");

            row.Visible = matchTexto && matchStatus;
        }
    }

    private void BtnNovo_Click(object? sender, EventArgs e)
    {
        using var modal = new FormEditarCurso(null);
        if (modal.ShowDialog() == DialogResult.OK)
        {
            CarregarCursos();
        }
    }

    private void BtnEditar_Click(object? sender, EventArgs e)
    {
        if (_grid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Selecione um curso para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int id = Convert.ToInt32(_grid.SelectedRows[0].Cells["Id"].Value);
        var curso = DataRepository.Instance.Cursos.FirstOrDefault(c => c.Id == id);
        if (curso != null)
        {
            using var modal = new FormEditarCurso(curso);
            if (modal.ShowDialog() == DialogResult.OK)
            {
                CarregarCursos();
            }
        }
    }

    private void BtnToggleStatus_Click(object? sender, EventArgs e)
    {
        if (_grid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Selecione um curso para alterar a situação.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int id = Convert.ToInt32(_grid.SelectedRows[0].Cells["Id"].Value);
        var curso = DataRepository.Instance.Cursos.FirstOrDefault(c => c.Id == id);
        if (curso != null)
        {
            curso.Ativo = !curso.Ativo;
            DataRepository.Instance.Salvar();
            CarregarCursos();
            MessageBox.Show($"O curso '{curso.Titulo}' foi {(curso.Ativo ? "ativado" : "inativado")} com sucesso!", "Status Atualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

public class FormEditarCurso : Form
{
    private readonly Curso? _cursoExistente;
    private readonly TextBox _txtCodigo = new() { Width = 340, Font = Theme.FontBody };
    private readonly TextBox _txtTitulo = new() { Width = 340, Font = Theme.FontBody };
    private readonly TextBox _txtCategoria = new() { Width = 340, Font = Theme.FontBody };
    private readonly NumericUpDown _numCarga = new() { Width = 340, Minimum = 10, Maximum = 400, Value = 40, Font = Theme.FontBody };
    private readonly TextBox _txtProfessor = new() { Width = 340, Font = Theme.FontBody };
    private readonly NumericUpDown _numVagas = new() { Width = 340, Minimum = 5, Maximum = 500, Value = 50, Font = Theme.FontBody };
    private readonly TextBox _txtDescricao = new() { Width = 340, Height = 60, Multiline = true, Font = Theme.FontBody };

    public FormEditarCurso(Curso? curso)
    {
        _cursoExistente = curso;
        Text = curso == null ? "Cadastrar Novo Curso" : "Editar Informações do Curso";
        Size = new Size(410, 560);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(24),
            AutoScroll = true
        };

        panel.Controls.Add(new Label { Text = "Código do Curso (Ex: CRS-ADS01):", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true });
        panel.Controls.Add(_txtCodigo);

        panel.Controls.Add(new Label { Text = "Nome / Título do Curso:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 0, 0) });
        panel.Controls.Add(_txtTitulo);

        panel.Controls.Add(new Label { Text = "Categoria / Eixo Tecnológico:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 0, 0) });
        panel.Controls.Add(_txtCategoria);

        panel.Controls.Add(new Label { Text = "Carga Horária (horas):", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 0, 0) });
        panel.Controls.Add(_numCarga);

        panel.Controls.Add(new Label { Text = "Professor / Coordenador Responsável:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 0, 0) });
        panel.Controls.Add(_txtProfessor);

        panel.Controls.Add(new Label { Text = "Vagas Totais:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 0, 0) });
        panel.Controls.Add(_numVagas);

        panel.Controls.Add(new Label { Text = "Descrição / Ementa Resumida:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 0, 0) });
        panel.Controls.Add(_txtDescricao);

        var btnSalvar = new Button { Text = curso == null ? "Cadastrar Curso" : "Atualizar Curso", Width = 340, Height = 38, Margin = new Padding(0, 16, 0, 0) };
        Theme.ApplyModernButton(btnSalvar, Theme.Primary, Color.White);
        btnSalvar.Click += Salvar;
        panel.Controls.Add(btnSalvar);

        Controls.Add(panel);

        if (_cursoExistente != null)
        {
            _txtCodigo.Text = _cursoExistente.Codigo;
            _txtTitulo.Text = _cursoExistente.Titulo;
            _txtCategoria.Text = _cursoExistente.Categoria;
            _numCarga.Value = _cursoExistente.CargaHoraria;
            _txtProfessor.Text = _cursoExistente.ProfessorResponsavel;
            _numVagas.Value = _cursoExistente.VagasTotais;
            _txtDescricao.Text = _cursoExistente.Descricao;
        }
        else
        {
            _txtCodigo.Text = $"CRS-{(DataRepository.Instance.Cursos.Count + 1):D2}";
            _txtProfessor.Text = "Coordenação Geral";
            _txtCategoria.Text = "Tecnologia da Informação";
        }
    }

    private void Salvar(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtTitulo.Text) || string.IsNullOrWhiteSpace(_txtCodigo.Text))
        {
            MessageBox.Show("Código e Título são obrigatórios.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var repo = DataRepository.Instance;

        if (_cursoExistente == null)
        {
            int novoId = (repo.Cursos.MaxBy(c => c.Id)?.Id ?? 0) + 1;
            var novoCurso = new Curso
            {
                Id = novoId,
                Codigo = _txtCodigo.Text.Trim(),
                Titulo = _txtTitulo.Text.Trim(),
                Categoria = _txtCategoria.Text.Trim(),
                CargaHoraria = (int)_numCarga.Value,
                ProfessorResponsavel = _txtProfessor.Text.Trim(),
                VagasTotais = (int)_numVagas.Value,
                VagasOcupadas = 0,
                Descricao = _txtDescricao.Text.Trim(),
                Ativo = true,
                Avaliacao = 5.0
            };
            repo.Cursos.Add(novoCurso);
        }
        else
        {
            _cursoExistente.Codigo = _txtCodigo.Text.Trim();
            _cursoExistente.Titulo = _txtTitulo.Text.Trim();
            _cursoExistente.Categoria = _txtCategoria.Text.Trim();
            _cursoExistente.CargaHoraria = (int)_numCarga.Value;
            _cursoExistente.ProfessorResponsavel = _txtProfessor.Text.Trim();
            _cursoExistente.VagasTotais = (int)_numVagas.Value;
            _cursoExistente.Descricao = _txtDescricao.Text.Trim();
        }

        repo.Salvar();
        DialogResult = DialogResult.OK;
        Close();
    }
}
