using System.Drawing;
using System.Windows.Forms;
using CursaAqui.App.Models;
using CursaAqui.App.Services;

namespace CursaAqui.App.UI.Controls;

public class UcGerenciarPermissoes : UserControl
{
    private readonly DataGridView _grid;
    private readonly Label _lblInfo;

    public UcGerenciarPermissoes()
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
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

        // 1. Cabeçalho
        var headerPanel = new Panel { Dock = DockStyle.Fill };
        var lblTitulo = new Label
        {
            Text = "Gerenciamento de Permissões de Acesso",
            Font = Theme.FontTitleLarge,
            ForeColor = Theme.TextDark,
            AutoSize = true,
            Location = new Point(0, 5)
        };
        var lblSub = new Label
        {
            Text = "Controle de matriz de autorização para os perfis Administrador e Secretaria",
            Font = Theme.FontBody,
            ForeColor = Theme.TextMuted,
            AutoSize = true,
            Location = new Point(0, 35)
        };
        headerPanel.Controls.AddRange(new Control[] { lblTitulo, lblSub });

        // 2. Grid de Permissões
        _grid = new DataGridView { Dock = DockStyle.Fill };
        Theme.ApplyDataGridStyle(_grid);
        _grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
        _grid.ReadOnly = false;
        ConfigurarGrid();

        // 3. Rodapé com Ações
        var footer = new Panel { Dock = DockStyle.Fill };
        var btnSalvar = new Button
        {
            Text = "Salvar Alterações de Permissão",
            Location = new Point(0, 10),
            Width = 260,
            Height = 38
        };
        Theme.ApplyModernButton(btnSalvar, Theme.Primary, Color.White);
        btnSalvar.Click += SalvarPermissoes;

        var btnRestaurar = new Button
        {
            Text = "Restaurar Padrão",
            Location = new Point(275, 10),
            Width = 160,
            Height = 38
        };
        Theme.ApplyModernButton(btnRestaurar, Theme.Secondary, Color.White);
        btnRestaurar.Click += RestaurarPadrao;

        _lblInfo = new Label
        {
            Text = "Dica: Marque ou desmarque os privilégios e clique em 'Salvar' para atualizar as diretrizes.",
            Font = Theme.FontSmall,
            ForeColor = Theme.TextMuted,
            Location = new Point(450, 20),
            AutoSize = true
        };

        footer.Controls.AddRange(new Control[] { btnSalvar, btnRestaurar, _lblInfo });

        mainPanel.Controls.Add(headerPanel, 0, 0);
        mainPanel.Controls.Add(_grid, 0, 1);
        mainPanel.Controls.Add(footer, 0, 2);

        Controls.Add(mainPanel);

        CarregarPermissoes();
    }

    private void ConfigurarGrid()
    {
        _grid.Columns.Clear();

        var colChave = new DataGridViewTextBoxColumn
        {
            Name = "Chave",
            HeaderText = "Identificador",
            ReadOnly = true,
            Width = 180
        };

        var colNome = new DataGridViewTextBoxColumn
        {
            Name = "Nome",
            HeaderText = "Funcionalidade / Caso de Uso",
            ReadOnly = true,
            Width = 240
        };

        var colModulo = new DataGridViewTextBoxColumn
        {
            Name = "Modulo",
            HeaderText = "Módulo",
            ReadOnly = true,
            Width = 130
        };

        var colAdmin = new DataGridViewCheckBoxColumn
        {
            Name = "Admin",
            HeaderText = "Acesso Administrador",
            Width = 150
        };

        var colSec = new DataGridViewCheckBoxColumn
        {
            Name = "Secretaria",
            HeaderText = "Acesso Secretaria",
            Width = 150
        };

        var colDesc = new DataGridViewTextBoxColumn
        {
            Name = "Descricao",
            HeaderText = "Escopo da Permissão",
            ReadOnly = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        };

        _grid.Columns.AddRange(colChave, colNome, colModulo, colAdmin, colSec, colDesc);
    }

    private void CarregarPermissoes()
    {
        _grid.Rows.Clear();
        var lista = DataRepository.Instance.Permissoes;

        foreach (var p in lista)
        {
            _grid.Rows.Add(p.ChaveFuncionalidade, p.NomeExibicao, p.Modulo, p.PermitidoAdmin, p.PermitidoSecretaria, p.Descricao);
        }
    }

    private void SalvarPermissoes(object? sender, EventArgs e)
    {
        var repo = DataRepository.Instance;

        foreach (DataGridViewRow row in _grid.Rows)
        {
            if (row.IsNewRow) continue;

            var chave = row.Cells["Chave"].Value?.ToString();
            var adminPerm = Convert.ToBoolean(row.Cells["Admin"].Value);
            var secPerm = Convert.ToBoolean(row.Cells["Secretaria"].Value);

            var item = repo.Permissoes.FirstOrDefault(p => p.ChaveFuncionalidade == chave);
            if (item != null)
            {
                item.PermitidoAdmin = adminPerm;
                item.PermitidoSecretaria = secPerm;
            }
        }

        repo.Salvar();
        MessageBox.Show("Permissões salvas e aplicadas com sucesso a todos os perfis!", "Permissões Atualizadas", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void RestaurarPadrao(object? sender, EventArgs e)
    {
        if (MessageBox.Show("Deseja restaurar as permissões padrão do CursaAqui?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            var repo = DataRepository.Instance;
            foreach (var p in repo.Permissoes)
            {
                p.PermitidoAdmin = true;
                p.PermitidoSecretaria = p.ChaveFuncionalidade switch
                {
                    "GerenciarMatriculas" => true,
                    "ConsultarCadastro" => true,
                    "RegistrarAtendimentos" => true,
                    "GerarRelatoriosAdministrativos" => true,
                    _ => false
                };
            }
            repo.Salvar();
            CarregarPermissoes();
        }
    }
}
