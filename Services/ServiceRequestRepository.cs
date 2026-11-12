#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    public enum RequestSortMode
    {
        ByIdAvl,
        NewestFirstRedBlack,
        UrgencyHeap
    }

    public class TrackResult
    {
        public ServiceRequest Request { get; set; }
        public int AvlComparisons { get; set; }
        public int BstComparisons { get; set; }
        public int LinearComparisons { get; set; }
    }

    public class CrewRoutePlan
    {
        public List<string> AreasWithOpenRequests { get; } = new List<string>();
        public List<GraphEdge<string>> PrimEdges { get; } = new List<GraphEdge<string>>();
        public Dictionary<string, List<string>> RoadsForEdge { get; } = new Dictionary<string, List<string>>();
        public double PrimTotalKm { get; set; }
        public List<GraphEdge<string>> KruskalNetworkEdges { get; } = new List<GraphEdge<string>>();
        public double KruskalNetworkKm { get; set; }
        public double PrimNetworkKm { get; set; }
    }

    /// <summary>
    /// Owns every service request and keeps all the index structures in step:
    ///   AVL tree (by ID)            - tracking by unique reference number, ID-ordered list
    ///   plain BST (by ID)           - baseline that shows why balancing matters
    ///   red-black tree (by date)    - newest-first list and submitted-date range filter
    ///   min-heap (urgency)          - which open request to dispatch next
    ///   general tree                - Category > Status > Request overview
    ///   relation graph              - requests sharing an area or category (BFS / DFS)
    ///   road graph                  - Dijkstra distances and Prim / Kruskal crew routes
    /// </summary>
    public static class ServiceRequestRepository
    {
        public const string Depot = "Municipal Depot";

        // Declared before RoadNetwork: static fields initialise in textual order.
        public static readonly string[] Areas =
        {
            Depot, "Johannesburg CBD", "Braamfontein", "Soweto", "Sandton", "Randburg",
            "Roodepoort", "Midrand", "Alexandra", "Germiston", "Pretoria"
        };

        private static readonly List<ServiceRequest> requests = new List<ServiceRequest>();
        private static readonly HashSet<string> importedIssues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static bool seeded;

        public static AvlTree<string, ServiceRequest> ById { get; private set; } = new AvlTree<string, ServiceRequest>();
        public static BinarySearchTree<string, ServiceRequest> ByIdUnbalanced { get; private set; } = new BinarySearchTree<string, ServiceRequest>();
        public static RedBlackTree<string, ServiceRequest> ByDate { get; private set; } = new RedBlackTree<string, ServiceRequest>();
        public static MinHeap<ServiceRequest> UrgentQueue { get; private set; } = new MinHeap<ServiceRequest>(ServiceRequest.CompareUrgency);
        public static GeneralTreeNode<string> CategoryTree { get; private set; } = new GeneralTreeNode<string>("Service Requests");
        public static WeightedGraph<string> RelationGraph { get; private set; } = new WeightedGraph<string>();
        public static WeightedGraph<string> RoadNetwork { get; } = BuildRoadNetwork();

        public static IReadOnlyList<ServiceRequest> All => requests;
        public static int OpenCount => requests.Count(r => r.IsOpen);
        public static int ResolvedCount => requests.Count(r => !r.IsOpen);

        // ------------------------------------------------------------------ loading

        /// <summary>Seeds the demo requests once and imports any Part 1 issues not seen yet.</summary>
        public static void Load(IEnumerable<Issue> reportedIssues)
        {
            if (!seeded)
            {
                SeedDemoData(DateTime.Now);
                seeded = true;
            }
            if (reportedIssues != null)
                foreach (var issue in reportedIssues) ImportIssue(issue);
            Rebuild();
        }

        /// <summary>Clears everything (used by the unit tests).</summary>
        public static void Reset()
        {
            requests.Clear();
            importedIssues.Clear();
            seeded = false;
            Rebuild();
        }

        public static void AddRequest(ServiceRequest request)
        {
            if (ById.Contains(request.RequestId))
                throw new InvalidOperationException("A request with ID " + request.RequestId + " already exists.");
            requests.Add(request);
            Rebuild();
        }

        /// <summary>Unique request ID for a reported issue: SR-{date reported}-{issue number}, e.g. SR-20261109-0007.</summary>
        public static string RequestIdFor(Issue issue)
        {
            return "SR-" + issue.ReportedDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "-" + issue.Id.ToString("D4", CultureInfo.InvariantCulture);
        }

        private static void ImportIssue(Issue issue)
        {
            if (issue == null) return;
            string id = RequestIdFor(issue);
            if (!importedIssues.Add(id)) return;
            if (requests.Any(r => r.RequestId.Equals(id, StringComparison.OrdinalIgnoreCase))) return;
            var request = new ServiceRequest(id, issue.Category, issue.Location,
                DetectArea(issue.Location), issue.Description, issue.ReportedDate, PriorityFor(issue.Category));
            // Restore progress saved in issues_data.json ("Pending" means Received).
            RequestStatus saved = StatusFromText(issue.Status);
            DateTime stamp = issue.ReportedDate;
            while (request.Status < saved)
            {
                stamp = stamp.AddMinutes(1);
                request.Advance(null, stamp);
            }
            request.SourceIssue = issue;
            requests.Add(request);
        }

        public static RequestStatus StatusFromText(string status)
        {
            foreach (RequestStatus s in Enum.GetValues(typeof(RequestStatus)))
                if (string.Equals(ServiceRequest.Describe(s), status, StringComparison.OrdinalIgnoreCase)) return s;
            return RequestStatus.Received;
        }

        /// <summary>Rebuilds every index from the master list: O(n log n) for the trees and heap, O(n²) for the relation graph.</summary>
        public static void Rebuild()
        {
            var byId = new AvlTree<string, ServiceRequest>();
            var bst = new BinarySearchTree<string, ServiceRequest>();
            var byDate = new RedBlackTree<string, ServiceRequest>();
            foreach (var r in requests.OrderBy(r => r.SubmittedDate))
            {
                byId.Insert(r.RequestId.ToUpperInvariant(), r);
                bst.Insert(r.RequestId.ToUpperInvariant(), r);
                byDate.Insert(DateKey(r.SubmittedDate, r.RequestId), r);
            }
            ById = byId;
            ByIdUnbalanced = bst;
            ByDate = byDate;
            UrgentQueue = new MinHeap<ServiceRequest>(requests.Where(r => r.IsOpen), ServiceRequest.CompareUrgency);
            CategoryTree = BuildCategoryTree();
            RelationGraph = BuildRelationGraph();
        }

        // ------------------------------------------------------------------ queries

        /// <summary>Find a request by its unique ID through the AVL tree (O(log n)), and report the BST / linear cost for comparison.</summary>
        public static TrackResult Track(string requestId)
        {
            string key = (requestId ?? "").Trim().ToUpperInvariant();
            // A plain number is the issue number shown when the issue was reported.
            if (int.TryParse(key, out int issueNumber))
            {
                var fromIssue = requests.FirstOrDefault(r => r.SourceIssue != null && r.SourceIssue.Id == issueNumber);
                if (fromIssue != null) key = fromIssue.RequestId.ToUpperInvariant();
            }
            var result = new TrackResult();
            ById.TryGetValue(key, out var found, out int avl);
            ByIdUnbalanced.TryGetValue(key, out _, out int bst);
            result.Request = found;
            result.AvlComparisons = avl;
            result.BstComparisons = bst;
            int index = requests.FindIndex(r => r.RequestId.Equals(key, StringComparison.OrdinalIgnoreCase));
            result.LinearComparisons = index >= 0 ? index + 1 : requests.Count;
            return result;
        }

        /// <summary>IDs starting with a prefix, via an AVL range query between "prefix" and "prefix + highest char".</summary>
        public static List<ServiceRequest> SearchByPrefix(string prefix)
        {
            string low = (prefix ?? "").Trim().ToUpperInvariant();
            if (low.Length == 0) return new List<ServiceRequest>();
            return ById.Range(low, low + char.MaxValue);
        }

        /// <summary>Requests submitted between two dates (inclusive), oldest first, via a red-black range query: O(log n + k).</summary>
        public static List<ServiceRequest> SubmittedBetween(DateTime from, DateTime to, out int nodesVisited)
        {
            string low = from.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            string high = to.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + "|" + char.MaxValue;
            return ByDate.Range(low, high, out nodesVisited);
        }

        public static List<ServiceRequest> GetSorted(RequestSortMode mode)
        {
            switch (mode)
            {
                case RequestSortMode.NewestFirstRedBlack:
                    var byDate = ByDate.InOrder().Select(kv => kv.Value).ToList();
                    byDate.Reverse();
                    return byDate;
                case RequestSortMode.UrgencyHeap:
                    var urgent = UrgentQueue.ToSortedList();
                    urgent.AddRange(ByDate.InOrder().Select(kv => kv.Value).Where(r => !r.IsOpen).Reverse());
                    return urgent;
                default:
                    return ById.InOrder().Select(kv => kv.Value).ToList();
            }
        }

        public static bool Advance(string requestId, string note = null)
        {
            var found = Track(requestId).Request;
            if (found == null || !found.Advance(note)) return false;
            Rebuild();
            return true;
        }

        /// <summary>Pops the most urgent open request from the min-heap and moves it to its next status.</summary>
        public static ServiceRequest DispatchMostUrgent()
        {
            if (UrgentQueue.IsEmpty) return null;
            var next = UrgentQueue.Peek();
            if (next.Status < RequestStatus.Assigned) next.AssignedCrew = CrewFor(next.Category);
            next.Advance("Dispatched from the urgent queue.");
            Rebuild();
            return next;
        }

        /// <summary>Breadth-first list of related requests with their hop distance from the chosen request.</summary>
        public static List<KeyValuePair<ServiceRequest, int>> RelatedByBreadthFirst(string requestId)
        {
            return RelationGraph.BreadthFirst(Normalise(requestId))
                .Select(kv => new KeyValuePair<ServiceRequest, int>(Track(kv.Key).Request, kv.Value))
                .ToList();
        }

        public static List<ServiceRequest> ClusterByDepthFirst(string requestId)
        {
            return RelationGraph.DepthFirst(Normalise(requestId)).Select(id => Track(id).Request).ToList();
        }

        // ------------------------------------------------------------------ crew route (graph + MST)

        /// <summary>
        /// Cheapest set of roads joining the depot to every area with open requests:
        /// Dijkstra gives the real road distance between each pair of those areas, then Prim's MST runs on
        /// that complete distance graph. Kruskal and Prim are also run on the whole road network as a cross-check.
        /// </summary>
        public static CrewRoutePlan BuildCrewRoute()
        {
            var plan = new CrewRoutePlan();
            var areas = requests.Where(r => r.IsOpen).Select(r => r.Area).Distinct()
                .Where(RoadNetwork.ContainsVertex).OrderBy(a => a).ToList();
            plan.AreasWithOpenRequests.AddRange(areas);

            var stops = new List<string> { Depot };
            stops.AddRange(areas.Where(a => a != Depot));
            var distances = new WeightedGraph<string>();
            foreach (var s in stops) distances.AddVertex(s);
            for (int i = 0; i < stops.Count; i++)
            {
                var fromHere = RoadNetwork.ShortestDistances(stops[i]);
                for (int j = i + 1; j < stops.Count; j++)
                    distances.AddEdge(stops[i], stops[j], fromHere[stops[j]]);
            }
            foreach (var edge in distances.PrimMinimumSpanningTree(Depot))
            {
                plan.PrimEdges.Add(edge);
                plan.RoadsForEdge[edge.ToString()] = RoadNetwork.ShortestPath(edge.From, edge.To, out _);
            }
            plan.PrimTotalKm = WeightedGraph<string>.TotalWeight(plan.PrimEdges);

            plan.KruskalNetworkEdges.AddRange(RoadNetwork.KruskalMinimumSpanningTree());
            plan.KruskalNetworkKm = WeightedGraph<string>.TotalWeight(plan.KruskalNetworkEdges);
            plan.PrimNetworkKm = WeightedGraph<string>.TotalWeight(RoadNetwork.PrimMinimumSpanningTree(Depot));
            return plan;
        }

        public static string DescribeCrewRoute()
        {
            var plan = BuildCrewRoute();
            var sb = new StringBuilder();
            sb.AppendLine("CREW ROUTE PLAN  (Dijkstra shortest paths + Prim minimum spanning tree)");
            sb.AppendLine(new string('=', 72));
            if (plan.AreasWithOpenRequests.Count == 0)
            {
                sb.AppendLine("There are no open requests - no crew route is needed.");
            }
            else
            {
                sb.AppendLine("Areas with open requests: " + string.Join(", ", plan.AreasWithOpenRequests));
                sb.AppendLine();
                sb.AppendLine("Cheapest links from the " + Depot + " to every area:");
                foreach (var e in plan.PrimEdges)
                {
                    int count = requests.Count(r => r.IsOpen && r.Area == e.To);
                    sb.AppendLine($"  {e.From,-18} -> {e.To,-18} {e.Weight,5:0} km   ({count} open request(s))");
                    var roads = plan.RoadsForEdge[e.ToString()];
                    if (roads.Count > 2) sb.AppendLine("       via " + string.Join(" > ", roads));
                }
                sb.AppendLine($"  Total road distance to cover: {plan.PrimTotalKm:0} km");
            }
            sb.AppendLine();
            sb.AppendLine("Whole road network minimum spanning tree (cross-check):");
            sb.AppendLine($"  Kruskal (sorted edges + union-find): {plan.KruskalNetworkEdges.Count} roads, {plan.KruskalNetworkKm:0} km");
            sb.AppendLine($"  Prim (grown from the depot with a heap): {plan.PrimNetworkKm:0} km");
            foreach (var e in plan.KruskalNetworkEdges)
                sb.AppendLine($"    {e.From} - {e.To}: {e.Weight:0} km");
            return sb.ToString();
        }

        // ------------------------------------------------------------------ helpers

        public static string DateKey(DateTime submitted, string requestId)
        {
            return submitted.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + "|" + requestId.ToUpperInvariant();
        }

        private static string Normalise(string id) => (id ?? "").Trim().ToUpperInvariant();

        /// <summary>Finds a known area name inside free-text location (e.g. "12 Vilakazi St, Soweto" gives Soweto).</summary>
        public static string DetectArea(string location)
        {
            string text = (location ?? "").ToLowerInvariant();
            foreach (var area in Areas.Skip(1))
                if (text.Contains(area.ToLowerInvariant())) return area;
            if (text.Contains("cbd") || text.Contains("joburg") || text.Contains("johannesburg")) return "Johannesburg CBD";
            if (text.Contains("tshwane")) return "Pretoria";
            if (text.Contains("alex")) return "Alexandra";
            return "Johannesburg CBD";
        }

        public static RequestPriority PriorityFor(string category)
        {
            string c = (category ?? "").ToLowerInvariant();
            if (c == "utilities" || c == "sanitation" || c == "traffic") return RequestPriority.High;
            if (c == "roads" || c == "building maintenance") return RequestPriority.Medium;
            return RequestPriority.Low;
        }

        private static string CrewFor(string category)
        {
            string c = (category ?? "").ToLowerInvariant();
            if (c == "utilities") return "Water & Electricity Crew";
            if (c == "roads" || c == "traffic") return "Roads & Transport Crew";
            if (c == "sanitation") return "Waste Management Crew";
            if (c == "building maintenance") return "Facilities Crew";
            return "General Maintenance Crew";
        }

        private static WeightedGraph<string> BuildRoadNetwork()
        {
            var g = new WeightedGraph<string>();
            foreach (var a in Areas) g.AddVertex(a);
            g.AddEdge(Depot, "Johannesburg CBD", 3);
            g.AddEdge(Depot, "Braamfontein", 2);
            g.AddEdge("Braamfontein", "Randburg", 12);
            g.AddEdge("Braamfontein", "Sandton", 13);
            g.AddEdge("Johannesburg CBD", "Soweto", 18);
            g.AddEdge("Johannesburg CBD", "Germiston", 16);
            g.AddEdge("Johannesburg CBD", "Alexandra", 14);
            g.AddEdge("Sandton", "Alexandra", 6);
            g.AddEdge("Sandton", "Randburg", 9);
            g.AddEdge("Sandton", "Midrand", 20);
            g.AddEdge("Randburg", "Roodepoort", 15);
            g.AddEdge("Soweto", "Roodepoort", 17);
            g.AddEdge("Alexandra", "Midrand", 19);
            g.AddEdge("Germiston", "Alexandra", 21);
            g.AddEdge("Midrand", "Pretoria", 28);
            g.AddEdge("Germiston", "Pretoria", 55);
            return g;
        }

        private static GeneralTreeNode<string> BuildCategoryTree()
        {
            var rootNode = new GeneralTreeNode<string>($"Service Requests ({requests.Count})");
            foreach (var group in requests.GroupBy(r => r.Category).OrderBy(g => g.Key))
            {
                var categoryNode = rootNode.AddChild($"{group.Key} ({group.Count()})");
                foreach (var statusGroup in group.GroupBy(r => r.Status).OrderBy(g => g.Key))
                {
                    var statusNode = categoryNode.AddChild(ServiceRequest.Describe(statusGroup.Key));
                    foreach (var r in statusGroup.OrderBy(r => r.RequestId))
                        statusNode.AddChild($"{r.RequestId} - {r.Area}");
                }
            }
            return rootNode;
        }

        /// <summary>Edge weight 1 = same area and category, 2 = same area, 3 = same category.</summary>
        private static WeightedGraph<string> BuildRelationGraph()
        {
            var g = new WeightedGraph<string>();
            var ordered = requests.OrderBy(r => r.SubmittedDate).ToList();
            foreach (var r in ordered) g.AddVertex(r.RequestId.ToUpperInvariant());
            for (int i = 0; i < ordered.Count; i++)
            {
                for (int j = i + 1; j < ordered.Count; j++)
                {
                    bool sameArea = ordered[i].Area == ordered[j].Area;
                    bool sameCategory = ordered[i].Category == ordered[j].Category;
                    if (!sameArea && !sameCategory) continue;
                    double weight = sameArea && sameCategory ? 1 : sameArea ? 2 : 3;
                    g.AddEdge(ordered[i].RequestId.ToUpperInvariant(), ordered[j].RequestId.ToUpperInvariant(), weight);
                }
            }
            return g;
        }

        private static void SeedDemoData(DateTime now)
        {
            var demo = new (string Category, string Location, string Area, double DaysAgo, RequestStatus Stage, string Description)[]
            {
                ("Roads", "Main Reef Road, Roodepoort", "Roodepoort", 20.3, RequestStatus.Resolved, "Large pothole damaging cars in the left lane."),
                ("Utilities", "Vilakazi Street, Soweto", "Soweto", 16.6, RequestStatus.InProgress, "Burst water main flooding the street."),
                ("Utilities", "Rivonia Road, Sandton", "Sandton", 14.2, RequestStatus.Assigned, "Six street lights out along the main road."),
                ("Sanitation", "7th Avenue, Alexandra", "Alexandra", 12.8, RequestStatus.UnderReview, "Overflowing sewer manhole next to homes."),
                ("Sanitation", "Bree Street taxi rank, Johannesburg CBD", "Johannesburg CBD", 10.4, RequestStatus.Assigned, "Building rubble dumped on the pavement."),
                ("Traffic", "Bram Fischer Drive, Randburg", "Randburg", 9.1, RequestStatus.Received, "Stop sign knocked over at the intersection."),
                ("Utilities", "Grand Central, Midrand", "Midrand", 7.5, RequestStatus.InProgress, "Electricity outage affecting the whole block."),
                ("Parks", "Pieter Roos Park, Braamfontein", "Braamfontein", 6.3, RequestStatus.Received, "Broken play equipment in the children's area."),
                ("Roads", "Old Potchefstroom Road, Soweto", "Soweto", 5.2, RequestStatus.UnderReview, "Several potholes after the heavy rain."),
                ("Sanitation", "Grayston Drive, Sandton", "Sandton", 4.7, RequestStatus.Received, "Refuse not collected for two weeks."),
                ("Building Maintenance", "President Street, Germiston", "Germiston", 3.4, RequestStatus.Assigned, "Roof leaking at the public library."),
                ("Roads", "Church Street, Pretoria", "Pretoria", 2.6, RequestStatus.Resolved, "Collapsed stormwater drain cover."),
                ("Traffic", "Jan Smuts Avenue, Randburg", "Randburg", 1.2, RequestStatus.Received, "Traffic light stuck on flashing red."),
                ("Sanitation", "Open field near Chris Hani Road, Soweto", "Soweto", 0.3, RequestStatus.Received, "Old tyres dumped and set alight.")
            };
            int[] suffixes = { 9001, 9002, 9003, 9004, 9005, 9006, 9007, 9008, 9009, 9010, 9011, 9012, 9013, 9014 };

            for (int i = 0; i < demo.Length; i++)
            {
                var d = demo[i];
                DateTime submitted = now.AddDays(-d.DaysAgo);
                submitted = new DateTime(submitted.Year, submitted.Month, submitted.Day, submitted.Hour, submitted.Minute, 0);
                string id = "SR-" + submitted.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "-" + suffixes[i];
                var request = new ServiceRequest(id, d.Category, d.Location, d.Area, d.Description, submitted, PriorityFor(d.Category));
                double span = Math.Max(d.DaysAgo, 0.1);
                int steps = (int)d.Stage;
                for (int s = 1; s <= steps; s++)
                {
                    if ((RequestStatus)s == RequestStatus.Assigned) request.AssignedCrew = CrewFor(d.Category);
                    request.Advance(null, submitted.AddDays(span * s / (steps + 1)));
                }
                requests.Add(request);
            }
        }
    }
}
