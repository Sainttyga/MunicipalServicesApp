using System.Collections.Concurrent;

namespace MunicipalServicesApp.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public int Priority { get; set; } = 1; // For priority queue (1 = low, 5 = high)
        public DateTime CreatedDate { get; set; }

        public Event()
        {
            CreatedDate = DateTime.Now;
        }
    }

    // Event Data Storage - manages persistence
    public static class EventDataStorage
    {
        private static readonly string StorageFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "events_data.json"
        );

        public static List<Event> LoadEvents()
        {
            try
            {
                if (File.Exists(StorageFilePath))
                {
                    string json = File.ReadAllText(StorageFilePath);
                    var events = System.Text.Json.JsonSerializer.Deserialize<List<Event>>(json);
                    return events ?? new List<Event>();
                }
            }
            catch (Exception)
            {
                // If loading fails, return empty list
            }
            return new List<Event>();
        }

        public static void SaveEvents(List<Event> events)
        {
            try
            {
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = System.Text.Json.JsonSerializer.Serialize(events, options);
                File.WriteAllText(StorageFilePath, json);
            }
            catch (Exception)
            {
                // Handle save error silently or log it
            }
        }

        public static int GetNextEventId(List<Event> events)
        {
            if (events.Count == 0)
                return 1;
            return events.Max(e => e.Id) + 1;
        }
    }

    // Event Manager - implements advanced data structures
    public class EventManager
    {
        private List<Event> _allEvents;
        
        // Stack for managing recently viewed events (LIFO)
        private Stack<Event> _recentlyViewedEvents;
        
        // Queue for managing upcoming events (FIFO)
        private Queue<Event> _upcomingEventsQueue;
        
        // Priority Queue for high-priority events (using SortedList)
        private SortedList<int, List<Event>> _priorityQueue;
        
        // Dictionary for quick lookup by ID (Hash Table)
        private Dictionary<int, Event> _eventsById;
        
        // Dictionary for category-based lookup
        private Dictionary<string, List<Event>> _eventsByCategory;
        
        // SortedDictionary for date-ordered events
        private SortedDictionary<DateTime, List<Event>> _eventsByDate;
        
        // HashSet for unique categories
        private HashSet<string> _uniqueCategories;
        
        // HashSet for unique dates
        private HashSet<DateTime> _uniqueDates;
        
        // User search history for recommendations
        private List<string> _userSearchHistory;
        
        // Dictionary to track search frequency
        private Dictionary<string, int> _searchFrequency;

        public EventManager()
        {
            _allEvents = EventDataStorage.LoadEvents();
            
            // Initialize data structures
            _recentlyViewedEvents = new Stack<Event>();
            _upcomingEventsQueue = new Queue<Event>();
            _priorityQueue = new SortedList<int, List<Event>>(new ReverseComparer<int>());
            _eventsById = new Dictionary<int, Event>();
            _eventsByCategory = new Dictionary<string, List<Event>>();
            _eventsByDate = new SortedDictionary<DateTime, List<Event>>();
            _uniqueCategories = new HashSet<string>();
            _uniqueDates = new HashSet<DateTime>();
            _userSearchHistory = new List<string>();
            _searchFrequency = new Dictionary<string, int>();
            
            RebuildDataStructures();
        }

        // Rebuild all data structures when events change
        private void RebuildDataStructures()
        {
            _eventsById.Clear();
            _eventsByCategory.Clear();
            _eventsByDate.Clear();
            _priorityQueue.Clear();
            _uniqueCategories.Clear();
            _uniqueDates.Clear();
            _upcomingEventsQueue.Clear();
            _recentlyViewedEvents.Clear();

            foreach (var evt in _allEvents)
            {
                // Build ID dictionary
                _eventsById[evt.Id] = evt;
                
                // Build category dictionary
                if (!_eventsByCategory.ContainsKey(evt.Category))
                    _eventsByCategory[evt.Category] = new List<Event>();
                _eventsByCategory[evt.Category].Add(evt);
                
                // Build date dictionary
                var dateKey = evt.EventDate.Date;
                if (!_eventsByDate.ContainsKey(dateKey))
                    _eventsByDate[dateKey] = new List<Event>();
                _eventsByDate[dateKey].Add(evt);
                
                // Build priority queue
                if (!_priorityQueue.ContainsKey(evt.Priority))
                    _priorityQueue[evt.Priority] = new List<Event>();
                _priorityQueue[evt.Priority].Add(evt);
                
                // Build unique categories set
                _uniqueCategories.Add(evt.Category);
                
                // Build unique dates set
                _uniqueDates.Add(evt.EventDate.Date);
                
                // Build upcoming events queue (only future events)
                if (evt.EventDate >= DateTime.Now)
                    _upcomingEventsQueue.Enqueue(evt);
            }
        }

        // Add a new event
        public void AddEvent(Event newEvent)
        {
            newEvent.Id = EventDataStorage.GetNextEventId(_allEvents);
            _allEvents.Add(newEvent);
            RebuildDataStructures();
            EventDataStorage.SaveEvents(_allEvents);
        }

        // Get all events
        public List<Event> GetAllEvents()
        {
            return new List<Event>(_allEvents);
        }

        // Get event by ID (O(1) lookup using Dictionary)
        public Event? GetEventById(int id)
        {
            _eventsById.TryGetValue(id, out var evt);
            if (evt != null)
            {
                _recentlyViewedEvents.Push(evt);
            }
            return evt;
        }

        // Get events by category (O(1) lookup using Dictionary)
        public List<Event> GetEventsByCategory(string category)
        {
            _eventsByCategory.TryGetValue(category, out var events);
            return events ?? new List<Event>();
        }

        // Get events by date range (using SortedDictionary)
        public List<Event> GetEventsByDateRange(DateTime startDate, DateTime endDate)
        {
            var result = new List<Event>();
            foreach (var kvp in _eventsByDate)
            {
                if (kvp.Key >= startDate.Date && kvp.Key <= endDate.Date)
                {
                    result.AddRange(kvp.Value);
                }
            }
            return result;
        }

        // Get events by specific date
        public List<Event> GetEventsByDate(DateTime date)
        {
            _eventsByDate.TryGetValue(date.Date, out var events);
            return events ?? new List<Event>();
        }

        // Get all unique categories (using HashSet)
        public HashSet<string> GetUniqueCategories()
        {
            return new HashSet<string>(_uniqueCategories);
        }

        // Get all unique dates (using HashSet)
        public HashSet<DateTime> GetUniqueDates()
        {
            return new HashSet<DateTime>(_uniqueDates);
        }

        // Get events sorted by date (using SortedDictionary)
        public List<Event> GetEventsSortedByDate()
        {
            var result = new List<Event>();
            foreach (var kvp in _eventsByDate)
            {
                result.AddRange(kvp.Value.OrderBy(e => e.EventDate));
            }
            return result;
        }

        // Get high-priority events (using Priority Queue)
        public List<Event> GetHighPriorityEvents(int minPriority = 3)
        {
            var result = new List<Event>();
            foreach (var kvp in _priorityQueue)
            {
                if (kvp.Key >= minPriority)
                {
                    result.AddRange(kvp.Value);
                }
            }
            return result;
        }

        // Get recently viewed events (using Stack)
        public List<Event> GetRecentlyViewedEvents(int count = 5)
        {
            return _recentlyViewedEvents.Take(count).ToList();
        }

        // Get upcoming events (using Queue)
        public List<Event> GetUpcomingEvents(int count = 10)
        {
            return _upcomingEventsQueue.Take(count).ToList();
        }

        // Search events by keyword
        public List<Event> SearchEvents(string keyword)
        {
            // Record search for recommendations
            RecordSearch(keyword);
            
            var lowerKeyword = keyword.ToLower();
            return _allEvents
                .Where(e => e.Title.ToLower().Contains(lowerKeyword) ||
                           e.Description.ToLower().Contains(lowerKeyword) ||
                           e.Category.ToLower().Contains(lowerKeyword) ||
                           e.Location.ToLower().Contains(lowerKeyword))
                .ToList();
        }

        // Record user search for recommendation system (public method)
        public void RecordSearch(string keyword)
        {
            _userSearchHistory.Add(keyword);
            
            if (_searchFrequency.ContainsKey(keyword))
                _searchFrequency[keyword]++;
            else
                _searchFrequency[keyword] = 1;
            
            // Keep only last 50 searches
            if (_userSearchHistory.Count > 50)
                _userSearchHistory.RemoveAt(0);
        }

        // Get recommended events based on search history
        public List<Event> GetRecommendedEvents(int count = 5)
        {
            if (_searchFrequency.Count == 0)
                return new List<Event>();

            // Find most searched categories/keywords
            var topSearches = _searchFrequency
                .OrderByDescending(kvp => kvp.Value)
                .Take(3)
                .Select(kvp => kvp.Key)
                .ToList();

            var recommended = new List<Event>();
            var usedIds = new HashSet<int>();

            foreach (var search in topSearches)
            {
                // Try exact category match first
                var categoryMatches = _allEvents.Where(e => e.Category == search).ToList();
                foreach (var evt in categoryMatches)
                {
                    if (!usedIds.Contains(evt.Id) && recommended.Count < count)
                    {
                        recommended.Add(evt);
                        usedIds.Add(evt.Id);
                    }
                }

                // If still need more, try keyword search
                if (recommended.Count < count)
                {
                    var matchingEvents = SearchEvents(search);
                    foreach (var evt in matchingEvents)
                    {
                        if (!usedIds.Contains(evt.Id) && recommended.Count < count)
                        {
                            recommended.Add(evt);
                            usedIds.Add(evt.Id);
                        }
                    }
                }
            }

            // If not enough recommendations, add high-priority events
            if (recommended.Count < count)
            {
                var highPriority = GetHighPriorityEvents();
                foreach (var evt in highPriority)
                {
                    if (!usedIds.Contains(evt.Id) && recommended.Count < count)
                    {
                        recommended.Add(evt);
                        usedIds.Add(evt.Id);
                    }
                }
            }

            return recommended;
        }

        // Get search statistics
        public Dictionary<string, int> GetSearchStatistics()
        {
            return new Dictionary<string, int>(_searchFrequency);
        }

        // Clear search history
        public void ClearSearchHistory()
        {
            _userSearchHistory.Clear();
            _searchFrequency.Clear();
        }
    }

    // Custom comparer for reverse sorting (for priority queue)
    public class ReverseComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T? x, T? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return y.CompareTo(x); // Reverse order
        }
    }
}
