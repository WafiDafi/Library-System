// first modified in 18/04/20
//
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Library_app_test
{
    internal static class ThemeManager
    {
        public static void ApplyTheme(bool dark)
        {
            Color back = dark ? Color.FromArgb(45, 45, 48) : SystemColors.Control;
            Color panelBack = dark ? Color.FromArgb(37, 37, 38) : SystemColors.Control;
            Color fore = dark ? Color.White : SystemColors.ControlText;
            Color textboxBack = dark ? Color.FromArgb(30, 30, 30) : Color.White;

            foreach (Form f in Application.OpenForms)
            {
                ApplyToControl(f, dark, back, panelBack, fore, textboxBack);
                f.Refresh();
            }
        }

        private static void ApplyToControl(Control c, bool dark, Color back, Color panelBack, Color fore, Color textboxBack)
        {
            // Keep PictureBox images unchanged
            if (c is PictureBox)
            {
                // do nothing
            }
            else if (c is Panel)
            {
                c.BackColor = panelBack;
                c.ForeColor = fore;
            }
            else if (c is DataGridView dgv)
            {
                dgv.EnableHeadersVisualStyles = false;
                dgv.BackgroundColor = dark ? Color.FromArgb(30, 30, 30) : Color.White;
                dgv.GridColor = dark ? Color.DimGray : SystemColors.InactiveBorder;
                dgv.DefaultCellStyle.BackColor = dark ? Color.FromArgb(30, 30, 30) : Color.White;
                dgv.DefaultCellStyle.ForeColor = fore;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = dark ? Color.FromArgb(45, 45, 48) : SystemColors.Control;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = fore;
                dgv.RowHeadersDefaultCellStyle.BackColor = dgv.DefaultCellStyle.BackColor;
            }
            else if (c is TextBox)
            {
                c.BackColor = textboxBack;
                c.ForeColor = fore;
            }
            else if (c is Button)
            {
                c.BackColor = dark ? Color.FromArgb(63, 63, 70) : SystemColors.Control;
                c.ForeColor = fore;
            }
            else if (c is Label)
            {
                c.ForeColor = fore;
                c.BackColor = Color.Transparent;
            }
            else
            {
                c.BackColor = back;
                c.ForeColor = fore;
            }

            foreach (Control child in c.Controls)
            {
                ApplyToControl(child, dark, back, panelBack, fore, textboxBack);
            }
        }
    }
}
