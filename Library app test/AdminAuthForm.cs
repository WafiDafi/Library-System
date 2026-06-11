// first modified in 18/04/20
//
using System;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public class AdminAuthForm : Form
    {
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnOk;
        private Button btnCancel;
        public (int Id, string Username, bool IsBuiltin, bool MustChange)? Admin { get; private set; }

        public AdminAuthForm()
        {
            this.Text = "Admin Authentication";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 420;
            this.Height = 240;
            this.MinimumSize = new System.Drawing.Size(380, 220);

            // Big title label for sign-in
            var lblTitle = new Label { Text = "Library System", Left = 12, Top = 8, AutoSize = false };
            lblTitle.Width = 380;
            lblTitle.Height = 40;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Font = new System.Drawing.Font(lblTitle.Font.FontFamily, 14F, System.Drawing.FontStyle.Bold);

            var lblUser = new Label { Text = "Username:", Left = 12, Top = 56, AutoSize = true };
            txtUser = new TextBox { Left = 110, Top = 52, Width = 280 };
            var lblPass = new Label { Text = "Password:", Left = 12, Top = 92, AutoSize = true };
            txtPass = new TextBox { Left = 110, Top = 88, Width = 280, UseSystemPasswordChar = true };

            btnOk = new Button { Text = "OK", Left = 110, Top = 132, Width = 120, Height = 36 };
            btnCancel = new Button { Text = "Cancel", Left = 250, Top = 132, Width = 120, Height = 36 };

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUser);
            this.Controls.Add(txtUser);
            this.Controls.Add(lblPass);
            this.Controls.Add(txtPass);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            try
            {
                var admin = Repository.GetAdminByCredentials(txtUser.Text.Trim(), txtPass.Text);
                if (admin == null)
                {
                    MessageBox.Show("Invalid admin credentials.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (admin.Value.MustChange)
                {
                    MessageBox.Show("This account requires a password change before it can be used. Please sign in to change your password.", "Change Password Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                this.Admin = admin;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to authenticate: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
