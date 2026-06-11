using System;
using System.Windows.Forms;

namespace Library_app_test
{
    public class AddBookForm : Form
    {
        private TextBox txtTitle;
        private TextBox txtYear;
        private NumericUpDown numCopies;
        private Button btnOk;
        private Button btnCancel;

        public string TitleText => txtTitle.Text.Trim();
        public int? PublishedYear
        {
            get
            {
                if (int.TryParse(txtYear.Text.Trim(), out var y)) return y;
                return null;
            }
        }
        public int Copies => (int)numCopies.Value;

        public AddBookForm()
        {
            this.Text = "Add Book";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 420;
            this.Height = 220;

            var lblTitle = new Label { Text = "Title:", Left = 12, Top = 16, AutoSize = true };
            txtTitle = new TextBox { Left = 110, Top = 12, Width = 280 };

            var lblYear = new Label { Text = "Published Year:", Left = 12, Top = 52, AutoSize = true };
            txtYear = new TextBox { Left = 110, Top = 48, Width = 120 };

            var lblCopies = new Label { Text = "Copies:", Left = 12, Top = 88, AutoSize = true };
            numCopies = new NumericUpDown { Left = 110, Top = 84, Width = 80, Minimum = 1, Maximum = 100, Value = 1 };

            btnOk = new Button { Text = "OK", Left = 110, Top = 130, Width = 100 };
            btnCancel = new Button { Text = "Cancel", Left = 230, Top = 130, Width = 100 };

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblTitle);
            this.Controls.Add(txtTitle);
            this.Controls.Add(lblYear);
            this.Controls.Add(txtYear);
            this.Controls.Add(lblCopies);
            this.Controls.Add(numCopies);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleText))
            {
                MessageBox.Show("Please enter a title.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtYear.Text) && !int.TryParse(txtYear.Text.Trim(), out var y))
            {
                MessageBox.Show("Published year must be a number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
