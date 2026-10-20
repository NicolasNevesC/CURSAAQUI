using System.Drawing;
using System.Windows.Forms;
using CursaAqui.App.Models;
using CursaAqui.App.Services;

namespace CursaAqui.App.UI.Controls;

public class UcRegistrarAtendimentos : UserControl
{
    private readonly DataGridView _grid;
    private readonly TextBox _txtBusca;
    private readonly ComboBox _cbFiltroStatus;
    private readonly Label _lblContador;

    public UcRegistrarAtendimentos()
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
            Text = "Registro e Controle de Atendimentos",
            Font = Theme.FontTitleLarge,
            ForeColor = Theme.TextDark,
            AutoSize = true,
            Location = new Point(0, 5)
        };
        var lblSub = new Label
        {
            Text = "Histórico de solicitações, contatos, providências e suporte prestado pela Secretaria",
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
        _cbFiltroStatus.Items.AddRange(new object[] { "Todos", "Aberto", "EmAndamento", "Concluido", "Cancelado" });
        _cbFiltroStatus.SelectedIndex = 0;
        _cbFiltroStatus.SelectedIndexChanged += (s, e) => Filtrar();

        var btnNovo = new Button { Text = "+ Novo Atendimento", Width = 160, Height = 34, Margin = new Padding(0, 2, 8, 0) };
        Theme.ApplyModernButton(btnNovo, Theme.Primary, Color.White);
        btnNovo.Click += BtnNovo_Click;

        var btnVer = new Button { Text = "Ver / Registrar Providência", Width = 195, Height = 34, Margin = new Padding(0, 2, 12, 0) };
        Theme.ApplyModernButton(btnVer, Theme.Secondary, Color.White);
        btnVer.Click += BtnVer_Click;

        _lblContador = new Label { Text = "", AutoSize = true, Font = Theme.FontSmall, ForeColor = Theme.TextMuted, Margin = new Padding(0, 10, 0, 0) };

        toolbar.Controls.AddRange(new Control[] { lblBuscar, _txtBusca, lblStatus, _cbFiltroStatus, btnNovo, btnVer, _lblContador });

        // 3. Grid
        _grid = new DataGridView { Dock = DockStyle.Fill };
        Theme.ApplyDataGridStyle(_grid);
        ConfigurarGrid();

        mainPanel.Controls.Add(headerPanel, 0, 0);
        mainPanel.Controls.Add(toolbar, 0, 1);
        mainPanel.Controls.Add(_grid, 0, 2);

        Controls.Add(mainPanel);

        CarregarAtendimentos();
    }

    private void ConfigurarGrid()
    {
        _grid.Columns.Clear();
        _grid.Columns.Add("Protocolo", "Protocolo");
        _grid.Columns.Add("Solicitante", "Solicitante");
        _grid.Columns.Add("Tipo", "Tipo");
        _grid.Columns.Add("Assunto", "Assunto");
        _grid.Columns.Add("Contato", "Contato");
        _grid.Columns.Add("Data", "Data / Hora");
        _grid.Columns.Add("Atendente", "Atendente");
        _grid.Columns.Add("Status", "Situação");

        _grid.Columns["Protocolo"]!.Width = 130;
        _grid.Columns["Tipo"]!.Width = 85;
        _grid.Columns["Data"]!.Width = 120;
        _grid.Columns["Status"]!.Width = 110;
    }

    private void CarregarAtendimentos()
    {
        _grid.Rows.Clear();
        var lista = DataRepository.Instance.Atendimentos;

        foreach (var a in lista)
        {
            int rowIndex = _grid.Rows.Add(
                a.Protocolo,
                a.SolicitanteNome,
                a.TipoSolicitante.ToString(),
                a.Assunto,
                a.Contato,
                a.DataAbertura.ToString("dd/MM/yyyy HH:mm"),
                a.AtendenteNome,
                a.Status.ToString()
            );

            var row = _grid.Rows[rowIndex];
            if (a.Status == StatusAtendimento.Concluido)
                row.Cells["Status"].Style.ForeColor = Theme.Success;
            else if (a.Status == StatusAtendimento.Aberto)
                row.Cells["Status"].Style.ForeColor = Theme.Warning;
            else if (a.Status == StatusAtendimento.EmAndamento)
                row.Cells["Status"].Style.ForeColor = Theme.Info;
        }

        _lblContador.Text = $"Total: {lista.Count} registros";
    }

    private void Filtrar()
    {
        var termo = _txtBusca.Text.Trim().ToLowerInvariant();
        var filtroStatus = _cbFiltroStatus.SelectedItem?.ToString();

        foreach (DataGridViewRow row in _grid.Rows)
        {
            var prot = row.Cells["Protocolo"].Value?.ToString()?.ToLowerInvariant() ?? "";
            var sol = row.Cells["Solicitante"].Value?.ToString()?.ToLowerInvariant() ?? "";
            var ass = row.Cells["Assunto"].Value?.ToString()?.ToLowerInvariant() ?? "";
            var status = row.Cells["Status"].Value?.ToString() ?? "";

            bool matchTexto = string.IsNullOrEmpty(termo) || prot.Contains(termo) || sol.Contains(termo) || ass.Contains(termo);
            bool matchStatus = filtroStatus == "Todos" || status == filtroStatus;

            row.Visible = matchTexto && matchStatus;
        }
    }

    private void BtnNovo_Click(object? sender, EventArgs e)
    {
        using var modal = new FormNovoAtendimento();
        if (modal.ShowDialog() == DialogResult.OK)
        {
            CarregarAtendimentos();
        }
    }

    private void BtnVer_Click(object? sender, EventArgs e)
    {
        if (_grid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Selecione um atendimento para visualizar os detalhes.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string protocolo = _grid.SelectedRows[0].Cells["Protocolo"].Value?.ToString() ?? "";
        var atend = DataRepository.Instance.Atendimentos.FirstOrDefault(a => a.Protocolo == protocolo);
        if (atend != null)
        {
            using var modal = new FormDetalhesAtendimento(atend);
            if (modal.ShowDialog() == DialogResult.OK)
            {
                CarregarAtendimentos();
            }
        }
    }
}

public class FormNovoAtendimento : Form
{
    private readonly TextBox _txtProtocolo = new() { Width = 340, ReadOnly = true, BackColor = Color.FromArgb(241, 245, 249), Font = Theme.FontBodyBold };
    private readonly TextBox _txtSolicitante = new() { Width = 340, Font = Theme.FontBody };
    private readonly ComboBox _cbTipo = new() { Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontBody };
    private readonly TextBox _txtContato = new() { Width = 340, Font = Theme.FontBody };
    private readonly TextBox _txtAssunto = new() { Width = 340, Font = Theme.FontBody };
    private readonly TextBox _txtDescricao = new() { Width = 340, Height = 70, Multiline = true, Font = Theme.FontBody };
    private readonly TextBox _txtProvidencia = new() { Width = 340, Height = 60, Multiline = true, Font = Theme.FontBody };

    public FormNovoAtendimento()
    {
        Text = "Registrar Atendimento - Secretaria";
        Size = new Size(410, 580);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        int proximoNumero = DataRepository.Instance.Atendimentos.Count + 1;
        _txtProtocolo.Text = $"ATEND-2026-{proximoNumero:D3}";

        foreach (var t in Enum.GetValues<TipoSolicitante>())
        {
            _cbTipo.Items.Add(t);
        }
        _cbTipo.SelectedIndex = 0;

        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(24),
            AutoScroll = true
        };

        panel.Controls.Add(new Label { Text = "Número de Protocolo Automático:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true });
        panel.Controls.Add(_txtProtocolo);

        panel.Controls.Add(new Label { Text = "Nome do Solicitante:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 8, 0, 0), AutoSize = true });
        panel.Controls.Add(_txtSolicitante);

        panel.Controls.Add(new Label { Text = "Tipo de Solicitante:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 8, 0, 0), AutoSize = true });
        panel.Controls.Add(_cbTipo);

        panel.Controls.Add(new Label { Text = "Contato (Telefone / E-mail):", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 8, 0, 0), AutoSize = true });
        panel.Controls.Add(_txtContato);

        panel.Controls.Add(new Label { Text = "Assunto Principal:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 8, 0, 0), AutoSize = true });
        panel.Controls.Add(_txtAssunto);

        panel.Controls.Add(new Label { Text = "Descrição da Solicitação:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 8, 0, 0), AutoSize = true });
        panel.Controls.Add(_txtDescricao);

        panel.Controls.Add(new Label { Text = "Providência Inicial (Opcional):", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 8, 0, 0), AutoSize = true });
        panel.Controls.Add(_txtProvidencia);

        var btnSalvar = new Button { Text = "Gravar Atendimento", Width = 340, Height = 38, Margin = new Padding(0, 16, 0, 0) };
        Theme.ApplyModernButton(btnSalvar, Theme.Primary, Color.White);
        btnSalvar.Click += Salvar;
        panel.Controls.Add(btnSalvar);

        Controls.Add(panel);
    }

    private void Salvar(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtSolicitante.Text) || string.IsNullOrWhiteSpace(_txtAssunto.Text))
        {
            MessageBox.Show("Informe o solicitante e o assunto.", "Campos obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var repo = DataRepository.Instance;
        int novoId = (repo.Atendimentos.MaxBy(a => a.Id)?.Id ?? 0) + 1;
        var atendente = AuthService.Instance.UsuarioLogado;

        var atend = new Atendimento
        {
            Id = novoId,
            Protocolo = _txtProtocolo.Text,
            SolicitanteNome = _txtSolicitante.Text.Trim(),
            TipoSolicitante = (TipoSolicitante)_cbTipo.SelectedItem!,
            Contato = _txtContato.Text.Trim(),
            Assunto = _txtAssunto.Text.Trim(),
            Descricao = _txtDescricao.Text.Trim(),
            Providencia = _txtProvidencia.Text.Trim(),
            DataAbertura = DateTime.Now,
            AtendenteId = atendente?.Id ?? 0,
            AtendenteNome = atendente?.Nome ?? "Secretaria",
            Status = string.IsNullOrWhiteSpace(_txtProvidencia.Text) ? StatusAtendimento.Aberto : StatusAtendimento.EmAndamento
        };

        repo.Atendimentos.Add(atend);
        repo.Salvar();

        MessageBox.Show($"Atendimento registrado com sucesso sob o protocolo {_txtProtocolo.Text}!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
        Close();
    }
}

public class FormDetalhesAtendimento : Form
{
    private readonly Atendimento _atendimento;
    private readonly ComboBox _cbStatus = new() { Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontBody };
    private readonly TextBox _txtProvidencia = new() { Width = 340, Height = 90, Multiline = true, Font = Theme.FontBody };

    public FormDetalhesAtendimento(Atendimento a)
    {
        _atendimento = a;
        Text = $"Atendimento {a.Protocolo}";
        Size = new Size(410, 520);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        foreach (var s in Enum.GetValues<StatusAtendimento>())
        {
            _cbStatus.Items.Add(s);
        }
        _cbStatus.SelectedItem = a.Status;
        _txtProvidencia.Text = a.Providencia;

        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(24),
            AutoScroll = true
        };

        panel.Controls.Add(new Label { Text = $"Protocolo: {a.Protocolo}", Font = Theme.FontSubtitle, ForeColor = Theme.Primary, AutoSize = true });
        panel.Controls.Add(new Label { Text = $"Solicitante: {a.SolicitanteNome} ({a.TipoSolicitante})", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true });
        panel.Controls.Add(new Label { Text = $"Contato: {a.Contato}", Font = Theme.FontBody, ForeColor = Theme.TextMuted, AutoSize = true });
        panel.Controls.Add(new Label { Text = $"Data de Abertura: {a.DataAbertura:dd/MM/yyyy HH:mm}", Font = Theme.FontSmall, ForeColor = Theme.TextMuted, AutoSize = true });
        panel.Controls.Add(new Label { Text = $"Atendente Original: {a.AtendenteNome}", Font = Theme.FontSmall, ForeColor = Theme.TextMuted, AutoSize = true });

        panel.Controls.Add(new Label { Text = "Descrição:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 10, 0, 0), AutoSize = true });
        var lblDesc = new Label { Text = a.Descricao, Font = Theme.FontBody, ForeColor = Theme.TextDark, Width = 340, Height = 55 };
        panel.Controls.Add(lblDesc);

        panel.Controls.Add(new Label { Text = "Atualizar Situação:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 10, 0, 0), AutoSize = true });
        panel.Controls.Add(_cbStatus);

        panel.Controls.Add(new Label { Text = "Providências Tomadas:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, Margin = new Padding(0, 10, 0, 0), AutoSize = true });
        panel.Controls.Add(_txtProvidencia);

        var btnSalvar = new Button { Text = "Atualizar Atendimento", Width = 340, Height = 38, Margin = new Padding(0, 16, 0, 0) };
        Theme.ApplyModernButton(btnSalvar, Theme.Primary, Color.White);
        btnSalvar.Click += Salvar;
        panel.Controls.Add(btnSalvar);

        Controls.Add(panel);
    }

    private void Salvar(object? sender, EventArgs e)
    {
        _atendimento.Status = (StatusAtendimento)_cbStatus.SelectedItem!;
        _atendimento.Providencia = _txtProvidencia.Text.Trim();

        if (_atendimento.Status == StatusAtendimento.Concluido && _atendimento.DataFechamento == null)
        {
            _atendimento.DataFechamento = DateTime.Now;
        }

        DataRepository.Instance.Salvar();
        MessageBox.Show("Atendimento atualizado com sucesso!", "Atualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
        Close();
    }
}
