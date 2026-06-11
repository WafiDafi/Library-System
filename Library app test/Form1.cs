// first modified in 18/04/20
//
namespace Library_app_test
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelTitle;
        private int progressValue = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // initialize progress
            progressBar1.Value = 0;
            progressValue = 0;
            timer1.Start();
        }

        private void timer1_Tick(object? sender, EventArgs e)
        {
            progressValue += 2; // increment progress
            if (progressValue > 100) progressValue = 100;
            progressBar1.Value = progressValue;
            labelStatus.Text = $"Loading... {progressValue}%";

            if (progressValue >= 100)
            {
                timer1.Stop();
                // Close the splash to allow Program to continue
                this.Close();
            }
        }
    }
}
