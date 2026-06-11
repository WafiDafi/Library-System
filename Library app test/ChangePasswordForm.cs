// added on 2026/06/10
using System;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public class ChangePasswordForm : Form
    {
        private readonly int adminId;
        private readonly string username;
        private readonly bool isLegacyCreated;
        private TextBox txtNew;
        private TextBox txtConfirm;
        private Button btnOk;
        private Button btnCancel;

        public ChangePasswordForm(int adminId, string username, bool isLegacyCreated = false)
        {
            this.adminId = adminId;
            this.username = username;
            this.isLegacyCreated = isLegacyCreated;
            this.Text = "Change Password";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 420;
            this.Height = 220;

            var lblInfo = new Label { Text = $"Change password for '{username}'", Left = 12, Top = 8, Width = 360, Height = 24 };
            var lblNew = new Label { Text = "New Password:", Left = 12, Top = 44, AutoSize = true };
            txtNew = new TextBox { Left = 140, Top = 40, Width = 240, UseSystemPasswordChar = true };
            var lblConfirm = new Label { Text = "Confirm:", Left = 12, Top = 80, AutoSize = true };
            txtConfirm = new TextBox { Left = 140, Top = 76, Width = 240, UseSystemPasswordChar = true };

            btnOk = new Button { Text = "Change", Left = 140, Top = 116, Width = 120, Height = 36 };
            btnCancel = new Button { Text = "Cancel", Left = 280, Top = 116, Width = 100, Height = 36 };

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblInfo);
            this.Controls.Add(lblNew);
            this.Controls.Add(txtNew);
            this.Controls.Add(lblConfirm);
            this.Controls.Add(txtConfirm);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            var n = txtNew.Text ?? string.Empty;
            var c = txtConfirm.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(n))
            {
                MessageBox.Show("Enter a new password.", "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (n != c)
            {
                MessageBox.Show("New password and confirmation do not match.", "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Repository.ChangePassword(adminId, n);
                MessageBox.Show("Password changed.", "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // If this password change was invoked because a legacy default account was created,
                // offer to delete that legacy admin entry (yes/no)
                if (isLegacyCreated)
                {
                    using var confirm = new ConfirmDeleteForm();
                    var res = confirm.ShowDialog(this);
                    if (res == DialogResult.Yes)
                    {
                        try
                        {
                            var removed = Repository.RemoveAdmin(adminId);
                            if (removed)
                            {
                                MessageBox.Show("Legacy admin entry deleted.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Could not delete legacy admin (maybe builtin).", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Failed to delete legacy admin: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to change password: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
