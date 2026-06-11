// first modified in 18/04/20
//
using System;
using System.Linq;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public class SettingsForm : Form
    {
        private DataGridView dgvAdmins;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnAdd;
        private Button btnRemove;
        private CheckBox chkDarkMode;
        private Button btnSave;
        private Button btnClose;
        private bool initialDarkMode;

        public SettingsForm()
        {
            Text = "Settings";
            Width = 800;
            Height = 520;
            StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new System.Drawing.Size(700, 420);

            dgvAdmins = new DataGridView { Dock = DockStyle.Top, Height = 220, ReadOnly = true, AutoGenerateColumns = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells };
            dgvAdmins.AllowUserToAddRows = false;
            dgvAdmins.AllowUserToDeleteRows = false;
            dgvAdmins.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAdmins.MultiSelect = false;
            dgvAdmins.RowHeadersVisible = false;
            dgvAdmins.BackgroundColor = System.Drawing.Color.White;
            dgvAdmins.BorderStyle = BorderStyle.Fixed3D;
            Controls.Add(dgvAdmins);

            var panel = new Panel { Dock = DockStyle.Top, Height = 140, BackColor = System.Drawing.SystemColors.Control };
            var lblUser = new Label { Text = "Username:", Left = 10, Top = 16 };
            txtUsername = new TextBox { Left = 140, Top = 12, Width = 360, BorderStyle = BorderStyle.FixedSingle };
            var lblPass = new Label { Text = "Password:", Left = 10, Top = 52 };
            txtPassword = new TextBox { Left = 140, Top = 48, Width = 360, BorderStyle = BorderStyle.FixedSingle, UseSystemPasswordChar = true };
            btnAdd = new Button { Text = "Add Admin", Left = 520, Top = 12, Width = 160, Height = 36 };
            btnRemove = new Button { Text = "Remove Selected", Left = 520, Top = 56, Width = 160, Height = 36 };

            panel.Controls.Add(lblUser);
            panel.Controls.Add(txtUsername);
            panel.Controls.Add(lblPass);
            panel.Controls.Add(txtPassword);
            panel.Controls.Add(btnAdd);
            panel.Controls.Add(btnRemove);
            Controls.Add(panel);

            chkDarkMode = new CheckBox { Text = "Dark Mode", Left = 10, Top = 360, AutoSize = true };
            Controls.Add(chkDarkMode);

            btnSave = new Button { Text = "Save", Left = 140, Top = 400, Width = 120, Height = 36 };
            btnClose = new Button { Text = "Back", Left = 280, Top = 400, Width = 120, Height = 36 };
            Controls.Add(btnSave);
            Controls.Add(btnClose);

            btnAdd.Click += BtnAdd_Click;
            btnRemove.Click += BtnRemove_Click;
            chkDarkMode.CheckedChanged += ChkDarkMode_CheckedChanged;
            btnSave.Click += BtnSave_Click;
            btnClose.Click += BtnClose_Click;

            Load += SettingsForm_Load;
        }

        private void SettingsForm_Load(object? sender, EventArgs e)
        {
            LoadAdmins();
            initialDarkMode = Repository.GetDarkMode();
            chkDarkMode.Checked = initialDarkMode;
            // apply current theme immediately (initial)
            ThemeManager.ApplyTheme(initialDarkMode);
        }

        private void LoadAdmins()
        {
            var list = Repository.GetAdmins().Select(a => new { a.Id, a.Username, IsBuiltin = a.IsBuiltin ? "Yes" : "No" }).ToList();
            dgvAdmins.DataSource = list;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var u = txtUsername.Text?.Trim();
            var p = txtPassword.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(u) || string.IsNullOrWhiteSpace(p))
            {
                MessageBox.Show("Enter username and password", "Add Admin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Repository.AddAdmin(u, p);
                LoadAdmins();
                txtUsername.Text = "";
                txtPassword.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add admin: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemove_Click(object? sender, EventArgs e)
        {
            // If the username textbox contains something, prefer that input (allows entering an Id)
            var u = txtUsername.Text?.Trim();
            var p = txtPassword.Text ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(u))
            {
                // if username is numeric, treat it as Id removal
                if (int.TryParse(u, out var candidateId))
                {
                    var foundById = Repository.GetAdminById(candidateId);
                    if (foundById == null)
                    {
                        MessageBox.Show($"No admin found with Id {candidateId}.", "Remove", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var confById = MessageBox.Show($"Remove admin '{foundById.Value.Username}' (Id {candidateId})?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confById == DialogResult.Yes)
                    {
                        var removed = Repository.RemoveAdmin(candidateId);
                        if (!removed)
                        {
                            MessageBox.Show("Builtin admin cannot be removed.", "Remove", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            LoadAdmins();
                        }
                    }

                    return;
                }

                // fallback: username+password lookup
                if (string.IsNullOrWhiteSpace(p))
                {
                    MessageBox.Show("Enter password to remove by username, or enter numeric Id in Username.", "Remove", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var found = Repository.GetAdminByCredentials(u, p);
                if (found == null)
                {
                    MessageBox.Show("No matching admin found with provided credentials.", "Remove", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var conf = MessageBox.Show($"Remove admin '{u}'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (conf == DialogResult.Yes)
                {
                    var removed = Repository.RemoveAdmin(found.Value.Id);
                    if (!removed)
                    {
                        MessageBox.Show("Builtin admin cannot be removed.", "Remove", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        LoadAdmins();
                    }
                }

                return;
            }

            // If no input provided, fall back to removing the selected row if any.
            if (dgvAdmins.CurrentRow != null)
            {
                var id = (int)dgvAdmins.CurrentRow.Cells[0].Value;
                var r = MessageBox.Show("Remove selected admin?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    var removed = Repository.RemoveAdmin(id);
                    if (!removed)
                    {
                        MessageBox.Show("Builtin admin cannot be removed.", "Remove", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    LoadAdmins();
                }

                return;
            }

            MessageBox.Show("Select an admin row or enter username (or numeric Id) to remove.", "Remove", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ChkDarkMode_CheckedChanged(object? sender, EventArgs e)
        {
            // Preview only - do not persist until user clicks Save
            ThemeManager.ApplyTheme(chkDarkMode.Checked);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                Repository.SetDarkMode(chkDarkMode.Checked);
                initialDarkMode = chkDarkMode.Checked;
                MessageBox.Show("Dark mode setting saved.", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save setting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            // Revert preview if not saved
            try
            {
                ThemeManager.ApplyTheme(initialDarkMode);
            }
            catch { }
            this.Close();
        }
    }
}
