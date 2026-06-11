// first modified in 18/04/20
//
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public class BookCatalogForm : Form
    {
        private DataGridView dgv;
        private int? highlightId;
        private string? highlightTitle;
        // internal model used for binding and filtering
        private record BookRow(int Id, string Title, string? ISBN, int? PublishedYear, int Copies);
        private List<BookRow> allBooks = new();
        private TextBox txtFilter;
        private Button btnFilter;
        private Button btnClear;
        public BookCatalogForm(int? highlightId = null, string? highlightTitle = null)
        {
            this.highlightId = highlightId;
            this.highlightTitle = highlightTitle;
            this.Text = "Book Catalog";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 1100;
            this.Height = 700;
            this.MinimumSize = new Size(900, 600);

            // top filter panel
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(6) };
            txtFilter = new TextBox { Left = 6, Top = 10, Width = 420, Height = 26, Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right };
            // ensure search box is visible on dark background immediately
            txtFilter.BackColor = Color.Black;
            txtFilter.ForeColor = Color.White;
            btnFilter = new Button { Text = "Filter", Left = 440, Top = 10, Width = 80, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnClear = new Button { Text = "Clear", Left = 528, Top = 10, Width = 80, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnFilter.Click += (s, e) => ApplyFilter();
            btnClear.Click += (s, e) => { txtFilter.Text = string.Empty; ApplyFilter(); };
            txtFilter.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.Handled = true; e.SuppressKeyPress = true; ApplyFilter(); } };
            // live filter as you type
            txtFilter.TextChanged += (s, e) => { ApplyFilter(); ApplyFilterTextBoxTheme(); };
            // ensure colors update when focus changes (some themes can change focus visuals)
            txtFilter.GotFocus += (s, e) => ApplyFilterTextBoxTheme();
            txtFilter.LostFocus += (s, e) => ApplyFilterTextBoxTheme();
            pnlTop.Controls.Add(txtFilter);
            pnlTop.Controls.Add(btnFilter);
            pnlTop.Controls.Add(btnClear);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            };

            // create columns: Id, Title, ISBN, PublishedYear, Copies
            var colId = new DataGridViewTextBoxColumn { Name = "Id", DataPropertyName = "Id", HeaderText = "Id" };
            var colTitle = new DataGridViewTextBoxColumn { Name = "Title", DataPropertyName = "Title", HeaderText = "Title" };
            var colIsbn = new DataGridViewTextBoxColumn { Name = "ISBN", DataPropertyName = "ISBN", HeaderText = "ISBN" };
            var colYear = new DataGridViewTextBoxColumn { Name = "PublishedYear", DataPropertyName = "PublishedYear", HeaderText = "PublishedYear" };
            var colCopies = new DataGridViewTextBoxColumn { Name = "Copies", DataPropertyName = "Copies", HeaderText = "Copies" };
            // set sensible minimum widths so columns are readable
            colId.MinimumWidth = 50;
            colTitle.MinimumWidth = 300;
            colIsbn.MinimumWidth = 180;
            colYear.MinimumWidth = 90;
            colCopies.MinimumWidth = 70;

            dgv.Columns.AddRange(new DataGridViewColumn[] { colId, colTitle, colIsbn, colYear, colCopies });

            // bottom area: back button on top, action buttons under it
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 96, Padding = new Padding(6) };
            var btnBack = new Button { Text = "Back", Dock = DockStyle.Top, Height = 36 };
            btnBack.Click += (s, e) => this.Close();

            var actionPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 52, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(6) };
            var btnAdd = new Button { Text = "Add Book", Width = 120, Height = 36 }; 
            var btnDelete = new Button { Text = "Delete Book", Width = 120, Height = 36 };
            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            actionPanel.Controls.Add(btnAdd);
            actionPanel.Controls.Add(btnDelete);

            bottomPanel.Controls.Add(actionPanel);
            bottomPanel.Controls.Add(btnBack);

            // add controls in order: top panel, grid, bottom panel
            this.Controls.Add(dgv);
            this.Controls.Add(pnlTop);
            this.Controls.Add(bottomPanel);

            this.Load += BookCatalogForm_Load;
        }

        private void RefreshCatalog(int? highlightId = null, string? highlightTitle = null)
        {
            try
            {
                LibraryDb.EnsureDatabase();
                var list = Repository.GetAllBooks().Select(b => new BookRow(b.Id, b.Title, b.ISBN, b.PublishedYear, b.Copies)).ToList();
                allBooks = list;
                dgv.DataSource = allBooks.ToList();

                // ensure columns resize to fit content
                try { dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells); } catch { }
                this.highlightId = highlightId;
                this.highlightTitle = highlightTitle;
                ApplyFilter();
            }
            catch
            {
                // ignore
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                using var frm = new AddBookForm();
                if (frm.ShowDialog(this) != DialogResult.OK) return;

                // create unique isbn and add
                var isbn = Repository.GenerateUniqueIsbn();
                var id = Repository.AddBook(frm.TitleText, null, isbn, frm.PublishedYear, frm.Copies);
                MessageBox.Show($"Book added with Id {id} and ISBN {isbn}", "Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshCatalog(id, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add book: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            try
            {
                using var idPrompt = new IdPromptForm("Enter Book Id to delete");
                if (idPrompt.ShowDialog(this) != DialogResult.OK) return;
                var id = idPrompt.ParsedId;

                using var auth = new AdminAuthForm();
                if (auth.ShowDialog(this) != DialogResult.OK) return;

                var ok = Repository.DeleteBook(id);
                if (!ok)
                {
                    MessageBox.Show($"No book deleted. Check the Id or permissions.", "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"Book {id} deleted.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshCatalog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete book: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BookCatalogForm_Load(object? sender, EventArgs e)
        {
            try
            {
                LibraryDb.EnsureDatabase();
                var list = Repository.GetAllBooks().Select(b => new BookRow(b.Id, b.Title, b.ISBN, b.PublishedYear, b.Copies)).ToList();
                allBooks = list;
                dgv.DataSource = allBooks.ToList();

                // highlight a matching row if requested
                if (highlightId.HasValue || !string.IsNullOrWhiteSpace(highlightTitle))
                {
                    for (int i = 0; i < dgv.Rows.Count; i++)
                    {
                        var row = dgv.Rows[i];
                        try
                        {
                            if (highlightId.HasValue)
                            {
                                var cell = row.Cells["Id"].Value;
                                if (cell != null && int.TryParse(cell.ToString(), out var rid) && rid == highlightId.Value)
                                {
                                    row.Selected = true;
                                    dgv.CurrentCell = row.Cells[0];
                                    dgv.FirstDisplayedScrollingRowIndex = i;
                                    break;
                                }
                            }
                            else if (!string.IsNullOrWhiteSpace(highlightTitle))
                            {
                                var cell = row.Cells["Title"].Value?.ToString();
                                if (!string.IsNullOrWhiteSpace(cell) && cell.IndexOf(highlightTitle, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    row.Selected = true;
                                    dgv.CurrentCell = row.Cells[0];
                                    dgv.FirstDisplayedScrollingRowIndex = i;
                                    break;
                                }
                            }
                        }
                        catch
                        {
                            // ignore row-level errors
                        }
                    }
                }

                // wire double click to open details
                dgv.CellDoubleClick += Dgv_CellDoubleClick;
                // initial empty filter
                ApplyFilter();

                // ensure filter textbox colors follow theme
                ApplyFilterTextBoxTheme();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load catalog: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilter()
        {
            try
            {
                if (allBooks == null || allBooks.Count == 0)
                {
                    dgv.DataSource = new List<BookRow>();
                    return;
                }

                var term = txtFilter.Text?.Trim();
                // ensure grid shows full dataset
                dgv.DataSource = allBooks.ToList();

                if (string.IsNullOrWhiteSpace(term))
                {
                    dgv.ClearSelection();
                    return;
                }

                // prepare lower term
                var lowered = term.ToLowerInvariant();

                // iterate rows and select matches (multi-select)
                int? firstIndex = null;
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    var row = dgv.Rows[i];
                    var match = false;
                    try
                    {
                        var idCell = row.Cells["Id"].Value;
                        var titleCell = row.Cells["Title"].Value?.ToString() ?? string.Empty;
                        var isbnCell = row.Cells["ISBN"].Value?.ToString() ?? string.Empty;

                        if (int.TryParse(term, out var id))
                        {
                            if (idCell != null && int.TryParse(idCell.ToString(), out var rid) && rid == id)
                                match = true;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(titleCell) && titleCell.ToLowerInvariant().Contains(lowered)) match = true;
                            if (!match && !string.IsNullOrWhiteSpace(isbnCell) && isbnCell.ToLowerInvariant().Contains(lowered)) match = true;
                        }
                    }
                    catch { }

                    row.Selected = match;
                    if (match && firstIndex == null) firstIndex = i;
                }

                if (firstIndex.HasValue)
                {
                    try { dgv.FirstDisplayedScrollingRowIndex = firstIndex.Value; } catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Filter failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Dgv_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            try
            {
                var idCell = dgv.Rows[e.RowIndex].Cells["Id"].Value;
                if (idCell == null) return;
                if (!int.TryParse(idCell.ToString(), out var id)) return;
                using var frm = new BookDetailsForm(id);
                frm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilterTextBoxTheme()
        {
            try
            {
                var dark = Library_app_test.Data.Repository.GetDarkMode();
                if (dark)
                {
                    txtFilter.ForeColor = Color.White;
                    txtFilter.BackColor = Color.FromArgb(30, 30, 30);
                }
                else
                {
                    txtFilter.ForeColor = Color.Black;
                    txtFilter.BackColor = Color.White;
                }
            }
            catch
            {
                // ignore theme failures
            }
        }
    }
}
