namespace MunicipalServicesApp
{
    partial class LocalEventsForm
    {
        private System.ComponentModel.IContainer components = null;

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
            Panel filterPanel = new Panel();
            lblSearch = new Label();
            txtSearch = new TextBox();
            btnSearch = new Button();
            btnReset = new Button();
            Label lblCategory = new Label();
            cmbCategoryFilter = new ComboBox();
            Label lblDate = new Label();
            cmbDateFilter = new ComboBox();
            lblEvents = new Label();
            Panel eventsHost = new Panel();
            lstEvents = new ListView();
            Panel recommendationsPanel = new Panel();
            lblRecommendations = new Label();
            lstRecommendations = new ListView();
            Panel bottom = new Panel();
            lblStatus = new Label();
            btnBack = new Button();
            SuspendLayout();

            // header
            header.BackColor = primary;
            header.Dock = DockStyle.Top;
            header.Name = "header";
            header.Size = new Size(1080, 52);
            header.TabIndex = 0;

            // headerLabel
            headerLabel.AutoSize = true;
            headerLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            headerLabel.ForeColor = Color.White;
            headerLabel.Location = new Point(14, 10);
            headerLabel.Name = "headerLabel";
            headerLabel.Size = new Size(365, 30);
            headerLabel.TabIndex = 0;
            headerLabel.Text = "Local Events and Announcements";
            header.Controls.Add(headerLabel);

            // filterPanel
            filterPanel.BackColor = panelBack;
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Name = "filterPanel";
            filterPanel.Size = new Size(1080, 74);
            filterPanel.TabIndex = 1;

            // lblSearch
            lblSearch.AutoSize = true;
            lblSearch.Location = new Point(12, 15);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(45, 15);
            lblSearch.TabIndex = 0;
            lblSearch.Text = "Search:";

            // txtSearch
            txtSearch.Location = new Point(68, 11);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(190, 23);
            txtSearch.TabIndex = 0;

            // btnSearch
            btnSearch.BackColor = primary;
            btnSearch.Cursor = Cursors.Hand;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.ForeColor = Color.White;
            btnSearch.Location = new Point(266, 9);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(80, 28);
            btnSearch.TabIndex = 1;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = false;

            // btnReset
            btnReset.BackColor = Color.Gray;
            btnReset.Cursor = Cursors.Hand;
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.ForeColor = Color.White;
            btnReset.Location = new Point(354, 9);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(75, 28);
            btnReset.TabIndex = 2;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = false;

            // lblCategory
            lblCategory.AutoSize = true;
            lblCategory.Location = new Point(455, 15);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(58, 15);
            lblCategory.TabIndex = 5;
            lblCategory.Text = "Category:";

            // cmbCategoryFilter
            cmbCategoryFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategoryFilter.FormattingEnabled = true;
            cmbCategoryFilter.Location = new Point(525, 11);
            cmbCategoryFilter.Name = "cmbCategoryFilter";
            cmbCategoryFilter.Size = new Size(170, 23);
            cmbCategoryFilter.TabIndex = 3;

            // lblDate
            lblDate.AutoSize = true;
            lblDate.Location = new Point(711, 15);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(34, 15);
            lblDate.TabIndex = 7;
            lblDate.Text = "Date:";

            // cmbDateFilter
            cmbDateFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDateFilter.FormattingEnabled = true;
            cmbDateFilter.Location = new Point(751, 11);
            cmbDateFilter.Name = "cmbDateFilter";
            cmbDateFilter.Size = new Size(140, 23);
            cmbDateFilter.TabIndex = 4;

            // lblEvents
            lblEvents.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            lblEvents.ForeColor = primary;
            lblEvents.Location = new Point(12, 45);
            lblEvents.Name = "lblEvents";
            lblEvents.Size = new Size(1056, 22);
            lblEvents.TabIndex = 9;
            lblEvents.Text = "Showing all events:";

            filterPanel.Controls.Add(lblEvents);
            filterPanel.Controls.Add(cmbDateFilter);
            filterPanel.Controls.Add(lblDate);
            filterPanel.Controls.Add(cmbCategoryFilter);
            filterPanel.Controls.Add(lblCategory);
            filterPanel.Controls.Add(btnReset);
            filterPanel.Controls.Add(btnSearch);
            filterPanel.Controls.Add(txtSearch);
            filterPanel.Controls.Add(lblSearch);

            // eventsHost
            eventsHost.Dock = DockStyle.Fill;
            eventsHost.Name = "eventsHost";
            eventsHost.Padding = new Padding(8, 4, 8, 8);
            eventsHost.Size = new Size(1080, 404);
            eventsHost.TabIndex = 2;

            // lstEvents
            lstEvents.Dock = DockStyle.Fill;
            lstEvents.FullRowSelect = true;
            lstEvents.GridLines = true;
            lstEvents.HideSelection = false;
            lstEvents.Location = new Point(8, 4);
            lstEvents.MultiSelect = false;
            lstEvents.Name = "lstEvents";
            lstEvents.Size = new Size(1064, 392);
            lstEvents.TabIndex = 0;
            lstEvents.UseCompatibleStateImageBehavior = false;
            lstEvents.View = View.Details;
            eventsHost.Controls.Add(lstEvents);

            // recommendationsPanel
            recommendationsPanel.BackColor = Color.FromArgb(250, 250, 250);
            recommendationsPanel.Dock = DockStyle.Bottom;
            recommendationsPanel.Name = "recommendationsPanel";
            recommendationsPanel.Padding = new Padding(8);
            recommendationsPanel.Size = new Size(1080, 190);
            recommendationsPanel.TabIndex = 3;

            // lblRecommendations
            lblRecommendations.Dock = DockStyle.Top;
            lblRecommendations.Name = "lblRecommendations";
            lblRecommendations.Size = new Size(1064, 24);
            lblRecommendations.TabIndex = 0;
            lblRecommendations.Text = "Recommended for you (based on your searches):";
            lblRecommendations.TextAlign = ContentAlignment.MiddleLeft;

            // lstRecommendations
            lstRecommendations.Dock = DockStyle.Fill;
            lstRecommendations.FullRowSelect = true;
            lstRecommendations.GridLines = true;
            lstRecommendations.HideSelection = false;
            lstRecommendations.Location = new Point(8, 32);
            lstRecommendations.MultiSelect = false;
            lstRecommendations.Name = "lstRecommendations";
            lstRecommendations.Size = new Size(1064, 150);
            lstRecommendations.TabIndex = 0;
            lstRecommendations.UseCompatibleStateImageBehavior = false;
            lstRecommendations.View = View.Details;

            recommendationsPanel.Controls.Add(lstRecommendations);
            recommendationsPanel.Controls.Add(lblRecommendations);

            // bottom
            bottom.BackColor = panelBack;
            bottom.Dock = DockStyle.Bottom;
            bottom.Name = "bottom";
            bottom.Size = new Size(1080, 40);
            bottom.TabIndex = 4;

            // lblStatus
            lblStatus.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            lblStatus.Location = new Point(12, 4);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(890, 32);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Ready";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;

            // btnBack
            btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBack.BackColor = Color.Gray;
            btnBack.Cursor = Cursors.Hand;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.ForeColor = Color.White;
            btnBack.Location = new Point(918, 6);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(150, 28);
            btnBack.TabIndex = 1;
            btnBack.Text = "Back to Main Menu";
            btnBack.UseVisualStyleBackColor = false;

            bottom.Controls.Add(btnBack);
            bottom.Controls.Add(lblStatus);

            // LocalEventsForm
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(Math.Min(1080, workingArea.Width), Math.Min(720, workingArea.Height));
            Controls.Add(eventsHost);
            Controls.Add(recommendationsPanel);
            Controls.Add(bottom);
            Controls.Add(filterPanel);
            Controls.Add(header);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(Math.Min(960, workingArea.Width), Math.Min(640, workingArea.Height));
            Name = "LocalEventsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Municipal Services - Local Events and Announcements";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
