namespace MunicipalServicesApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private Button btnReportIssues;
        private Button btnViewReports;
        private Button btnLocalEvents;
        private Button btnServiceStatus;

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
            lblTitle = new Label();
            btnReportIssues = new Button();
            btnViewReports = new Button();
            btnLocalEvents = new Button();
            btnServiceStatus = new Button();
            SuspendLayout();
            
            // lblTitle
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold);
            lblTitle.Location = new Point(200, 30);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(400, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Municipal Services Application";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            
            // btnReportIssues
            btnReportIssues.BackColor = Color.FromArgb(0, 120, 215);
            btnReportIssues.FlatStyle = FlatStyle.Flat;
            btnReportIssues.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnReportIssues.ForeColor = Color.White;
            btnReportIssues.Location = new Point(250, 90);
            btnReportIssues.Name = "btnReportIssues";
            btnReportIssues.Size = new Size(300, 50);
            btnReportIssues.TabIndex = 1;
            btnReportIssues.Text = "Report Issues";
            btnReportIssues.UseVisualStyleBackColor = false;
            btnReportIssues.Click += new EventHandler(btnReportIssues_Click);
            
            // btnViewReports
            btnViewReports.BackColor = Color.FromArgb(0, 150, 0);
            btnViewReports.FlatStyle = FlatStyle.Flat;
            btnViewReports.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnViewReports.ForeColor = Color.White;
            btnViewReports.Location = new Point(250, 155);
            btnViewReports.Name = "btnViewReports";
            btnViewReports.Size = new Size(300, 50);
            btnViewReports.TabIndex = 2;
            btnViewReports.Text = "View Reports";
            btnViewReports.UseVisualStyleBackColor = false;
            btnViewReports.Click += new EventHandler(btnViewReports_Click);
            
            // btnLocalEvents
            btnLocalEvents.BackColor = Color.Gray;
            btnLocalEvents.FlatStyle = FlatStyle.Flat;
            btnLocalEvents.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnLocalEvents.ForeColor = Color.White;
            btnLocalEvents.Location = new Point(250, 220);
            btnLocalEvents.Name = "btnLocalEvents";
            btnLocalEvents.Size = new Size(300, 50);
            btnLocalEvents.TabIndex = 3;
            btnLocalEvents.Text = "Local Events and Announcements";
            btnLocalEvents.UseVisualStyleBackColor = false;
            btnLocalEvents.Click += new EventHandler(btnComingSoon_Click);
            
            // btnServiceStatus
            btnServiceStatus.BackColor = Color.Gray;
            btnServiceStatus.FlatStyle = FlatStyle.Flat;
            btnServiceStatus.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnServiceStatus.ForeColor = Color.White;
            btnServiceStatus.Location = new Point(250, 285);
            btnServiceStatus.Name = "btnServiceStatus";
            btnServiceStatus.Size = new Size(300, 50);
            btnServiceStatus.TabIndex = 4;
            btnServiceStatus.Text = "Service Request Status";
            btnServiceStatus.UseVisualStyleBackColor = false;
            btnServiceStatus.Click += new EventHandler(btnComingSoon_Click);
            
            // Form1
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(800, 450);
            Controls.Add(btnServiceStatus);
            Controls.Add(btnLocalEvents);
            Controls.Add(btnViewReports);
            Controls.Add(btnReportIssues);
            Controls.Add(lblTitle);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Municipal Services - Main Menu";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
