using MunicipalServicesApp.Models;

namespace MunicipalServicesApp
{
    public partial class Form1 : Form
    {
        public static List<Issue> ReportedIssues = new List<Issue>();

        public Form1()
        {
            InitializeComponent();
            LoadIssuesOnStartup();
        }

        private void LoadIssuesOnStartup()
        {
            ReportedIssues = IssueDataStorage.LoadIssues();
        }

        private void btnReportIssues_Click(object sender, EventArgs e)
        {
            ReportIssuesForm reportIssuesForm = new ReportIssuesForm();
            reportIssuesForm.ShowDialog();
            // Reload issues after returning from report form
            ReportedIssues = IssueDataStorage.LoadIssues();
        }

        private void btnLocalEvents_Click(object sender, EventArgs e)
        {
            LocalEventsForm localEventsForm = new LocalEventsForm();
            localEventsForm.ShowDialog();
        }

        private void btnComingSoon_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is coming soon!", "Coming Soon",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            CenterContentPanel();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CenterContentPanel();
        }

        private void CenterContentPanel()
        {
            if (contentPanel != null)
            {
                int x = (ClientSize.Width - contentPanel.Width) / 2;
                int y = (ClientSize.Height - contentPanel.Height) / 2;
                contentPanel.Location = new Point(Math.Max(0, x), Math.Max(0, y));
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Save issues when application closes
            IssueDataStorage.SaveIssues(ReportedIssues);
            base.OnFormClosing(e);
        }
    }
}
