// first modified in 18/04/20
//
namespace Library_app_test
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            // 
            // progressBar1
            // 
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressBar1.Location = new System.Drawing.Point(50, 380);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(760, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 0;
            // 
            // labelStatus
            // 
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelStatus.Location = new System.Drawing.Point(50, 340);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(120, 21);
            this.labelStatus.TabIndex = 1;
            this.labelStatus.Text = "Loading... 0%";
            // 
            // labelTitle
            // 
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelTitle.Location = new System.Drawing.Point(50, 120);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(760, 80);
            this.labelTitle.TabIndex = 2;
            this.labelTitle.Text = "LOADING LIBRARY SYSTEM";
            // 
            // timer1
            // 
            this.timer1 = new System.Windows.Forms.Timer();
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(866, 525);
            Controls.Add(this.labelTitle);
            Controls.Add(this.labelStatus);
            Controls.Add(this.progressBar1);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            Name = "Form1";
            Text = "Splash";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
