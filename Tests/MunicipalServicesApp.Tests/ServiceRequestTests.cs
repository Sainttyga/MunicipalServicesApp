using System;
using System.Linq;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;
using Xunit;

namespace MunicipalServicesApp.Tests
{
    public class ServiceRequestTests
    {
        public ServiceRequestTests()
        {
            ServiceRequestRepository.Reset();
            ServiceRequestRepository.Load(null);
        }

        [Fact]
        public void StatusAdvancesThroughTheFullPipeline()
        {
            var r = new ServiceRequest("MUN-1", "Roads", "x", "Soweto", "d", DateTime.Now, RequestPriority.Medium);
            Assert.Equal(0, r.ProgressPercent);
            Assert.True(r.Advance());
            Assert.Equal("Under Review", r.StatusText);
            r.Advance(); r.Advance(); r.Advance();
            Assert.Equal(RequestStatus.Resolved, r.Status);
            Assert.Equal(100, r.ProgressPercent);
            Assert.False(r.Advance());
            Assert.Equal(5, r.History.Count);
        }

        [Fact]
        public void DemoDataIsIndexedInEveryStructure()
        {
            int n = ServiceRequestRepository.All.Count;
            Assert.Equal(14, n);
            Assert.Equal(n, ServiceRequestRepository.ById.Count);
            Assert.Equal(n, ServiceRequestRepository.ByIdUnbalanced.Count);
            Assert.Equal(n, ServiceRequestRepository.ByDate.Count);
            Assert.Equal(ServiceRequestRepository.OpenCount, ServiceRequestRepository.UrgentQueue.Count);
            Assert.Equal(n, ServiceRequestRepository.RelationGraph.VertexCount);
            Assert.True(ServiceRequestRepository.ById.IsBalanced());
            Assert.True(ServiceRequestRepository.ByDate.IsValid());
            Assert.Equal(n, ServiceRequestRepository.CategoryTree.Leaves().Count);
            // IDs embed the date and arrive in date order, so the plain BST becomes a chain
            Assert.Equal(n, ServiceRequestRepository.ByIdUnbalanced.Height());
            Assert.True(ServiceRequestRepository.ById.Height() <= 5);
        }

        [Fact]
        public void TrackingUsesFewerComparisonsWithTheAvlTree()
        {
            var newest = ServiceRequestRepository.All.OrderBy(r => r.SubmittedDate).Last();
            var result = ServiceRequestRepository.Track(newest.RequestId.ToLowerInvariant());
            Assert.Same(newest, result.Request);
            Assert.True(result.AvlComparisons < result.BstComparisons);
            Assert.Null(ServiceRequestRepository.Track("SR-00000000-0000").Request);
        }

        [Fact]
        public void PrefixSearchFindsRequestsByPartialId()
        {
            var any = ServiceRequestRepository.All[3];
            string prefix = any.RequestId.Substring(0, 12);
            var found = ServiceRequestRepository.SearchByPrefix(prefix.ToLowerInvariant());
            Assert.Contains(any, found);
            Assert.All(found, r => Assert.StartsWith(prefix, r.RequestId));
        }

        [Fact]
        public void DateRangeMatchesLinearScanInDateOrder()
        {
            DateTime from = DateTime.Now.AddDays(-7), to = DateTime.Now;
            var viaTree = ServiceRequestRepository.SubmittedBetween(from, to, out int visited);
            var viaScan = ServiceRequestRepository.All
                .Where(r => r.SubmittedDate >= from.AddSeconds(-1) && r.SubmittedDate <= to)
                .OrderBy(r => r.SubmittedDate).ToList();
            Assert.Equal(viaScan.Select(r => r.RequestId), viaTree.Select(r => r.RequestId));
            Assert.True(visited <= ServiceRequestRepository.All.Count);
        }

        [Fact]
        public void HeapOrdersOpenRequestsByPriorityThenAge()
        {
            var order = ServiceRequestRepository.GetSorted(RequestSortMode.UrgencyHeap);
            var open = order.Where(r => r.IsOpen).ToList();
            for (int i = 1; i < open.Count; i++)
                Assert.True(ServiceRequest.CompareUrgency(open[i - 1], open[i]) < 0);
            Assert.All(order.Skip(open.Count), r => Assert.False(r.IsOpen));
            Assert.Equal(ServiceRequestRepository.All.Count, order.Count);
        }

        [Fact]
        public void DispatchAdvancesTheMostUrgentRequest()
        {
            var expected = ServiceRequestRepository.UrgentQueue.Peek();
            var before = expected.Status;
            var dispatched = ServiceRequestRepository.DispatchMostUrgent();
            Assert.Same(expected, dispatched);
            Assert.Equal(before + 1, dispatched.Status);
            Assert.True(ServiceRequestRepository.UrgentQueue.IsValid());
        }

        [Fact]
        public void SortModesUseTreeOrder()
        {
            var byId = ServiceRequestRepository.GetSorted(RequestSortMode.ByIdAvl).Select(r => r.RequestId).ToList();
            Assert.Equal(byId.OrderBy(x => x, StringComparer.Ordinal), byId);
            var newest = ServiceRequestRepository.GetSorted(RequestSortMode.NewestFirstRedBlack);
            for (int i = 1; i < newest.Count; i++) Assert.True(newest[i - 1].SubmittedDate >= newest[i].SubmittedDate);
        }

        [Fact]
        public void RelationGraphLinksRequestsInTheSameAreaOrCategory()
        {
            var soweto = ServiceRequestRepository.All.Where(r => r.Area == "Soweto").ToList();
            Assert.True(soweto.Count >= 2);
            var bfs = ServiceRequestRepository.RelatedByBreadthFirst(soweto[0].RequestId);
            Assert.Equal(soweto[0], bfs[0].Key);
            foreach (var other in soweto.Skip(1))
                Assert.Equal(1, bfs.Single(kv => kv.Key == other).Value);
            var cluster = ServiceRequestRepository.ClusterByDepthFirst(soweto[0].RequestId);
            Assert.Equal(bfs.Count, cluster.Count);
        }

        [Fact]
        public void CrewRouteConnectsTheDepotToEveryOpenArea()
        {
            var plan = ServiceRequestRepository.BuildCrewRoute();
            var stops = plan.AreasWithOpenRequests.Where(a => a != ServiceRequestRepository.Depot).ToList();
            Assert.Equal(stops.Count, plan.PrimEdges.Count);
            var reached = plan.PrimEdges.SelectMany(e => new[] { e.From, e.To }).ToHashSet();
            Assert.All(stops, a => Assert.Contains(a, reached));
            Assert.Equal(plan.KruskalNetworkKm, plan.PrimNetworkKm);
            Assert.Equal(ServiceRequestRepository.RoadNetwork.VertexCount - 1, plan.KruskalNetworkEdges.Count);
            Assert.Equal(127, plan.KruskalNetworkKm);
            Assert.Contains("Kruskal", ServiceRequestRepository.DescribeCrewRoute());
        }

        [Fact]
        public void ReportedIssuesFlowIntoTheStatusPage()
        {
            var issue = new Issue
            {
                Id = 7,
                Location = "Corner of Rivonia and 5th, Sandton",
                Category = "Utilities",
                Description = "No water",
                ReportedDate = new DateTime(2026, 11, 9, 10, 30, 0),
                Status = "Pending"
            };
            ServiceRequestRepository.Load(new[] { issue });
            ServiceRequestRepository.Load(new[] { issue });   // importing twice must not duplicate
            Assert.Equal("SR-20261109-0007", ServiceRequestRepository.RequestIdFor(issue));
            var tracked = ServiceRequestRepository.Track("sr-20261109-0007").Request;
            Assert.NotNull(tracked);
            Assert.Same(tracked, ServiceRequestRepository.Track("7").Request);   // the issue number also works
            Assert.Equal(15, ServiceRequestRepository.All.Count);
            Assert.Equal("Sandton", tracked.Area);
            Assert.Equal(RequestPriority.High, tracked.Priority);
            Assert.Equal(RequestStatus.Received, tracked.Status);
            ServiceRequestRepository.Advance(tracked.RequestId);
            Assert.Equal("Under Review", issue.Status);   // status is written back to the Part 1 issue
        }

        [Fact]
        public void SavedIssueStatusIsRestoredOnImport()
        {
            var issue = new Issue { Id = 12, Location = "Soweto", Category = "Roads", Description = "Pothole", ReportedDate = DateTime.Now.AddDays(-2), Status = "In Progress" };
            ServiceRequestRepository.Load(new[] { issue });
            var tracked = ServiceRequestRepository.Track("12").Request;
            Assert.Equal(RequestStatus.InProgress, tracked.Status);
            Assert.Equal(4, tracked.History.Count);
        }
    }
}

namespace MunicipalServicesApp.Tests
{
    using MunicipalServicesApp.Models;
    using Xunit;

    public class EventManagerTests
    {
        [Fact]
        public void BuildingRecommendationsDoesNotInflateSearchHistory()
        {
            var manager = new EventManager();
            if (manager.GetAllEvents().Count == 0)
                manager.AddEvent(new Event { Title = "Water Supply Notice", Category = "Announcement", Description = "Maintenance", Location = "North", EventDate = DateTime.Now.AddDays(2), Priority = 4 });
            manager.ClearSearchHistory();
            manager.SearchEvents("water");
            manager.SearchEvents("wat", recordSearch: false);
            manager.GetRecommendedEvents();
            manager.GetRecommendedEvents();
            var stats = manager.GetSearchStatistics();
            Assert.Single(stats);
            Assert.Equal(1, stats["water"]);
        }
    }
}
