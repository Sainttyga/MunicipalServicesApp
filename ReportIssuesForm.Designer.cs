namespace MunicipalServicesApp
{
    partial class ReportIssuesForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblLocation;
        private TextBox txtLocation;
        private Label lblCategory;
        private ComboBox cmbCategory;
        private Label lblDescription;
        private RichTextBox rtDescription;
        private Label lblAttachment;
        private Button btnAttachFile;
        private Label lblAttachmentStatus;
        private Button btnSubmit;
        private Button btnBack;
        private Label lblEngagement;
        private ProgressBar progressBar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Color primary = Color.FromArgb(0, 122, 204);
            Color panelBack = Color.FromArgb(240, 245, 250);

            Panel header = new Panel();
            Label headerLabel = new Label();
            Panel body = new Panel();
            Label lblHint = new Label();
            lblLocation = new Label();
            txtLocation = new TextBox();
            lblCategory = new Label();
            cmbCategory = new ComboBox();
            lblDescription = new Label();
            rtDescription = new RichTextBox();
            lblAttachment = new Label();
            btnAttachFile = new Button();
            lblAttachmentStatus = new Label();
            Panel bottom = new Panel();
            progressBar = new ProgressBar();
            lblEngagement = new Label();
            btnSubmit = new Button();
            btnBack = new Button();
            SuspendLayout();

            // header
            header.BackColor = primary;
            header.Dock = DockStyle.Top;
            header.Name = "header";
            header.Size = new Size(880, 52);
            header.TabIndex = 0;

            // headerLabel
            headerLabel.AutoSize = true;
            headerLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            headerLabel.ForeColor = Color.White;
            headerLabel.Location = new Point(14, 10);
            headerLabel.Name = "headerLabel";
            headerLabel.Size = new Size(177, 30);
            headerLabel.TabIndex = 0;
            headerLabel.Text = "Report an Issue";
            header.Controls.Add(headerLabel);

            // body
            body.BackColor = Color.White;
            body.Dock = DockStyle.Fill;
            body.Name = "body";
            body.Size = new Size(880, 504);
            body.TabIndex = 1;

            // lblHint
            lblHint.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            lblHint.ForeColor = primary;
            lblHint.Location = new Point(12, 14);
            lblHint.Name = "lblHint";
            lblHint.Size = new Size(856, 20);
            lblHint.TabIndex = 0;
            lblHint.Text = "Tell us where the problem is and describe what is wrong - the bar below tracks how complete your report is.";

            // lblLocation
            lblLocation.AutoSize = true;
            lblLocation.Location = new Point(12, 50);
            lblLocation.Name = "lblLocation";
            lblLocation.Size = new Size(53, 15);
            lblLocation.TabIndex = 1;
            lblLocation.Text = "Location:";

            // txtLocation
            txtLocation.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            txtLocation.Location = new Point(140, 47);
            txtLocation.Name = "txtLocation";
            txtLocation.Size = new Size(716, 23);
            txtLocation.TabIndex = 0;
            txtLocation.TextChanged += new EventHandler(txtLocation_TextChanged);

            // lblCategory
            lblCategory.AutoSize = true;
            lblCategory.Location = new Point(12, 88);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(58, 15);
            lblCategory.TabIndex = 3;
            lblCategory.Text = "Category:";

            // cmbCategory
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.FormattingEnabled = true;
            cmbCategory.Items.AddRange(new object[] { "Sanitation", "Roads", "Utilities", "Parks", "Traffic", "Building Maintenance", "Other" });
            cmbCategory.Location = new Point(140, 85);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.Size = new Size(300, 23);
            cmbCategory.TabIndex = 1;
            cmbCategory.SelectedIndexChanged += new EventHandler(cmbCategory_SelectedIndexChanged);

            // lblDescription
            lblDescription.AutoSize = true;
            lblDescription.Location = new Point(12, 126);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(70, 15);
            lblDescription.TabIndex = 5;
            lblDescription.Text = "Description:";

            // rtDescription
            rtDescription.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            rtDescription.Location = new Point(140, 123);
            rtDescription.Name = "rtDescription";
            rtDescription.Size = new Size(716, 190);
            rtDescription.TabIndex = 2;
            rtDescription.Text = "";
            rtDescription.TextChanged += new EventHandler(rtDescription_TextChanged);

            // lblAttachment
            lblAttachment.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            lblAttachment.AutoSize = true;
            lblAttachment.Location = new Point(12, 334);
            lblAttachment.Name = "lblAttachment";
            lblAttachment.Size = new Size(70, 15);
            lblAttachment.TabIndex = 7;
            lblAttachment.Text = "Attachment:";

            // btnAttachFile
            btnAttachFile.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            btnAttachFile.BackColor = primary;
            btnAttachFile.Cursor = Cursors.Hand;
            btnAttachFile.FlatStyle = FlatStyle.Flat;
            btnAttachFile.ForeColor = Color.White;
            btnAttachFile.Location = new Point(140, 329);
            btnAttachFile.Name = "btnAttachFile";
            btnAttachFile.Size = new Size(150, 28);
            btnAttachFile.TabIndex = 3;
            btnAttachFile.Text = "Attach File";
            btnAttachFile.UseVisualStyleBackColor = false;
            btnAttachFile.Click += new EventHandler(btnAttachFile_Click);

            // lblAttachmentStatus
            lblAttachmentStatus.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            lblAttachmentStatus.AutoSize = true;
            lblAttachmentStatus.ForeColor = Color.Gray;
            lblAttachmentStatus.Location = new Point(305, 334);
            lblAttachmentStatus.Name = "lblAttachmentStatus";
            lblAttachmentStatus.Size = new Size(91, 15);
            lblAttachmentStatus.TabIndex = 9;
            lblAttachmentStatus.Text = "No file attached";

            body.Controls.Add(lblAttachmentStatus);
            body.Controls.Add(btnAttachFile);
            body.Controls.Add(lblAttachment);
            body.Controls.Add(rtDescription);
            body.Controls.Add(lblDescription);
            body.Controls.Add(cmbCategory);
            body.Controls.Add(lblCategory);
            body.Controls.Add(txtLocation);
            body.Controls.Add(lblLocation);
            body.Controls.Add(lblHint);

            // bottom
            bottom.BackColor = panelBack;
            bottom.Dock = DockStyle.Bottom;
            bottom.Name = "bottom";
            bottom.Size = new Size(880, 44);
            bottom.TabIndex = 2;

            // progressBar
            progressBar.Location = new Point(12, 11);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(180, 22);
            progressBar.TabIndex = 0;

            // lblEngagement
            lblEngagement.ForeColor = primary;
            lblEngagement.Location = new Point(204, 4);
            lblEngagement.Name = "lblEngagement";
            lblEngagement.Size = new Size(360, 36);
            lblEngagement.TabIndex = 1;
            lblEngagement.Text = "Start by entering the location of the issue.";
            lblEngagement.TextAlign = ContentAlignment.MiddleLeft;

            // btnSubmit
            btnSubmit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSubmit.BackColor = primary;
            btnSubmit.Cursor = Cursors.Hand;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.ForeColor = Color.White;
            btnSubmit.Location = new Point(568, 8);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(140, 28);
            btnSubmit.TabIndex = 4;
            btnSubmit.Text = "Submit Report";
            btnSubmit.UseVisualStyleBackColor = false;
            btnSubmit.Click += new EventHandler(btnSubmit_Click);

            // btnBack
            btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBack.BackColor = Color.Gray;
            btnBack.Cursor = Cursors.Hand;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.ForeColor = Color.White;
            btnBack.Location = new Point(718, 8);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(150, 28);
            btnBack.TabIndex = 5;
            btnBack.Text = "Back to Main Menu";
            btnBack.UseVisualStyleBackColor = false;
            btnBack.Click += new EventHandler(btnBack_Click);

            bottom.Controls.Add(btnBack);
            bottom.Controls.Add(btnSubmit);
            bottom.Controls.Add(lblEngagement);
            bottom.Controls.Add(progressBar);

            // ReportIssuesForm
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(Math.Min(880, workingArea.Width), Math.Min(600, workingArea.Height));
            Controls.Add(body);
            Controls.Add(bottom);
            Controls.Add(header);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(Math.Min(840, workingArea.Width), Math.Min(560, workingArea.Height));
            Name = "ReportIssuesForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Municipal Services - Report Issues";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
