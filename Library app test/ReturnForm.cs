// first modified in 18/04/20
//
using System;
using System.Linq;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public class ReturnForm : Form
    {
        private DataGridView dgv;
        private Button btnReturn;
        private Button btnBack;

        public ReturnForm()
        {
            this.Text = "Return Book";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 600;
            this.Height = 400;

            dgv = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 280,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            btnReturn = new Button { Text = "Return Book", Left = 360, Top = 300, Width = 100 };
            btnBack = new Button { Text = "Back", Left = 480, Top = 300, Width = 80 };
            btnReturn.Click += BtnReturn_Click;
            btnBack.Click += (s, e) => this.Close();

            this.Controls.Add(dgv);
            this.Controls.Add(btnReturn);
            this.Controls.Add(btnBack);

            this.Load += ReturnForm_Load;
        }

        private void ReturnForm_Load(object? sender, EventArgs e)
        {
            try
            {
                LibraryDb.EnsureDatabase();
                var list = Repository.GetIssuedIssues().ToList();
                if (!list.Any())
                {
                    // show a placeholder row
                    var dt = new System.Data.DataTable();
                    dt.Columns.Add("Info");
                    var r = dt.NewRow();
                    r[0] = "None";
                    dt.Rows.Add(r);
                    dgv.DataSource = dt;
                    btnReturn.Enabled = false;
                    return;
                }

                dgv.DataSource = list.Select(i => new { i.Id, i.BookTitle, i.MemberName, IssueDate = i.IssueDate.ToString("yyyy-MM-dd"), DueDate = i.DueDate.ToString("yyyy-MM-dd"), i.Status }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load issued books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Please select an issued record to return.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            var idObj = dgv.SelectedRows[0].Cells[0].Value;
            if (idObj == null) return;
            if (!int.TryParse(idObj.ToString(), out var issueId)) return;

            // ask for returner name and book id (book id is in the issue record but prompt requests it)
            using var prompt = new Form { Width = 360, Height = 160, StartPosition = FormStartPosition.CenterParent, Text = "Return Book" };
            var lblName = new Label { Text = "Returner Name:", Left = 12, Top = 12, AutoSize = true };
            var txtName = new TextBox { Left = 120, Top = 8, Width = 220 };
            var lblBook = new Label { Text = "Book Id:", Left = 12, Top = 44, AutoSize = true };
            var txtBook = new TextBox { Left = 120, Top = 40, Width = 220 };
            var btnOk = new Button { Text = "Return", Left = 120, Top = 76, Width = 100 };
            var btnCancel = new Button { Text = "Cancel", Left = 240, Top = 76, Width = 100 };
            btnOk.Click += (s, ea) => { prompt.DialogResult = DialogResult.OK; prompt.Close(); };
            btnCancel.Click += (s, ea) => { prompt.DialogResult = DialogResult.Cancel; prompt.Close(); };
            prompt.Controls.Add(lblName); prompt.Controls.Add(txtName); prompt.Controls.Add(lblBook); prompt.Controls.Add(txtBook); prompt.Controls.Add(btnOk); prompt.Controls.Add(btnCancel);

            var dr = prompt.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            // validate
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtBook.Text))
            {
                MessageBox.Show("Please enter returner name and book id.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txtBook.Text.Trim(), out var bookId))
            {
                MessageBox.Show("Book id must be an integer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Repository.ReturnBook(issueId, DateTime.UtcNow);
                MessageBox.Show("Book returned successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to return book: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
