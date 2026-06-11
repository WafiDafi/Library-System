// first modified in 18/04/20
//
using System;
using System.Linq;
using System.Windows.Forms;

namespace Library_app_test
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.Load += Form2_Load;
        }

        private void buttonSignIn_Click(object sender, EventArgs e)
        {
            // validate non-empty
            if (string.IsNullOrWhiteSpace(textBoxUsername.Text) || string.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                MessageBox.Show("Please enter username and password.", "Sign-in", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = textBoxUsername.Text.Trim();
            var pass = textBoxPassword.Text;

            try
            {
                var admin = Library_app_test.Data.Repository.GetAdminByCredentials(user, pass);
                if (admin == null)
                {
                    // Special-case legacy default credentials: allow using admin/123 to trigger a forced password change
                    if (string.Equals(user, "admin", StringComparison.OrdinalIgnoreCase) && pass == "123")
                    {
                        // find existing admin by username if present
                        var existing = Library_app_test.Data.Repository.GetAdmins().FirstOrDefault(a => string.Equals(a.Username, user, StringComparison.OrdinalIgnoreCase));
                        int adminId;
                        if (existing != default)
                        {
                            adminId = existing.Id;
                        }
                        else
                        {
                            // create admin record marked to require change (hash stored)
                            adminId = Library_app_test.Data.Repository.AddAdmin(user, pass, mustChange: true);
                        }

                        using var cp = new ChangePasswordForm(adminId, user, isLegacyCreated: true);
                        var dr = cp.ShowDialog(this);
                        if (dr == DialogResult.OK)
                        {
                            // after password successfully changed, proceed to main form
                            var form3 = new Form3();
                            form3.Show();
                            this.Hide();
                            return;
                        }
                        else
                        {
                            MessageBox.Show("You must change your password before signing in.", "Sign-in", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    MessageBox.Show("Invalid username or password.", "Sign-in", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // If password change required, prompt now and require successful change before continuing
                if (admin.Value.MustChange)
                {
                    using var cp = new ChangePasswordForm(admin.Value.Id, admin.Value.Username, isLegacyCreated: false);
                    var dr = cp.ShowDialog(this);
                    if (dr != DialogResult.OK)
                    {
                        MessageBox.Show("You must change your password before signing in.", "Sign-in", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // credentials correct -> open Form3
                var form = new Form3();
                form.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sign-in failed: {ex.Message}", "Sign-in", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBoxShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPassword.UseSystemPasswordChar = !checkBoxShowPassword.Checked;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // show "Library System" in the title while the form is loading
            this.Text = "Library System";
            try
            {
                // Ensure DB exists and prompt to create initial admin if none present
                Library_app_test.Data.LibraryDb.EnsureDatabase();
                if (!Library_app_test.Data.Repository.GetAdmins().Any())
                {
                    using var create = new CreateAdminForm();
                    var dr = create.ShowDialog(this);
                    if (dr != DialogResult.OK)
                    {
                        MessageBox.Show("Administrator account is required. The application will exit.", "Sign-in", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Application.Exit();
                        return;
                    }
                }

                // apply theme instantly
                ThemeManager.ApplyTheme(Library_app_test.Data.Repository.GetDarkMode());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize sign-in: {ex.Message}", "Sign-in", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
