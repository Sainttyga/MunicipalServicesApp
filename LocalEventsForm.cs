using MunicipalServicesApp.Models;

namespace MunicipalServicesApp
{
    public partial class LocalEventsForm : Form
    {
        private EventManager _eventManager;

        public LocalEventsForm()
        {
            InitializeComponent();
            _eventManager = new EventManager();

            // Wire up event handlers
            btnSearch.Click += btnSearch_Click;
            btnReset.Click += btnReset_Click;
            cmbCategoryFilter.SelectedIndexChanged += cmbCategoryFilter_SelectedIndexChanged;
            cmbDateFilter.SelectedIndexChanged += cmbDateFilter_SelectedIndexChanged;
            txtSearch.TextChanged += txtSearch_TextChanged;
            lstEvents.SelectedIndexChanged += lstEvents_SelectedIndexChanged;
            btnBack.Click += btnBack_Click;

            InitializeEventLists();
            LoadSampleData();
            LoadFilters();
            RefreshEventDisplay();
            LoadRecommendations();
        }

        // Initialize ListView columns
        private void InitializeEventLists()
        {
            // Main events list
            lstEvents.Columns.Clear();
            lstEvents.Columns.Add("ID", 50);
            lstEvents.Columns.Add("Title", 260);
            lstEvents.Columns.Add("Category", 150);
            lstEvents.Columns.Add("Date", 110);
            lstEvents.Columns.Add("Location", 240);

            // Recommendations list
            lstRecommendations.Columns.Clear();
            lstRecommendations.Columns.Add("ID", 50);
            lstRecommendations.Columns.Add("Title", 260);
            lstRecommendations.Columns.Add("Category", 150);
            lstRecommendations.Columns.Add("Date", 110);
            lstRecommendations.Columns.Add("Location", 240);
        }

        // Load sample events for demonstration
        private void LoadSampleData()
        {
            var existingEvents = _eventManager.GetAllEvents();
            if (existingEvents.Count > 0)
                return; // Data already exists

            var sampleEvents = new List<Event>
            {
                new Event
                {
                    Title = "Community Clean-Up Day",
                    Description = "Join us for a community clean-up initiative to keep our neighborhood beautiful.",
                    Category = "Community",
                    EventDate = DateTime.Now.AddDays(7),
                    Location = "Central Park",
                    Priority = 4
                },
                new Event
                {
                    Title = "Town Hall Meeting",
                    Description = "Monthly town hall meeting to discuss municipal services and community concerns.",
                    Category = "Government",
                    EventDate = DateTime.Now.AddDays(14),
                    Location = "City Hall",
                    Priority = 5
                },
                new Event
                {
                    Title = "Farmers Market",
                    Description = "Weekly farmers market featuring local produce and crafts.",
                    Category = "Community",
                    EventDate = DateTime.Now.AddDays(3),
                    Location = "Market Square",
                    Priority = 3
                },
                new Event
                {
                    Title = "Road Maintenance Announcement",
                    Description = "Main Street will be closed for maintenance from 8 AM to 5 PM.",
                    Category = "Announcement",
                    EventDate = DateTime.Now.AddDays(1),
                    Location = "Main Street",
                    Priority = 5
                },
                new Event
                {
                    Title = "Youth Sports Festival",
                    Description = "Annual youth sports competition featuring various athletic activities.",
                    Category = "Sports",
                    EventDate = DateTime.Now.AddDays(21),
                    Location = "Sports Complex",
                    Priority = 3
                },
                new Event
                {
                    Title = "Water Supply Notice",
                    Description = "Scheduled water maintenance in the northern district.",
                    Category = "Announcement",
                    EventDate = DateTime.Now.AddDays(5),
                    Location = "Northern District",
                    Priority = 4
                },
                new Event
                {
                    Title = "Art Exhibition",
                    Description = "Local artists showcase their work at the community gallery.",
                    Category = "Culture",
                    EventDate = DateTime.Now.AddDays(10),
                    Location = "Community Gallery",
                    Priority = 2
                },
                new Event
                {
                    Title = "Public Safety Workshop",
                    Description = "Learn about home safety and emergency preparedness.",
                    Category = "Education",
                    EventDate = DateTime.Now.AddDays(28),
                    Location = "Community Center",
                    Priority = 3
                }
            };

            foreach (var evt in sampleEvents)
            {
                _eventManager.AddEvent(evt);
            }
        }

        // Load filter dropdowns using Sets
        private void LoadFilters()
        {
            // DATA STRUCTURE: HashSet - O(1) uniqueness check, no duplicate categories
            var categories = _eventManager.GetUniqueCategories();
            cmbCategoryFilter.Items.Clear();
            cmbCategoryFilter.Items.Add("All Categories");
            foreach (var category in categories.OrderBy(c => c))
            {
                cmbCategoryFilter.Items.Add(category);
            }
            cmbCategoryFilter.SelectedIndex = 0;

            // DATA STRUCTURE: HashSet - O(1) uniqueness check, no duplicate dates
            var dates = _eventManager.GetUniqueDates();
            cmbDateFilter.Items.Clear();
            cmbDateFilter.Items.Add("All Dates");
            foreach (var date in dates.Where(d => d >= DateTime.Now.Date).OrderBy(d => d))
            {
                cmbDateFilter.Items.Add(date.ToString("dd MMM yyyy"));
            }
            cmbDateFilter.SelectedIndex = 0;
        }

        // Refresh event display using SortedDictionary for date ordering
        private void RefreshEventDisplay()
        {
            lstEvents.Items.Clear();

            var selectedCategory = cmbCategoryFilter.SelectedItem?.ToString() ?? "All Categories";
            var selectedDateStr = cmbDateFilter.SelectedItem?.ToString() ?? "All Dates";

            bool hasCategory = selectedCategory != "All Categories";
            bool hasDate = DateTime.TryParse(selectedDateStr, out DateTime selectedDate) && selectedDateStr != "All Dates";

            List<Event> events;
            if (hasCategory)
                // DATA STRUCTURE: Dictionary - O(1) category lookup
                events = _eventManager.GetEventsByCategory(selectedCategory);
            else if (hasDate)
                // DATA STRUCTURE: SortedDictionary - O(log n) date lookup
                events = _eventManager.GetEventsByDate(selectedDate);
            else
                // DATA STRUCTURE: SortedDictionary - O(log n) for ordered date-based retrieval
                events = _eventManager.GetEventsSortedByDate();

            // Apply the date filter on top of a category lookup when both are set
            if (hasCategory && hasDate)
            {
                events = events.Where(e => e.EventDate.Date == selectedDate.Date).ToList();
            }

            foreach (var evt in events)
            {
                var item = new ListViewItem(evt.Id.ToString());
                item.SubItems.Add(evt.Title);
                item.SubItems.Add(evt.Category);
                item.SubItems.Add(evt.EventDate.ToString("dd MMM yyyy"));
                item.SubItems.Add(evt.Location);
                lstEvents.Items.Add(item);
            }

            string filterInfo = "";
            if (selectedCategory != "All Categories")
                filterInfo += $"Category: {selectedCategory} ";
            if (selectedDateStr != "All Dates")
                filterInfo += $"Date: {selectedDateStr} ";
            if (string.IsNullOrEmpty(filterInfo))
                filterInfo = "All events";

            lblEvents.Text = $"Showing {events.Count} event(s) - {filterInfo}:";
            lblStatus.Text = "DATA STRUCTURES ACTIVE: SortedDictionary (date-ordered) | Dictionary (ID/category lookup) | HashSet (unique categories/dates)";
        }

        // Load recommendations based on user search history
        private void LoadRecommendations()
        {
            lstRecommendations.Items.Clear();

            // DATA STRUCTURE: Priority Queue - recommends high-priority events
            // DATA STRUCTURE: Queue - manages upcoming events for recommendations
            var recommendations = _eventManager.GetRecommendedEvents(5);

            if (recommendations.Count == 0)
            {
                // DATA STRUCTURE: Queue (FIFO) - no search history yet, so show upcoming events in queue order.
                var upcoming = _eventManager.GetUpcomingEvents(5);
                if (upcoming.Count == 0)
                {
                    var item = new ListViewItem("0");
                    item.SubItems.Add("Search for events to get personalized recommendations");
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    lstRecommendations.Items.Add(item);
                    lblStatus.Text = "RECOMMENDATION SYSTEM: No search history yet. Search events to get recommendations!";
                    return;
                }
                lblRecommendations.Text = "Upcoming events (queue order - search to get recommendations):";
                foreach (var evt in upcoming)
                {
                    var item = new ListViewItem(evt.Id.ToString());
                    item.SubItems.Add(evt.Title);
                    item.SubItems.Add(evt.Category);
                    item.SubItems.Add(evt.EventDate.ToString("dd MMM yyyy"));
                    item.SubItems.Add(evt.Location);
                    lstRecommendations.Items.Add(item);
                }
                lblStatus.Text = "QUEUE ACTIVE: FIFO upcoming events shown until a search is recorded | RECOMMENDATION SYSTEM: no search history yet.";
                return;
            }

            lblRecommendations.Text = "Recommended for you (based on your searches):";
            foreach (var evt in recommendations)
            {
                var item = new ListViewItem(evt.Id.ToString());
                item.SubItems.Add(evt.Title);
                item.SubItems.Add(evt.Category);
                item.SubItems.Add(evt.EventDate.ToString("dd MMM yyyy"));
                item.SubItems.Add(evt.Location);
                lstRecommendations.Items.Add(item);
            }

            var searchCount = _eventManager.GetSearchStatistics().Count;
            lblStatus.Text = $"RECOMMENDATION SYSTEM ACTIVE: Priority Queue (high-priority events) | Queue (upcoming events) | Dictionary (search patterns) | {searchCount} searches tracked";
        }

        // Search button click - uses Dictionary for efficient lookup
        private void btnSearch_Click(object? sender, EventArgs e)
        {
            PerformSearch();
        }

        // Real-time search as user types
        private void txtSearch_TextChanged(object? sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                // Live filtering while typing is not recorded; only the Search button records history.
                PerformSearch(recordSearch: false);
            }
            else
            {
                RefreshEventDisplay();
            }
        }

        // Perform search using EventManager
        private void PerformSearch(bool recordSearch = true)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                RefreshEventDisplay();
                return;
            }

            // DATA STRUCTURE: Dictionary (Hash Table) - O(1) lookup for search indexing
            var results = _eventManager.SearchEvents(searchTerm, recordSearch);

            lstEvents.Items.Clear();

            foreach (var evt in results)
            {
                var item = new ListViewItem(evt.Id.ToString());
                item.SubItems.Add(evt.Title);
                item.SubItems.Add(evt.Category);
                item.SubItems.Add(evt.EventDate.ToString("dd MMM yyyy"));
                item.SubItems.Add(evt.Location);
                lstEvents.Items.Add(item);
            }

            lblEvents.Text = $"Search results ({results.Count} found):";
            lblStatus.Text = $"SEARCH ACTIVE: Dictionary (Hash Table) for keyword matching | Dictionary tracking search frequency";

            // Refresh recommendations after search
            LoadRecommendations();
        }

        // Reset button click
        private void btnReset_Click(object? sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbCategoryFilter.SelectedIndex = 0;
            cmbDateFilter.SelectedIndex = 0;
            RefreshEventDisplay();
            lblStatus.Text = "Filters reset. Showing all events.";
        }

        // Category filter change
        private void cmbCategoryFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var selectedCategory = cmbCategoryFilter.SelectedItem?.ToString();
            if (selectedCategory != null && selectedCategory != "All Categories")
            {
                // Record category selection as a search for recommendations
                _eventManager.RecordSearch(selectedCategory);
            }
            RefreshEventDisplay();
        }

        // Date filter change
        private void cmbDateFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var selectedDate = cmbDateFilter.SelectedItem?.ToString();
            if (selectedDate != null && selectedDate != "All Dates")
            {
                // Record date selection as a search for recommendations
                _eventManager.RecordSearch(selectedDate);
            }
            RefreshEventDisplay();
        }

        // Event selection - uses Stack to track recently viewed
        private void lstEvents_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstEvents.SelectedItems.Count > 0)
            {
                int eventId = int.Parse(lstEvents.SelectedItems[0].Text);
                
                // DATA STRUCTURE: Stack (LIFO) - tracks recently viewed events
                var evt = _eventManager.GetEventById(eventId);

                if (evt != null)
                {
                    var recent = _eventManager.GetRecentlyViewedEvents(5);
                    lblStatus.Text = $"STACK ACTIVE (LIFO): {string.Join(" > ", recent.Select(e => e.Title))}  ({recent.Count} recently viewed, newest first)";
                }
            }
        }

        // Back button
        private void btnBack_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}
