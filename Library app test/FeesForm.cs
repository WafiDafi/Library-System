using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public class FeesForm : Form
    {
        private DataGridView dgv;
        private Button btnPay;
        private Button btnPrint;

        public FeesForm()
        {
            this.Text = "Fees Management";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 900;
            this.Height = 600;

            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            btnPay = new Button { Text = "Pay Selected Issue", Dock = DockStyle.Bottom, Height = 36 };
            btnPrint = new Button { Text = "Print Receipt", Dock = DockStyle.Bottom, Height = 36 };
            btnPay.Click += BtnPay_Click;
            btnPrint.Click += BtnPrint_Click;

            this.Controls.Add(dgv);
            this.Controls.Add(btnPrint);
            this.Controls.Add(btnPay);

            this.Load += FeesForm_Load;
        }

        private void FeesForm_Load(object? sender, EventArgs e)
        {
            LoadFees();
        }

        private void LoadFees()
        {
            try
            {
                LibraryDb.EnsureDatabase();
                using var conn = LibraryDb.GetConnection();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT i.Id AS IssueId, b.Title AS BookTitle, m.Id AS MemberId, m.FullName, i.IssueDate, i.DueDate, i.ReturnDate
FROM Issues i
JOIN Books b ON b.Id = i.BookId
JOIN Members m ON m.Id = i.MemberId
WHERE i.ReturnDate IS NOT NULL
ORDER BY i.IssueDate DESC";
                using var rdr = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(rdr);
                dt.Columns.Add("DaysOverdue", typeof(int));
                dt.Columns.Add("Fee", typeof(decimal));
                dt.Columns.Add("Paid", typeof(decimal));
                dt.Columns.Add("Unpaid", typeof(decimal));

                foreach (DataRow row in dt.Rows)
                {
                    DateTime due = DateTime.Parse(row["DueDate"].ToString()!);
                    DateTime ret = DateTime.Parse(row["ReturnDate"].ToString()!);
                    int days = 0;
                    if (ret.Date > due.Date) days = (ret.Date - due.Date).Days;
                    row["DaysOverdue"] = days;
                    var fee = days * 1m;
                    row["Fee"] = fee;

                    using var pcmd = conn.CreateCommand();
                    pcmd.CommandText = "SELECT COALESCE(SUM(Amount),0) FROM Payments WHERE IssueId = @iss";
                    pcmd.Parameters.AddWithValue("@iss", row["IssueId"]);
                    var paid = Convert.ToDecimal(pcmd.ExecuteScalar());
                    row["Paid"] = paid;
                    row["Unpaid"] = Math.Max(0, fee - paid);
                }

                // show only rows with unpaid > 0
                var rows = dt.Select("Unpaid > 0");
                if (rows.Length > 0)
                {
                    var newDt = dt.Clone();
                    foreach (var r in rows) newDt.ImportRow(r);
                    dgv.DataSource = newDt;
                }
                else
                {
                    dgv.DataSource = null;
                }
            }
            catch (InvalidOperationException)
            {
                // no rows
                dgv.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load fees: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPay_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select an issue to pay.", "Pay", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var issueId = Convert.ToInt32(dgv.SelectedRows[0].Cells["IssueId"].Value);
            var memberId = Convert.ToInt32(dgv.SelectedRows[0].Cells["MemberId"].Value);
            using var dlg = new PaymentForm(memberId, issueId);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                LoadFees();
            }
        }

        private void BtnPrint_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a payment/issue to print.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var issueId = Convert.ToInt32(dgv.SelectedRows[0].Cells["IssueId"].Value);
            // simple receipt print of issue/payment summary
            try
            {
                using var conn = LibraryDb.GetConnection();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT i.Id AS IssueId, b.Title AS BookTitle, m.FullName, i.IssueDate, i.DueDate, i.ReturnDate
FROM Issues i
JOIN Books b ON b.Id = i.BookId
JOIN Members m ON m.Id = i.MemberId
WHERE i.Id = @id";
                cmd.Parameters.AddWithValue("@id", issueId);
                using var rdr = cmd.ExecuteReader();
                if (!rdr.Read()) return;
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"Issue ID: {rdr.GetInt32(0)}");
                sb.AppendLine($"Book: {rdr.GetString(1)}");
                sb.AppendLine($"Member: {rdr.GetString(2)}");
                sb.AppendLine($"IssueDate: {rdr.GetString(3)}");
                sb.AppendLine($"DueDate: {rdr.GetString(4)}");
                sb.AppendLine("ReturnDate: " + (rdr.IsDBNull(5) ? "" : rdr.GetString(5)));

                using var pd = new PrintPreviewDialog();
                var doc = new System.Drawing.Printing.PrintDocument();
                doc.PrintPage += (s, ev) =>
                {
                    var font = new Font("Segoe UI", 10);
                    ev.Graphics.DrawString(sb.ToString(), font, Brushes.Black, new PointF(10, 10));
                };
                pd.Document = doc;
                pd.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Print failed: {ex.Message}", "Print", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
