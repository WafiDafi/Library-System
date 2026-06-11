// first modified in 18/04/20
//
using System;
using System.Linq;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public class IssueForm : Form
    {
        private TextBox txtName;
        private TextBox txtMemberId;
        private ComboBox cbBooks;
        private Button btnIssue;
        private Button btnBack;

        private int? preselectBookId;
        public IssueForm()
        {
            BuildUI();
        }

        public IssueForm(int bookId) : this()
        {
            this.preselectBookId = bookId;
        }

        private void BuildUI()
        {
            this.Text = "Issue Book";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 420;
            this.Height = 240;

            var lblName = new Label { Text = "Name of borrower:", Left = 12, Top = 16, AutoSize = true };
            txtName = new TextBox { Left = 150, Top = 12, Width = 240 };

            var lblId = new Label { Text = "Borrower Id:", Left = 12, Top = 52, AutoSize = true };
            txtMemberId = new TextBox { Left = 150, Top = 48, Width = 240 };

            var lblBook = new Label { Text = "Book:", Left = 12, Top = 92, AutoSize = true };
            cbBooks = new ComboBox { Left = 150, Top = 88, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };

            btnIssue = new Button { Text = "Issue Book", Left = 150, Top = 132, Width = 110 };
            btnBack = new Button { Text = "Back", Left = 280, Top = 132, Width = 110 };

            btnIssue.Click += BtnIssue_Click;
            btnBack.Click += (s, e) => this.Close();

            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblId);
            this.Controls.Add(txtMemberId);
            this.Controls.Add(lblBook);
            this.Controls.Add(cbBooks);
            this.Controls.Add(btnIssue);
            this.Controls.Add(btnBack);

            this.Load += IssueForm_Load;
        }

        private void IssueForm_Load(object? sender, EventArgs e)
        {
            try
            {
                LibraryDb.EnsureDatabase();
                // load available books only
                var books = Repository.GetAvailableBooks().ToList();
                if (!books.Any())
                {
                    cbBooks.Items.Add("<none available>");
                    cbBooks.SelectedIndex = 0;
                    btnIssue.Enabled = false;
                    return;
                }

                foreach (var b in books)
                {
                    cbBooks.Items.Add(new ComboItem { Id = b.Id, Text = $"{b.Id} - {b.Title} ({b.Copies} copies)" });
                }
                if (cbBooks.Items.Count > 0) cbBooks.SelectedIndex = 0;

                // if a book was preselected, try to select it
                if (preselectBookId.HasValue)
                {
                    for (int i = 0; i < cbBooks.Items.Count; i++)
                    {
                        if (cbBooks.Items[i] is ComboItem ci && ci.Id == preselectBookId.Value)
                        {
                            cbBooks.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnIssue_Click(object? sender, EventArgs e)
        {
            if (cbBooks.SelectedItem == null || cbBooks.SelectedItem is string) return;
            var ci = cbBooks.SelectedItem as ComboItem;
            if (ci == null) return;

            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtMemberId.Text))
            {
                MessageBox.Show("Please enter borrower name and id.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtMemberId.Text.Trim(), out var memberId))
            {
                MessageBox.Show("Member id must be an integer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // ensure member exists or create quickly
                var members = Repository.GetAllMembers();
                var exists = false;
                foreach (var m in members)
                {
                    if (m.Id == memberId) { exists = true; break; }
                }
                if (!exists)
                {
                    Repository.AddMember(txtName.Text.Trim(), null, null);
                }

                // perform issue
                var issueDate = DateTime.UtcNow;
                var dueDate = issueDate.AddDays(7);
                Repository.IssueBook(ci.Id, memberId, issueDate, dueDate);
                MessageBox.Show("Book issued successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Issue Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to issue book: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class ComboItem
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public override string ToString() => Text;
        }
    }
}
