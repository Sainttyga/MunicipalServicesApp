namespace MunicipalServicesApp
{
    partial class LocalEventsForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private Label lblSearch;
        private TextBox txtSearch;
        private ComboBox cmbCategoryFilter;
        private ComboBox cmbDateFilter;
        private Button btnSearch;
        private Button btnReset;
        private Label lblEvents;
        private ListView lstEvents;
        private Label lblRecommendations;
        private ListView lstRecommendations;
        private Button btnBack;
        private Label lblStatus;
        private GroupBox grpFilters;
        private GroupBox grpEvents;
        private GroupBox grpRecommendations;
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
            grpFilters = new GroupBox();
            lblSearch = new Label();
            txtSearch = new TextBox();
            cmbCategoryFilter = new ComboBox();
            cmbDateFilter = new ComboBox();
            btnSearch = new Button();
            btnReset = new Button();
            grpEvents = new GroupBox();
            lblEvents = new Label();
            lstEvents = new ListView();
            grpRecommendations = new GroupBox();
            lblRecommendations = new Label();
            lstRecommendations = new ListView();
            btnBack = new Button();
            lblStatus = new Label();
            contentPanel = new Panel();
            grpFilters.SuspendLayout();
            grpEvents.SuspendLayout();
            grpRecommendations.SuspendLayout();
            SuspendLayout();

            // contentPanel
            contentPanel.BackColor = Color.White;
            contentPanel.Location = new Point(0, 0);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new Size(600, 650);
            contentPanel.TabIndex = 0;

            // lblTitle
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold);
            lblTitle.Location = new Point(150, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(320, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Local Events and Announcements";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // grpFilters
            grpFilters.Controls.Add(lblSearch);
            grpFilters.Controls.Add(txtSearch);
            grpFilters.Controls.Add(cmbCategoryFilter);
            grpFilters.Controls.Add(cmbDateFilter);
            grpFilters.Controls.Add(btnSearch);
            grpFilters.Controls.Add(btnReset);
            grpFilters.Location = new Point(20, 55);
            grpFilters.Name = "grpFilters";
            grpFilters.Size = new Size(560, 100);
            grpFilters.TabIndex = 1;
            grpFilters.TabStop = false;
            grpFilters.Text = "Search and Filter";

            // lblSearch
            lblSearch.AutoSize = true;
            lblSearch.Location = new Point(10, 25);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(50, 20);
            lblSearch.TabIndex = 0;
            lblSearch.Text = "Search:";

            // txtSearch
            txtSearch.Location = new Point(65, 22);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(200, 27);
            txtSearch.TabIndex = 1;

            // cmbCategoryFilter
            cmbCategoryFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategoryFilter.Location = new Point(280, 22);
            cmbCategoryFilter.Name = "cmbCategoryFilter";
            cmbCategoryFilter.Size = new Size(150, 28);
            cmbCategoryFilter.TabIndex = 2;

            // cmbDateFilter
            cmbDateFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDateFilter.Location = new Point(440, 22);
            cmbDateFilter.Name = "cmbDateFilter";
            cmbDateFilter.Size = new Size(110, 28);
            cmbDateFilter.TabIndex = 3;

            // btnSearch
            btnSearch.BackColor = Color.FromArgb(0, 120, 215);
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnSearch.ForeColor = Color.White;
            btnSearch.Location = new Point(65, 60);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(100, 30);
            btnSearch.TabIndex = 4;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = false;
            btnSearch.Click += new EventHandler(btnSearch_Click);

            // btnReset
            btnReset.BackColor = Color.FromArgb(128, 128, 128);
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnReset.ForeColor = Color.White;
            btnReset.Location = new Point(180, 60);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(100, 30);
            btnReset.TabIndex = 5;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = false;
            btnReset.Click += new EventHandler(btnReset_Click);

            // grpEvents
            grpEvents.Controls.Add(lblEvents);
            grpEvents.Controls.Add(lstEvents);
            grpEvents.Location = new Point(20, 165);
            grpEvents.Name = "grpEvents";
            grpEvents.Size = new Size(560, 220);
            grpEvents.TabIndex = 2;
            grpEvents.TabStop = false;
            grpEvents.Text = "Upcoming Events";

            // lblEvents
            lblEvents.AutoSize = true;
            lblEvents.Location = new Point(10, 20);
            lblEvents.Name = "lblEvents";
            lblEvents.Size = new Size(130, 20);
            lblEvents.TabIndex = 0;
            lblEvents.Text = "Showing all events:";

            // lstEvents
            lstEvents.FullRowSelect = true;
            lstEvents.GridLines = true;
            lstEvents.Location = new Point(10, 45);
            lstEvents.Name = "lstEvents";
            lstEvents.Size = new Size(540, 160);
            lstEvents.TabIndex = 1;
            lstEvents.UseCompatibleStateImageBehavior = false;
            lstEvents.View = View.Details;

            // grpRecommendations
            grpRecommendations.Controls.Add(lblRecommendations);
            grpRecommendations.Controls.Add(lstRecommendations);
            grpRecommendations.Location = new Point(20, 395);
            grpRecommendations.Name = "grpRecommendations";
            grpRecommendations.Size = new Size(560, 150);
            grpRecommendations.TabIndex = 3;
            grpRecommendations.TabStop = false;
            grpRecommendations.Text = "Recommended Events (Based on your searches)";

            // lblRecommendations
            lblRecommendations.AutoSize = true;
            lblRecommendations.Location = new Point(10, 20);
            lblRecommendations.Name = "lblRecommendations";
            lblRecommendations.Size = new Size(250, 20);
            lblRecommendations.TabIndex = 0;
            lblRecommendations.Text = "Events you might be interested in:";

            // lstRecommendations
            lstRecommendations.FullRowSelect = true;
            lstRecommendations.GridLines = true;
            lstRecommendations.Location = new Point(10, 45);
            lstRecommendations.Name = "lstRecommendations";
            lstRecommendations.Size = new Size(540, 90);
            lstRecommendations.TabIndex = 1;
            lstRecommendations.UseCompatibleStateImageBehavior = false;
            lstRecommendations.View = View.Details;

            // btnBack
            btnBack.BackColor = Color.FromArgb(150, 50, 50);
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnBack.ForeColor = Color.White;
            btnBack.Location = new Point(20, 555);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(150, 40);
            btnBack.TabIndex = 4;
            btnBack.Text = "Back to Menu";
            btnBack.UseVisualStyleBackColor = false;
            btnBack.Click += new EventHandler(btnBack_Click);

            // lblStatus
            lblStatus.AutoSize = false;
            lblStatus.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Italic);
            lblStatus.ForeColor = Color.FromArgb(0, 100, 0);
            lblStatus.Location = new Point(20, 605);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(540, 25);
            lblStatus.TabIndex = 5;
            lblStatus.Text = "Ready";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;

            // Add controls to contentPanel
            contentPanel.Controls.Add(lblStatus);
            contentPanel.Controls.Add(btnBack);
            contentPanel.Controls.Add(grpRecommendations);
            contentPanel.Controls.Add(grpEvents);
            contentPanel.Controls.Add(grpFilters);
            contentPanel.Controls.Add(lblTitle);

            // LocalEventsForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 240, 240);
            ClientSize = new Size(600, 650);
            Controls.Add(contentPanel);
            Name = "LocalEventsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Local Events and Announcements";
            Resize += new EventHandler(LocalEventsForm_Resize);
            Load += new EventHandler(LocalEventsForm_Load);
            grpFilters.ResumeLayout(false);
            grpFilters.PerformLayout();
            grpEvents.ResumeLayout(false);
            grpEvents.PerformLayout();
            grpRecommendations.ResumeLayout(false);
            grpRecommendations.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
