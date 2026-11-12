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
        private Button btnLocalEvents;
        private Button btnServiceStatus;
        private Button btnExit;
        private Panel contentPanel;

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
            btnLocalEvents = new Button();
            btnServiceStatus = new Button();
            btnExit = new Button();
            contentPanel = new Panel();
            SuspendLayout();

            // contentPanel
            contentPanel.BackColor = Color.White;
            contentPanel.Location = new Point(0, 0);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new Size(800, 400);
            contentPanel.TabIndex = 0;

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

            // btnLocalEvents
            btnLocalEvents.BackColor = Color.FromArgb(0, 120, 215);
            btnLocalEvents.FlatStyle = FlatStyle.Flat;
            btnLocalEvents.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnLocalEvents.ForeColor = Color.White;
            btnLocalEvents.Location = new Point(250, 155);
            btnLocalEvents.Name = "btnLocalEvents";
            btnLocalEvents.Size = new Size(300, 50);
            btnLocalEvents.TabIndex = 2;
            btnLocalEvents.Text = "Local Events and Announcements";
            btnLocalEvents.UseVisualStyleBackColor = false;
            btnLocalEvents.Click += new EventHandler(btnLocalEvents_Click);

            // btnServiceStatus
            btnServiceStatus.BackColor = Color.FromArgb(0, 120, 215);
            btnServiceStatus.FlatStyle = FlatStyle.Flat;
            btnServiceStatus.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnServiceStatus.ForeColor = Color.White;
            btnServiceStatus.Location = new Point(250, 220);
            btnServiceStatus.Name = "btnServiceStatus";
            btnServiceStatus.Size = new Size(300, 50);
            btnServiceStatus.TabIndex = 3;
            btnServiceStatus.Text = "Service Request Status";
            btnServiceStatus.UseVisualStyleBackColor = false;
            btnServiceStatus.Click += new EventHandler(btnServiceStatus_Click);

            // btnExit
            btnExit.BackColor = Color.FromArgb(150, 50, 50);
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnExit.ForeColor = Color.White;
            btnExit.Location = new Point(250, 285);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(300, 50);
            btnExit.TabIndex = 4;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += new EventHandler(btnExit_Click);

            // Add controls to contentPanel
            contentPanel.Controls.Add(btnExit);
            contentPanel.Controls.Add(btnServiceStatus);
            contentPanel.Controls.Add(btnLocalEvents);
            contentPanel.Controls.Add(btnReportIssues);
            contentPanel.Controls.Add(lblTitle);

            // Form1
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 240, 240);
            ClientSize = new Size(800, 400);
            Controls.Add(contentPanel);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Municipal Services - Main Menu";
            Resize += new EventHandler(Form1_Resize);
            Load += new EventHandler(Form1_Load);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
