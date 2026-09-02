using MunicipalServicesApp.Models;

namespace MunicipalServicesApp
{
    public partial class ViewReportsForm : Form
    {
        public ViewReportsForm()
        {
            InitializeComponent();
        }

        private void ViewReportsForm_Load(object sender, EventArgs e)
        {
            LoadReports();
        }

        private void LoadReports()
        {
            try
            {
                // Clear existing data
                if (dataGridViewReports != null)
                {
                    dataGridViewReports.DataSource = null;
                }

                // Load issues from storage
                var issues = IssueDataStorage.LoadIssues() ?? new List<Issue>();

                // Bind to DataGridView
                if (dataGridViewReports != null)
                {
                    dataGridViewReports.DataSource = issues;

                    // Format the columns after data binding
                    if (dataGridViewReports.Columns.Count > 0)
                    {
                        var idColumn = dataGridViewReports.Columns["Id"];
                        if (idColumn != null)
                        {
                            idColumn.HeaderText = "Issue ID";
                            idColumn.Width = 80;
                        }

                        var locationColumn = dataGridViewReports.Columns["Location"];
                        if (locationColumn != null)
                        {
                            locationColumn.HeaderText = "Location";
                            locationColumn.Width = 150;
                        }

                        var categoryColumn = dataGridViewReports.Columns["Category"];
                        if (categoryColumn != null)
                        {
                            categoryColumn.HeaderText = "Category";
                            categoryColumn.Width = 120;
                        }

                        var descriptionColumn = dataGridViewReports.Columns["Description"];
                        if (descriptionColumn != null)
                        {
                            descriptionColumn.HeaderText = "Description";
                            descriptionColumn.Width = 200;
                        }

                        var attachmentColumn = dataGridViewReports.Columns["AttachmentPath"];
                        if (attachmentColumn != null)
                        {
                            attachmentColumn.HeaderText = "Attachment";
                            attachmentColumn.Width = 100;
                            attachmentColumn.Visible = false;
                        }

                        var reportedDateColumn = dataGridViewReports.Columns["ReportedDate"];
                        if (reportedDateColumn != null)
                        {
                            reportedDateColumn.HeaderText = "Reported Date";
                            reportedDateColumn.Width = 120;
                            reportedDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
                        }

                        var statusColumn = dataGridViewReports.Columns["Status"];
                        if (statusColumn != null)
                        {
                            statusColumn.HeaderText = "Status";
                            statusColumn.Width = 80;
                        }
                    }
                }

                // Update count label
                if (lblReportCount != null)
                {
                    lblReportCount.Text = $"Total Reports: {issues.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading reports: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadReports();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            if (dataGridViewReports.SelectedRows.Count > 0)
            {
                var selectedIssue = dataGridViewReports.SelectedRows[0].DataBoundItem as Issue;
                if (selectedIssue != null)
                {
                    string attachmentInfo = string.IsNullOrEmpty(selectedIssue.AttachmentPath)
                        ? "No attachment"
                        : $"Attachment: {Path.GetFileName(selectedIssue.AttachmentPath)}";

                    MessageBox.Show(
                        $"Issue ID: {selectedIssue.Id}\n" +
                        $"Location: {selectedIssue.Location}\n" +
                        $"Category: {selectedIssue.Category}\n" +
                        $"Description: {selectedIssue.Description}\n" +
                        $"{attachmentInfo}\n" +
                        $"Reported Date: {selectedIssue.ReportedDate:yyyy-MM-dd HH:mm}\n" +
                        $"Status: {selectedIssue.Status}",
                        "Issue Details",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            else
            {
                MessageBox.Show("Please select a report to view details.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
