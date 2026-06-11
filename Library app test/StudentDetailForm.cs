using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public class StudentDetailForm : Form
    {
        private int memberId;
        private Label lblInfo;
        private DataGridView dgvIssues;
        private Button btnPrint;
        private Button btnPay;

        public StudentDetailForm(int memberId)
        {
            this.memberId = memberId;
            this.Text = "Student Details";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 800;
            this.Height = 600;

            lblInfo = new Label { Dock = DockStyle.Top, Height = 80, Font = new Font("Segoe UI", 10F), Padding = new Padding(8) };
            dgvIssues = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells };
            dgvIssues.CellDoubleClick += DgvIssues_CellDoubleClick;

            var pnl = new Panel { Dock = DockStyle.Bottom, Height = 40 };
            btnPay = new Button { Text = "Pay Fee", Dock = DockStyle.Right, Width = 120 };
            btnPrint = new Button { Text = "Print", Dock = DockStyle.Right, Width = 120 };
            btnPay.Click += BtnPay_Click;
            btnPrint.Click += BtnPrint_Click;
            pnl.Controls.Add(btnPay);
            pnl.Controls.Add(btnPrint);

            this.Controls.Add(dgvIssues);
            this.Controls.Add(lblInfo);
            this.Controls.Add(pnl);

            this.Load += StudentDetailForm_Load;
        }

        private void DgvIssues_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                {
                    MessageBox.Show("no student clicked", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var row = dgvIssues.Rows[e.RowIndex];
                if (row == null || row.Cells.Count == 0) { MessageBox.Show("no student clicked", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                var issueIdObj = row.Cells["Id"].Value;
                if (issueIdObj == null || issueIdObj == DBNull.Value)
                {
                    MessageBox.Show("no student clicked", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var issueId = Convert.ToInt32(issueIdObj);
                using var dlg = new PaymentForm(memberId, issueId);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LoadDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StudentDetailForm_Load(object? sender, EventArgs e)
        {
            LoadDetails();
        }

        private void LoadDetails()
        {
            try
            {
                using var conn = LibraryDb.GetConnection();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT Id, FullName, Course, IdNumber, Email, Phone, COALESCE(OverduesCount,0) AS OverduesCount FROM Members WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", memberId);
                using var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    var name = rdr.GetString(1);
                    var course = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
                    var idnum = rdr.IsDBNull(3) ? "" : rdr.GetString(3);
                    var overdues = rdr.GetInt32(6);
                    lblInfo.Text = $"Name: {name}\nCourse: {course}\nID: {idnum}\nOverdues count: {overdues}";
                }

                // load issues and compute fees
                using var icmd = conn.CreateCommand();
                icmd.CommandText = @"SELECT i.Id, b.Title AS BookTitle, i.IssueDate, i.DueDate, i.ReturnDate
FROM Issues i JOIN Books b ON b.Id = i.BookId
WHERE i.MemberId = @mid ORDER BY i.IssueDate DESC";
                icmd.Parameters.AddWithValue("@mid", memberId);
                using var ir = icmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(ir);
                // add computed columns
                dt.Columns.Add("DaysOverdue", typeof(int));
                dt.Columns.Add("FeeDue", typeof(decimal));

                foreach (DataRow row in dt.Rows)
                {
                    DateTime due = DateTime.Parse(row["DueDate"].ToString()!);
                    DateTime? ret = row["ReturnDate"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(row["ReturnDate"].ToString()!);
                    int days = 0;
                    if (ret == null)
                    {
                        if (DateTime.UtcNow.Date > due.Date)
                        {
                            days = (DateTime.UtcNow.Date - due.Date).Days;
                        }
                    }
                    else
                    {
                        if (ret.Value.Date > due.Date)
                        {
                            days = (ret.Value.Date - due.Date).Days;
                        }
                    }
                    row["DaysOverdue"] = days;
                    row["FeeDue"] = days * 1m; // 1 peso per day
                }

                // compute payments total
                using var pcmd = conn.CreateCommand();
                pcmd.CommandText = "SELECT COALESCE(SUM(Amount),0) FROM Payments WHERE MemberId = @mid";
                pcmd.Parameters.AddWithValue("@mid", memberId);
                var paid = Convert.ToDecimal(pcmd.ExecuteScalar());

                // add summary row to top
                var summaryTable = dt.Copy();
                dgvIssues.DataSource = dt;
                // show unpaid total in the form title or below
                decimal totalDue = dt.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["FeeDue"]));
                decimal unpaid = Math.Max(0, totalDue - paid);
                lblInfo.Text += $"\nTotal fees due: {totalDue:0.00}  |  Paid: {paid:0.00}  |  Unpaid: {unpaid:0.00}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPay_Click(object? sender, EventArgs e)
        {
            using var dlg = new PaymentForm(memberId);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                LoadDetails();
            }
        }

        private void BtnPrint_Click(object? sender, EventArgs e)
        {
            try
            {
                using var pd = new PrintPreviewDialog();
                var doc = new System.Drawing.Printing.PrintDocument();
                doc.PrintPage += (s, ev) =>
                {
                    var font = new Font("Segoe UI", 10);
                    ev.Graphics.DrawString(lblInfo.Text, font, Brushes.Black, new PointF(10, 10));
                    int y = 120;
                    var dgv = dgvIssues;
                    // print header
                    for (int c = 0; c < dgv.Columns.Count; c++)
                    {
                        ev.Graphics.DrawString(dgv.Columns[c].HeaderText, font, Brushes.Black, new PointF(10 + c * 150, y));
                    }
                    y += 24;
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        for (int c = 0; c < dgv.Columns.Count; c++)
                        {
                            var txt = row.Cells[c].Value?.ToString() ?? "";
                            ev.Graphics.DrawString(txt, font, Brushes.Black, new PointF(10 + c * 150, y));
                        }
                        y += 20;
                    }
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
