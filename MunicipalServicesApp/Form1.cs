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

        private void btnViewReports_Click(object sender, EventArgs e)
        {
            ViewReportsForm viewReportsForm = new ViewReportsForm();
            viewReportsForm.ShowDialog();
            // Reload issues after returning from view reports form
            ReportedIssues = IssueDataStorage.LoadIssues();
        }

        private void btnComingSoon_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is coming soon!", "Coming Soon", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Save issues when application closes
            IssueDataStorage.SaveIssues(ReportedIssues);
            base.OnFormClosing(e);
        }
    }
}
