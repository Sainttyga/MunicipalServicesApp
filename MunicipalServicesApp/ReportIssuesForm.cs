using MunicipalServicesApp.Models;

namespace MunicipalServicesApp
{
    public partial class ReportIssuesForm : Form
    {
        private string? attachmentPath;

        public ReportIssuesForm()
        {
            InitializeComponent();
            UpdateEngagementMessage();
        }

        private void UpdateEngagementMessage()
        {
            int progress = progressBar.Value;
            string message = progress switch
            {
                0 => "Start by entering the location of the issue.",
                25 => "Great! Now select a category for the issue.",
                50 => "Excellent! Please provide a detailed description.",
                75 => "Almost done! You can attach a file if needed.",
                100 => "Perfect! You're ready to submit your report.",
                _ => "Thank you for helping improve our community!"
            };
            lblEngagement.Text = message;
        }

        private void btnAttachFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif|Documents|*.pdf;*.doc;*.docx|All Files|*.*";
            openFileDialog.Title = "Select a file to attach";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                attachmentPath = openFileDialog.FileName;
                lblAttachmentStatus.Text = $"Attached: {Path.GetFileName(attachmentPath)}";
                lblAttachmentStatus.ForeColor = Color.Green;
                UpdateProgress();
                UpdateEngagementMessage();
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Please enter the location of the issue.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a category for the issue.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(rtDescription.Text))
            {
                MessageBox.Show("Please provide a description of the issue.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int nextId = IssueDataStorage.GetNextIssueId(Form1.ReportedIssues);
            
            Issue newIssue = new Issue
            {
                Id = nextId,
                Location = txtLocation.Text.Trim(),
                Category = cmbCategory.SelectedItem?.ToString() ?? string.Empty,
                Description = rtDescription.Text.Trim(),
                AttachmentPath = attachmentPath,
                ReportedDate = DateTime.Now,
                Status = "Pending"
            };

            Form1.ReportedIssues.Add(newIssue);
            IssueDataStorage.SaveIssues(Form1.ReportedIssues);

            progressBar.Value = 100;
            MessageBox.Show($"Issue reported successfully!\n\nIssue ID: {newIssue.Id}\nLocation: {newIssue.Location}\nCategory: {newIssue.Category}", 
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ClearForm();
            UpdateEngagementMessage();
        }

        private void ClearForm()
        {
            txtLocation.Clear();
            cmbCategory.SelectedIndex = -1;
            rtDescription.Clear();
            attachmentPath = null;
            lblAttachmentStatus.Text = "No file attached";
            lblAttachmentStatus.ForeColor = Color.Gray;
            progressBar.Value = 0;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtLocation_TextChanged(object sender, EventArgs e)
        {
            UpdateProgress();
            UpdateEngagementMessage();
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateProgress();
            UpdateEngagementMessage();
        }

        private void rtDescription_TextChanged(object sender, EventArgs e)
        {
            UpdateProgress();
            UpdateEngagementMessage();
        }

        private void UpdateProgress()
        {
            int progress = 0;
            if (!string.IsNullOrWhiteSpace(txtLocation.Text)) progress += 25;
            if (cmbCategory.SelectedIndex != -1) progress += 25;
            if (!string.IsNullOrWhiteSpace(rtDescription.Text)) progress += 25;
            if (!string.IsNullOrWhiteSpace(attachmentPath)) progress += 25;
            progressBar.Value = progress;
        }
    }
}
