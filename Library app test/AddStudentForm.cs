using System;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public class AddStudentForm : Form
    {
        private TextBox txtName;
        private TextBox txtCourse;
        private TextBox txtIdNumber;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private Button btnOk;

        public AddStudentForm()
        {
            this.Text = "Add Student";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 400;
            this.Height = 300;

            var pnl = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8), RowCount = 6, ColumnCount = 2 };
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            pnl.Controls.Add(new Label { Text = "Full Name", AutoSize = true }, 0, 0);
            txtName = new TextBox { Dock = DockStyle.Fill };
            pnl.Controls.Add(txtName, 1, 0);

            pnl.Controls.Add(new Label { Text = "Course", AutoSize = true }, 0, 1);
            txtCourse = new TextBox { Dock = DockStyle.Fill };
            pnl.Controls.Add(txtCourse, 1, 1);

            pnl.Controls.Add(new Label { Text = "ID Number", AutoSize = true }, 0, 2);
            txtIdNumber = new TextBox { Dock = DockStyle.Fill };
            pnl.Controls.Add(txtIdNumber, 1, 2);

            pnl.Controls.Add(new Label { Text = "Email", AutoSize = true }, 0, 3);
            txtEmail = new TextBox { Dock = DockStyle.Fill };
            pnl.Controls.Add(txtEmail, 1, 3);

            pnl.Controls.Add(new Label { Text = "Phone", AutoSize = true }, 0, 4);
            txtPhone = new TextBox { Dock = DockStyle.Fill };
            pnl.Controls.Add(txtPhone, 1, 4);

            btnOk = new Button { Text = "OK", Dock = DockStyle.Fill };
            btnOk.Click += BtnOk_Click;
            pnl.Controls.Add(btnOk, 1, 5);

            this.Controls.Add(pnl);
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Enter student name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var conn = LibraryDb.GetConnection();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO Members (FullName, Email, Phone, Course, IdNumber, OverduesCount, PrevBorrowedCount) VALUES (@n,@e,@p,@c,@idn,0,0)";
                cmd.Parameters.AddWithValue("@n", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@e", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@p", txtPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@c", txtCourse.Text.Trim());
                cmd.Parameters.AddWithValue("@idn", txtIdNumber.Text.Trim());
                cmd.ExecuteNonQuery();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add student: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
