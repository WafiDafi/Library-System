using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
{
    public class StudentsForm : Form
    {
        private DataGridView dgv;
        private Button btnAdd;
        private Button btnRemove;

        public StudentsForm()
        {
            this.Text = "Students";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 900;
            this.Height = 600;

            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            dgv.CellDoubleClick += Dgv_CellDoubleClick;

            btnAdd = new Button { Text = "Add Student", Dock = DockStyle.Bottom, Height = 36 };
            btnRemove = new Button { Text = "Remove Selected", Dock = DockStyle.Bottom, Height = 36 };
            btnAdd.Click += BtnAdd_Click;
            btnRemove.Click += BtnRemove_Click;

            this.Controls.Add(dgv);
            this.Controls.Add(btnRemove);
            this.Controls.Add(btnAdd);

            this.Load += StudentsForm_Load;
        }

        private void StudentsForm_Load(object? sender, EventArgs e)
        {
            LoadStudents();
        }

        private void LoadStudents()
        {
            try
            {
                LibraryDb.EnsureDatabase();
                using var conn = LibraryDb.GetConnection();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT m.Id, m.FullName, m.Course, m.IdNumber, m.Email, m.Phone, COALESCE(m.OverduesCount,0) AS OverduesCount, COALESCE(m.PrevBorrowedCount,0) AS PrevBorrowedCount,
 (SELECT COUNT(*) FROM Issues i WHERE i.MemberId = m.Id AND i.ReturnDate IS NULL) AS CurrentlyBorrowed
FROM Members m ORDER BY m.FullName";
                using var rdr = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(rdr);
                dgv.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load students: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Dgv_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                {
                    MessageBox.Show("no student clicked", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var cell = dgv.Rows[e.RowIndex].Cells["Id"];
                if (cell == null || cell.Value == null || cell.Value == DBNull.Value)
                {
                    MessageBox.Show("no student clicked", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var id = Convert.ToInt32(cell.Value);
                using var frm = new StudentDetailForm(id);
                frm.ShowDialog(this);
                // refresh after returning
                LoadStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening student details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            // simple add prompt
            using var dlg = new AddStudentForm();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                LoadStudents();
            }
        }

        private void BtnRemove_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a student to remove.", "Remove", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var id = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
            var name = dgv.SelectedRows[0].Cells["FullName"].Value?.ToString();
            var res = MessageBox.Show($"Remove student '{name}'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res != DialogResult.Yes) return;
            try
            {
                using var conn = LibraryDb.GetConnection();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Members WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                LoadStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to remove: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
