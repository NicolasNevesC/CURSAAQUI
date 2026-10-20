using System.Drawing;
using System.Windows.Forms;

namespace CursaAqui.App.UI;

public static class Theme
{
    // Cores Principais do CursaAqui
    public static readonly Color Primary = Color.FromArgb(79, 70, 229);      // Indigo vibrante
    public static readonly Color PrimaryDark = Color.FromArgb(67, 56, 202);  // Indigo escuro
    public static readonly Color Secondary = Color.FromArgb(30, 41, 59);     // Slate 800
    public static readonly Color DarkBg = Color.FromArgb(15, 23, 42);        // Slate 900
    public static readonly Color SidebarBg = Color.FromArgb(24, 33, 47);     // Sidebar navy
    public static readonly Color LightBg = Color.FromArgb(248, 250, 252);    // Fundo cinza suave
    public static readonly Color CardBg = Color.White;
    public static readonly Color BorderColor = Color.FromArgb(226, 232, 240);

    // Cores Semânticas
    public static readonly Color Success = Color.FromArgb(16, 185, 129);     // Verde esmeralda
    public static readonly Color Warning = Color.FromArgb(245, 158, 11);     // Âmbar
    public static readonly Color Danger = Color.FromArgb(239, 68, 68);       // Vermelho
    public static readonly Color Info = Color.FromArgb(14, 165, 233);        // Azul céu

    // Textos
    public static readonly Color TextDark = Color.FromArgb(30, 41, 59);
    public static readonly Color TextMuted = Color.FromArgb(100, 116, 139);
    public static readonly Color TextLight = Color.FromArgb(241, 245, 249);

    // Fontes
    public static readonly Font FontTitleLarge = new("Segoe UI", 16, FontStyle.Bold);
    public static readonly Font FontTitle = new("Segoe UI", 13, FontStyle.Bold);
    public static readonly Font FontSubtitle = new("Segoe UI", 10.5f, FontStyle.Bold);
    public static readonly Font FontBody = new("Segoe UI", 9.5f, FontStyle.Regular);
    public static readonly Font FontBodyBold = new("Segoe UI", 9.5f, FontStyle.Bold);
    public static readonly Font FontSmall = new("Segoe UI", 8.5f, FontStyle.Regular);

    public static void ApplyModernButton(Button btn, Color bg, Color fg)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.BackColor = bg;
        btn.ForeColor = fg;
        btn.Font = FontBodyBold;
        btn.Cursor = Cursors.Hand;
        btn.Padding = new Padding(12, 6, 12, 6);
    }

    public static void ApplyDataGridStyle(DataGridView grid)
    {
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.EnableHeadersVisualStyles = false;
        grid.GridColor = Color.FromArgb(241, 245, 249);
        grid.RowHeadersVisible = false;
        // Grids são preenchidos via código: sem a linha "nova" em branco no final
        // (ela não pode ser ocultada pelos filtros e gerava InvalidOperationException)
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.RowTemplate.Height = 36;

        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105);
        grid.ColumnHeadersDefaultCellStyle.Font = FontSubtitle;
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(6);
        grid.ColumnHeadersHeight = 40;

        grid.DefaultCellStyle.BackColor = Color.White;
        grid.DefaultCellStyle.ForeColor = TextDark;
        grid.DefaultCellStyle.Font = FontBody;
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 242, 255);
        grid.DefaultCellStyle.SelectionForeColor = Primary;
        grid.DefaultCellStyle.Padding = new Padding(6);
    }
}
