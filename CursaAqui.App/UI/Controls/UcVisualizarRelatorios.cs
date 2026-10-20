using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using CursaAqui.App.Models;
using CursaAqui.App.Services;

namespace CursaAqui.App.UI.Controls;

public class UcVisualizarRelatorios : UserControl
{
    private readonly Panel _cardsPanel;
    private readonly Panel _chartPanel;
    private readonly DataGridView _gridAnalise;

    public UcVisualizarRelatorios()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.LightBg;

        var mainPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 4,
            ColumnCount = 1,
            Padding = new Padding(24)
        };
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 55));

        // 1. Cabeçalho
        var headerPanel = new Panel { Dock = DockStyle.Fill };
        var lblTitulo = new Label
        {
            Text = "Relatórios Gerenciais e Analíticos",
            Font = Theme.FontTitleLarge,
            ForeColor = Theme.TextDark,
            AutoSize = true,
            Location = new Point(0, 5)
        };
        var lblSub = new Label
        {
            Text = "Visão executiva: indicadores de desempenho institucional, matrículas e conclusões",
            Font = Theme.FontBody,
            ForeColor = Theme.TextMuted,
            AutoSize = true,
            Location = new Point(0, 35)
        };

        var btnExportar = new Button
        {
            Text = "Exportar Relatório Gerencial (.txt)",
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(780, 15),
            Width = 240,
            Height = 36
        };
        Theme.ApplyModernButton(btnExportar, Theme.Primary, Color.White);
        btnExportar.Click += ExportarRelatorio;

        headerPanel.Controls.AddRange(new Control[] { lblTitulo, lblSub, btnExportar });

        // 2. Cards de Métricas
        _cardsPanel = new Panel { Dock = DockStyle.Fill };
        CriarCardsMetricas();

        // 3. Gráfico de Matrículas por Curso
        _chartPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Margin = new Padding(0, 0, 0, 12)
        };
        _chartPanel.Paint += ChartPanel_Paint;

        // 4. Grid Analítico
        _gridAnalise = new DataGridView { Dock = DockStyle.Fill };
        Theme.ApplyDataGridStyle(_gridAnalise);
        ConfigurarGridAnalise();

        mainPanel.Controls.Add(headerPanel, 0, 0);
        mainPanel.Controls.Add(_cardsPanel, 0, 1);
        mainPanel.Controls.Add(_chartPanel, 0, 2);
        mainPanel.Controls.Add(_gridAnalise, 0, 3);

        Controls.Add(mainPanel);

        CarregarDadosAnaliticos();
    }

    private void CriarCardsMetricas()
    {
        _cardsPanel.Controls.Clear();
        var repo = DataRepository.Instance;

        int totalAlunos = repo.Alunos.Count;
        int cursosAtivos = repo.Cursos.Count(c => c.Ativo);
        int matriculasAtivas = repo.Matriculas.Count(m => m.Status == StatusMatricula.Ativa);
        int concluidas = repo.Matriculas.Count(m => m.Status == StatusMatricula.Concluida);
        double taxaConclusao = repo.Matriculas.Count > 0 ? (concluidas * 100.0 / repo.Matriculas.Count) : 0;

        int cardW = 200;
        int gap = 14;

        _cardsPanel.Controls.Add(CriarCard(0 * (cardW + gap), "ALUNOS CADASTRADOS", totalAlunos.ToString(), "Base de estudantes", Theme.Primary));
        _cardsPanel.Controls.Add(CriarCard(1 * (cardW + gap), "CURSOS ATIVOS", cursosAtivos.ToString(), "Catálogo acadêmico", Theme.Secondary));
        _cardsPanel.Controls.Add(CriarCard(2 * (cardW + gap), "MATRÍCULAS EM CURSO", matriculasAtivas.ToString(), "Estudando no momento", Theme.Success));
        _cardsPanel.Controls.Add(CriarCard(3 * (cardW + gap), "CERTIFICADOS EMITIDOS", concluidas.ToString(), "Conclusões com êxito", Theme.Warning));
        _cardsPanel.Controls.Add(CriarCard(4 * (cardW + gap), "TAXA DE CONCLUSÃO", $"{taxaConclusao:F1}%", "Aproveitamento geral", Theme.Info));
    }

    private Panel CriarCard(int x, string titulo, string valor, string subtitulo, Color corBorda)
    {
        var card = new Panel
        {
            Location = new Point(x, 5),
            Size = new Size(200, 85),
            BackColor = Color.White
        };

        var bar = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = corBorda };
        var lblTitle = new Label
        {
            Text = titulo,
            Font = Theme.FontSmall,
            ForeColor = Theme.TextMuted,
            Location = new Point(14, 10),
            AutoSize = true
        };
        var lblVal = new Label
        {
            Text = valor,
            Font = Theme.FontTitleLarge,
            ForeColor = Theme.TextDark,
            Location = new Point(14, 28),
            AutoSize = true
        };
        var lblSub = new Label
        {
            Text = subtitulo,
            Font = Theme.FontSmall,
            ForeColor = Theme.TextMuted,
            Location = new Point(14, 58),
            AutoSize = true
        };

        card.Controls.AddRange(new Control[] { bar, lblTitle, lblVal, lblSub });
        return card;
    }

    private void ConfigurarGridAnalise()
    {
        _gridAnalise.Columns.Clear();
        _gridAnalise.Columns.Add("Curso", "Curso / Disciplina");
        _gridAnalise.Columns.Add("Categoria", "Eixo Acadêmico");
        _gridAnalise.Columns.Add("Carga", "Carga Horária");
        _gridAnalise.Columns.Add("TotalMatriculas", "Total Matrículas");
        _gridAnalise.Columns.Add("Ativas", "Em Andamento");
        _gridAnalise.Columns.Add("Concluidas", "Concluídas");
        _gridAnalise.Columns.Add("Trancadas", "Trancadas / Canc.");
        _gridAnalise.Columns.Add("MediaNota", "Média de Aproveitamento");
    }

    private void CarregarDadosAnaliticos()
    {
        _gridAnalise.Rows.Clear();
        var repo = DataRepository.Instance;

        foreach (var c in repo.Cursos)
        {
            var mats = repo.Matriculas.Where(m => m.CursoId == c.Id).ToList();
            int total = mats.Count;
            int ativas = mats.Count(m => m.Status == StatusMatricula.Ativa);
            int conc = mats.Count(m => m.Status == StatusMatricula.Concluida);
            int tranc = mats.Count(m => m.Status == StatusMatricula.Trancada || m.Status == StatusMatricula.Cancelada);
            double media = mats.Count > 0 ? mats.Average(m => m.NotaFinal) : 0.0;

            _gridAnalise.Rows.Add(c.Titulo, c.Categoria, $"{c.CargaHoraria}h", total, ativas, conc, tranc, $"{media:F1} / 10.0");
        }

        _chartPanel.Invalidate();
    }

    private void ChartPanel_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var repo = DataRepository.Instance;
        var cursos = repo.Cursos.Take(6).ToList();

        g.DrawString("Distribuição Visual de Matrículas por Curso", Theme.FontSubtitle, new SolidBrush(Theme.TextDark), 15, 10);

        int maxMatriculas = 1;
        foreach (var c in cursos)
        {
            int count = repo.Matriculas.Count(m => m.CursoId == c.Id);
            if (count > maxMatriculas) maxMatriculas = count;
        }

        int startY = 40;
        int barHeight = 20;
        int spacing = 28;
        int maxBarWidth = _chartPanel.Width - 340;
        if (maxBarWidth < 100) maxBarWidth = 100;

        for (int i = 0; i < cursos.Count; i++)
        {
            var c = cursos[i];
            int count = repo.Matriculas.Count(m => m.CursoId == c.Id);
            int barWidth = (int)((double)count / maxMatriculas * maxBarWidth);
            if (count > 0 && barWidth < 20) barWidth = 20;

            int y = startY + i * spacing;

            string nomeCurto = c.Titulo.Length > 28 ? c.Titulo[..25] + "..." : c.Titulo;
            g.DrawString(nomeCurto, Theme.FontSmall, new SolidBrush(Theme.TextDark), 15, y + 2);

            var rect = new Rectangle(240, y, barWidth, barHeight);
            using (var brush = new LinearGradientBrush(new Rectangle(240, y, Math.Max(10, barWidth), barHeight), Theme.Primary, Theme.PrimaryDark, LinearGradientMode.Horizontal))
            {
                g.FillRectangle(brush, rect);
            }

            g.DrawString($"{count} aluno(s)", Theme.FontSmall, new SolidBrush(Theme.TextMuted), 245 + barWidth, y + 3);
        }
    }

    private void ExportarRelatorio(object? sender, EventArgs e)
    {
        var repo = DataRepository.Instance;
        var sb = new StringBuilder();

        sb.AppendLine("==========================================================================");
        sb.AppendLine("                 CURSA AQUI - RELATÓRIO GERENCIAL ANALÍTICO               ");
        sb.AppendLine("==========================================================================");
        sb.AppendLine($"Data de Emissão: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        sb.AppendLine($"Emitido por: {AuthService.Instance.UsuarioLogado?.Nome} ({AuthService.Instance.UsuarioLogado?.TipoPerfil})");
        sb.AppendLine($"Período Letivo Vigente: {repo.Parametros.PeriodoLetivoAtual}");
        sb.AppendLine();
        sb.AppendLine("--- 1. INDICADORES GERAIS ---");
        sb.AppendLine($"Total de Alunos Cadastrados: {repo.Alunos.Count}");
        sb.AppendLine($"Total de Cursos Ativos: {repo.Cursos.Count(c => c.Ativo)}");
        sb.AppendLine($"Total de Matrículas Realizadas: {repo.Matriculas.Count}");
        sb.AppendLine($"Matrículas Ativas: {repo.Matriculas.Count(m => m.Status == StatusMatricula.Ativa)}");
        sb.AppendLine($"Cursos Concluídos (Certificados): {repo.Matriculas.Count(m => m.Status == StatusMatricula.Concluida)}");
        sb.AppendLine();
        sb.AppendLine("--- 2. DETALHAMENTO POR CURSO ---");
        sb.AppendLine(string.Format("{0,-40} | {1,-8} | {2,-10} | {3,-10}", "Curso", "Carga", "Matrículas", "Média"));
        sb.AppendLine(new string('-', 76));

        foreach (var c in repo.Cursos)
        {
            var mats = repo.Matriculas.Where(m => m.CursoId == c.Id).ToList();
            double media = mats.Count > 0 ? mats.Average(m => m.NotaFinal) : 0.0;
            string titulo = c.Titulo.Length > 38 ? c.Titulo[..35] + "..." : c.Titulo;
            sb.AppendLine(string.Format("{0,-40} | {1,-8} | {2,-10} | {3,-10:F1}", titulo, $"{c.CargaHoraria}h", mats.Count, media));
        }

        sb.AppendLine();
        sb.AppendLine("==========================================================================");
        sb.AppendLine("Relatório para uso interno e apoio à tomada de decisões gerenciais.");

        using var saveDialog = new SaveFileDialog
        {
            Filter = "Arquivo de Texto (*.txt)|*.txt",
            FileName = $"RelatorioGerencial_CursaAqui_{DateTime.Now:yyyyMMdd_HHmm}.txt"
        };

        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            File.WriteAllText(saveDialog.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show("Relatório gerencial exportado com sucesso!", "Exportação Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
