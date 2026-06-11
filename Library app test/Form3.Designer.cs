// first modified in 18/04/20
//
namespace Library_app_test
{
    partial class Form3
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Label labelDashboard;
        private System.Windows.Forms.Button buttonSearchCatalog;
        private System.Windows.Forms.Button buttonAddBooks;
        private System.Windows.Forms.Button buttonIssueBook;
        private System.Windows.Forms.Button buttonReturnBook;
        private System.Windows.Forms.Button buttonReports;
        private System.Windows.Forms.Button buttonStudents;
        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.Button buttonLogout;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel cardsContainer;
        private System.Windows.Forms.Panel cardAvailable;
        private System.Windows.Forms.Panel cardIssued;
        private System.Windows.Forms.Panel cardReturned;
        private System.Windows.Forms.Label labelAvailableCount;
        private System.Windows.Forms.Label labelIssuedCount;
        private System.Windows.Forms.Label labelReturnedCount;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabAvailable;
        private System.Windows.Forms.TabPage tabIssued;
        private System.Windows.Forms.TabPage tabReturned;
        private System.Windows.Forms.Label labelMain;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.Button buttonSearchGo;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            headerPanel = new Panel();
            headerLabel = new Label();
            panelLeft = new Panel();
            buttonLogout = new Button();
            buttonReports = new Button();
            buttonStudents = new Button();
            buttonSettings = new Button();
            buttonReturnBook = new Button();
            buttonIssueBook = new Button();
            buttonAddBooks = new Button();
            buttonSearchCatalog = new Button();
            labelDashboard = new Label();
            pictureBoxLogo = new PictureBox();
            panelMain = new Panel();
            cardsContainer = new Panel();
            cardReturned = new Panel();
            labelReturnedCount = new Label();
            cardIssued = new Panel();
            labelIssuedCount = new Label();
            cardAvailable = new Panel();
            labelAvailableCount = new Label();
            tabControlMain = new TabControl();
            tabAvailable = new TabPage();
            tabIssued = new TabPage();
            tabReturned = new TabPage();
            searchPanel = new Panel();
            textBoxSearch = new TextBox();
            buttonSearchGo = new Button();
            labelMain = new Label();
            headerPanel.SuspendLayout();
            panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).BeginInit();
            panelMain.SuspendLayout();
            cardsContainer.SuspendLayout();
            cardReturned.SuspendLayout();
            cardIssued.SuspendLayout();
            cardAvailable.SuspendLayout();
            tabControlMain.SuspendLayout();
            searchPanel.SuspendLayout();
            SuspendLayout();
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.FromArgb(38, 88, 135);
            headerPanel.Controls.Add(headerLabel);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Margin = new Padding(5, 6, 5, 6);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(1714, 80);
            headerPanel.TabIndex = 2;
            // 
            // headerLabel
            // 
            headerLabel.Dock = DockStyle.Left;
            headerLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            headerLabel.ForeColor = Color.White;
            headerLabel.Location = new Point(0, 0);
            headerLabel.Margin = new Padding(5, 0, 5, 0);
            headerLabel.Name = "headerLabel";
            headerLabel.Padding = new Padding(21, 0, 0, 0);
            headerLabel.Size = new Size(171, 80);
            headerLabel.TabIndex = 0;
            headerLabel.Text = "Library System";
            headerLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelLeft
            // 
            panelLeft.BackColor = Color.FromArgb(66, 125, 169);
            panelLeft.Controls.Add(buttonLogout);
            panelLeft.Controls.Add(buttonReports);
            panelLeft.Controls.Add(buttonStudents);
            panelLeft.Controls.Add(buttonSettings);
            panelLeft.Controls.Add(buttonReturnBook);
            panelLeft.Controls.Add(buttonIssueBook);
            panelLeft.Controls.Add(buttonAddBooks);
            panelLeft.Controls.Add(buttonSearchCatalog);
            panelLeft.Controls.Add(labelDashboard);
            panelLeft.Controls.Add(pictureBoxLogo);
            panelLeft.Dock = DockStyle.Left;
            panelLeft.Location = new Point(0, 80);
            panelLeft.Margin = new Padding(5, 6, 5, 6);
            panelLeft.Name = "panelLeft";
            panelLeft.Size = new Size(309, 1120);
            panelLeft.TabIndex = 0;
            // 
            // buttonLogout
            // 
            buttonLogout.BackColor = Color.FromArgb(50, 100, 200);
            buttonLogout.FlatAppearance.BorderColor = Color.White;
            buttonLogout.FlatAppearance.BorderSize = 2;
            buttonLogout.FlatStyle = FlatStyle.Flat;
            buttonLogout.ForeColor = Color.White;
            buttonLogout.Location = new Point(26, 1005);
            buttonLogout.Margin = new Padding(5, 6, 5, 6);
            buttonLogout.Name = "buttonLogout";
            buttonLogout.Size = new Size(257, 72);
            buttonLogout.TabIndex = 0;
            buttonLogout.Text = "LOGOUT";
            buttonLogout.UseVisualStyleBackColor = false;
            buttonLogout.Click += buttonLogout_Click;
            // 
            // buttonReports
            // 
            buttonReports.BackColor = Color.FromArgb(66, 125, 169);
            buttonReports.FlatAppearance.BorderColor = Color.White;
            buttonReports.FlatAppearance.BorderSize = 2;
            buttonReports.FlatStyle = FlatStyle.Flat;
            buttonReports.ForeColor = Color.White;
            buttonReports.Location = new Point(26, 793);
            buttonReports.Margin = new Padding(5, 6, 5, 6);
            buttonReports.Name = "buttonReports";
            buttonReports.Size = new Size(257, 72);
            buttonReports.TabIndex = 1;
            buttonReports.Text = "REPORTS";
            buttonReports.UseVisualStyleBackColor = false;
            buttonReports.Click += buttonReports_Click;
            // 
            // buttonStudents
            // 
            buttonStudents.BackColor = Color.FromArgb(66, 125, 169);
            buttonStudents.FlatAppearance.BorderColor = Color.White;
            buttonStudents.FlatAppearance.BorderSize = 2;
            buttonStudents.FlatStyle = FlatStyle.Flat;
            buttonStudents.ForeColor = Color.White;
            buttonStudents.Location = new Point(26, 702);
            buttonStudents.Margin = new Padding(5, 6, 5, 6);
            buttonStudents.Name = "buttonStudents";
            buttonStudents.Size = new Size(257, 72);
            buttonStudents.TabIndex = 2;
            buttonStudents.Text = "STUDENTS";
            buttonStudents.UseVisualStyleBackColor = false;
            buttonStudents.Click += buttonStudents_Click;
            // 
            // buttonSettings
            // 
            buttonSettings.BackColor = Color.FromArgb(66, 125, 169);
            buttonSettings.FlatAppearance.BorderColor = Color.White;
            buttonSettings.FlatAppearance.BorderSize = 2;
            buttonSettings.FlatStyle = FlatStyle.Flat;
            buttonSettings.ForeColor = Color.White;
            buttonSettings.Location = new Point(26, 885);
            buttonSettings.Margin = new Padding(5, 6, 5, 6);
            buttonSettings.Name = "buttonSettings";
            buttonSettings.Size = new Size(257, 72);
            buttonSettings.TabIndex = 3;
            buttonSettings.Text = "SETTINGS";
            buttonSettings.UseVisualStyleBackColor = false;
            buttonSettings.Click += buttonSettings_Click;
            // 
            // buttonReturnBook
            // 
            buttonReturnBook.BackColor = Color.FromArgb(66, 125, 169);
            buttonReturnBook.FlatAppearance.BorderColor = Color.White;
            buttonReturnBook.FlatAppearance.BorderSize = 2;
            buttonReturnBook.FlatStyle = FlatStyle.Flat;
            buttonReturnBook.ForeColor = Color.White;
            buttonReturnBook.Location = new Point(26, 603);
            buttonReturnBook.Margin = new Padding(5, 6, 5, 6);
            buttonReturnBook.Name = "buttonReturnBook";
            buttonReturnBook.Size = new Size(257, 72);
            buttonReturnBook.TabIndex = 4;
            buttonReturnBook.Text = "RETURN BOOK";
            buttonReturnBook.UseVisualStyleBackColor = false;
            buttonReturnBook.Click += buttonReturnBook_Click;
            // 
            // buttonIssueBook
            // 
            buttonIssueBook.BackColor = Color.FromArgb(66, 125, 169);
            buttonIssueBook.FlatAppearance.BorderColor = Color.White;
            buttonIssueBook.FlatAppearance.BorderSize = 2;
            buttonIssueBook.FlatStyle = FlatStyle.Flat;
            buttonIssueBook.ForeColor = Color.White;
            buttonIssueBook.Location = new Point(26, 511);
            buttonIssueBook.Margin = new Padding(5, 6, 5, 6);
            buttonIssueBook.Name = "buttonIssueBook";
            buttonIssueBook.Size = new Size(257, 72);
            buttonIssueBook.TabIndex = 5;
            buttonIssueBook.Text = "ISSUE BOOK";
            buttonIssueBook.UseVisualStyleBackColor = false;
            buttonIssueBook.Click += buttonIssueBook_Click;
            // 
            // buttonAddBooks
            // 
            buttonAddBooks.BackColor = Color.FromArgb(66, 125, 169);
            buttonAddBooks.FlatAppearance.BorderColor = Color.White;
            buttonAddBooks.FlatAppearance.BorderSize = 2;
            buttonAddBooks.FlatStyle = FlatStyle.Flat;
            buttonAddBooks.ForeColor = Color.White;
            buttonAddBooks.Location = new Point(26, 419);
            buttonAddBooks.Margin = new Padding(5, 6, 5, 6);
            buttonAddBooks.Name = "buttonAddBooks";
            buttonAddBooks.Size = new Size(257, 72);
            buttonAddBooks.TabIndex = 6;
            buttonAddBooks.Text = "BOOK CATALOG";
            buttonAddBooks.UseVisualStyleBackColor = false;
            buttonAddBooks.Click += buttonAddBooks_Click;
            // 
            // buttonSearchCatalog
            // 
            buttonSearchCatalog.BackColor = Color.FromArgb(66, 125, 169);
            buttonSearchCatalog.FlatAppearance.BorderColor = Color.White;
            buttonSearchCatalog.FlatAppearance.BorderSize = 2;
            buttonSearchCatalog.FlatStyle = FlatStyle.Flat;
            buttonSearchCatalog.ForeColor = Color.White;
            buttonSearchCatalog.Location = new Point(26, 326);
            buttonSearchCatalog.Margin = new Padding(5, 6, 5, 6);
            buttonSearchCatalog.Name = "buttonSearchCatalog";
            buttonSearchCatalog.Size = new Size(257, 72);
            buttonSearchCatalog.TabIndex = 7;
            buttonSearchCatalog.Text = "SEARCH";
            buttonSearchCatalog.UseVisualStyleBackColor = false;
            buttonSearchCatalog.Click += buttonSearchCatalog_Click;
            // 
            // labelDashboard
            // 
            labelDashboard.BackColor = Color.FromArgb(66, 125, 169);
            labelDashboard.ForeColor = Color.White;
            labelDashboard.Location = new Point(26, 248);
            labelDashboard.Margin = new Padding(5, 0, 5, 0);
            labelDashboard.Name = "labelDashboard";
            labelDashboard.Size = new Size(257, 72);
            labelDashboard.TabIndex = 8;
            labelDashboard.Text = "DASHBOARD";
            labelDashboard.TextAlign = ContentAlignment.MiddleCenter;
            labelDashboard.Click += buttonDashboard_Click;
            // 
            // pictureBoxLogo
            // 
            pictureBoxLogo.BackColor = Color.Transparent;
            pictureBoxLogo.BackgroundImage = (Image)resources.GetObject("pictureBoxLogo.BackgroundImage");
            pictureBoxLogo.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxLogo.ImageLocation = "https://upload.wikimedia.org/wikipedia/commons/7/73/Lion_waiting_in_Namibia.jpg";
            pictureBoxLogo.Location = new Point(14, -126);
            pictureBoxLogo.Margin = new Padding(5, 6, 5, 6);
            pictureBoxLogo.Name = "pictureBoxLogo";
            pictureBoxLogo.Size = new Size(277, 368);
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxLogo.TabIndex = 0;
            pictureBoxLogo.TabStop = false;
            pictureBoxLogo.Click += pictureBoxLogo_Click;
            // 
            // panelMain
            // 
            panelMain.Controls.Add(cardsContainer);
            panelMain.Controls.Add(tabControlMain);
            panelMain.Controls.Add(searchPanel);
            panelMain.Controls.Add(labelMain);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(309, 80);
            panelMain.Margin = new Padding(5, 6, 5, 6);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(1405, 1120);
            panelMain.TabIndex = 1;
            // 
            // cardsContainer
            // 
            cardsContainer.BackColor = Color.Transparent;
            cardsContainer.Controls.Add(cardReturned);
            cardsContainer.Controls.Add(cardIssued);
            cardsContainer.Controls.Add(cardAvailable);
            cardsContainer.Dock = DockStyle.Top;
            cardsContainer.Location = new Point(0, 600);
            cardsContainer.Margin = new Padding(5, 6, 5, 6);
            cardsContainer.Name = "cardsContainer";
            cardsContainer.Padding = new Padding(21, 24, 21, 24);
            cardsContainer.Size = new Size(1405, 240);
            cardsContainer.TabIndex = 0;
            // 
            // cardReturned
            // 
            cardReturned.BackColor = Color.White;
            cardReturned.Controls.Add(labelReturnedCount);
            cardReturned.Dock = DockStyle.Left;
            cardReturned.Location = new Point(843, 24);
            cardReturned.Margin = new Padding(10, 12, 10, 12);
            cardReturned.Name = "cardReturned";
            cardReturned.Padding = new Padding(14, 16, 14, 16);
            cardReturned.Size = new Size(411, 192);
            cardReturned.TabIndex = 0;
            // 
            // labelReturnedCount
            // 
            labelReturnedCount.Dock = DockStyle.Fill;
            labelReturnedCount.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelReturnedCount.Location = new Point(14, 16);
            labelReturnedCount.Margin = new Padding(5, 0, 5, 0);
            labelReturnedCount.Name = "labelReturnedCount";
            labelReturnedCount.Size = new Size(383, 160);
            labelReturnedCount.TabIndex = 0;
            labelReturnedCount.Text = "Returned Books\n\n0";
            labelReturnedCount.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cardIssued
            // 
            cardIssued.BackColor = Color.White;
            cardIssued.Controls.Add(labelIssuedCount);
            cardIssued.Dock = DockStyle.Left;
            cardIssued.Location = new Point(432, 24);
            cardIssued.Margin = new Padding(10, 12, 10, 12);
            cardIssued.Name = "cardIssued";
            cardIssued.Padding = new Padding(14, 16, 14, 16);
            cardIssued.Size = new Size(411, 192);
            cardIssued.TabIndex = 1;
            // 
            // labelIssuedCount
            // 
            labelIssuedCount.Dock = DockStyle.Fill;
            labelIssuedCount.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelIssuedCount.Location = new Point(14, 16);
            labelIssuedCount.Margin = new Padding(5, 0, 5, 0);
            labelIssuedCount.Name = "labelIssuedCount";
            labelIssuedCount.Size = new Size(383, 160);
            labelIssuedCount.TabIndex = 0;
            labelIssuedCount.Text = "Issued Books\n\n0";
            labelIssuedCount.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cardAvailable
            // 
            cardAvailable.BackColor = Color.White;
            cardAvailable.Controls.Add(labelAvailableCount);
            cardAvailable.Dock = DockStyle.Left;
            cardAvailable.Location = new Point(21, 24);
            cardAvailable.Margin = new Padding(10, 12, 10, 12);
            cardAvailable.Name = "cardAvailable";
            cardAvailable.Padding = new Padding(14, 16, 14, 16);
            cardAvailable.Size = new Size(411, 192);
            cardAvailable.TabIndex = 2;
            // 
            // labelAvailableCount
            // 
            labelAvailableCount.Dock = DockStyle.Fill;
            labelAvailableCount.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelAvailableCount.Location = new Point(14, 16);
            labelAvailableCount.Margin = new Padding(5, 0, 5, 0);
            labelAvailableCount.Name = "labelAvailableCount";
            labelAvailableCount.Size = new Size(383, 160);
            labelAvailableCount.TabIndex = 0;
            labelAvailableCount.Text = "Available Books\n\n0";
            labelAvailableCount.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabAvailable);
            tabControlMain.Controls.Add(tabIssued);
            tabControlMain.Controls.Add(tabReturned);
            tabControlMain.Dock = DockStyle.Top;
            tabControlMain.Location = new Point(0, 0);
            tabControlMain.Margin = new Padding(5, 6, 5, 6);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(1405, 600);
            tabControlMain.TabIndex = 1;
            // 
            // tabAvailable
            // 
            tabAvailable.Location = new Point(4, 39);
            tabAvailable.Margin = new Padding(5, 6, 5, 6);
            tabAvailable.Name = "tabAvailable";
            tabAvailable.Size = new Size(1397, 557);
            tabAvailable.TabIndex = 0;
            tabAvailable.Text = "recources";
            // 
            // tabIssued
            // 
            tabIssued.Location = new Point(4, 39);
            tabIssued.Margin = new Padding(5, 6, 5, 6);
            tabIssued.Name = "tabIssued";
            tabIssued.Size = new Size(1397, 557);
            tabIssued.TabIndex = 1;
            tabIssued.Text = "Issued Books";
            // 
            // tabReturned
            // 
            tabReturned.Location = new Point(4, 39);
            tabReturned.Margin = new Padding(5, 6, 5, 6);
            tabReturned.Name = "tabReturned";
            tabReturned.Size = new Size(1397, 557);
            tabReturned.TabIndex = 2;
            tabReturned.Text = "Returned Books";
            // 
            // searchPanel
            // 
            searchPanel.BackColor = Color.Transparent;
            searchPanel.Controls.Add(textBoxSearch);
            searchPanel.Controls.Add(buttonSearchGo);
            searchPanel.Location = new Point(0, 0);
            searchPanel.Margin = new Padding(5, 6, 5, 6);
            searchPanel.Name = "searchPanel";
            searchPanel.Size = new Size(891, 84);
            searchPanel.TabIndex = 2;
            searchPanel.Visible = false;
            // 
            // textBoxSearch
            // 
            textBoxSearch.Font = new Font("Segoe UI", 11F);
            textBoxSearch.ForeColor = Color.Gray;
            textBoxSearch.Location = new Point(0, 12);
            textBoxSearch.Margin = new Padding(5, 6, 5, 6);
            textBoxSearch.Name = "textBoxSearch";
            textBoxSearch.Size = new Size(717, 42);
            textBoxSearch.TabIndex = 0;
            textBoxSearch.Text = "enter book name or id";
            textBoxSearch.GotFocus += textBoxSearch_GotFocus;
            textBoxSearch.KeyDown += textBoxSearch_KeyDown;
            textBoxSearch.LostFocus += textBoxSearch_LostFocus;
            // 
            // buttonSearchGo
            // 
            buttonSearchGo.Location = new Point(737, 12);
            buttonSearchGo.Margin = new Padding(5, 6, 5, 6);
            buttonSearchGo.Name = "buttonSearchGo";
            buttonSearchGo.Size = new Size(137, 60);
            buttonSearchGo.TabIndex = 1;
            buttonSearchGo.Text = "Search";
            buttonSearchGo.Click += buttonSearchGo_Click;
            // 
            // labelMain
            // 
            labelMain.Dock = DockStyle.Fill;
            labelMain.Font = new Font("Segoe UI", 14F);
            labelMain.Location = new Point(0, 0);
            labelMain.Margin = new Padding(5, 0, 5, 0);
            labelMain.Name = "labelMain";
            labelMain.Size = new Size(1405, 1120);
            labelMain.TabIndex = 3;
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1714, 1200);
            Controls.Add(panelMain);
            Controls.Add(panelLeft);
            Controls.Add(headerPanel);
            Margin = new Padding(5, 6, 5, 6);
            Name = "Form3";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Library - Main";
            headerPanel.ResumeLayout(false);
            panelLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).EndInit();
            panelMain.ResumeLayout(false);
            cardsContainer.ResumeLayout(false);
            cardReturned.ResumeLayout(false);
            cardIssued.ResumeLayout(false);
            cardAvailable.ResumeLayout(false);
            tabControlMain.ResumeLayout(false);
            searchPanel.ResumeLayout(false);
            searchPanel.PerformLayout();
            ResumeLayout(false);

        }

        #endregion
    }
}
