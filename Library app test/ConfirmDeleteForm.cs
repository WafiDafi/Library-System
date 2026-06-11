// added on 2026/06/10
using System;
using System.Windows.Forms;

namespace Library_app_test
{
    public class ConfirmDeleteForm : Form
    {
        private Button btnYes;
        private Button btnNo;
        public ConfirmDeleteForm()
        {
            this.Text = "Delete legacy admin entry?";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 420;
            this.Height = 160;

            var lbl = new Label { Text = "Do you want to delete the legacy default admin (username 'admin') entry?", Left = 12, Top = 12, Width = 380, Height = 48 };
            btnYes = new Button { Text = "Yes", Left = 90, Top = 70, Width = 100, Height = 36 };
            btnNo = new Button { Text = "No", Left = 220, Top = 70, Width = 100, Height = 36 };

            btnYes.Click += (s, e) => { this.DialogResult = DialogResult.Yes; };
            btnNo.Click += (s, e) => { this.DialogResult = DialogResult.No; };

            this.Controls.Add(lbl);
            this.Controls.Add(btnYes);
            this.Controls.Add(btnNo);
        }
    }
}
