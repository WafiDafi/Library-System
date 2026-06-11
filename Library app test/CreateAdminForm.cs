// first added on 2026/06/10
// simple form to create the initial admin account when none exist
using System;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public class CreateAdminForm : Form
    {
        private TextBox txtUser;
        private TextBox txtPass;
        private TextBox txtConfirm;
        private Button btnCreate;
        private Button btnCancel;

        public CreateAdminForm()
        {
            this.Text = "Create Administrator";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 420;
            this.Height = 260;
            this.MinimumSize = new System.Drawing.Size(380, 240);

            var lblInfo = new Label { Text = "No administrator account was found.\nCreate an administrator account to continue.", Left = 12, Top = 8, Width = 360, Height = 40 };
            var lblUser = new Label { Text = "Username:", Left = 12, Top = 60, AutoSize = true };
            txtUser = new TextBox { Left = 110, Top = 56, Width = 260 };
            var lblPass = new Label { Text = "Password:", Left = 12, Top = 96, AutoSize = true };
            txtPass = new TextBox { Left = 110, Top = 92, Width = 260, UseSystemPasswordChar = true };
            var lblConfirm = new Label { Text = "Confirm:", Left = 12, Top = 132, AutoSize = true };
            txtConfirm = new TextBox { Left = 110, Top = 128, Width = 260, UseSystemPasswordChar = true };

            btnCreate = new Button { Text = "Create", Left = 110, Top = 170, Width = 120, Height = 36 };
            btnCancel = new Button { Text = "Cancel", Left = 250, Top = 170, Width = 120, Height = 36 };

            btnCreate.Click += BtnCreate_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblInfo);
            this.Controls.Add(lblUser);
            this.Controls.Add(txtUser);
            this.Controls.Add(lblPass);
            this.Controls.Add(txtPass);
            this.Controls.Add(lblConfirm);
            this.Controls.Add(txtConfirm);
            this.Controls.Add(btnCreate);
            this.Controls.Add(btnCancel);
        }

        private void BtnCreate_Click(object? sender, EventArgs e)
        {
            var user = txtUser.Text?.Trim() ?? string.Empty;
            var pass = txtPass.Text ?? string.Empty;
            var conf = txtConfirm.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Username and password are required.", "Create Admin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (pass != conf)
            {
                MessageBox.Show("Password and confirmation do not match.", "Create Admin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // mark initial admin to require password change on first interactive sign-in
                Repository.AddAdmin(user, pass, mustChange: true);
                MessageBox.Show("Administrator account created.", "Create Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create admin: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
