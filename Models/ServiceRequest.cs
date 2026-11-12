#nullable disable
using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.Models
{
    public enum RequestStatus
    {
        Received = 0,
        UnderReview = 1,
        Assigned = 2,
        InProgress = 3,
        Resolved = 4
    }

    /// <summary>Lower number = more urgent (this is the order the min-heap uses).</summary>
    public enum RequestPriority
    {
        High = 1,
        Medium = 2,
        Low = 3
    }

    public class StatusUpdate
    {
        public StatusUpdate(DateTime timestamp, RequestStatus status, string note)
        {
            Timestamp = timestamp;
            Status = status;
            Note = note;
        }

        public DateTime Timestamp { get; }
        public RequestStatus Status { get; }
        public string Note { get; }
    }

    /// <summary>A service request tracked by its unique reference number (e.g. SR-20261109-0001).</summary>
    public class ServiceRequest
    {
        private readonly List<StatusUpdate> history = new List<StatusUpdate>();

        public ServiceRequest(string requestId, string category, string location, string area,
            string description, DateTime submitted, RequestPriority priority)
        {
            RequestId = requestId;
            Category = category;
            Location = location;
            Area = area;
            Description = description;
            SubmittedDate = submitted;
            Priority = priority;
            Status = RequestStatus.Received;
            history.Add(new StatusUpdate(submitted, RequestStatus.Received, "Request logged with the municipality."));
        }

        public string RequestId { get; }
        public string Category { get; }
        public string Location { get; }
        public string Area { get; }
        public string Description { get; }
        public DateTime SubmittedDate { get; }
        public RequestPriority Priority { get; }
        public RequestStatus Status { get; private set; }
        public string AssignedCrew { get; set; }
        /// <summary>The Part 1 issue this request came from (null for demo data).</summary>
        public Issue SourceIssue { get; set; }
        public IReadOnlyList<StatusUpdate> History => history;

        public bool IsOpen => Status != RequestStatus.Resolved;
        public int ProgressPercent => (int)Status * 100 / (int)RequestStatus.Resolved;
        public string StatusText => Describe(Status);
        public DateTime LastUpdated => history[history.Count - 1].Timestamp;

        /// <summary>Moves the request to the next stage of Received > Under Review > Assigned > In Progress > Resolved.</summary>
        public bool Advance(string note, DateTime when)
        {
            if (!IsOpen) return false;
            Status = Status + 1;
            history.Add(new StatusUpdate(when, Status, note ?? DefaultNote(Status)));
            if (SourceIssue != null) SourceIssue.Status = StatusText;
            return true;
        }

        public bool Advance(string note = null) => Advance(note, DateTime.Now);

        public static string Describe(RequestStatus status)
        {
            switch (status)
            {
                case RequestStatus.UnderReview: return "Under Review";
                case RequestStatus.InProgress: return "In Progress";
                default: return status.ToString();
            }
        }

        public static string DefaultNote(RequestStatus status)
        {
            switch (status)
            {
                case RequestStatus.UnderReview: return "An official is reviewing the request.";
                case RequestStatus.Assigned: return "A field crew has been assigned.";
                case RequestStatus.InProgress: return "The crew is on site working on the problem.";
                case RequestStatus.Resolved: return "The problem has been fixed and the request closed.";
                default: return "Request logged with the municipality.";
            }
        }

        /// <summary>Urgency order for the min-heap: priority first, then the oldest request.</summary>
        public static int CompareUrgency(ServiceRequest a, ServiceRequest b)
        {
            int byPriority = ((int)a.Priority).CompareTo((int)b.Priority);
            if (byPriority != 0) return byPriority;
            int byAge = a.SubmittedDate.CompareTo(b.SubmittedDate);
            return byAge != 0 ? byAge : string.CompareOrdinal(a.RequestId, b.RequestId);
        }
    }
}
