using System;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public class PaymentForm : Form
    {
        private int memberId;
        private int? issueId;
        private TextBox txtAmount;
        private Button btnOk;

        public PaymentForm(int memberId, int? issueId = null)
        {
            this.memberId = memberId;
            this.issueId = issueId;
            this.Text = "Record Payment";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 300;
            this.Height = 160;

            var pnl = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 2, Padding = new Padding(8) };
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

            pnl.Controls.Add(new Label { Text = "Amount", AutoSize = true }, 0, 0);
            txtAmount = new TextBox { Dock = DockStyle.Fill };
            pnl.Controls.Add(txtAmount, 1, 0);

            btnOk = new Button { Text = "Pay", Dock = DockStyle.Right, Width = 80 };
            btnOk.Click += BtnOk_Click;
            pnl.Controls.Add(btnOk, 1, 1);

            this.Controls.Add(pnl);
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (!decimal.TryParse(txtAmount.Text.Trim(), out var amt) || amt <= 0)
            {
                MessageBox.Show("Enter a valid amount.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using var conn = LibraryDb.GetConnection();
                using var cmd = conn.CreateCommand();
                if (issueId.HasValue)
                {
                    cmd.CommandText = "INSERT INTO Payments (MemberId, Amount, PaidAt, IssueId) VALUES (@m,@a,@t,@iss)";
                    cmd.Parameters.AddWithValue("@m", memberId);
                    cmd.Parameters.AddWithValue("@a", amt);
                    cmd.Parameters.AddWithValue("@t", DateTime.UtcNow.ToString("o"));
                    cmd.Parameters.AddWithValue("@iss", issueId.Value);
                }
                else
                {
                    cmd.CommandText = "INSERT INTO Payments (MemberId, Amount, PaidAt) VALUES (@m,@a,@t)";
                    cmd.Parameters.AddWithValue("@m", memberId);
                    cmd.Parameters.AddWithValue("@a", amt);
                    cmd.Parameters.AddWithValue("@t", DateTime.UtcNow.ToString("o"));
                }
                cmd.ExecuteNonQuery();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to record payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
