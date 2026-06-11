// added on 2026/06/10
using System;
using System.Linq;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public class ResetAdminForm : Form
    {
        private ComboBox cmbAdmins;
        private TextBox txtNew;
        private TextBox txtConfirm;
        private Button btnOk;
        private Button btnCancel;

        public ResetAdminForm()
        {
            this.Text = "Reset Admin Password";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 480;
            this.Height = 260;

            var lblInstr = new Label { Text = "Select an administrator and enter a new password:", Left = 12, Top = 8, Width = 440, Height = 24 };
            var lblAdmin = new Label { Text = "Administrator:", Left = 12, Top = 44, AutoSize = true };
            cmbAdmins = new ComboBox { Left = 140, Top = 40, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            var lblNew = new Label { Text = "New Password:", Left = 12, Top = 84, AutoSize = true };
            txtNew = new TextBox { Left = 140, Top = 80, Width = 300, UseSystemPasswordChar = true };
            var lblConfirm = new Label { Text = "Confirm:", Left = 12, Top = 120, AutoSize = true };
            txtConfirm = new TextBox { Left = 140, Top = 116, Width = 300, UseSystemPasswordChar = true };

            btnOk = new Button { Text = "Reset", Left = 140, Top = 156, Width = 120, Height = 36 };
            btnCancel = new Button { Text = "Cancel", Left = 280, Top = 156, Width = 120, Height = 36 };

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblInstr);
            this.Controls.Add(lblAdmin);
            this.Controls.Add(cmbAdmins);
            this.Controls.Add(lblNew);
            this.Controls.Add(txtNew);
            this.Controls.Add(lblConfirm);
            this.Controls.Add(txtConfirm);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            Load += ResetAdminForm_Load;
        }

        private void ResetAdminForm_Load(object? sender, EventArgs e)
        {
            try
            {
                var list = Repository.GetAdmins().Select(a => new { a.Id, a.Username }).ToList();
                cmbAdmins.Items.Clear();
                foreach (var a in list)
                {
                    cmbAdmins.Items.Add(new ComboItem { Id = a.Id, Text = a.Username });
                }
                if (cmbAdmins.Items.Count > 0) cmbAdmins.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load administrators: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (cmbAdmins.SelectedItem == null)
            {
                MessageBox.Show("Select an administrator.", "Reset", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var item = cmbAdmins.SelectedItem as ComboItem;
            var newp = txtNew.Text ?? string.Empty;
            var conf = txtConfirm.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(newp))
            {
                MessageBox.Show("Enter a new password.", "Reset", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (newp != conf)
            {
                MessageBox.Show("Password and confirmation do not match.", "Reset", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Repository.ChangePassword(item.Id, newp);
                MessageBox.Show("Password has been reset.", "Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to reset password: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class ComboItem
        {
            public int Id { get; set; }
            public string Text { get; set; } = string.Empty;
            public override string ToString() => Text;
        }
    }
}
