// first modified in 18/04/20
//
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public partial class Form3 : Form
    {
        // grids for each tab
        private DataGridView? dgvAvailable;
        private DataGridView? dgvIssued;
        private DataGridView? dgvReturned;

        public Form3()
        {
            InitializeComponent();
            this.Load += Form3_Load;
            this.tabControlMain.SelectedIndexChanged += TabControlMain_SelectedIndexChanged;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            try
            {
                // prefer a local image named "lion.jpg" placed next to the executable
                var localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lion.jpg");
                if (File.Exists(localPath))
                {
                    pictureBoxLogo.Image = Image.FromFile(localPath);
                }
                else
                {
                    // fall back to loading from the web asynchronously
                    pictureBoxLogo.LoadAsync("https://upload.wikimedia.org/wikipedia/commons/7/73/Lion_waiting_in_Namibia.jpg");
                }
            }
            catch
            {
                // ignore failures — image will remain blank
            }

            // initialize data grids and load first tab
            InitializeDataGrids();
            LoadCurrentTabData();
            // apply theme
            ThemeManager.ApplyTheme(Library_app_test.Data.Repository.GetDarkMode());
        }

        private void TabControlMain_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LoadCurrentTabData();
        }

        private void InitializeDataGrids()
        {
            // available books grid
            dgvAvailable = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            tabAvailable.Controls.Add(dgvAvailable);

            // issued grid
            dgvIssued = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            tabIssued.Controls.Add(dgvIssued);

            // returned grid
            dgvReturned = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            tabReturned.Controls.Add(dgvReturned);
        }

        private void LoadCurrentTabData()
        {
            try
            {
                switch (tabControlMain.SelectedTab?.Text)
                {
                    case "Available Books":
                        LoadAvailableBooks();
                        break;
                    case "Issued Books":
                        LoadIssuedBooks();
                        break;
                    case "Returned Books":
                        LoadReturnedBooks();
                        break;
                    default:
                        // fallback: load available
                        LoadAvailableBooks();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAvailableBooks()
        {
            var list = Repository.GetAvailableBooks().ToList();
            dgvAvailable.DataSource = list.Select(b => new { b.Id, b.Title, b.ISBN, b.PublishedYear, b.Copies }).ToList();
            labelAvailableCount.Text = $"Available Books\n\n{list.Count}";
        }

        private void LoadIssuedBooks()
        {
            var list = Repository.GetIssuedIssues().ToList();
            dgvIssued.DataSource = list.Select(i => new { i.Id, i.BookTitle, i.MemberName, IssueDate = i.IssueDate.ToString("yyyy-MM-dd"), DueDate = i.DueDate.ToString("yyyy-MM-dd"), i.Status }).ToList();
            labelIssuedCount.Text = $"Issued Books\n\n{list.Count}";
        }

        private void LoadReturnedBooks()
        {
            var list = Repository.GetReturnedIssues().ToList();
            dgvReturned.DataSource = list.Select(i => new { i.Id, i.BookTitle, i.MemberName, IssueDate = i.IssueDate.ToString("yyyy-MM-dd"), ReturnDate = i.ReturnDate?.ToString("yyyy-MM-dd") }).ToList();
            labelReturnedCount.Text = $"Returned Books\n\n{list.Count}";
        }

        private void buttonDashboard_Click(object sender, EventArgs e)
        {
            // hide search when navigating away
            HideSearchPanel();
            labelMain.Text = "Dashboard";
        }

        private void buttonSearchCatalog_Click(object sender, EventArgs e)
        {
            labelMain.Text = "Search / Book Catalog";
            // show the centered search panel
            ShowSearchPanel();
            // ensure visible and focused
            searchPanel.BringToFront();
            textBoxSearch.Focus();
        }

        private void HideSearchPanel()
        {
            // hide and reset placeholder state
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Action)HideSearchPanel);
                return;
            }

            searchPanel.Visible = false;
            // remove accept button so Enter won't trigger search when panel is hidden
            this.AcceptButton = null;
            if (string.IsNullOrWhiteSpace(textBoxSearch.Text))
            {
                textBoxSearch.Text = "enter book name or id";
                textBoxSearch.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void ShowSearchPanel()
        {
            // position the searchPanel after layout to avoid timing issues
            panelMain.BeginInvoke((Action)(() =>
            {
                var x = Math.Max(10, (panelMain.ClientSize.Width - searchPanel.Width) / 2);
                var y = cardsContainer.Height + 20;
                searchPanel.Location = new System.Drawing.Point(x, y);
                // set placeholder if empty
                if (string.IsNullOrWhiteSpace(textBoxSearch.Text) || textBoxSearch.Text == "enter book name or id")
                {
                    textBoxSearch.Text = "enter book name or id";
                    textBoxSearch.ForeColor = System.Drawing.Color.Gray;
                }
                searchPanel.Visible = true;
                searchPanel.BringToFront();
                textBoxSearch.Focus();
                // make Enter trigger the search button while the panel is shown
                this.AcceptButton = buttonSearchGo;
            }));
        }

        private void textBoxSearch_GotFocus(object sender, EventArgs e)
        {
            if (textBoxSearch.Text == "enter book name or id")
            {
                textBoxSearch.Text = "";
                textBoxSearch.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            // clear placeholder on first key press if present
            if (textBoxSearch.Text == "enter book name or id")
            {
                textBoxSearch.Text = "";
                textBoxSearch.ForeColor = System.Drawing.Color.Black;
            }

            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                buttonSearchGo_Click(this, EventArgs.Empty);
            }
        }

        private void textBoxSearch_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxSearch.Text))
            {
                textBoxSearch.Text = "enter book name or id";
                textBoxSearch.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void buttonSearchGo_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBoxSearch == null)
                {
                    MessageBox.Show("Search box not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var query = textBoxSearch.Text?.Trim();
                if (string.IsNullOrWhiteSpace(query) || query == "enter book name or id")
                {
                    MessageBox.Show("Please enter a search term.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // try parse as id
                if (int.TryParse(query, out var id))
                {
                    // open catalog and highlight the id
                    using var frm = new BookCatalogForm(id, null);
                    frm.ShowDialog(this);
                }
                else
                {
                    // open catalog and attempt to highlight by title substring
                    using var frm = new BookCatalogForm(null, query);
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                // show full exception to help debugging
                MessageBox.Show($"Search failed: {ex}\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonAddBooks_Click(object sender, EventArgs e)
        {
            HideSearchPanel();
            labelMain.Text = "Add Books";
            try
            {
                LibraryDb.EnsureDatabase();
                using var frm = new BookCatalogForm();
                frm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Book Catalog: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonIssueBook_Click(object sender, EventArgs e)
        {
            HideSearchPanel();
            // ask for admin auth first
            using var auth = new AdminAuthForm();
            var dr = auth.ShowDialog(this);
            if (dr != DialogResult.OK) return;

            try
            {
                using var frm = new IssueForm();
                frm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Issue form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonReturnBook_Click(object sender, EventArgs e)
        {
            HideSearchPanel();
            try
            {
                using var frm = new ReturnForm();
                frm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Return form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonReports_Click(object sender, EventArgs e)
        {
            HideSearchPanel();
            try
            {
                using var frm = new ReportsForm();
                frm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Reports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonStudents_Click(object sender, EventArgs e)
        {
            HideSearchPanel();
            try
            {
                using var frm = new StudentsForm();
                frm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Students: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            HideSearchPanel();
            try
            {
                // ensure DB exists before opening settings (helps if Form3 was opened directly)
                Library_app_test.Data.LibraryDb.EnsureDatabase();
                using var frm = new SettingsForm();
                frm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Settings: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            HideSearchPanel();
            var result = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // user confirmed logout: show sign-in and close main form
                var signIn = new Form2();
                signIn.Show();
                this.Close();
            }
        }

        private void pictureBoxLogo_Click(object sender, EventArgs e)
        {

        }
    }
}
