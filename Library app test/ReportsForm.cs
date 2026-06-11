// first modified in 18/04/20
//
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using Library_app_test.Data;

namespace Library_app_test
{
    public class ReportsForm : Form
    {
        private TabControl tabs;
        private TabPage tabSummary;
        private TabPage tabOverdue;
        private TabPage tabStudents;
        private TabPage tabOngoing;
        private TabPage tabTransactions;
        private Label lblSummary;
        private DataGridView dgvOverdue;
        private Button btnExportOverdue;
        private Button btnClose;
        private DataGridView dgvStudents;
        private DataGridView dgvOngoing;
        private DataGridView dgvTransactions;
        private Button btnTransToday;
        private Button btnTransPrint;

        public ReportsForm()
        {
            this.Text = "Reports";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 1100;
            this.Height = 700;
            this.MinimumSize = new System.Drawing.Size(900, 600);

            tabs = new TabControl { Dock = DockStyle.Fill };
            tabSummary = new TabPage("Summary");
            tabOverdue = new TabPage("Overdue");
            tabStudents = new TabPage("Students");
            tabOngoing = new TabPage("Ongoing Borrowed");
            tabTransactions = new TabPage("Transactions");

            lblSummary = new Label { Dock = DockStyle.Fill, Font = new System.Drawing.Font("Segoe UI", 10F), TextAlign = System.Drawing.ContentAlignment.TopLeft, Padding = new Padding(12) };

            dgvOverdue = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells };
            btnExportOverdue = new Button { Text = "Export Overdue CSV", Dock = DockStyle.Bottom, Height = 36 };
            btnExportOverdue.Click += BtnExportOverdue_Click;
            var btnFees = new Button { Text = "Fees", Dock = DockStyle.Bottom, Height = 36 };
            btnFees.Click += (s, e) =>
            {
                try
                {
                    using var f = new FeesForm();
                    f.ShowDialog(this);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to open Fees: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            dgvStudents = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells };
            dgvStudents.CellDoubleClick += DgvStudents_CellDoubleClick;

            dgvOngoing = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells };

            dgvTransactions = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells };
            btnTransToday = new Button { Text = "Today", Dock = DockStyle.Bottom, Height = 30 };
            btnTransPrint = new Button { Text = "Print", Dock = DockStyle.Bottom, Height = 30 };
            btnTransToday.Click += (s, e) => LoadTransactions(DateTime.UtcNow.Date);
            btnTransPrint.Click += BtnTransPrint_Click;


            btnClose = new Button { Text = "Close", Dock = DockStyle.Bottom, Height = 36 };
            btnClose.Click += (s, e) => this.Close();

            tabSummary.Controls.Add(lblSummary);

            tabOverdue.Controls.Add(dgvOverdue);
            tabOverdue.Controls.Add(btnExportOverdue);
            tabOverdue.Controls.Add(btnFees);
            tabStudents.Controls.Add(dgvStudents);
            tabOngoing.Controls.Add(dgvOngoing);
            tabTransactions.Controls.Add(dgvTransactions);
            tabTransactions.Controls.Add(btnTransPrint);
            tabTransactions.Controls.Add(btnTransToday);

            tabs.TabPages.Add(tabSummary);
            tabs.TabPages.Add(tabOverdue);
            tabs.TabPages.Add(tabStudents);
            tabs.TabPages.Add(tabOngoing);
            tabs.TabPages.Add(tabTransactions);

            this.Controls.Add(tabs);
            this.Controls.Add(btnClose);

            this.Load += ReportsForm_Load;
        }

        private void ReportsForm_Load(object? sender, EventArgs e)
        {
            try
            {
                LibraryDb.EnsureDatabase();
                LoadSummary();
                LoadOverdue();
                LoadStudents();
                LoadOngoing();
                LoadTransactions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load reports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStudents()
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT m.Id, m.FullName, m.Course, m.IdNumber, m.Email, COALESCE(m.OverduesCount,0) AS OverduesCount, COALESCE(m.PrevBorrowedCount,0) AS PrevBorrowedCount,
 (SELECT COUNT(*) FROM Issues i WHERE i.MemberId = m.Id AND i.ReturnDate IS NULL) AS CurrentlyBorrowed
FROM Members m ORDER BY m.FullName";
            using var rdr = cmd.ExecuteReader();
            var dt = new System.Data.DataTable();
            dt.Load(rdr);
            dgvStudents.DataSource = dt;
            try { dgvStudents.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells); } catch { }
        }

        private void LoadOngoing()
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT i.Id, b.Title AS BookTitle, m.Id AS MemberId, m.FullName, i.IssueDate, i.DueDate
FROM Issues i
JOIN Books b ON b.Id = i.BookId
JOIN Members m ON m.Id = i.MemberId
WHERE i.ReturnDate IS NULL
ORDER BY i.IssueDate DESC";
            using var rdr = cmd.ExecuteReader();
            var dt = new System.Data.DataTable();
            dt.Load(rdr);
            dgvOngoing.DataSource = dt;
            try { dgvOngoing.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells); } catch { }
        }

        private void LoadTransactions(DateTime? onDate = null)
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT i.Id AS IssueId, b.Id AS BookId, b.Title AS BookTitle, m.Id AS MemberId, m.FullName AS MemberName, i.IssueDate, i.DueDate, i.ReturnDate,
CASE WHEN i.ReturnDate IS NULL THEN 'Borrowed' ELSE 'Returned' END AS Status
FROM Issues i
JOIN Books b ON b.Id = i.BookId
JOIN Members m ON m.Id = i.MemberId
ORDER BY i.IssueDate DESC";
            using var rdr = cmd.ExecuteReader();
            var dt = new System.Data.DataTable();
            dt.Load(rdr);

            if (onDate.HasValue)
            {
                // filter rows where IssueDate or ReturnDate is on the requested date (UTC date portion)
                var rows = dt.AsEnumerable().Where(r =>
                {
                    DateTime.TryParse(r.Field<string>("IssueDate"), out var idt);
                    DateTime? rdt = null;
                    if (!r.IsNull("ReturnDate"))
                    {
                        if (DateTime.TryParse(r.Field<string>("ReturnDate"), out var tmp))
                        {
                            rdt = tmp;
                        }
                    }
                    return idt.Date == onDate.Value.Date || (rdt.HasValue && rdt.Value.Date == onDate.Value.Date);
                }).ToArray();

                if (rows.Length > 0)
                {
                    var ndt = dt.Clone();
                    foreach (var r in rows) ndt.ImportRow(r);
                    dgvTransactions.DataSource = ndt;
                }
                else
                {
                    dgvTransactions.DataSource = null;
                }
            }
            else
            {
                dgvTransactions.DataSource = dt;
            }

            try { dgvTransactions.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells); } catch { }
        }

        private void DgvStudents_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                {
                    MessageBox.Show("no student clicked", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var dt = dgvStudents.DataSource as System.Data.DataTable;
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("no student clicked", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var idObj = dt.Rows[e.RowIndex]["Id"];
                if (idObj == null || idObj == DBNull.Value)
                {
                    MessageBox.Show("no student clicked", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var id = Convert.ToInt32(idObj);
                using var frm = new StudentDetailForm(id);
                frm.ShowDialog(this);
                LoadStudents();
                LoadOngoing();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open student details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTransPrint_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvTransactions.DataSource == null)
                {
                    MessageBox.Show("No transactions to print.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var dt = dgvTransactions.DataSource as DataTable;
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No transactions to print.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var pd = new PrintPreviewDialog();
                var doc = new PrintDocument();
                // capture the datatable locally for the print handler
                var printTable = dt.Copy();
                doc.PrintPage += (s, ev) =>
                {
                    var font = new Font("Segoe UI", 9);
                    float x = ev.MarginBounds.Left;
                    float y = ev.MarginBounds.Top;
                    // header
                    ev.Graphics.DrawString("Transactions Report", new Font("Segoe UI", 12, FontStyle.Bold), Brushes.Black, x, y);
                    y += 30;

                    // draw column headers
                    for (int c = 0; c < printTable.Columns.Count; c++)
                    {
                        ev.Graphics.DrawString(printTable.Columns[c].ColumnName, font, Brushes.Black, x + c * 150, y);
                    }
                    y += 22;

                    // draw rows
                    foreach (DataRow row in printTable.Rows)
                    {
                        for (int c = 0; c < printTable.Columns.Count; c++)
                        {
                            var txt = row[c]?.ToString() ?? "";
                            ev.Graphics.DrawString(txt, font, Brushes.Black, x + c * 150, y);
                        }
                        y += 18;
                        if (y > ev.MarginBounds.Bottom - 40)
                        {
                            ev.HasMorePages = true;
                            return;
                        }
                    }
                    ev.HasMorePages = false;
                };

                pd.Document = doc;
                pd.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Print failed: {ex.Message}", "Print", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSummary()
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();

            // total books
            cmd.CommandText = "SELECT COUNT(*) FROM Books";
            var totalBooks = Convert.ToInt32(cmd.ExecuteScalar());

            cmd.CommandText = "SELECT COUNT(*) FROM Books WHERE Copies > 0";
            var available = Convert.ToInt32(cmd.ExecuteScalar());

            cmd.CommandText = "SELECT COUNT(*) FROM Issues WHERE ReturnDate IS NULL";
            var issued = Convert.ToInt32(cmd.ExecuteScalar());

            cmd.CommandText = "SELECT COUNT(*) FROM Issues WHERE ReturnDate IS NOT NULL";
            var returned = Convert.ToInt32(cmd.ExecuteScalar());

            cmd.CommandText = "SELECT COUNT(*) FROM Issues WHERE ReturnDate IS NULL AND DueDate < @now";
            cmd.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("o"));
            var overdue = Convert.ToInt32(cmd.ExecuteScalar());

            var sb = new StringBuilder();
            sb.AppendLine($"Total books: {totalBooks}");
            sb.AppendLine($"Available books: {available}");
            sb.AppendLine($"Currently issued: {issued}");
            sb.AppendLine($"Returned records: {returned}");
            sb.AppendLine($"Overdue: {overdue}");

            lblSummary.Text = sb.ToString();
        }

        private void LoadOverdue()
        {
            using var conn = LibraryDb.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT i.Id, b.Id as BookId, b.Title, m.Id as MemberId, m.FullName, i.IssueDate, i.DueDate
FROM Issues i
JOIN Books b ON b.Id = i.BookId
JOIN Members m ON m.Id = i.MemberId
WHERE i.ReturnDate IS NULL AND i.DueDate < @now
ORDER BY i.DueDate ASC";
            cmd.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("o"));

            using var rdr = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(rdr);
            dgvOverdue.DataSource = dt;
            try { dgvOverdue.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells); } catch { }
        }

        private void BtnExportOverdue_Click(object? sender, EventArgs e)
        {
            if (dgvOverdue.DataSource == null)
            {
                MessageBox.Show("No data to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dlg = new SaveFileDialog { Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*", FileName = "overdue.csv" };
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                var dt = (DataTable)dgvOverdue.DataSource;
                using var sw = new StreamWriter(dlg.FileName, false, System.Text.Encoding.UTF8);
                // header
                sw.WriteLine(string.Join(',', dt.Columns.Cast<DataColumn>().Select(c => EscapeCsv(c.ColumnName))));
                foreach (DataRow row in dt.Rows)
                {
                    sw.WriteLine(string.Join(',', dt.Columns.Cast<DataColumn>().Select(c => EscapeCsv(row[c]?.ToString() ?? ""))));
                }
                MessageBox.Show("Exported successfully.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string EscapeCsv(string s)
        {
            if (s.Contains(',') || s.Contains('"') || s.Contains('\n'))
            {
                return '"' + s.Replace("\"", "\"\"") + '"';
            }
            return s;
        }
    }
}
