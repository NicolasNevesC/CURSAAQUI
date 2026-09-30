using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CursaAqui.App.Models;
using CursaAqui.App.Services;

namespace CursaAqui.App.UI.Controls;

public class UcRelatoriosAdministrativos : UserControl
{
    private readonly ComboBox _cbTipoRelatorio;
    private readonly ComboBox _cbFiltroCurso;
    private readonly TextBox _txtPeriodo;
    private readonly RichTextBox _txtVisualizador;
    private readonly Button _btnGerar;
    private readonly Button _btnExportar;

    public UcRelatoriosAdministrativos()
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
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // 1. Cabeçalho
        var headerPanel = new Panel { Dock = DockStyle.Fill };
        var lblTitulo = new Label
        {
            Text = "Relatórios Administrativos Operacionais",
            Font = Theme.FontTitleLarge,
            ForeColor = Theme.TextDark,
            AutoSize = true,
            Location = new Point(0, 5)
        };
        var lblSub = new Label
        {
            Text = "Emissão de relatórios da rotina da Secretaria: listas de matriculados, atendimentos e pendências",
            Font = Theme.FontBody,
            ForeColor = Theme.TextMuted,
            AutoSize = true,
            Location = new Point(0, 35)
        };
        headerPanel.Controls.AddRange(new Control[] { lblTitulo, lblSub });

        // 2. Barra de Opções e Filtros Responsiva
        var filterPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoSize = true,
            Padding = new Padding(0, 4, 0, 4)
        };

        var lblTipo = new Label { Text = "Tipo de Relatório:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 6, 0) };
        _cbTipoRelatorio = new ComboBox { Width = 270, DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontBody, Margin = new Padding(0, 5, 12, 0) };
        _cbTipoRelatorio.Items.AddRange(new object[]
        {
            "1. Lista de Alunos Matriculados por Curso",
            "2. Relatório de Atendimentos Realizados",
            "3. Relação de Pendências e Alunos em Risco"
        });
        _cbTipoRelatorio.SelectedIndex = 0;

        var lblCurso = new Label { Text = "Curso:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 6, 0) };
        _cbFiltroCurso = new ComboBox { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontBody, Margin = new Padding(0, 5, 12, 0) };
        _cbFiltroCurso.Items.Add("Todos os Cursos");
        foreach (var c in DataRepository.Instance.Cursos)
        {
            _cbFiltroCurso.Items.Add(c.Titulo);
        }
        _cbFiltroCurso.SelectedIndex = 0;

        var lblPer = new Label { Text = "Período:", Font = Theme.FontBodyBold, ForeColor = Theme.TextDark, AutoSize = true, Margin = new Padding(0, 8, 6, 0) };
        _txtPeriodo = new TextBox { Text = DataRepository.Instance.Parametros.PeriodoLetivoAtual, Width = 80, Font = Theme.FontBody, Margin = new Padding(0, 5, 12, 0) };

        _btnGerar = new Button { Text = "Gerar Relatório", Width = 135, Height = 34, Margin = new Padding(0, 2, 8, 0) };
        Theme.ApplyModernButton(_btnGerar, Theme.Primary, Color.White);
        _btnGerar.Click += (s, e) => GerarRelatorio();

        _btnExportar = new Button { Text = "Exportar TXT", Width = 115, Height = 34, Margin = new Padding(0, 2, 10, 0) };
        Theme.ApplyModernButton(_btnExportar, Theme.Secondary, Color.White);
        _btnExportar.Click += ExportarArquivo;

        filterPanel.Controls.AddRange(new Control[] { lblTipo, _cbTipoRelatorio, lblCurso, _cbFiltroCurso, lblPer, _txtPeriodo, _btnGerar, _btnExportar });

        // 3. Área de Visualização do Relatório
        _txtVisualizador = new RichTextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 10, FontStyle.Regular),
            BackColor = Color.White,
            ForeColor = Color.FromArgb(15, 23, 42),
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            WordWrap = false
        };

        mainPanel.Controls.Add(headerPanel, 0, 0);
        mainPanel.Controls.Add(filterPanel, 0, 1);
        mainPanel.Controls.Add(_txtVisualizador, 0, 2);

        Controls.Add(mainPanel);

        GerarRelatorio();
    }

    private void GerarRelatorio()
    {
        int tipo = _cbTipoRelatorio.SelectedIndex;
        var repo = DataRepository.Instance;
        var sb = new StringBuilder();

        sb.AppendLine("==========================================================================================");
        sb.AppendLine("                       CURSA AQUI - RELATÓRIO ADMINISTRATIVO                              ");
        sb.AppendLine("                             SECRETARIA ACADÊMICA                                         ");
        sb.AppendLine("==========================================================================================");
        sb.AppendLine($"Data e Hora de Geração: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        sb.AppendLine($"Operador Responsável:   {AuthService.Instance.UsuarioLogado?.Nome} (Secretaria)");
        sb.AppendLine($"Período de Referência:  {_txtPeriodo.Text.Trim()}");
        sb.AppendLine("------------------------------------------------------------------------------------------");
        sb.AppendLine();

        if (tipo == 0) // Lista de Alunos Matriculados
        {
            sb.AppendLine("RELATÓRIO: LISTA DE ALUNOS MATRICULADOS");
            string cursoFiltro = _cbFiltroCurso.SelectedItem?.ToString() ?? "Todos os Cursos";
            sb.AppendLine($"Filtro Aplicado: {cursoFiltro}");
            sb.AppendLine();
            sb.AppendLine(string.Format("{0,-6} | {1,-26} | {2,-30} | {3,-10} | {4,-10}", "MATR", "ALUNO", "CURSO", "SITUAÇÃO", "NOTA FINAL"));
            sb.AppendLine(new string('-', 90));

            var mats = repo.Matriculas.Where(m =>
                m.PeriodoLetivo == _txtPeriodo.Text.Trim() &&
                (cursoFiltro == "Todos os Cursos" || m.CursoNome.Equals(cursoFiltro, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            foreach (var m in mats)
            {
                string nome = m.AlunoNome.Length > 24 ? m.AlunoNome[..22] + ".." : m.AlunoNome;
                string curso = m.CursoNome.Length > 28 ? m.CursoNome[..26] + ".." : m.CursoNome;
                sb.AppendLine(string.Format("{0,-6} | {1,-26} | {2,-30} | {3,-10} | {4,-10:F1}", m.Id, nome, curso, m.Status, m.NotaFinal));
            }

            sb.AppendLine();
            sb.AppendLine($"Total de Matrículas Encontradas: {mats.Count}");
            sb.AppendLine($"Matrículas Ativas: {mats.Count(m => m.Status == StatusMatricula.Ativa)} | Concluídas: {mats.Count(m => m.Status == StatusMatricula.Concluida)} | Trancadas: {mats.Count(m => m.Status == StatusMatricula.Trancada)}");
        }
        else if (tipo == 1) // Relatório de Atendimentos Realizados
        {
            sb.AppendLine("RELATÓRIO: HISTÓRICO DE ATENDIMENTOS DA SECRETARIA");
            sb.AppendLine();
            sb.AppendLine(string.Format("{0,-15} | {1,-20} | {2,-10} | {3,-24} | {4,-12}", "PROTOCOLO", "SOLICITANTE", "TIPO", "ASSUNTO", "SITUAÇÃO"));
            sb.AppendLine(new string('-', 90));

            var atendimentos = repo.Atendimentos.OrderByDescending(a => a.DataAbertura).ToList();
            foreach (var a in atendimentos)
            {
                string sol = a.SolicitanteNome.Length > 18 ? a.SolicitanteNome[..16] + ".." : a.SolicitanteNome;
                string ass = a.Assunto.Length > 22 ? a.Assunto[..20] + ".." : a.Assunto;
                sb.AppendLine(string.Format("{0,-15} | {1,-20} | {2,-10} | {3,-24} | {4,-12}", a.Protocolo, sol, a.TipoSolicitante, ass, a.Status));
                if (!string.IsNullOrEmpty(a.Providencia))
                {
                    sb.AppendLine($"   -> Providência: {a.Providencia}");
                }
            }

            sb.AppendLine();
            sb.AppendLine($"Total de Atendimentos Registrados: {atendimentos.Count}");
            sb.AppendLine($"Concluídos: {atendimentos.Count(a => a.Status == StatusAtendimento.Concluido)} | Em Aberto: {atendimentos.Count(a => a.Status == StatusAtendimento.Aberto)} | Em Andamento: {atendimentos.Count(a => a.Status == StatusAtendimento.EmAndamento)}");
        }
        else if (tipo == 2) // Pendências e Alunos em Risco
        {
            sb.AppendLine("RELATÓRIO: ALUNOS COM PENDÊNCIAS OU BAIXO RENDIMENTO");
            double mediaMin = repo.Parametros.MediaAprovacaoMinima;
            sb.AppendLine($"Critério de Alerta: Nota inferior a {mediaMin:F1} ou Matrícula Trancada");
            sb.AppendLine();
            sb.AppendLine(string.Format("{0,-25} | {1,-30} | {2,-10} | {3,-10} | {4,-10}", "ALUNO", "CURSO", "NOTA", "PROGRESSO", "STATUS"));
            sb.AppendLine(new string('-', 90));

            var pendentes = repo.Matriculas.Where(m =>
                (m.Status == StatusMatricula.Ativa && m.NotaFinal < mediaMin) ||
                m.Status == StatusMatricula.Trancada ||
                m.Status == StatusMatricula.Cancelada
            ).ToList();

            foreach (var m in pendentes)
            {
                string nome = m.AlunoNome.Length > 23 ? m.AlunoNome[..21] + ".." : m.AlunoNome;
                string curso = m.CursoNome.Length > 28 ? m.CursoNome[..26] + ".." : m.CursoNome;
                sb.AppendLine(string.Format("{0,-25} | {1,-30} | {2,-10:F1} | {3,-10} | {4,-10}", nome, curso, m.NotaFinal, $"{m.ProgressoPercentual}%", m.Status));
            }

            sb.AppendLine();
            sb.AppendLine($"Total de Ocorrências com Pendência: {pendentes.Count}");
            sb.AppendLine("Ação recomendada: Contatar alunos para orientação pedagógica e suporte acadêmico.");
        }

        sb.AppendLine();
        sb.AppendLine("==========================================================================================");
        sb.AppendLine("Documento gerado automaticamente pelo Sistema CursaAqui - Módulo Secretaria.");

        _txtVisualizador.Text = sb.ToString();
    }

    private void ExportarArquivo(object? sender, EventArgs e)
    {
        using var saveDialog = new SaveFileDialog
        {
            Filter = "Arquivo de Texto (*.txt)|*.txt",
            FileName = $"Relatorio_Secretaria_{DateTime.Now:yyyyMMdd_HHmm}.txt"
        };

        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            File.WriteAllText(saveDialog.FileName, _txtVisualizador.Text, Encoding.UTF8);
            MessageBox.Show("Relatório administrativo exportado com sucesso!", "Exportação Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
