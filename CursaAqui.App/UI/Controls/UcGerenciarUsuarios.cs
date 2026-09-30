using System.Drawing;
using System.Windows.Forms;
using CursaAqui.App.Models;
using CursaAqui.App.Services;

namespace CursaAqui.App.UI.Controls;

public class UcGerenciarUsuarios : UserControl
{
    private readonly DataGridView _grid;
    private readonly TextBox _txtBusca;
    private readonly ComboBox _cbFiltroPerfil;
    private readonly Label _lblTotal;

    public UcGerenciarUsuarios()
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
            Text = "Gerenciamento de Usuários Internos",
            Font = Theme.FontTitleLarge,
            ForeColor = Theme.TextDark,
            AutoSize = true,
            Location = new Point(0, 5)
        };
        var lblSub = new Label
        {
            Text = "Administração de acessos institucionais (Administradores e Secretaria)",
            Font = Theme.FontBody,
            ForeColor = Theme.TextMuted,
            AutoSize = true,
            Location = new Point(0, 35)
        };
        headerPanel.Controls.AddRange(new Control[] { lblTitulo, lblSub });

        // 2. Barra de Ferramentas / Filtros Responsiva
        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoSize = true,
            Padding = new Padding(0, 4, 0, 4)
        };

        var lblBuscar = new Label { Text = "Buscar:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 6, 0) };
        _txtBusca = new TextBox { Width = 200, Font = Theme.FontBody, Margin = new Padding(0, 5, 14, 0) };
        _txtBusca.TextChanged += (s, e) => Filtrar();

        var lblPerfil = new Label { Text = "Perfil:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 6, 0) };
        _cbFiltroPerfil = new ComboBox { Width = 150, DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontBody, Margin = new Padding(0, 5, 14, 0) };
        _cbFiltroPerfil.Items.AddRange(new object[] { "Todos os Perfis", "Administrador", "Secretaria" });
        _cbFiltroPerfil.SelectedIndex = 0;
        _cbFiltroPerfil.SelectedIndexChanged += (s, e) => Filtrar();

        var btnNovo = new Button { Text = "+ Novo Usuário Interno", Width = 175, Height = 34, Margin = new Padding(0, 2, 8, 0) };
        Theme.ApplyModernButton(btnNovo, Theme.Primary, Color.White);
        btnNovo.Click += BtnNovo_Click;

        var btnStatus = new Button { Text = "Alternar Ativo/Inativo", Width = 160, Height = 34, Margin = new Padding(0, 2, 12, 0) };
        Theme.ApplyModernButton(btnStatus, Theme.Secondary, Color.White);
        btnStatus.Click += BtnStatus_Click;

        _lblTotal = new Label { Text = "", AutoSize = true, Font = Theme.FontSmall, ForeColor = Theme.TextMuted, Margin = new Padding(0, 10, 0, 0) };

        toolbar.Controls.AddRange(new Control[] { lblBuscar, _txtBusca, lblPerfil, _cbFiltroPerfil, btnNovo, btnStatus, _lblTotal });

        // 3. Grid de Usuários
        _grid = new DataGridView { Dock = DockStyle.Fill };
        Theme.ApplyDataGridStyle(_grid);
        ConfigurarGrid();

        mainPanel.Controls.Add(headerPanel, 0, 0);
        mainPanel.Controls.Add(toolbar, 0, 1);
        mainPanel.Controls.Add(_grid, 0, 2);

        Controls.Add(mainPanel);

        CarregarUsuarios();
    }

    private void ConfigurarGrid()
    {
        _grid.Columns.Clear();
        _grid.Columns.Add("Id", "ID");
        _grid.Columns.Add("Nome", "Nome Completo");
        _grid.Columns.Add("Email", "E-mail Institucional");
        _grid.Columns.Add("Cpf", "CPF");
        _grid.Columns.Add("Perfil", "Perfil / Especialização");
        _grid.Columns.Add("Detalhes", "Detalhes do Cargo");
        _grid.Columns.Add("Status", "Situação");

        _grid.Columns["Id"]!.Width = 60;
        _grid.Columns["Cpf"]!.Width = 130;
        _grid.Columns["Perfil"]!.Width = 140;
        _grid.Columns["Status"]!.Width = 90;
    }

    private void CarregarUsuarios()
    {
        _grid.Rows.Clear();
        var lista = DataRepository.Instance.UsuariosInternos;

        foreach (var u in lista)
        {
            var detalhes = u is Administrador adm ? $"Nível: {adm.NivelAutoridade}" : (u is Secretaria sec ? $"Setor: {sec.SetorAtendimento}" : "-");
            var statusStr = u.Ativo ? "Ativo" : "Inativo";

            int rowIndex = _grid.Rows.Add(u.Id, u.Nome, u.Email, u.Cpf, u.TipoPerfil.ToString(), detalhes, statusStr);
            if (!u.Ativo)
            {
                _grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Theme.TextMuted;
            }
        }

        _lblTotal.Text = $"Total: {lista.Count} usuários internos";
    }

    private void Filtrar()
    {
        var termo = _txtBusca.Text.Trim().ToLowerInvariant();
        var filtroPerfil = _cbFiltroPerfil.SelectedIndex; // 0=Todos, 1=Admin, 2=Secretaria

        foreach (DataGridViewRow row in _grid.Rows)
        {
            var nome = row.Cells["Nome"].Value?.ToString()?.ToLowerInvariant() ?? "";
            var email = row.Cells["Email"].Value?.ToString()?.ToLowerInvariant() ?? "";
            var perfil = row.Cells["Perfil"].Value?.ToString() ?? "";

            bool matchTexto = string.IsNullOrEmpty(termo) || nome.Contains(termo) || email.Contains(termo);
            bool matchPerfil = filtroPerfil == 0 || (filtroPerfil == 1 && perfil == "Administrador") || (filtroPerfil == 2 && perfil == "Secretaria");

            row.Visible = matchTexto && matchPerfil;
        }
    }

    private void BtnNovo_Click(object? sender, EventArgs e)
    {
        using var modal = new FormCriarUsuario();
        if (modal.ShowDialog() == DialogResult.OK)
        {
            CarregarUsuarios();
        }
    }

    private void BtnStatus_Click(object? sender, EventArgs e)
    {
        if (_grid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Selecione um usuário para alterar a situação.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int id = Convert.ToInt32(_grid.SelectedRows[0].Cells["Id"].Value);
        var usuario = DataRepository.Instance.UsuariosInternos.FirstOrDefault(u => u.Id == id);
        if (usuario != null)
        {
            if (usuario.Id == AuthService.Instance.UsuarioLogado?.Id)
            {
                MessageBox.Show("Não é permitido inativar o próprio usuário logado.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            usuario.Ativo = !usuario.Ativo;
            DataRepository.Instance.Salvar();
            CarregarUsuarios();
            MessageBox.Show($"Usuário '{usuario.Nome}' agora está {(usuario.Ativo ? "Ativo" : "Inativo")}.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

// Modal simplificado e moderno para criação de usuário interno
public class FormCriarUsuario : Form
{
    private readonly TextBox _txtNome = new() { Width = 320, Font = Theme.FontBody };
    private readonly TextBox _txtEmail = new() { Width = 320, Font = Theme.FontBody };
    private readonly TextBox _txtSenha = new() { Width = 320, Font = Theme.FontBody, UseSystemPasswordChar = true };
    private readonly TextBox _txtCpf = new() { Width = 320, Font = Theme.FontBody };
    private readonly ComboBox _cbPerfil = new() { Width = 320, DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontBody };
    private readonly TextBox _txtEspecializacao = new() { Width = 320, Font = Theme.FontBody };
    private readonly Label _lblEspecializacao = new() { Text = "Nível de Autoridade:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true };

    public FormCriarUsuario()
    {
        Text = "Novo Usuário Interno";
        Size = new Size(390, 480);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        _cbPerfil.Items.AddRange(new object[] { "Administrador", "Secretaria" });
        _cbPerfil.SelectedIndex = 0;
        _cbPerfil.SelectedIndexChanged += (s, e) =>
        {
            if (_cbPerfil.SelectedIndex == 0)
            {
                _lblEspecializacao.Text = "Nível de Autoridade (Ex: Diretoria, TI):";
                _txtEspecializacao.Text = "Supervisão";
            }
            else
            {
                _lblEspecializacao.Text = "Setor de Atendimento:";
                _txtEspecializacao.Text = "Secretaria Acadêmica";
            }
        };

        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(24),
            AutoScroll = true
        };

        panel.Controls.Add(new Label { Text = "Nome Completo:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true });
        panel.Controls.Add(_txtNome);

        panel.Controls.Add(new Label { Text = "E-mail Institucional:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
        panel.Controls.Add(_txtEmail);

        panel.Controls.Add(new Label { Text = "Senha de Acesso:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
        panel.Controls.Add(_txtSenha);

        panel.Controls.Add(new Label { Text = "CPF:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
        panel.Controls.Add(_txtCpf);

        panel.Controls.Add(new Label { Text = "Perfil do Usuário:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
        panel.Controls.Add(_cbPerfil);

        panel.Controls.Add(_lblEspecializacao);
        panel.Controls.Add(_txtEspecializacao);
        _txtEspecializacao.Text = "Coordenação Geral";

        var btnSalvar = new Button { Text = "Salvar Usuário", Width = 320, Height = 38, Margin = new Padding(0, 20, 0, 0) };
        Theme.ApplyModernButton(btnSalvar, Theme.Primary, Color.White);
        btnSalvar.Click += Salvar;
        panel.Controls.Add(btnSalvar);

        Controls.Add(panel);
    }

    private void Salvar(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtNome.Text) || string.IsNullOrWhiteSpace(_txtEmail.Text) || string.IsNullOrWhiteSpace(_txtSenha.Text))
        {
            MessageBox.Show("Preencha nome, e-mail e senha obrigatoriamente.", "Campos obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var repo = DataRepository.Instance;
        int novoId = (repo.UsuariosInternos.MaxBy(u => u.Id)?.Id ?? 0) + 1;

        if (_cbPerfil.SelectedIndex == 0) // Administrador
        {
            var admin = new Administrador
            {
                Id = novoId,
                Nome = _txtNome.Text.Trim(),
                Email = _txtEmail.Text.Trim(),
                Senha = _txtSenha.Text.Trim(),
                Cpf = _txtCpf.Text.Trim(),
                NivelAutoridade = string.IsNullOrWhiteSpace(_txtEspecializacao.Text) ? "Geral" : _txtEspecializacao.Text.Trim(),
                Ativo = true,
                DataCadastro = DateTime.Now
            };
            repo.UsuariosInternos.Add(admin);
        }
        else // Secretaria
        {
            var sec = new Secretaria
            {
                Id = novoId,
                Nome = _txtNome.Text.Trim(),
                Email = _txtEmail.Text.Trim(),
                Senha = _txtSenha.Text.Trim(),
                Cpf = _txtCpf.Text.Trim(),
                SetorAtendimento = string.IsNullOrWhiteSpace(_txtEspecializacao.Text) ? "Secretaria Geral" : _txtEspecializacao.Text.Trim(),
                Ativo = true,
                DataCadastro = DateTime.Now
            };
            repo.UsuariosInternos.Add(sec);
        }

        repo.Salvar();
        DialogResult = DialogResult.OK;
        Close();
    }
}
