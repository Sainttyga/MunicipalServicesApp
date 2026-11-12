#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp
{
    /// <summary>
    /// Part 3: Service Request Status. Lists every request with its status, tracks a request by its unique ID
    /// and shows how the trees, heap and graphs in <see cref="ServiceRequestRepository"/> are used.
    /// </summary>
    public class ServiceRequestStatusForm : Form
    {
        private static readonly Color Primary = Color.FromArgb(0, 122, 204);
        private static readonly Font Mono = new Font("Consolas", 9.5F);

        private TextBox txtTrack;
        private Label lblQueryInfo;
        private ComboBox cmbStatus;
        private ComboBox cmbSubmitted;
        private ComboBox cmbSort;
        private TabControl tabs;
        private ListView lstRequests;
        private Label lblDetails;
        private ProgressBar progressRequest;
        private TextBox txtTimeline;
        private Button btnAdvance;
        private ListView lstUrgent;
        private Label lblHeapInfo;
        private TextBox txtGraph;
        private TextBox txtRoute;
        private ComboBox cmbTree;
        private ComboBox cmbTraversal;
        private TextBox txtTree;
        private Label lblSummary;

        private string selectedId;
        private bool loading;

        public ServiceRequestStatusForm() : this(null)
        {
        }

        public ServiceRequestStatusForm(string requestIdToTrack)
        {
            BuildUi();
            ServiceRequestRepository.Load(Form1.ReportedIssues);
            RefreshAll();
            if (!string.IsNullOrWhiteSpace(requestIdToTrack))
            {
                txtTrack.Text = requestIdToTrack;
                TrackRequest(false);
            }
        }

        // ------------------------------------------------------------------ layout

        private void BuildUi()
        {
            loading = true;
            Text = "Municipal Services - Service Request Status";
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            Size = new Size(Math.Min(1080, workingArea.Width), Math.Min(720, workingArea.Height));
            MinimumSize = new Size(Math.Min(960, workingArea.Width), Math.Min(640, workingArea.Height));
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9F);

            var header = new Panel { Dock = DockStyle.Top, Size = new Size(1080, 52), BackColor = Primary };
            header.Controls.Add(new Label
            {
                Text = "Service Request Status",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(14, 10)
            });

            var filterPanel = new Panel { Dock = DockStyle.Top, Size = new Size(1080, 74), BackColor = Color.FromArgb(240, 245, 250) };
            filterPanel.Controls.Add(new Label { Text = "Track by Request ID:", AutoSize = true, Location = new Point(12, 12) });
            txtTrack = new TextBox { Location = new Point(135, 9), Width = 190, CharacterCasing = CharacterCasing.Upper };
            txtTrack.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; TrackRequest(true); } };
            txtTrack.TextChanged += (s, e) => { if (!loading) PopulateRequests(); };
            var btnTrack = MakeButton("Track", new Point(332, 7), 70);
            btnTrack.Click += (s, e) => TrackRequest(true);

            filterPanel.Controls.Add(new Label { Text = "Status:", AutoSize = true, Location = new Point(420, 12) });
            cmbStatus = MakeCombo(new Point(470, 8), 130, new[] { "All statuses", "Open only", "Received", "Under Review", "Assigned", "In Progress", "Resolved" });
            filterPanel.Controls.Add(new Label { Text = "Submitted:", AutoSize = true, Location = new Point(612, 12) });
            cmbSubmitted = MakeCombo(new Point(680, 8), 120, new[] { "Any time", "Last 24 hours", "Last 3 days", "Last 7 days", "Last 30 days" });
            filterPanel.Controls.Add(new Label { Text = "Sort by:", AutoSize = true, Location = new Point(812, 12) });
            cmbSort = MakeCombo(new Point(865, 8), 190, new[] { "Request ID (AVL in-order)", "Newest first (Red-Black)", "Urgency (Min-Heap)" });
            foreach (var combo in new[] { cmbStatus, cmbSubmitted, cmbSort })
                combo.SelectedIndexChanged += (s, e) => { if (!loading) PopulateRequests(); };

            lblQueryInfo = new Label
            {
                AutoSize = false,
                Location = new Point(12, 42),
                Size = new Size(1040, 26),
                ForeColor = Primary,
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                Text = "Enter a request ID such as SR-20261109-0001, your issue number (e.g. 3), or a partial ID to see suggestions."
            };
            filterPanel.Controls.AddRange(new Control[] { txtTrack, btnTrack, cmbStatus, cmbSubmitted, cmbSort, lblQueryInfo });

            tabs = new TabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(BuildRequestsTab());
            tabs.TabPages.Add(BuildUrgentTab());
            tabs.TabPages.Add(BuildGraphTab());
            tabs.TabPages.Add(BuildRouteTab());
            tabs.TabPages.Add(BuildTreeTab());

            var bottom = new Panel { Dock = DockStyle.Bottom, Size = new Size(1080, 40), BackColor = Color.FromArgb(240, 245, 250) };
            lblSummary = new Label { AutoSize = false, Location = new Point(12, 4), Size = new Size(880, 32), TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right };
            var btnBack = MakeButton("Back to Main Menu", new Point(898, 4), 170);
            btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBack.Height = 32;
            btnBack.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnBack.BackColor = Color.FromArgb(90, 90, 90);
            btnBack.Click += (s, e) => Close();
            bottom.Controls.Add(lblSummary);
            bottom.Controls.Add(btnBack);

            // Docking is applied in reverse order of addition, so the fill control goes first.
            Controls.Add(tabs);
            Controls.Add(bottom);
            Controls.Add(filterPanel);
            Controls.Add(header);
            loading = false;
        }

        private TabPage BuildRequestsTab()
        {
            var page = new TabPage("All Requests");
            lstRequests = MakeListView(new[] { ("Request ID", 150), ("Category", 185), ("Area", 125), ("Priority", 70), ("Status", 95), ("Submitted", 120), ("Progress", 70) });
            lstRequests.SelectedIndexChanged += (s, e) =>
            {
                if (lstRequests.SelectedItems.Count == 1) ShowDetails((ServiceRequest)lstRequests.SelectedItems[0].Tag);
            };

            var details = new Panel { Dock = DockStyle.Bottom, Size = new Size(1080, 200), BackColor = Color.FromArgb(250, 250, 250) };
            lblDetails = new Label { Location = new Point(8, 6), Size = new Size(470, 160), Text = "Select a request to see its details and progress." };
            progressRequest = new ProgressBar { Location = new Point(8, 170), Size = new Size(470, 20) };
            txtTimeline = new TextBox
            {
                Location = new Point(490, 6),
                Size = new Size(380, 186),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = Mono,
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
            };
            btnAdvance = MakeButton("Simulate Next\nStatus Update", new Point(880, 6), 160);
            btnAdvance.Height = 50;
            btnAdvance.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdvance.Enabled = false;
            btnAdvance.Click += (s, e) => AdvanceSelected();
            details.Controls.AddRange(new Control[] { lblDetails, progressRequest, txtTimeline, btnAdvance });

            page.Controls.Add(lstRequests);
            page.Controls.Add(details);
            return page;
        }

        private TabPage BuildUrgentTab()
        {
            var page = new TabPage("Urgent Queue (Min-Heap)");
            var top = new Panel { Dock = DockStyle.Top, Size = new Size(1080, 70) };
            lblHeapInfo = new Label { Location = new Point(8, 6), Size = new Size(860, 60), Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right };
            var btnDispatch = MakeButton("Dispatch Most Urgent", new Point(880, 14), 160);
            btnDispatch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDispatch.BackColor = Color.FromArgb(200, 70, 50);
            btnDispatch.Click += (s, e) => DispatchMostUrgent();
            top.Controls.Add(lblHeapInfo);
            top.Controls.Add(btnDispatch);
            lstUrgent = MakeListView(new[] { ("#", 40), ("Request ID", 150), ("Priority", 70), ("Category", 185), ("Area", 125), ("Status", 95), ("Waiting", 90) });
            page.Controls.Add(lstUrgent);
            page.Controls.Add(top);
            return page;
        }

        private TabPage BuildGraphTab()
        {
            var page = new TabPage("Related Requests (Graph)");
            var top = new Panel { Dock = DockStyle.Top, Size = new Size(1080, 46) };
            top.Controls.Add(new Label
            {
                Text = "Requests are vertices. An edge joins two requests in the same area (weight 2), the same category (3) or both (1).\nSelect a request on the All Requests tab, then analyse it with breadth-first and depth-first search.",
                Location = new Point(8, 6),
                Size = new Size(860, 36)
            });
            var btnAnalyse = MakeButton("Analyse Selected", new Point(880, 8), 160);
            btnAnalyse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAnalyse.Click += (s, e) => AnalyseSelected();
            top.Controls.Add(btnAnalyse);
            txtGraph = MakeOutputBox();
            page.Controls.Add(txtGraph);
            page.Controls.Add(top);
            return page;
        }

        private TabPage BuildRouteTab()
        {
            var page = new TabPage("Crew Route (MST)");
            var top = new Panel { Dock = DockStyle.Top, Size = new Size(1080, 46) };
            top.Controls.Add(new Label
            {
                Text = "Plans the cheapest set of roads that links the municipal depot to every area with open requests\n(Dijkstra shortest paths + Prim's minimum spanning tree; Kruskal is used as a cross-check).",
                Location = new Point(8, 6),
                Size = new Size(860, 36)
            });
            var btnRecalc = MakeButton("Recalculate", new Point(880, 8), 160);
            btnRecalc.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRecalc.Click += (s, e) => txtRoute.Text = Lines(ServiceRequestRepository.DescribeCrewRoute());
            top.Controls.Add(btnRecalc);
            txtRoute = MakeOutputBox();
            page.Controls.Add(txtRoute);
            page.Controls.Add(top);
            return page;
        }

        private TabPage BuildTreeTab()
        {
            var page = new TabPage("Tree Explorer");
            var top = new Panel { Dock = DockStyle.Top, Size = new Size(1080, 40) };
            top.Controls.Add(new Label { Text = "Structure:", AutoSize = true, Location = new Point(8, 12) });
            cmbTree = MakeCombo(new Point(75, 8), 260, new[]
            {
                "Overview of all trees",
                "General tree (Category > Status > Request)",
                "Binary search tree (by ID, unbalanced)",
                "AVL tree (by ID)",
                "Red-black tree (by submission date)"
            });
            top.Controls.Add(new Label { Text = "Traversal:", AutoSize = true, Location = new Point(350, 12) });
            cmbTraversal = MakeCombo(new Point(415, 8), 130, new[] { "In-order", "Pre-order", "Post-order", "Level-order" });
            var btnShow = MakeButton("Show", new Point(560, 6), 90);
            btnShow.Click += (s, e) => txtTree.Text = Lines(DescribeTree());
            top.Controls.AddRange(new Control[] { cmbTree, cmbTraversal, btnShow });
            txtTree = MakeOutputBox();
            page.Controls.Add(txtTree);
            page.Controls.Add(top);
            return page;
        }

        private static Button MakeButton(string text, Point location, int width)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = new Size(width, 28),
                BackColor = Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
        }

        private static ComboBox MakeCombo(Point location, int width, string[] items)
        {
            var combo = new ComboBox { Location = location, Width = width, DropDownStyle = ComboBoxStyle.DropDownList };
            combo.Items.AddRange(items);
            combo.SelectedIndex = 0;
            return combo;
        }

        private static ListView MakeListView((string Title, int Width)[] columns)
        {
            var list = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = false,
                HideSelection = false
            };
            foreach (var c in columns) list.Columns.Add(c.Title, c.Width);
            return list;
        }

        private static TextBox MakeOutputBox()
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                Font = Mono,
                BackColor = Color.White
            };
        }

        /// <summary>TextBox needs Windows line endings.</summary>
        private static string Lines(string text) => text.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

        // ------------------------------------------------------------------ refresh

        private void RefreshAll()
        {
            PopulateRequests();
            PopulateUrgent();
            txtGraph.Text = Lines(BuildGraphOverview());
            txtRoute.Text = Lines(ServiceRequestRepository.DescribeCrewRoute());
            txtTree.Text = Lines(DescribeTree());
            lblSummary.Text = BuildSummary();
        }

        private void PopulateRequests()
        {
            var mode = (RequestSortMode)cmbSort.SelectedIndex;
            IEnumerable<ServiceRequest> rows = ServiceRequestRepository.GetSorted(mode);

            TimeSpan? window = SubmittedWindow();
            if (window.HasValue)
            {
                var inRange = ServiceRequestRepository.SubmittedBetween(DateTime.Now - window.Value, DateTime.Now, out int visited);
                var set = new HashSet<ServiceRequest>(inRange);
                rows = rows.Where(set.Contains);
                lblQueryInfo.Text = $"Red-black range query: {inRange.Count} request(s) submitted in the {cmbSubmitted.Text.ToLowerInvariant()}. " +
                                    $"Visited {visited} of {ServiceRequestRepository.ByDate.Count} tree nodes instead of scanning every request.";
            }

            string status = cmbStatus.Text;
            if (status == "Open only") rows = rows.Where(r => r.IsOpen);
            else if (status != "All statuses") rows = rows.Where(r => r.StatusText == status);

            string typed = txtTrack.Text.Trim();
            if (typed.Length > 0)
            {
                List<ServiceRequest> matches;
                // A plain number is the issue number shown when the issue was reported.
                if (int.TryParse(typed, out int issueNo))
                    matches = rows.Where(r => r.SourceIssue != null && r.SourceIssue.Id == issueNo).ToList();
                else
                {
                    var set = new HashSet<ServiceRequest>(ServiceRequestRepository.SearchByPrefix(typed));
                    matches = rows.Where(set.Contains).ToList();
                }
                rows = matches;
                lblQueryInfo.Text = $"{matches.Count} request(s) match \"{typed}\" (AVL prefix range query while you type). " +
                                    "Press Enter or Track to jump to a request.";
            }

            lstRequests.BeginUpdate();
            lstRequests.Items.Clear();
            foreach (var r in rows)
            {
                var item = new ListViewItem(new[]
                {
                    r.RequestId, r.Category, r.Area, r.Priority.ToString(), r.StatusText,
                    r.SubmittedDate.ToString("yyyy-MM-dd HH:mm"), r.ProgressPercent + "%"
                }) { Tag = r };
                if (!r.IsOpen) item.ForeColor = Color.ForestGreen;
                else if (r.Priority == RequestPriority.High) item.ForeColor = Color.Firebrick;
                lstRequests.Items.Add(item);
                if (r.RequestId == selectedId) item.Selected = true;
            }
            lstRequests.EndUpdate();
            if (lstRequests.SelectedItems.Count == 1) lstRequests.SelectedItems[0].EnsureVisible();
        }

        private TimeSpan? SubmittedWindow()
        {
            switch (cmbSubmitted.SelectedIndex)
            {
                case 1: return TimeSpan.FromDays(1);
                case 2: return TimeSpan.FromDays(3);
                case 3: return TimeSpan.FromDays(7);
                case 4: return TimeSpan.FromDays(30);
                default: return null;
            }
        }

        private void PopulateUrgent()
        {
            var ordered = ServiceRequestRepository.UrgentQueue.ToSortedList();
            lstUrgent.BeginUpdate();
            lstUrgent.Items.Clear();
            for (int i = 0; i < ordered.Count; i++)
            {
                var r = ordered[i];
                var waiting = DateTime.Now - r.SubmittedDate;
                var item = new ListViewItem(new[]
                {
                    (i + 1).ToString(), r.RequestId, r.Priority.ToString(), r.Category, r.Area, r.StatusText,
                    waiting.TotalDays >= 1 ? $"{waiting.TotalDays:0} day(s)" : $"{waiting.TotalHours:0} hour(s)"
                }) { Tag = r };
                if (r.Priority == RequestPriority.High) item.ForeColor = Color.Firebrick;
                lstUrgent.Items.Add(item);
            }
            lstUrgent.EndUpdate();

            var heap = ServiceRequestRepository.UrgentQueue;
            lblHeapInfo.Text = heap.IsEmpty
                ? "The heap is empty - every request has been resolved."
                : $"{heap.Count} open requests in a binary min-heap ordered by priority, then by age. The root (next to dispatch) is {heap.Peek().RequestId}.\n" +
                  "Heap array (level order): " + string.Join(", ", heap.RawArray.Take(8).Select(r => r.RequestId.Substring(r.RequestId.Length - 4))) +
                  (heap.Count > 8 ? ", ..." : "") + "    Peek O(1), dispatch (pop) O(log n).";
        }

        private string BuildSummary()
        {
            var avl = ServiceRequestRepository.ById;
            var bst = ServiceRequestRepository.ByIdUnbalanced;
            var rb = ServiceRequestRepository.ByDate;
            return $"Total: {ServiceRequestRepository.All.Count}   Open: {ServiceRequestRepository.OpenCount}   Resolved: {ServiceRequestRepository.ResolvedCount}   |   " +
                   $"AVL height {avl.Height()} vs plain BST height {bst.Height()}   |   Red-black height {rb.Height()}   |   " +
                   $"Heap size {ServiceRequestRepository.UrgentQueue.Count}   |   Relation graph {ServiceRequestRepository.RelationGraph.VertexCount} vertices / {ServiceRequestRepository.RelationGraph.EdgeCount} edges";
        }

        // ------------------------------------------------------------------ actions

        private void TrackRequest(bool showPopup)
        {
            string id = txtTrack.Text.Trim();
            if (id.Length == 0)
            {
                MessageBox.Show("Please enter a request ID to track.", "Track Request", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = ServiceRequestRepository.Track(id);
            if (result.Request == null)
            {
                var suggestions = ServiceRequestRepository.SearchByPrefix(id);
                if (suggestions.Count > 0)
                {
                    lblQueryInfo.Text = $"No exact match. {suggestions.Count} request(s) start with \"{id}\" (AVL range query): " +
                                        string.Join(", ", suggestions.Take(5).Select(r => r.RequestId)) + (suggestions.Count > 5 ? ", ..." : "");
                    SelectRequest(suggestions[0]);
                }
                else
                {
                    lblQueryInfo.Text = $"No request found with ID \"{id}\" ({result.AvlComparisons} AVL comparisons).";
                    if (showPopup)
                        MessageBox.Show($"No service request was found with the reference number {id}.\n\nCheck the number on your confirmation message and try again.",
                            "Request Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }

            var r = result.Request;
            lblQueryInfo.Text = $"Found {r.RequestId} in the AVL tree after {result.AvlComparisons} comparison(s). " +
                                $"The unbalanced BST needed {result.BstComparisons} and a linear scan {result.LinearComparisons}.";
            SelectRequest(r);
            if (showPopup)
            {
                MessageBox.Show($"{r.RequestId}\n\nCategory: {r.Category}\nLocation: {r.Location}\nStatus: {r.StatusText} ({r.ProgressPercent}% complete)\n" +
                                $"Last update: {r.History[r.History.Count - 1].Note}\n\n" +
                                $"Lookup cost: AVL {result.AvlComparisons}, BST {result.BstComparisons}, linear scan {result.LinearComparisons} comparisons.",
                    "Request Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SelectRequest(ServiceRequest r)
        {
            selectedId = r.RequestId;
            tabs.SelectedIndex = 0;
            bool visible = lstRequests.Items.Cast<ListViewItem>().Any(i => i.Tag == r);
            if (!visible)
            {
                loading = true;
                cmbStatus.SelectedIndex = 0;
                cmbSubmitted.SelectedIndex = 0;
                loading = false;
            }
            PopulateRequests();
            ShowDetails(r);
        }

        private void ShowDetails(ServiceRequest r)
        {
            selectedId = r.RequestId;
            lblDetails.Text =
                $"Request ID:   {r.RequestId}\n" +
                $"Category:     {r.Category}\n" +
                $"Location:     {r.Location}  ({r.Area})\n" +
                $"Priority:     {r.Priority}\n" +
                $"Status:       {r.StatusText}  -  {r.ProgressPercent}% complete\n" +
                $"Crew:         {r.AssignedCrew ?? "Not assigned yet"}\n" +
                $"Submitted:    {r.SubmittedDate:dd MMM yyyy HH:mm}\n" +
                $"Last update:  {r.LastUpdated:dd MMM yyyy HH:mm}\n" +
                $"Description:  {r.Description}";
            progressRequest.Value = r.ProgressPercent;

            var sb = new StringBuilder();
            var stages = Enum.GetValues(typeof(RequestStatus)).Cast<RequestStatus>();
            sb.AppendLine(string.Join(" > ", stages.Select(s => s == r.Status ? "[" + ServiceRequest.Describe(s) + "]" : ServiceRequest.Describe(s))));
            sb.AppendLine();
            foreach (var update in r.History.Reverse())
                sb.AppendLine($"{update.Timestamp:dd MMM HH:mm}  {ServiceRequest.Describe(update.Status),-12} {update.Note}");
            txtTimeline.Text = Lines(sb.ToString());
            btnAdvance.Enabled = r.IsOpen;
        }

        private void AdvanceSelected()
        {
            if (selectedId == null) return;
            if (ServiceRequestRepository.Advance(selectedId))
            {
                RefreshAll();
                var r = ServiceRequestRepository.Track(selectedId).Request;
                SaveIssueStatus(r);
                ShowDetails(r);
                lblQueryInfo.Text = $"{r.RequestId} moved to \"{r.StatusText}\". Every index (AVL, BST, red-black, heap, graphs) was rebuilt.";
            }
        }

        private void DispatchMostUrgent()
        {
            var r = ServiceRequestRepository.DispatchMostUrgent();
            if (r == null)
            {
                MessageBox.Show("There are no open requests to dispatch.", "Urgent Queue", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SaveIssueStatus(r);
            RefreshAll();
            MessageBox.Show($"Dispatched {r.RequestId} ({r.Priority} priority, {r.Category}, {r.Area}).\nNew status: {r.StatusText}.",
                "Most Urgent Request Dispatched", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AnalyseSelected()
        {
            if (selectedId == null)
            {
                MessageBox.Show("Select a request on the All Requests tab first.", "Related Requests", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var origin = ServiceRequestRepository.Track(selectedId).Request;
            var sb = new StringBuilder();
            sb.AppendLine($"RELATED REQUESTS FOR {origin.RequestId}  ({origin.Category}, {origin.Area})");
            sb.AppendLine(new string('=', 72));
            sb.AppendLine("Breadth-first search (queue) - hop distance from the selected request:");
            foreach (var group in ServiceRequestRepository.RelatedByBreadthFirst(origin.RequestId).Skip(1).GroupBy(kv => kv.Value))
            {
                sb.AppendLine($"  {group.Key} hop(s):");
                foreach (var kv in group)
                    sb.AppendLine($"    {kv.Key.RequestId}  {kv.Key.Category,-30} {kv.Key.Area,-17} {kv.Key.StatusText,-12} {Relation(origin, kv.Key)}");
            }
            sb.AppendLine();
            var cluster = ServiceRequestRepository.ClusterByDepthFirst(origin.RequestId);
            sb.AppendLine($"Depth-first search (stack) - visiting order of the whole cluster ({cluster.Count} requests):");
            sb.AppendLine("  " + string.Join(" > ", cluster.Select(r => r.RequestId.Substring(r.RequestId.Length - 4))));
            sb.AppendLine();
            var batch = ServiceRequestRepository.All.Where(r => r.IsOpen && r != origin && r.Area == origin.Area).ToList();
            sb.AppendLine("Batch suggestion: open requests a crew can handle on the same visit to " + origin.Area + ":");
            if (batch.Count == 0) sb.AppendLine("  (none)");
            foreach (var r in batch) sb.AppendLine($"  {r.RequestId}  {r.Category} - {r.StatusText}");
            txtGraph.Text = Lines(sb.ToString());
            tabs.SelectedIndex = 2;
        }

        private static string Relation(ServiceRequest a, ServiceRequest b)
        {
            bool area = a.Area == b.Area, category = a.Category == b.Category;
            if (area && category) return "same area + category";
            if (area) return "same area";
            if (category) return "same category";
            return "linked through another request";
        }

        private static string BuildGraphOverview()
        {
            var graph = ServiceRequestRepository.RelationGraph;
            var sb = new StringBuilder();
            sb.AppendLine($"REQUEST RELATION GRAPH: {graph.VertexCount} requests (vertices), {graph.EdgeCount} relations (edges)");
            sb.AppendLine(new string('=', 72));
            var components = graph.ConnectedComponents();
            sb.AppendLine($"Connected clusters found with depth-first search: {components.Count}");
            foreach (var c in components.OrderByDescending(c => c.Count))
                sb.AppendLine($"  [{c.Count}] " + string.Join(", ", c));
            sb.AppendLine();
            sb.AppendLine("Most connected requests (highest degree):");
            foreach (var v in graph.Vertices.OrderByDescending(graph.Degree).Take(5))
            {
                var r = ServiceRequestRepository.Track(v).Request;
                sb.AppendLine($"  {v}  {graph.Degree(v)} links   ({r.Category}, {r.Area})");
            }
            sb.AppendLine();
            sb.AppendLine("Strongest links (same area AND same category, weight 1) - ideal to fix together:");
            var strong = graph.Edges().Where(e => e.Weight == 1).ToList();
            if (strong.Count == 0) sb.AppendLine("  (none)");
            foreach (var e in strong) sb.AppendLine($"  {e.From} <-> {e.To}");
            sb.AppendLine();
            sb.AppendLine("Select a request on the All Requests tab and click 'Analyse Selected' for its BFS / DFS traversal.");
            return sb.ToString();
        }

        private string DescribeTree()
        {
            var avl = ServiceRequestRepository.ById;
            var bst = ServiceRequestRepository.ByIdUnbalanced;
            var rb = ServiceRequestRepository.ByDate;
            var general = ServiceRequestRepository.CategoryTree;
            var sb = new StringBuilder();

            switch (cmbTree.SelectedIndex)
            {
                case 1:
                    sb.AppendLine($"GENERAL TREE  -  {general.Count()} nodes, height {general.Height()}");
                    sb.AppendLine(new string('=', 72));
                    sb.AppendLine(general.Render(v => v));
                    sb.AppendLine(cmbTraversal.SelectedIndex == 3 ? "Level-order walk:" : "Pre-order walk:");
                    var walk = cmbTraversal.SelectedIndex == 3 ? general.LevelOrder() : general.PreOrder();
                    sb.AppendLine("  " + string.Join(" | ", walk.Select(n => n.Value)));
                    return sb.ToString();
                case 2:
                    return DescribeBinaryTree(bst, n => ShortId(n.Key),
                        "Keys arrive in date order and the IDs embed the date, so every new key is the largest: the tree becomes a chain.");
                case 3:
                    return DescribeBinaryTree(avl, n => $"{ShortId(n.Key)} (h={n.Height}, bf={AvlTree<string, ServiceRequest>.BalanceFactor(n)})",
                        $"The same keys stay balanced: {avl.Rotations} rotation(s) were needed while inserting.");
                case 4:
                    return DescribeBinaryTree(rb, n => $"{n.Value.SubmittedDate:dd MMM HH:mm} {ShortId(n.Value.RequestId)} {(n.IsRed ? "RED" : "BLACK")}",
                        $"Black height {rb.BlackHeight()}, {rb.Rotations} rotation(s); all red-black rules valid: {rb.IsValid()}.");
            }

            sb.AppendLine($"TREE INDEXES BUILT OVER {ServiceRequestRepository.All.Count} SERVICE REQUESTS");
            sb.AppendLine(new string('=', 72));
            sb.AppendLine($"{"Structure",-30}{"Key",-18}{"Nodes",7}{"Height",8}{"Rotations",11}");
            sb.AppendLine($"{"General tree",-30}{"category/status",-18}{general.Count(),7}{general.Height(),8}{"-",11}");
            sb.AppendLine($"{"Binary search tree",-30}{"request ID",-18}{bst.Count,7}{bst.Height(),8}{bst.Rotations,11}");
            sb.AppendLine($"{"AVL tree",-30}{"request ID",-18}{avl.Count,7}{avl.Height(),8}{avl.Rotations,11}");
            sb.AppendLine($"{"Red-black tree",-30}{"submitted date",-18}{rb.Count,7}{rb.Height(),8}{rb.Rotations,11}");
            sb.AppendLine();
            sb.AppendLine($"AVL balanced: {avl.IsBalanced()}    Red-black rules valid: {rb.IsValid()}    Red-black black height: {rb.BlackHeight()}");
            if (ServiceRequestRepository.All.Count > 0)
            {
                string newest = ServiceRequestRepository.ByIdUnbalanced.MaxKey();
                avl.TryGetValue(newest, out _, out int a);
                bst.TryGetValue(newest, out _, out int b);
                sb.AppendLine($"Finding the newest request ({newest}): AVL {a} comparisons, plain BST {b} comparisons.");
            }
            sb.AppendLine();
            sb.AppendLine("Choose a structure and traversal above and click 'Show' to draw it.");
            return sb.ToString();
        }

        private string DescribeBinaryTree(BinaryTreeBase<string, ServiceRequest> tree,
            Func<BinaryTreeNode<string, ServiceRequest>, string> label, string note)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{tree.Name.ToUpperInvariant()}  -  {tree.Count} nodes, height {tree.Height()}");
            sb.AppendLine(new string('=', 72));
            sb.AppendLine(note);
            sb.AppendLine("(R: = right child, L: = left child)");
            sb.AppendLine();
            sb.AppendLine(tree.Render(label));
            List<KeyValuePair<string, ServiceRequest>> order;
            switch (cmbTraversal.SelectedIndex)
            {
                case 1: order = tree.PreOrder(); break;
                case 2: order = tree.PostOrder(); break;
                case 3: order = tree.LevelOrder(); break;
                default: order = tree.InOrder(); break;
            }
            sb.AppendLine(cmbTraversal.Text + " traversal:");
            sb.AppendLine("  " + string.Join(" > ", order.Select(kv => ShortId(kv.Value.RequestId))));
            return sb.ToString();
        }

        /// <summary>SR-20261109-0001 is shown as 1109-0001 to keep the drawings narrow.</summary>
        private static string ShortId(string key)
        {
            int bar = key.IndexOf('|');
            if (bar >= 0) key = key.Substring(bar + 1);
            return key.Length >= 16 && key.StartsWith("SR-") ? key.Substring(7) : key;
        }

        /// <summary>Writes status changes of reported issues back to issues_data.json.</summary>
        private static void SaveIssueStatus(ServiceRequest r)
        {
            if (r != null && r.SourceIssue != null) IssueDataStorage.SaveIssues(Form1.ReportedIssues);
        }
    }
}
