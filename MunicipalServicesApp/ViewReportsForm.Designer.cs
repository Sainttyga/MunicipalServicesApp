namespace MunicipalServicesApp
{
    partial class ViewReportsForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private DataGridView dataGridViewReports;
        private Label lblReportCount;
        private Button btnRefresh;
        private Button btnViewDetails;
        private Button btnBack;

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
            dataGridViewReports = new DataGridView();
            lblReportCount = new Label();
            btnRefresh = new Button();
            btnViewDetails = new Button();
            btnBack = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewReports).BeginInit();
            SuspendLayout();

            // lblTitle
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold);
            lblTitle.Location = new Point(200, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(200, 24);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "View Reported Issues";

            // dataGridViewReports
            dataGridViewReports.AllowUserToAddRows = false;
            dataGridViewReports.AllowUserToDeleteRows = false;
            dataGridViewReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewReports.BackgroundColor = Color.White;
            dataGridViewReports.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewReports.Location = new Point(50, 60);
            dataGridViewReports.Name = "dataGridViewReports";
            dataGridViewReports.ReadOnly = true;
            dataGridViewReports.RowHeadersVisible = false;
            dataGridViewReports.RowTemplate.Height = 25;
            dataGridViewReports.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewReports.Size = new Size(700, 300);
            dataGridViewReports.TabIndex = 1;

            // lblReportCount
            lblReportCount.AutoSize = true;
            lblReportCount.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            lblReportCount.Location = new Point(50, 370);
            lblReportCount.Name = "lblReportCount";
            lblReportCount.Size = new Size(120, 17);
            lblReportCount.TabIndex = 2;
            lblReportCount.Text = "Total Reports: 0";

            // btnRefresh
            btnRefresh.BackColor = Color.FromArgb(0, 120, 215);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(50, 400);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(120, 35);
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += new EventHandler(btnRefresh_Click);

            // btnViewDetails
            btnViewDetails.BackColor = Color.FromArgb(0, 150, 0);
            btnViewDetails.FlatStyle = FlatStyle.Flat;
            btnViewDetails.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnViewDetails.ForeColor = Color.White;
            btnViewDetails.Location = new Point(180, 400);
            btnViewDetails.Name = "btnViewDetails";
            btnViewDetails.Size = new Size(120, 35);
            btnViewDetails.TabIndex = 4;
            btnViewDetails.Text = "View Details";
            btnViewDetails.UseVisualStyleBackColor = false;
            btnViewDetails.Click += new EventHandler(btnViewDetails_Click);

            // btnBack
            btnBack.BackColor = Color.FromArgb(100, 100, 100);
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnBack.ForeColor = Color.White;
            btnBack.Location = new Point(630, 400);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(120, 35);
            btnBack.TabIndex = 5;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = false;
            btnBack.Click += new EventHandler(btnBack_Click);

            // ViewReportsForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(800, 450);
            Controls.Add(btnBack);
            Controls.Add(btnViewDetails);
            Controls.Add(btnRefresh);
            Controls.Add(lblReportCount);
            Controls.Add(dataGridViewReports);
            Controls.Add(lblTitle);
            Name = "ViewReportsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "View Reported Issues";
            Load += ViewReportsForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewReports).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
