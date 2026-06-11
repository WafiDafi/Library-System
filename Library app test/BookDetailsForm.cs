// first modified in 18/04/20
//
using System;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public class BookDetailsForm : Form
    {
        private int bookId;
        public BookDetailsForm(int bookId)
        {
            this.bookId = bookId;
            this.Text = "Book Details";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 480;
            this.Height = 260;

            var lblId = new Label { Text = "Id:", Left = 12, Top = 12, AutoSize = true };
            var lblIdVal = new Label { Left = 120, Top = 12, AutoSize = true };

            var lblTitle = new Label { Text = "Title:", Left = 12, Top = 40, AutoSize = true };
            var lblTitleVal = new Label { Left = 120, Top = 40, AutoSize = true, MaximumSize = new System.Drawing.Size(320, 0) };

            var lblIsbn = new Label { Text = "ISBN:", Left = 12, Top = 80, AutoSize = true };
            var lblIsbnVal = new Label { Left = 120, Top = 80, AutoSize = true };

            var lblYear = new Label { Text = "Published Year:", Left = 12, Top = 110, AutoSize = true };
            var lblYearVal = new Label { Left = 120, Top = 110, AutoSize = true };

            var lblCopies = new Label { Text = "Copies:", Left = 12, Top = 140, AutoSize = true };
            var lblCopiesVal = new Label { Left = 120, Top = 140, AutoSize = true };

            var btnIssue = new Button { Text = "Issue Book", Left = 120, Top = 180, Width = 120 };
            var btnBack = new Button { Text = "Back", Left = 260, Top = 180, Width = 120 };

            btnIssue.Click += BtnIssue_Click;
            btnBack.Click += (s, e) => this.Close();

            Controls.Add(lblId); Controls.Add(lblIdVal);
            Controls.Add(lblTitle); Controls.Add(lblTitleVal);
            Controls.Add(lblIsbn); Controls.Add(lblIsbnVal);
            Controls.Add(lblYear); Controls.Add(lblYearVal);
            Controls.Add(lblCopies); Controls.Add(lblCopiesVal);
            Controls.Add(btnIssue); Controls.Add(btnBack);

            this.Load += (s, e) =>
            {
                try
                {
                    LibraryDb.EnsureDatabase();
                    var b = Repository.GetAllBooks();
                    foreach (var bk in b)
                    {
                        if (bk.Id == this.bookId)
                        {
                            lblIdVal.Text = bk.Id.ToString();
                            lblTitleVal.Text = bk.Title;
                            lblIsbnVal.Text = bk.ISBN ?? "";
                            lblYearVal.Text = bk.PublishedYear?.ToString() ?? "";
                            lblCopiesVal.Text = bk.Copies.ToString();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load book details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private void BtnIssue_Click(object? sender, EventArgs e)
        {
            using var auth = new AdminAuthForm();
            var dr = auth.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            try
            {
                using var frm = new IssueForm(bookId);
                frm.ShowDialog(this);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Issue form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
