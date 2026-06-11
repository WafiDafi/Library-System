using System;
using System.Windows.Forms;

namespace Library_app_test
{
    public class IdPromptForm : Form
    {
        private TextBox txtId;
        private Button btnOk;
        private Button btnCancel;
        public int ParsedId { get; private set; }

        public IdPromptForm(string message)
        {
            this.Text = "Enter Id";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 360;
            this.Height = 150;

            var lbl = new Label { Text = message, Left = 12, Top = 12, AutoSize = true };
            txtId = new TextBox { Left = 12, Top = 36, Width = 320 };
            btnOk = new Button { Text = "OK", Left = 110, Top = 68, Width = 80 };
            btnCancel = new Button { Text = "Cancel", Left = 200, Top = 68, Width = 80 };

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lbl);
            this.Controls.Add(txtId);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text.Trim(), out var id))
            {
                MessageBox.Show("Please enter a valid numeric Id.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.ParsedId = id;
            this.DialogResult = DialogResult.OK;
        }
    }
}
