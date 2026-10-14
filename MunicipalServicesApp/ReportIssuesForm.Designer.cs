namespace MunicipalServicesApp
{
    partial class ReportIssuesForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
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
        private Panel contentPanel;

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
            lblTitle = new Label();
            lblLocation = new Label();
            txtLocation = new TextBox();
            lblCategory = new Label();
            cmbCategory = new ComboBox();
            lblDescription = new Label();
            rtDescription = new RichTextBox();
            lblAttachment = new Label();
            btnAttachFile = new Button();
            lblAttachmentStatus = new Label();
            btnSubmit = new Button();
            btnBack = new Button();
            lblEngagement = new Label();
            progressBar = new ProgressBar();
            contentPanel = new Panel();
            SuspendLayout();

            // contentPanel
            contentPanel.BackColor = Color.White;
            contentPanel.Location = new Point(0, 0);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new Size(600, 450);
            contentPanel.TabIndex = 0;

            // lblTitle
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold);
            lblTitle.Location = new Point(150, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(300, 24);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Report an Issue";

            // lblLocation
            lblLocation.AutoSize = true;
            lblLocation.Font = new Font("Microsoft Sans Serif", 10F);
            lblLocation.Location = new Point(50, 70);
            lblLocation.Name = "lblLocation";
            lblLocation.Size = new Size(60, 17);
            lblLocation.TabIndex = 1;
            lblLocation.Text = "Location:";

            // txtLocation
            txtLocation.Font = new Font("Microsoft Sans Serif", 10F);
            txtLocation.Location = new Point(140, 67);
            txtLocation.Name = "txtLocation";
            txtLocation.Size = new Size(400, 23);
            txtLocation.TabIndex = 2;
            txtLocation.TextChanged += new EventHandler(txtLocation_TextChanged);

            // lblCategory
            lblCategory.AutoSize = true;
            lblCategory.Font = new Font("Microsoft Sans Serif", 10F);
            lblCategory.Location = new Point(50, 110);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(63, 17);
            lblCategory.TabIndex = 3;
            lblCategory.Text = "Category:";

            // cmbCategory
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.Font = new Font("Microsoft Sans Serif", 10F);
            cmbCategory.FormattingEnabled = true;
            cmbCategory.Items.AddRange(new object[] { "Sanitation", "Roads", "Utilities", "Parks", "Traffic", "Building Maintenance", "Other" });
            cmbCategory.Location = new Point(140, 107);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.Size = new Size(400, 24);
            cmbCategory.TabIndex = 4;
            cmbCategory.SelectedIndexChanged += new EventHandler(cmbCategory_SelectedIndexChanged);

            // lblDescription
            lblDescription.AutoSize = true;
            lblDescription.Font = new Font("Microsoft Sans Serif", 10F);
            lblDescription.Location = new Point(50, 150);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(77, 17);
            lblDescription.TabIndex = 5;
            lblDescription.Text = "Description:";

            // rtDescription
            rtDescription.Font = new Font("Microsoft Sans Serif", 10F);
            rtDescription.Location = new Point(140, 150);
            rtDescription.Name = "rtDescription";
            rtDescription.Size = new Size(400, 100);
            rtDescription.TabIndex = 6;
            rtDescription.Text = "";
            rtDescription.TextChanged += new EventHandler(rtDescription_TextChanged);

            // lblAttachment
            lblAttachment.AutoSize = true;
            lblAttachment.Font = new Font("Microsoft Sans Serif", 10F);
            lblAttachment.Location = new Point(50, 270);
            lblAttachment.Name = "lblAttachment";
            lblAttachment.Size = new Size(71, 17);
            lblAttachment.TabIndex = 7;
            lblAttachment.Text = "Attachment:";

            // btnAttachFile
            btnAttachFile.BackColor = Color.FromArgb(0, 120, 215);
            btnAttachFile.FlatStyle = FlatStyle.Flat;
            btnAttachFile.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnAttachFile.ForeColor = Color.White;
            btnAttachFile.Location = new Point(140, 265);
            btnAttachFile.Name = "btnAttachFile";
            btnAttachFile.Size = new Size(150, 30);
            btnAttachFile.TabIndex = 8;
            btnAttachFile.Text = "Attach File";
            btnAttachFile.UseVisualStyleBackColor = false;
            btnAttachFile.Click += new EventHandler(btnAttachFile_Click);

            // lblAttachmentStatus
            lblAttachmentStatus.AutoSize = true;
            lblAttachmentStatus.Font = new Font("Microsoft Sans Serif", 9F);
            lblAttachmentStatus.ForeColor = Color.Gray;
            lblAttachmentStatus.Location = new Point(300, 273);
            lblAttachmentStatus.Name = "lblAttachmentStatus";
            lblAttachmentStatus.Size = new Size(100, 15);
            lblAttachmentStatus.TabIndex = 9;
            lblAttachmentStatus.Text = "No file attached";

            // lblEngagement
            lblEngagement.AutoSize = true;
            lblEngagement.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Italic);
            lblEngagement.ForeColor = Color.FromArgb(0, 100, 0);
            lblEngagement.Location = new Point(50, 310);
            lblEngagement.Name = "lblEngagement";
            lblEngagement.Size = new Size(470, 17);
            lblEngagement.TabIndex = 10;
            lblEngagement.Text = "Thank you for helping improve our community!";

            // progressBar
            progressBar.Location = new Point(140, 340);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(400, 23);
            progressBar.TabIndex = 11;

            // btnSubmit
            btnSubmit.BackColor = Color.FromArgb(0, 150, 0);
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            btnSubmit.ForeColor = Color.White;
            btnSubmit.Location = new Point(140, 380);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(150, 40);
            btnSubmit.TabIndex = 12;
            btnSubmit.Text = "Submit";
            btnSubmit.UseVisualStyleBackColor = false;
            btnSubmit.Click += new EventHandler(btnSubmit_Click);

            // btnBack
            btnBack.BackColor = Color.FromArgb(100, 100, 100);
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            btnBack.ForeColor = Color.White;
            btnBack.Location = new Point(390, 380);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(150, 40);
            btnBack.TabIndex = 13;
            btnBack.Text = "Back to Main Menu";
            btnBack.UseVisualStyleBackColor = false;
            btnBack.Click += new EventHandler(btnBack_Click);

            // Add controls to contentPanel
            contentPanel.Controls.Add(btnBack);
            contentPanel.Controls.Add(btnSubmit);
            contentPanel.Controls.Add(progressBar);
            contentPanel.Controls.Add(lblEngagement);
            contentPanel.Controls.Add(lblAttachmentStatus);
            contentPanel.Controls.Add(btnAttachFile);
            contentPanel.Controls.Add(lblAttachment);
            contentPanel.Controls.Add(rtDescription);
            contentPanel.Controls.Add(lblDescription);
            contentPanel.Controls.Add(cmbCategory);
            contentPanel.Controls.Add(lblCategory);
            contentPanel.Controls.Add(txtLocation);
            contentPanel.Controls.Add(lblLocation);
            contentPanel.Controls.Add(lblTitle);

            // ReportIssuesForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 240, 240);
            ClientSize = new Size(600, 450);
            Controls.Add(contentPanel);
            Name = "ReportIssuesForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Report Issues";
            Resize += new EventHandler(ReportIssuesForm_Resize);
            Load += new EventHandler(ReportIssuesForm_Load);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
